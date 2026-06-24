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
    public class InternationalLicenseController(
        InternationalLicenseService InternationalLicenseService,
        IWebHostEnvironment _webHostEnvironment) : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment = _webHostEnvironment;

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllInternationalLicensees()
        {
            var InternationalLicenses = InternationalLicenseService.GetAllInternationalLicenses();
            return (InternationalLicenses == null || InternationalLicenses.Count == 0)
                ? NotFound("No InternationalLicense found.")
                : Ok(InternationalLicenses);
        }

        [HttpGet("{id}", Name = "GetInternationalLicenseByID")]
        public IActionResult GetInternationalLicenseByID(int id)
        {
            if (id <= 0)
                return BadRequest("معرف الرخصة غير صالح.");

            var InternationalLicense = InternationalLicenseService.GetInternationalLicenseByID(id);
            return InternationalLicense == null ? NotFound("الرخصة غير موجودة.") : Ok(InternationalLicense);
        }

        [HttpGet("InternationalImagePath/{id}", Name = "GetInternationalImagePathByApplicationID")]
        public IActionResult GetImagePathByApplicationID(int id)
        {
            if (id <= 0)
                return BadRequest("معرف  غير صالح.");

            var InternationalLicense = InternationalLicenseService.GetImagePathByApplicationID(id);
            return InternationalLicense == null ? NotFound(" غير موجودة.") : Ok(InternationalLicense);
        }


        [HttpGet("ActiveInternationalLicense/{DriverID}", Name = "GetActiveInternationalLicense")]
        public IActionResult GetInternationalLicenseByDriverID(int DriverID)
        {
            if (DriverID <= 0) return BadRequest("معرف السائق غير صالح.");

            var test = InternationalLicenseService.GetInternationalLicenseByDriverID(DriverID);
            return test == null ? NotFound($"الرخصة غير موجودة") : Ok(test);
        }

        [HttpGet("ActiveInternationalLicenses/{DriverID}", Name = "GetActiveInternationalLicenses")]
        public IActionResult GetInternationalLicensesByDriverID(int DriverID)
        {
            if (DriverID <= 0) return BadRequest("معرف السائق غير صالح.");

            var test = InternationalLicenseService.GetInternationalLicensesByDriverID(DriverID);
            return test == null ? NotFound($"الرخصة غير موجودة") : Ok(test);
        }

        [HttpPost("AddInternationalLicense")]
        public IActionResult InsertInternationalLicense([FromBody] InternationalLicense InternationalLicense)
        {
            try
            {
                if (InternationalLicense == null)
                    return BadRequest("بيانات الرخصة مطلوبة.");

                var insertedId = InternationalLicenseService.InsertInternationalLicense(InternationalLicense);

                if (insertedId <= 0)
                    return BadRequest("فشل في إضافة الرخصة.");

                InternationalLicense.InternationalLicenseID = insertedId;

                // التصحيح هنا:
                return CreatedAtRoute(
                    "GetActiveInternationalLicense",           // اسم الراوت المذكور في Get
                    new { DriverID = insertedId },    // اسم المتغير يجب أن يطابق الموجود في Get
                    InternationalLicense                     // الكائن المرجوع
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي: {ex.Message}");
            }
        }

        [HttpPut("UpdateInternationalLicense/{id}")]
        public IActionResult UpdateInternationalLicense(int id, [FromBody] InternationalLicense InternationalLicense)
        {
            if (id <= 0 || id != InternationalLicense.InternationalLicenseID)
                return BadRequest("المعرف غير متطابق.");

            try
            {
                var result = InternationalLicenseService.UpdateInternationalLicense(InternationalLicense);
                return result > 0 ? Ok(InternationalLicense) : NotFound("الشخص غير موجود.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteInternationalLicense/{id}")]
        public IActionResult DeleteInternationalLicense(int id)
        {
            try
            {
                if (id <= 0) return BadRequest("المعرف غير صالح.");
                var InternationalLicense = InternationalLicenseService.GetInternationalLicenseByID(id);
                if (InternationalLicense == null) return NotFound();



                int result = InternationalLicenseService.DeleteInternationalLicense(id);

                if (result > 0) return Ok("تم الحذف بنجاح");
                return BadRequest("فشل حذف الشخص من قاعدة البيانات");
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

    }
}