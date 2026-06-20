using DAL.DataAccessLayer.Common;
using DataAccessLayer.Dtos;
using Entites;
using Entites.AurhModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Business_Layer.Services
{
    public class AuthService
    {
        private readonly PasswordHasher<string> _passwordHasher = new();
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtToken _jwt;

        public AuthService(IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            JwtToken jwt)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _jwt = jwt;
        }

        public (string Token, DateTime Expiration) GenerateAccessToken(AuthUser user)
        {
            var secretKey = _configuration["JwtSettings:Secret"]
                ?? throw new InvalidOperationException("JWT Secret Key is missing in configuration.");
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var duration = double.Parse(_configuration["JwtSettings:DurationInMinutes"] ?? "15");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
            };

            var expires = DateTime.UtcNow.AddMinutes(duration);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _jwt.Token = tokenString;
            _jwt.Expiration = expires;

            return (tokenString, expires);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public void IssueTokensToCookies(AuthUser user, string? oldTokenToRevoke = null)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return;

            // إذا مررنا توكن قديم لتبطيله (في حالة الـ Refresh) نقوم بإبطاله فوراً
            if (!string.IsNullOrEmpty(oldTokenToRevoke))
            {
                SqlHelper.ExecuteNonQuery("SpRevokeRefreshToken", new { Token = oldTokenToRevoke });
            }
            else
            {
                // في حالة الـ Login العادي، نقرأ الكوكي الحالية ونبطلها منعاً للتراكم
                var currentCookieToken = context.Request.Cookies["refreshToken"];
                if (!string.IsNullOrEmpty(currentCookieToken))
                {
                    SqlHelper.ExecuteNonQuery("SpRevokeRefreshToken", new { Token = currentCookieToken });
                }
            }

            // توليد الطقم الجديد بدقة
            var (accessToken, expiration) = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // حفظ الـ Refresh Token الجديد في قاعدة البيانات
            var refreshEntity = new UserRefreshToken()
            {
                UserId = user.UserId,
                Token = newRefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            SqlHelper.ExecuteNonQuery("SpInsertRefreshToken", refreshEntity);

            // إعدادات الكوكيز الصارمة الآمنة
            var baseCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = _httpContextAccessor.HttpContext?.Request.IsHttps ?? false,
                SameSite = SameSiteMode.Strict,
                Path = "/" // حيوية جداً لكي تظهر الكوكي في كل الـ Endpoints
            };

            // حقن الـ Access Token
            context.Response.Cookies.Append("accessToken", accessToken, new CookieOptions
            {
                HttpOnly = baseCookieOptions.HttpOnly,
                Secure = baseCookieOptions.Secure,
                SameSite = baseCookieOptions.SameSite,
                Path = baseCookieOptions.Path,
                Expires = expiration
            });

            // حقن الـ Refresh Token
            context.Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = baseCookieOptions.HttpOnly,
                Secure = baseCookieOptions.Secure,
                SameSite = baseCookieOptions.SameSite,
                Path = baseCookieOptions.Path,
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }

        public AuthUser Login(Login login)
        {
            var user = SqlHelper.ExecuteeQuerySingl<AuthUser>("SpGetAuthUserByEmail", new { Email = login.Email }, out int ID);

            if (user == null || !VerifyPassword(user.PasswordHash, login.Password))
            {
                throw new Exception("Invalid email or password.");
            }

            IssueTokensToCookies(user);
            return user;
        }

        public AuthUser Register(Register register)
        {
            if (register.Password != register.RePassword)
            {
                throw new Exception("Passwords do not match.");
            }

            Emailvalidation(register.Email);
            PasswordValidation(register.Password);

            var newUser = new AuthUser
            {
                Name = register.Name,
                Email = register.Email,
                PasswordHash = HashPassword(register.Password),
                IsActive = 1
            };

            int newUserId = SqlHelper.ExecuteNonQuery("SpInsertAuthUser", newUser, true);
            newUser.UserId = newUserId;

            IssueTokensToCookies(newUser);
            return newUser;
        }

        public void Logout()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return;

            var refreshToken = context.Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                RevokeRefreshToken(refreshToken);
            }

            var deleteCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            context.Response.Cookies.Append("accessToken", "", deleteCookieOptions);
            context.Response.Cookies.Append("refreshToken", "", deleteCookieOptions);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var secretKey = _configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret Key is missing.");
            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false 
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;
                return principal;
            }
            catch { return null; }
        }

        public string HashPassword(string password) => _passwordHasher.HashPassword("UserRegistration", password);
        public bool VerifyPassword(string hashedPassword, string providedPassword) => _passwordHasher.VerifyHashedPassword("UserLogin", hashedPassword, providedPassword) == PasswordVerificationResult.Success;
        public UserRefreshToken GetRefreshToken(string token) => SqlHelper.ExecuteQueryAll<UserRefreshToken>("SpGetRefreshToken", new { Token = token }).FirstOrDefault();
        public void RevokeRefreshToken(string token) => SqlHelper.ExecuteNonQuery("SpRevokeRefreshToken", new { Token = token });
        public AuthUser GetAuthUserById(int userId) => SqlHelper.ExecuteeQuerySingl<AuthUser>("SpGetAuthUserById", new { Id = userId }, out int ID);

        private void Emailvalidation(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern))
            {
                throw new Exception("Invalid email format.");
            }
        }

        private void PasswordValidation(string password)
        {
            if (password.Length < 6) 
            {
                throw new Exception("Password must be at least 6 characters long.");
            }
        }

        public List<AuthUserDto> GetAllUsers()
        {
            return SqlHelper.ExecuteQueryAll<AuthUserDto>("SP_GetAllUsers");
        }
        public AuthUserDto GetUserById(int id)
        {
            int returnedId;

            var person = SqlHelper.ExecuteeQuerySingl<AuthUserDto>(
                "SP_GetUserbyId",
                new { ID = id }, 
                out returnedId
            );

            return person;
        }
        public int InsertUser(AuthUser User)
        {
            return SqlHelper.ExecuteNonQuery("SP_InsertUpdateUser", new { Name = User.Name, Email = User.Email, PasswordHash = HashPassword(User.PasswordHash), IsActive = User.IsActive }, true);
        }
        public int UpdateUser(AuthUser User)
        {
            return SqlHelper.ExecuteNonQuery("SP_InsertUpdateUser", new { ID = User.UserId, Name = User.Name, Email = User.Email, PasswordHash = HashPassword(User.PasswordHash), IsActive = User.IsActive}, isInsert: false);
        }
        public int DeleteUser(int id)
        {
            return SqlHelper.ExecuteNonQuery("SP_DeleteUser", new { ID = id });
        }
    }
}