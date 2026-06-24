using Business_Layer.Extentions;
using Business_Layer.Services;
using BusinessLayer;
using BusinessLayer.Services;
using DAL.DataAccessLayer.Common;
using Entites.AurhModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// 1. تسجيل الـ Controllers وإعدادات الـ JSON (مرة واحدة فقط وبأفضل ممارسة)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddScoped<PeopleService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ApplicationTypeService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<TestService>();
builder.Services.AddScoped<TestTypeService>();
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<LicenseClassService>();
builder.Services.AddScoped<LicenseService>();
builder.Services.AddScoped<LocalDrivingLicenseApplicationService>();
builder.Services.AddScoped<TestAppointmentService>();
builder.Services.AddScoped<InternationalLicenseService>();
builder.Services.AddScoped<DetainedLicenseService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtToken>();

builder.Services.AddHttpContextAccessor();

SqlHelper.Init(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "AuthPolicy", fixedOptions =>
    {
        fixedOptions.PermitLimit = 4;
        fixedOptions.Window = TimeSpan.FromMinutes(1);
        fixedOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        fixedOptions.QueueLimit = 0;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "لقد تجاوزت حد الطلبات المسموح به. يرجى المحاولة مرة أخرى بعد دقيقة."
        }, token);
    };
});

builder.Services.AddCors(o =>
{
    o.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});


var secretKey = builder.Configuration["JwtSettings:Secret"] ?? "اكتب_هنا_مفتاح_سري_قوي_جدا_وطويل";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var cookieToken = context.Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(cookieToken))
            {
                context.Token = cookieToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AngularPolicy");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();