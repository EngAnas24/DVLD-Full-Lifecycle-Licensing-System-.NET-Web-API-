using Business_Layer.Services;
using BusinessLayer.Services;
using Entites;
using Entites.AurhModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace DVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [EnableRateLimiting("AuthPolicy")] // 🔒 حماية جميع الإندبوينتس داخل هذا الكنترولر بالسياسة التي عرفناها
    public class AuthUserController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthUserController(AuthService authService)
        {
            _authService = authService;
        }

        // 1️⃣ إندبوينت إنشاء حساب جديد
        [HttpPost("register")] // أضفنا "register" لتكون واضحة
        public IActionResult Register([FromBody] Register request)
        {
            try
            {
                var user = _authService.Register(request);

                // نُعيد للمتصفح بياناته الأساسية فقط (التوكنز أصبحت داخل الكوكيز تلقائياً)
                return Ok(new
                {
                    message = "تم إنشاء الحساب بنجاح وتسجيل الدخول آمن.",
                    username = user.Name,
                    email = user.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 2️⃣ إندبوينت تسجيل الدخول
        [HttpPost("login")]
        public IActionResult Login([FromBody] Login request)
        {
            try
            {
                var user = _authService.Login(request);

                // الاستجابة نظيفة تماماً ولا تحتوي على نصوص التوكنات المكشوفة
                return Ok(new
                {
                    message = "تم تسجيل الدخول بنجاح وآمن تماماً.",
                    username = user.Name,
                    email = user.Email
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { error = ex.Message }); // استخدام Unauthorized أصح هندسياً عند فشل الهوية
            }
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {

            _authService.Logout();

            return Ok(new { message = "تم تسجيل الخروج بنجاح وتدمير الجلسة آمنياً." });
        }
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _authService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي: {ex.Message}");

            }
        }
        [HttpGet("users/{id}")]
        public IActionResult GetUserbyId(int id)
        {
            try
            {
                var user = _authService.GetUserById(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي: {ex.Message}");

            }
        }
        [HttpPost("AddUser")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult InsertUser([FromBody] AuthUser user)
        {
            try
            {
                if (user == null) return BadRequest("بيانات المستخدم مطلوبة.");

                user.UserId = 0;

                var insertedId = _authService.InsertUser(user);

                if (insertedId > 0)
                {
                    user.UserId  = insertedId;
                    return Ok(user);
                }

                return BadRequest("فشل الحفظ: تأكد من أن الـ UserId موجود وصحيح، وأن اسم المستخدم غير مكرر.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ تقني: {ex.Message}");
            }
        }

        [HttpPut("UpdateUser/{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateUser(int id, [FromBody] AuthUser User)
        {
            if (id <= 0 || id != User.UserId)
            {
                return BadRequest("معرف الشخص غير صالح أو غير متطابق.");
            }

            try
            {
                var resultId = _authService.UpdateUser(User);

                if (resultId > 0)
                {
                    return Ok(User);
                }
                else
                {
                    return NotFound($"الشخص ذو الرقم {id} غير موجود.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        [Route("DeleteUser/{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("معرف الشخص غير صالح.");
            }
            try
            {
                var resultId = _authService.DeleteUser(id);
                if (resultId > 0)
                {
                    return Ok($"تم حذف الشخص ذو الرقم {id} بنجاح.");
                }
                else
                {
                    return NotFound($"الشخص ذو الرقم {id} غير موجود.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            // 🌟 نقرأ فقط الـ refreshToken لأنه الوحيد الذي يضمن المتصفح بقاءه (صلاحيته 7 أيام)
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("جلسة العمل منتهية أو غير صالحة. يرجى إعادة تسجيل الدخول.");
            }

            // 🌟 جلب بيانات التوكن مباشرة من قاعدة البيانات
            var storedToken = _authService.GetRefreshToken(refreshToken);

            if (storedToken == null) return Unauthorized("جلسة العمل غير موجودة بالسيرفر.");
            if (storedToken.IsRevoked) return Unauthorized("هذا التوكن تم إبطاله سابقاً (محاولة اختراق محتملة).");
            if (storedToken.ExpiryDate < DateTime.UtcNow) return Unauthorized("انتهت صلاحية الجلسة، يرجى تسجيل الدخول مجدداً.");

            // 🌟 جلب بيانات المستخدم مباشرة بناءً على الـ UserId المخزن مع التوكن في الداتا بيز
            var user = _authService.GetAuthUserById(storedToken.UserId);
            if (user == null) return NotFound("المستخدم غير موجود.");

            // تدوير الكوكيز وإصدار طقم جديد تماماً
            _authService.IssueTokensToCookies(user, refreshToken);

            return Ok(new { message = "تم تجديد الصلاحية بنجاح بنظام الكوكيز الآمن." });
        }

        [AllowAnonymous] // 🌟 حاسمة جداً لكي يسمح الـ Middleware للدالة بفحص الكوكيز يدوياً وتجديدها صامتاً دون رفض مسبق
        [HttpGet("me")]
        public IActionResult CheckAuth()
        {
            var accessToken = Request.Cookies["accessToken"];
            var refreshToken = Request.Cookies["refreshToken"];

            // 1️⃣ إذا لم يكن هناك توكن تجديد أصلاً، فالجلسة ميتة تماماً
            if (string.IsNullOrEmpty(refreshToken)) return Unauthorized();

            // 2️⃣ محاولة قراءة البيانات من الـ Access Token (حتى لو كان منتهياً)
            ClaimsPrincipal? principal = null;
            if (!string.IsNullOrEmpty(accessToken))
            {
                principal = _authService.GetPrincipalFromExpiredToken(accessToken);
            }

            // 3️⃣ السيناريو الأخطر: الـ Access Token غير موجود أو تالف أو انتهت دقائق صلاحيته!
            if (principal == null)
            {
                // نتحقق هل الـ Refresh Token المخزن في المتصفح صالح في قاعدة البيانات؟
                var storedToken = _authService.GetRefreshToken(refreshToken);

                if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
                {
                    return Unauthorized("انتهت صلاحية الجلسة بالكامل.");
                }

                // جلب المستخدم صاحب الجلسة لإصدار طقم كوكيز جديد له فوراً (Silent Auto-Refresh)
                var user = _authService.GetAuthUserById(storedToken.UserId);
                if (user == null) return Unauthorized();

                // 🌟 تجديد الكوكيز فوراً في المتصفح صامتاً من السيرفر!
                _authService.IssueTokensToCookies(user, refreshToken);

                // إعادة بناء الـ Principal لقراءة الـ Claims بعد التجديد بنجاح
                var newAccessToken = Request.HttpContext.Response.Cookies; // الكوكي الجديدة حُقنت

                // نرسل الرد مباشرة باسم وايميل المستخدم بسلام
                return Ok(new { username = user.Name, email = user.Email, userId = user.UserId });
            }

            // 4️⃣ السيناريو الطبيعي: الـ Access Token لا يزال حياً وصالحاً
            var nameClaim = principal.FindFirst(ClaimTypes.Name)?.Value;
            var emailClaim = principal.FindFirst(ClaimTypes.Email)?.Value;
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Ok(new { username = nameClaim, email = emailClaim, userId = userIdClaim });
        }

    }
}