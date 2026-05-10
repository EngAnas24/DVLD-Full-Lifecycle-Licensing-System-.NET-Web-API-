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
    public class TestTypeController(
        TestTypeService TestTypeService,
        IWebHostEnvironment _webHostEnvironment) : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment = _webHostEnvironment;

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllTestType()
        {
            var TestTypes = TestTypeService.GetAllTestType();
            return (TestTypes == null || TestTypes.Count == 0)
                ? NotFound("No TestType found.")
                : Ok(TestTypes);
        }

        [HttpGet("{id}", Name = "GetTestTypeById")] 
        public IActionResult GetTestTypeById(int id)
        {
            if (id <= 0) return BadRequest("معرف نوع الاختبار غير صالح.");

            var TestType = TestTypeService.GetTestTypeById(id);
            return TestType == null ? NotFound($"نوع الاختبار غير موجود.") : Ok(TestType);
        }

        [HttpPost("AddTestType")]
        public IActionResult InsertTestType([FromBody] TestType TestType)
        {
            try
            {
                if (TestType == null) return BadRequest("بيانات نوع الاختبار مطلوبة.");

                var insertedId = TestTypeService.InsertTestType(TestType);
                if (insertedId > 0)
                {
                    return CreatedAtRoute("GetTestTypeById", new { id = insertedId }, TestType);
                }
                return BadRequest("فشل في إضافة نوع الاختبار.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي: {ex.Message}");
            }
        }

        [HttpPut("UpdateTestType/{id}")]
        public IActionResult UpdateTestType(int id, [FromBody] TestType TestType)
        {
            if (id <= 0 || id != TestType.TestTypeID)
                return BadRequest("المعرف غير متطابق.");

            try
            {
                var result = TestTypeService.UpdateTestType(TestType);
                return result > 0 ? Ok(TestType) : NotFound("نوع الاختبار غير موجود.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}