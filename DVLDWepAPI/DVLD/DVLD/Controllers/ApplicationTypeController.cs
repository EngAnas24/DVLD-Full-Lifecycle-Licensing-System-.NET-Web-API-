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
    public class ApplicationTypeController(
        ApplicationTypeService ApplicationTypeService,
        IWebHostEnvironment _webHostEnvironment) : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment = _webHostEnvironment;

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllApplicationType()
        {
            var ApplicationTypes = ApplicationTypeService.GetAllApplicationType();
            return (ApplicationTypes == null || ApplicationTypes.Count == 0)
                ? NotFound("No ApplicationType found.")
                : Ok(ApplicationTypes);
        }

        [HttpGet("{id}", Name = "GetApplicationTypeById")] 
        public IActionResult GetApplicationTypeById(int id)
        {
            if (id <= 0) return BadRequest("معرف نوع التقديم غير صالح.");

            var ApplicationType = ApplicationTypeService.GetApplicationTypeById(id);
            return ApplicationType == null ? NotFound($"نوع التقديم غير موجود.") : Ok(ApplicationType);
        }

        [HttpPost("AddApplicationType")]
        public IActionResult InsertApplicationType([FromBody] ApplicationType ApplicationType)
        {
            try
            {
                if (ApplicationType == null) return BadRequest("بيانات نوع التقديم مطلوبة.");

                var insertedId = ApplicationTypeService.InsertApplicationType(ApplicationType);
                if (insertedId > 0)
                {
                    // الآن سيعمل التوجيه بشكل صحيح لميثود GetById
                    return CreatedAtRoute("GetApplicationTypeById", new { id = insertedId }, ApplicationType);
                }
                return BadRequest("فشل في إضافة نوع التقديم.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي: {ex.Message}");
            }
        }

        [HttpPut("UpdateApplicationType/{id}")]
        public IActionResult UpdateApplicationType(int id, [FromBody] ApplicationType ApplicationType)
        {
            if (id <= 0 || id != ApplicationType.ApplicationTypeID)
                return BadRequest("المعرف غير متطابق.");

            try
            {
                var result = ApplicationTypeService.UpdateApplicationType(ApplicationType);
                return result > 0 ? Ok(ApplicationType) : NotFound("نوع التقديم غير موجود.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}