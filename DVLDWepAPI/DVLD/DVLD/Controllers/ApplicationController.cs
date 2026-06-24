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
    public class ApplicationController(
        ApplicationService ApplicationService,
        IWebHostEnvironment _webHostEnvironment) : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment = _webHostEnvironment;

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllApplication()
        {
            var Applications = ApplicationService.GetAllApplication();
            return (Applications == null || Applications.Count == 0)
                ? NotFound("No Application found.")
                : Ok(Applications);
        }

        [HttpGet("{id}", Name = "GetApplicationById")] 
        public IActionResult GetApplicationById(int id)
        {
            if (id <= 0) return BadRequest("معرف التقديم غير صالح.");

            var Application = ApplicationService.GetApplicationById(id);
            return Application == null ? NotFound($"التقديم غير موجود.") : Ok(Application);
        }


        [HttpGet("ChangeStatus/{id}")] 
        public IActionResult ChangeStatusToCompleted(int id)
        {
            if (id <= 0) return BadRequest("معرف التقديم غير صالح.");

            var Application = ApplicationService.ChangeStatusToCompleted(id);
            return Application <= 0 ? NotFound($"التقديم غير موجود.") : Ok("تم التحديث حالة التطبيق إلى Compeleted");
        }

        [HttpPost("AddApplication")]
        public IActionResult InsertApplication([FromBody] Application Application)
        {
            try
            {
                if (Application == null) return BadRequest("بيانات التقديم مطلوبة.");

                var insertedId = ApplicationService.InsertApplication(Application);
                if (insertedId > 0)
                {
                    // الآن سيعمل التوجيه بشكل صحيح لميثود GetById
                    return CreatedAtRoute("GetApplicationById", new { id = insertedId }, Application);
                }
                return BadRequest("فشل في إضافة التقديم.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي: {ex.Message}");
            }
        }

        [HttpPut("UpdateApplication/{id}")]
        public IActionResult UpdateApplication(int id, [FromBody] Application Application)
        {
            if (id <= 0 || id != Application.ApplicationID)
                return BadRequest("المعرف غير متطابق.");

            try
            {
                var result = ApplicationService.UpdateApplication(Application);
                return result > 0 ? Ok(Application) : NotFound("التقديم غير موجود.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteApplication/{id}")]
        public IActionResult DeleteApplication(int id)
        {
            try
            {
            if (id <= 0) return BadRequest("المعرف غير صالح.");
            var application = ApplicationService.GetApplicationById(id);
            if (application == null) return NotFound();


         
            int result = ApplicationService.DeleteApplication(id);

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

      
    }
}