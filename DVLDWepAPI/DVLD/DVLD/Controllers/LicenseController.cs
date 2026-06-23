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
  
    public class LicenseController(
        LicenseService LicenseService,
        IWebHostEnvironment _webHostEnvironment) : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment = _webHostEnvironment;

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllLicensees()
        {
            var Licenses = LicenseService.GetAllLicenses();
            return (Licenses == null || Licenses.Count == 0)
                ? NotFound("No License found.")
                : Ok(Licenses);
        }

        [HttpGet("{id}", Name = "GetLicenseByID")]
        public IActionResult GetLicenseByID(int id)
        {
            if (id <= 0)
                return BadRequest("معرف الرخصة غير صالح.");

            var License = LicenseService.GetLicenseByID(id);
            return License == null ? NotFound("الرخصة غير موجودة.") : Ok(License);
        }

        [HttpGet("LicensesBy/{DriverID}", Name = "GetLicensesByDriverID")]
        public IActionResult GetLicensesByDriverID(int DriverID)
        {
            if (DriverID <= 0)
                return BadRequest("معرف الرخصة غير صالح.");

            var Licenses = LicenseService.GetLicensesByDriverID(DriverID);
            return Licenses == null ? NotFound("الرخصة غير موجودة.") : Ok(Licenses);
        }



        [HttpGet("Deactivate/{Id}", Name = "DeactivateLicense")]
        public IActionResult DeactivateLicense(int Id)
        {
            if (Id <= 0)
                return BadRequest("معرف الرخصة غير صالح.");

            var DeactivateLicense = LicenseService.DeactivateLicense(Id);
            return DeactivateLicense == 0 ? NotFound("الرخصة غير موجودة.") : Ok(DeactivateLicense);
        }



        [HttpGet("ImagePath/{id}", Name = "GetImagePathByApplicationID")]
        public IActionResult GetImagePathByApplicationID(int id)
        {
            if (id <= 0)
                return BadRequest("معرف  غير صالح.");

            var License = LicenseService.GetImagePathByApplicationID(id);
            return License == null ? NotFound(" غير موجودة.") : Ok(License);
        }


        [HttpGet("ActiveLicense/{PersonId}/{LicenseClassId}", Name = "GetActiveLicense")]
        public IActionResult GetActiveLicenseByPersonID(int PersonId, int LicenseClassId)
        {
            if (PersonId <= 0) return BadRequest("معرف الاختبار غير صالح.");

            var test = LicenseService.GetActiveLicenseByPersonID(PersonId, LicenseClassId);
            return test == null ? NotFound($"الاختبار غير موجود.") : Ok(test);
        }

        [HttpGet("GetPersonIdAndLicenseClass/{Id}", Name = "GetPersonIdAndLicenseClass")]
        public IActionResult GetPersonIdAndLicenseClass(int Id )
        {
            if (Id <= 0) return BadRequest("معرف الاختبار غير صالح.");

            var test = LicenseService.GetPersonIdAndLicenseClass(Id);
            return test == null ? NotFound($"الاختبار غير موجود.") : Ok(test);
        }

        [HttpPost("AddLicense")]
        public IActionResult InsertLicense([FromBody] License License)
        {
            try
            {
                if (License == null)
                    return BadRequest("بيانات الرخصة مطلوبة.");

                var insertedId = LicenseService.InsertLicense(License);

                if (insertedId <= 0)
                    return BadRequest("فشل في إضافة الرخصة.");

                License.LicenseID = insertedId;

                // التصحيح هنا:
                return CreatedAtRoute(
                    "GetLicenseByID",           // اسم الراوت المذكور في Get
                    new { id = insertedId },    // اسم المتغير يجب أن يطابق الموجود في Get
                    License                     // الكائن المرجوع
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي: {ex.Message}");
            }
        }

        [HttpPut("UpdateLicense/{id}")]
        public IActionResult UpdateLicense(int id, [FromBody] License License)
        {
            if (id <= 0 || id != License.LicenseID)
                return BadRequest("المعرف غير متطابق.");

            try
            {
                var result = LicenseService.UpdateLicense(License);
                return result > 0 ? Ok(License) : NotFound("الشخص غير موجود.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteLicense/{id}")]
        public IActionResult DeleteLicense(int id)
        {
            if (id <= 0) return BadRequest("المعرف غير صالح.");
            var License = LicenseService.GetLicenseByID(id);
            if (License == null) return NotFound();


         
            int result = LicenseService.DeleteLicense(id);

            if (result > 0) return Ok("تم الحذف بنجاح");
            return BadRequest("فشل حذف الشخص من قاعدة البيانات");

        }



    
    }
}