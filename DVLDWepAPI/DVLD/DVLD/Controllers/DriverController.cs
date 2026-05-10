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
    public class DriverController(
        DriverService DriverService,
        IWebHostEnvironment _webHostEnvironment) : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment = _webHostEnvironment;

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllDrivers()
        {
            var Drivers = DriverService.GetAllDriver();
            return (Drivers == null || Drivers.Count == 0)
                ? NotFound("No Driver found.")
                : Ok(Drivers);
        }
        [HttpGet("ByDriverID/{DriverID}", Name = "GetDriverByDriverID")]
        public IActionResult GetDriverByDriverID(int DriverID)
        {
            if (DriverID <= 0)
                return BadRequest("معرف السائق غير صالح.");

            var driver = DriverService.GetDriverByDriverID(DriverID);
            return driver == null ? NotFound("السائق غير موجود.") : Ok(driver);
        }


        [HttpGet("ByPersonID/{PersonID}", Name = "GetDriverByPersonID")]
        public IActionResult GetDriverByPersonID(int PersonID)
        {
            if (PersonID <= 0)
                return BadRequest("معرف الشخص غير صالح.");

            var driver = DriverService.GetDriverByPersonID(PersonID);
            return driver == null ? NotFound("السائق غير موجود.") : Ok(driver);
        }
        [HttpPost("AddDriver")]
        public IActionResult InsertDriver([FromBody] Driver driver)
        {
            try
            {
                if (driver == null)
                    return BadRequest("بيانات السائق مطلوبة.");

                var insertedId = DriverService.InsertDriver(driver);

                if (insertedId <= 0)
                    return BadRequest("فشل في إضافة السائق.");

                driver.DriverID = insertedId;

                return CreatedAtRoute(
                    "GetDriverByDriverID",
                    new { DriverID = insertedId },
                    driver
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي: {ex.Message}");
            }
        }

        [HttpPut("UpdateDriver/{DriverID}")]
        public IActionResult UpdateDriver(int DriverID, [FromBody] Driver Driver)
        {
            if (DriverID <= 0 || DriverID != Driver.DriverID)
                return BadRequest("المعرف غير متطابق.");

            try
            {
                var result = DriverService.UpdateDriver(Driver);
                return result > 0 ? Ok(Driver) : NotFound("الشخص غير موجود.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteDriver/{DriverID}")]
        public IActionResult DeleteDriver(int DriverID)
        {
            if (DriverID <= 0) return BadRequest("المعرف غير صالح.");
            var driver = DriverService.GetDriverByDriverID(DriverID);
            if (driver == null) return NotFound();


         
            int result = DriverService.DeleteDriver(DriverID);

            if (result > 0) return Ok("تم الحذف بنجاح");
            return BadRequest("فشل حذف الشخص من قاعدة البيانات");

        }


    }
}