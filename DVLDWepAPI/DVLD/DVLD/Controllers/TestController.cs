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
    public class TestController(
        TestService TestService,
        IWebHostEnvironment _webHostEnvironment) : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment = _webHostEnvironment;

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllTest()
        {
            var Tests = TestService.GetAllTest();
            return (Tests == null || Tests.Count == 0)
                ? NotFound("No Test found.")
                : Ok(Tests);
        }

        [HttpGet("{id:int}", Name = "GetTestById")]
        public IActionResult GetTestById(int id)
        {
            if (id <= 0) return BadRequest("معرف الاختبار غير صالح.");

            var test = TestService.GetTestById(id);
            return test == null ? NotFound($"الاختبار غير موجود.") : Ok(test);
        }


        [HttpGet("appointment/{TestAppointmentId:int}", Name = "GetTestByTestAppointmentId")]
        public IActionResult GetTestByTestAppointmentId(int TestAppointmentId)
        {
            if (TestAppointmentId <= 0) return BadRequest("معرف الموعد غير صالح.");

            var test = TestService.GetTestByTestAppointmentId(TestAppointmentId);
            return test == null ? NotFound($"لا يوجد اختبار مرتبط بهذا الموعد.") : Ok(test);
        }


        [HttpGet("GetLastTest/{LicenseClassID}/{ApplicantPersonID}/{TestTypeID}")]
        public IActionResult GetLastTest(int LicenseClassID, int ApplicantPersonID, int TestTypeID)
        {
            // استخدام OR (||) لضمان أن جميع المعرفات صحيحة
            if (LicenseClassID <= 0 || ApplicantPersonID <= 0 || TestTypeID <= 0)
            {
                return BadRequest("بيانات الاستعلام غير كاملة أو غير صالحة.");
            }

            var lastTest = TestService.GetLastTestByPersonAndTestTypeAndLicenseClass(LicenseClassID, TestTypeID, ApplicantPersonID);

            if (lastTest == null)
            {
                return NotFound("لا يوجد سجل اختبار سابق لهذا الشخص في هذه الفئة.");
            }

            return Ok(lastTest);
        }

    

        [HttpGet("GetClassID/{ClassName}")]
        public IActionResult GetClassID(string ClassName)
        {
            if (string.IsNullOrEmpty(ClassName))
            {
                return BadRequest("بيانات الاستعلام غير كاملة أو غير صالحة.");
            }

            var ClassID = TestService.GetClassID(ClassName);

            if (ClassID == null)
            {
                return NotFound("لا يوجد سجل اختبار سابق لهذا الشخص في هذه الفئة.");
            }

            return Ok(ClassID);
        }
        [HttpPost("AddTest")]
        public IActionResult InsertTest([FromBody] Test Test)
        {
            try
            {
                if (Test == null) return BadRequest("بيانات نوع الاختبار مطلوبة.");

                var insertedId = TestService.InsertTest(Test);
                if (insertedId > 0)
                {
                    return CreatedAtRoute("GetTestById", new { id = insertedId }, Test);
                }
                return BadRequest("فشل في إضافة نوع الاختبار.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي: {ex.Message}");
            }
        }

        [HttpPut("UpdateTest/{id}")]
        public IActionResult UpdateTest(int id, [FromBody] Test Test)
        {
            if (id <= 0 || id != Test.TestID)
                return BadRequest("المعرف غير متطابق.");

            try
            {
                var result = TestService.UpdateTest(Test);
                return result > 0 ? Ok(Test) : NotFound("نوع الاختبار غير موجود.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}