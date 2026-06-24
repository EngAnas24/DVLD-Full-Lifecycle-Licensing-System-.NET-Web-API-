using Business_Layer.Services;
using BusinessLayer;
using BusinessLayer.Services;
using DAL.DataAccessLayer.Common;
using DVDL.Domain.Entities;
using Entites;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalDrivingLicenseApplicationController(
        LocalDrivingLicenseApplicationService LocalDrivingLicenseApplicationService,
        IWebHostEnvironment _webHostEnvironment) : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment = _webHostEnvironment;


        [HttpPost("create")]
        public IActionResult CreateApplication([FromBody] CreateLocalDrivingLicenseAppDto targetDto)
        {
            // 1. التحقق من صحة المدخلات (Data Validation)
            if (targetDto == null)
            {
                return BadRequest("Invalid application data.");
            }

            try
            {
                // 2. استدعاء الـ Service لتنفيذ الـ Transaction في الـ DB
                int newLocalAppId = LocalDrivingLicenseApplicationService.CreateLocalDrivingLicenseApplication(targetDto);

                if (newLocalAppId > 0)
                {
                    // 3. الإرجاع الناجح بحسب معايير REST API (Status 201 Created)
                    return CreatedAtAction(
                        nameof(GetLocalApplicationByIdAsync), // دالة جلب التفاصيل (إن وجدت مستقبلاً)
                        new { id = newLocalAppId },
                        new { message = "Application created successfully within transaction.", id = newLocalAppId }
                    );
                }

                return BadRequest("Failed to create the application.");
            }
            catch (Exception ex)
            {
                // 4. معالجة الأخطاء القادمة من الـ Database بأمان (مثل فشل شروط الـ Business Logic)
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateApplication(int id, [FromBody] UpdateLocalDrivingLicenseAppDto targetDto)
        {
            if (targetDto == null)
            {
                return BadRequest("Invalid application data.");
            }

            try
            {
                int resultLocalAppId = LocalDrivingLicenseApplicationService.UpdateLocalDrivingLicenseApplication(id, targetDto);

                if (resultLocalAppId > 0)
                {
                    return Ok(new { message = "Application updated successfully within transaction.", id = resultLocalAppId });
                }

                return BadRequest("Failed to update the application.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("GetLocalDrivingLicenseApplications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllLocalDrivingLicenseApplications()
        {
            var LocalDrivingLicenseApplications = LocalDrivingLicenseApplicationService.GetLocalDrivingLicenseApplications();
            return (LocalDrivingLicenseApplications == null || LocalDrivingLicenseApplications.Count == 0)
                ? NotFound("No LocalDrivingLicenseApplication found.")
                : Ok(LocalDrivingLicenseApplications);
        }
        [HttpGet("{id}", Name = "GetLocalApplicationByIdAsync")]
        public IActionResult GetLocalApplicationByIdAsync(int id)
        {
            if (id <= 0) return BadRequest("معرف التقديم غير صالح.");

            var LocalDrivingLicenseApplication = LocalDrivingLicenseApplicationService.GetLocalApplicationByIdAsync(id);
            return LocalDrivingLicenseApplication == null ? NotFound($"التقديم غير موجود.") : Ok(LocalDrivingLicenseApplication);
        }

        [HttpGet("LocalApplication/{id}", Name = "GetLocalApplicationByApplicationId")]
        public IActionResult GetLocalApplicationByApplicationId(int id)
        {
            if (id <= 0) return BadRequest("معرف التقديم غير صالح.");

            var LocalDrivingLicenseApplication = LocalDrivingLicenseApplicationService.GetLocalApplicationByApplicationId(id);
            return LocalDrivingLicenseApplication == null ? NotFound($"التقديم غير موجود.") : Ok(LocalDrivingLicenseApplication);
        }


        [HttpPost("InsertLocalApplication")]
        public IActionResult InsertLocalApplication([FromBody] LocalDrivingLicenseApplications LocalApplication)
        {
            try
            {
                if (LocalApplication == null) return BadRequest("بيانات التقديم مطلوبة.");

                var insertedId = LocalDrivingLicenseApplicationService.InsertLocalApplication(LocalApplication);
                if (insertedId > 0)
                {
         
                    return CreatedAtRoute("GetApplicationById", new { id = insertedId }, LocalApplication);
                }
                return BadRequest("فشل في إضافة التقديم.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي: {ex.Message}");
            }
        }

        [HttpPut("UpdateLocalApplication/{id}")]
        public IActionResult UpdateLocalApplication(int id, [FromBody] LocalDrivingLicenseApplications LocalApplication)
        {
            if (id <= 0 || id != LocalApplication.LocalDrivingLicenseApplicationID)
                return BadRequest("المعرف غير متطابق.");

            try
            {
                var result = LocalDrivingLicenseApplicationService.UpdateLocalApplication(LocalApplication);
                return result > 0 ? Ok(LocalApplication) : NotFound("التقديم غير موجود.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("DeleteLocalApplication/{id}")]
        public IActionResult DeleteLocalApplication(int id)
        {
            try
            {

            if (id <= 0) return BadRequest("المعرف غير صالح.");
            var LocalDrivingLicenseApplication = LocalDrivingLicenseApplicationService.GetLocalApplicationByIdAsync(id);
            if (LocalDrivingLicenseApplication == null) return NotFound();



            int result = LocalDrivingLicenseApplicationService.DeleteLocalApplication(id);

            if (result > 0) return Ok("تم الحذف بنجاح");
            return BadRequest("فشل حذف التقديم من قاعدة البيانات");
            }
            catch (DeleteConflictException ex)
            {
                return Conflict(new
                {
                    message = "لا يمكن حذف هذا السجل لارتباطه ببيانات أخرى.",
                    dependentTable = ex.DependentTable
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي بالسيرفر: {ex.Message}");
            }

        }

        [HttpGet("GetPeronIdbyApplicationId/{id}", Name = "GetPeronIdbyApplicationId")]
        public IActionResult GetPeronIdbyApplicationId(int id)
        {
            if (id <= 0) return BadRequest("معرف التقديم غير صالح.");

            var LocalDrivingLicenseApplication = LocalDrivingLicenseApplicationService.GetPeronIdbyApplicationId(id);
            return LocalDrivingLicenseApplication <= 0 ? NotFound($"التقديم غير موجود.") : Ok(LocalDrivingLicenseApplication);
        }

        [HttpGet("has/{personId}/{licenseClassId}", Name = "HasApplication")]
        public IActionResult HasApplication(int personId, int licenseClassId)
        {
            if (personId <= 0) return BadRequest("معرف الشخص غير صالح.");
            if (licenseClassId <= 0) return BadRequest("معرف فئة الرخصة غير صالح.");

            var LocalDrivingLicenseApplication = LocalDrivingLicenseApplicationService.HasApplication(personId, licenseClassId  );
            return LocalDrivingLicenseApplication <= 0 ? NotFound($" التقديم غير موجود مسبقا.") : Ok(LocalDrivingLicenseApplication);
        }

        [HttpGet("HasAlreadyLicense/{personId}/{licenseClassId}", Name = "HasAlreadyLicense")]
        public IActionResult HasAlreadyLicense(int personId, int licenseClassId)
        {
            if (personId <= 0) return BadRequest("معرف الشخص غير صالح.");
            if (licenseClassId <= 0) return BadRequest("معرف فئة الرخصة غير صالح.");

            var LocalDrivingLicenseApplication = LocalDrivingLicenseApplicationService.HasAlreadyLicense(personId, licenseClassId);
            return LocalDrivingLicenseApplication <= 0 ? NotFound($"ما عنده رخصة مسبقا") : Ok(LocalDrivingLicenseApplication);
        }

    }
}