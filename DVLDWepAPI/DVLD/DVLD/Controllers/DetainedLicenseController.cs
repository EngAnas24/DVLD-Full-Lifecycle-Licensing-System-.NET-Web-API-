using Business_Layer.Services;
using BusinessLayer;
using BusinessLayer.Services;
using DAL.DataAccessLayer.Common;
using DVDL.Domain.Entities;
using Entites;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using static Business_Layer.Services.DetainedLicenseService;

namespace DVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetainedLicenseController(
         DetainedLicenseService DetainedLicenseService,
        IWebHostEnvironment _webHostEnvironment) : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment = _webHostEnvironment;

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllDetainedLicenses()
        {
            var DetainedLicenses = DetainedLicenseService.GetAllDetainedLicenses();
            return (DetainedLicenses == null || DetainedLicenses.Count == 0)
                ? NotFound("No  DetainedLicense found.")
                : Ok(DetainedLicenses);
        }
        [HttpGet("{LicenseID}", Name = "GetDetainedLicenseByLicenseID")]
        public IActionResult GetDetainedLicenseByLicenseID(int LicenseID)
        {
            if (LicenseID <= 0)
                return BadRequest("معرف الرخصة غير صالح.");

            var DetainedLicense = DetainedLicenseService.GetDetainedLicenseByID(LicenseID);
            return DetainedLicense == null ? NotFound("السجل غير موجودة.") : Ok(DetainedLicense);
        }


        [HttpGet("ByPersonID/{PersonID}", Name = "GetDetainedLicenseByPersonID")]
        public IActionResult GetDetainedLicenseByPersonID(int PersonID)
        {
            if (PersonID <= 0)
                return BadRequest("معرف الشخص غير صالح.");

            var DetainedLicense = DetainedLicenseService.GetDetainedLicenseByPersonID(PersonID);
            return DetainedLicense == null ? NotFound("الرخصة غير موجودة.") : Ok(DetainedLicense);
        }
        [HttpPost("AddDetainedLicense")]
        public IActionResult InsertDetainedLicense([FromBody] DetainedLicense DetainedLicense)
        {
            try
            {
                if (DetainedLicense == null)
                    return BadRequest("بيانات الرخصة مطلوبة.");

                var insertedId = DetainedLicenseService.InsertDetainedLicense(DetainedLicense);

                if (insertedId <= 0)
                    return BadRequest("فشل في إضافة الرخصة.");

                DetainedLicense.LicenseID = insertedId;

                return CreatedAtRoute(
                "GetDetainedLicenseByDetainedLicenseID", // الاسم هنا
                new { DetainedLicenseID = insertedId },
                DetainedLicense
            );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي: {ex.Message}");
            }
        }

        [HttpPut("UpdateDetainedLicense/{DetainedLicenseID}")]
        public IActionResult UpdateDetainedLicense(int DetainedLicenseID, [FromBody] DetainedLicense DetainedLicense)
        {
            if (DetainedLicenseID <= 0 || DetainedLicenseID != DetainedLicense.ID)
                return BadRequest("المعرف غير متطابق.");

            try
            {
                var result = DetainedLicenseService.UpdateDetainedLicense(DetainedLicense);
                return result > 0 ? Ok(DetainedLicense) : NotFound("الشخص غير موجود.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteDetainedLicense/{DetainedLicenseID}")]
        public IActionResult DeleteDetainedLicense(int DetainedLicenseID)
        {
            if (DetainedLicenseID <= 0) return BadRequest("المعرف غير صالح.");
            var DetainedLicense = DetainedLicenseService.GetDetainedLicenseByID(DetainedLicenseID);
            if (DetainedLicense == null) return NotFound();



            int result = DetainedLicenseService.DeleteDetainedLicense(DetainedLicenseID);

            if (result > 0) return Ok("تم الحذف بنجاح");
            return BadRequest("فشل حذف الشخص من قاعدة البيانات");

        }


        [HttpGet("IsDetained/{LicenseID}")]
        public IActionResult IsLicenseDetained(int LicenseID)
        {
            if (LicenseID <= 0)
                return BadRequest("معرف الرخصة غير صالح.");

            try
            {
                bool isDetained = DetainedLicenseService.CheckIsLicenseDetained(LicenseID);

                return Ok(isDetained);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ داخلي أثناء التحقق: {ex.Message}");
            }
        }
        [HttpPost("Release")]
        public IActionResult ReleaseLicense([FromBody] ReleaseTransactionParams releaseParams)
        {
            if (releaseParams == null || releaseParams.LicenseID <= 0)
                return BadRequest("بيانات فك الاحتجاز غير مكتملة أو غير صالحة.");

            try
            {
                var updatedParams = DetainedLicenseService.ReleaseDetainedLicense(releaseParams);

                if (updatedParams != null)
                {
                    return Ok(updatedParams);
                }
                else
                {
                    return BadRequest("فشلت عملية فك الاحتجاز، يرجى التحقق من حالة الرخصة.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي بالسيرفر أثناء الـ Transaction: {ex.Message}");
            }
        }
        [HttpPost("{licenseId}")]
        public IActionResult FindPersonByLicenseId(int licenseId)
        {
            if (licenseId <= 0)
                return BadRequest(" الرخصة غير صالحة");

            try
            {
                int personId = DetainedLicenseService.FindPersonByLicenseId(licenseId);

                if (personId > 0)
                {
                    return Ok(personId);
                }
                else
                {
                    return BadRequest("لم يتم العثور على الشخص.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي بالسيرفر أثناء الـ Transaction: {ex.Message}");
            }
        }
    }
}