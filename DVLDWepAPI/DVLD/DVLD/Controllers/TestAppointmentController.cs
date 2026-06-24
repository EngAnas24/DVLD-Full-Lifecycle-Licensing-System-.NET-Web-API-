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
    public class TestAppointmentController(
        TestAppointmentService TestAppointmentService,
        IWebHostEnvironment _webHostEnvironment) : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment = _webHostEnvironment;

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllTestAppointment()
        {
            var TestAppointments = TestAppointmentService.GetAllTestAppointment();
            return (TestAppointments == null || TestAppointments.Count == 0)
                ? NotFound("No TestAppointment found.")
                : Ok(TestAppointments);
        }

        [HttpGet("{id}", Name = "GetTestAppointmentById")]
        public IActionResult GetTestAppointmentById(int id)
        {
            if (id <= 0) return BadRequest("معرف نوع الاختبار غير صالح.");

            var TestAppointment = TestAppointmentService.GetTestAppointmentById(id);
            return TestAppointment == null ? NotFound($"نوع الاختبار غير موجود.") : Ok(TestAppointment);
        }




        [HttpGet("TestTypeID/{id}", Name = "GetTestAppointmentByTestTypeID")]
        public IActionResult GetTestAppointmentByTestTypeID(int id)
        {
            if (id <= 0) return BadRequest("معرف نوع الاختبار غير صالح.");

            var TestAppointment = TestAppointmentService.GetTestAppointmentByTestTypeID(id);
            return TestAppointment == null ? NotFound($"نوع الاختبار غير موجود.") : Ok(TestAppointment);
        }

        [HttpPost("AddTestAppointment")]
        public IActionResult InsertTestAppointment([FromBody] TestAppointment TestAppointment)
        {
            try
            {
                if (TestAppointment == null) return BadRequest("بيانات نوع الاختبار مطلوبة.");

                var insertedId = TestAppointmentService.InsertTestAppointment(TestAppointment);
                if (insertedId > 0)
                {
                    return CreatedAtRoute("GetTestAppointmentById", new { id = insertedId }, TestAppointment);
                }
                return BadRequest("فشل في إضافة نوع الاختبار.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي: {ex.Message}");
            }
        }

        [HttpPut("UpdateTestAppointment/{id}")]
        public IActionResult UpdateTestAppointment(int id, [FromBody] TestAppointment TestAppointment)
        {
            if (id <= 0 || id != TestAppointment.TestAppointmentID)
                return BadRequest("المعرف غير متطابق.");

            try
            {
                var result = TestAppointmentService.UpdateTestAppointment(TestAppointment);
                return result > 0 ? Ok(TestAppointment) : NotFound("نوع الاختبار غير موجود.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteTestAppointment/{id}")]
        public IActionResult DeleteTestAppointment(int id)
        {
            try
            {

            if (id <= 0) return BadRequest("المعرف غير صالح.");
            var License = TestAppointmentService.GetTestAppointmentById(id);
            if (License == null) return NotFound();



            int result = TestAppointmentService.DeleteTestAppointment(id);

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


        [HttpGet("DoesPassTestType/{localAppID}/{testTypeID}")]
        public IActionResult DoesPassTestType(int localAppID, int testTypeID)
        {
            if (localAppID <= 0 || testTypeID <= 0)
            {
                return BadRequest("الرجاء إدخال معرفات صحيحة.");
            }

            try
            {
                bool hasPassed = TestAppointmentService.CheckIfPassedTest(localAppID, testTypeID);

                // نستخدم أسماء تطابق الكلاس TestResultResult
                return Ok(new
                {
                    TestResult  = hasPassed,
                    returnNum   = hasPassed ? 1 : 0,
                    Message     = hasPassed ? "لقد اجتاز هذا الاختبار بنجاح." : "لم يجتز الاختبار بعد أو لم يتقدم له."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "حدث خطأ أثناء التحقق من نتيجة الاختبار: " + ex.Message);
            }
        }

        [HttpGet("GetTotalTrialsPerTest/{localAppID}/{testTypeID}")]
        public IActionResult GetTotalTrialsPerTest(int localAppID, int testTypeID)
        {
            if (localAppID <= 0 || testTypeID <= 0)
            {
                return BadRequest("الرجاء إدخال معرفات صحيحة.");
            }

            try
            {
                int time = TestAppointmentService.GetTotalTrialsPerTest(localAppID, testTypeID);

                return Ok(new
                {
                    time = time,
                    Message = $"لقد تم تقديم هذا الاختبار {time} مرة"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "حدث خطأ أثناء التحقق من نتيجة الاختبار: " + ex.Message);
            }
        }

        [HttpGet("GetTestAppointmentBy/{localAppID}/{testTypeID}")]
        public IActionResult GetTestAppointmentByLocalAppIdAndTestTypeId (int localAppID, int testTypeID)
        {
            if (localAppID <= 0 || testTypeID <= 0)
            {
                return BadRequest("الرجاء إدخال معرفات صحيحة.");
            }

            try
            {
                var TestAppointment = TestAppointmentService.GetTestAppointmentByLocalAppIdAndTestTypeId(localAppID, testTypeID);
                return Ok(TestAppointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "حدث خطأ أثناء التحقق من موعد الاختبار: " + ex.Message);
            }
        }
        [HttpGet("IsThereAnActiveScheduledTest/{localAppID}/{testTypeID}")]
        public IActionResult IsThereAnActiveScheduledTest(int localAppID, int testTypeID)
        {
            int resultNum = TestAppointmentService.IsThereAnActiveScheduledTest(localAppID, testTypeID);

            return Ok(new { returnNum = resultNum });
        }



        [HttpGet("DoesAttendTestType/{LocalicenseID}/{testTypeID}")]
        public IActionResult DoesAttendTestType(int LocalicenseID, int testTypeID)
        {
            if (LocalicenseID <= 0 || testTypeID <= 0)
            {
                return BadRequest("الرجاء إدخال معرفات صحيحة.");
            }

            try
            {
                bool found = TestAppointmentService.DoesAttendTestType(LocalicenseID, testTypeID);
                if (found)
                {
                    return Ok(found);
                }
                else
                    return NotFound("لم يحضر أي  اختبار  ");

            }
            catch (Exception ex)
            {
                return StatusCode(500, "حدث خطأ أثناء التحقق من الاختبار: " + ex.Message);
            }
        }
    }
}