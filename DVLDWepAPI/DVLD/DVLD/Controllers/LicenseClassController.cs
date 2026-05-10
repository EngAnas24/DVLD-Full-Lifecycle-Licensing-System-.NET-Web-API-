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
    // استخدام Primary Constructor بشكل صحيح
    public class LicenseClassController(
        LicenseClassService LicenseClassService,
        IWebHostEnvironment _webHostEnvironment) : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment = _webHostEnvironment;

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllLicenseClasses()
        {
            var LicenseClasss = LicenseClassService.GetAllLicenseClasses();
            return (LicenseClasss == null || LicenseClasss.Count == 0)
                ? NotFound("No LicenseClass found.")
                : Ok(LicenseClasss);
        }

        [HttpGet("{ClassID}", Name = "GetLicenseClassByClassID")]
        public IActionResult GetLicenseClassByClassID(int ClassID)
        {
            if (ClassID <= 0)
                return BadRequest("معرف  نوع الرخصة غير صالح.");

            var licenseclass = LicenseClassService.GetLicenseClassByClassID(ClassID);
            return licenseclass == null ? NotFound(" نوع الرخصة غير موجود.") : Ok(licenseclass);
        }

        [HttpGet("GetLicenseClassIDByLocalAppID/{Id}")]
        public IActionResult GetLicenseClassIDByLocalAppID(int Id)
        {
            if (Id <= 0)
                return BadRequest("معرف الطلب غير صالح.");

            var licenseclassId = LicenseClassService.GetLicenseClassIDByLocalAppID(Id);

            if (licenseclassId <= 0)
                return NotFound("فئة الرخصة غير موجودة لهذا الطلب.");

            return Ok(licenseclassId);
        }


        [HttpPost("AddLicenseClass")]
        public IActionResult InsertLicenseClass([FromBody] LicenseClass licenseclass)
        {
            try
            {
                if (licenseclass == null)
                    return BadRequest("بيانات نوع الرخصة مطلوبة.");

                var insertedId = LicenseClassService.InsertLicenseClass(licenseclass);

                if (insertedId <= 0)
                    return BadRequest("فشل في إضافة  نوع الرخصة.");

                licenseclass.LicenseClassID = insertedId;

                return CreatedAtRoute(
                    "GetLicenseClassByClassID",
                    new { ClassID = insertedId },
                    licenseclass
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي: {ex.Message}");
            }
        }

        [HttpPut("UpdateLicenseClass/{id}")]
        public IActionResult UpdateLicenseClass(int id, [FromBody] LicenseClass LicenseClass)
        {
            if (id <= 0 || id != LicenseClass.LicenseClassID)
                return BadRequest("المعرف غير متطابق.");

            try
            {
                var result = LicenseClassService.UpdateLicenseClass(LicenseClass);
                return result > 0 ? Ok(LicenseClass) : NotFound("الشخص غير موجود.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteLicenseClass/{id}")]
        public IActionResult DeleteLicenseClass(int id)
        {
            if (id <= 0) return BadRequest("المعرف غير صالح.");
            var licenseclass = LicenseClassService.GetLicenseClassByClassID(id);
            if (licenseclass == null) return NotFound();


         
            int result = LicenseClassService.DeleteLicenseClass(id);

            if (result > 0) return Ok("تم الحذف بنجاح");
            return BadRequest("فشل حذف الشخص من قاعدة البيانات");

        }

      
    }
}