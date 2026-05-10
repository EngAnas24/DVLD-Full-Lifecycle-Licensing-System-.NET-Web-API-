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
            if (id <= 0) return BadRequest("المعرف غير صالح.");
            var LocalDrivingLicenseApplication = LocalDrivingLicenseApplicationService.GetLocalApplicationByIdAsync(id);
            if (LocalDrivingLicenseApplication == null) return NotFound();



            int result = LocalDrivingLicenseApplicationService.DeleteLocalApplication(id);

            if (result > 0) return Ok("تم الحذف بنجاح");
            return BadRequest("فشل حذف التقديم من قاعدة البيانات");

        }



    }
}