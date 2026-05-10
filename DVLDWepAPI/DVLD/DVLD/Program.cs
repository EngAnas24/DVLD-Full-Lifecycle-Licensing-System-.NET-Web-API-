using Business_Layer.Extentions;
using Business_Layer.Services;
using BusinessLayer;
using BusinessLayer.Services;
using DAL.DataAccessLayer.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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



SqlHelper.Init(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddCors(o =>
{
    o.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AngularPolicy");   
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

app.Run();
