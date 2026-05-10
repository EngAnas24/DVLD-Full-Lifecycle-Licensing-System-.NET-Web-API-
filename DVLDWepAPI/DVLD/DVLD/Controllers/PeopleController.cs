using BusinessLayer;
using BusinessLayer.Services;
using DAL.DataAccessLayer.Common;
using DVDL.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // استخدام Primary Constructor بشكل صحيح
    public class PeopleController(
        PeopleService peopleService,
        IWebHostEnvironment _webHostEnvironment) : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment = _webHostEnvironment;

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllPeople()
        {
            var peoples = peopleService.GetAllPeople();
            return (peoples == null || peoples.Count == 0)
                ? NotFound("No people found.")
                : Ok(peoples);
        }

        [HttpGet("{id}", Name = "GetPersonById")] // أضفنا اسماً للـ Route هنا
        public IActionResult GetPeopleById(int id)
        {
            if (id <= 0) return BadRequest("معرف الشخص غير صالح.");

            var person = peopleService.GetPersonById(id);
            return person == null ? NotFound($"الشخص غير موجود.") : Ok(person);
        }

        [HttpPost("AddPerson")]
        public IActionResult InsertPeople([FromBody] People people)
        {
            try
            {
                if (people == null) return BadRequest("بيانات الشخص مطلوبة.");

                var insertedId = peopleService.InsertPeople(people);
                if (insertedId > 0)
                {
                    // الآن سيعمل التوجيه بشكل صحيح لميثود GetById
                    return CreatedAtRoute("GetPersonById", new { id = insertedId }, people);
                }
                return BadRequest("فشل في إضافة الشخص.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ داخلي: {ex.Message}");
            }
        }

        [HttpPut("UpdatePerson/{id}")]
        public IActionResult UpdatePerson(int id, [FromBody] People people)
        {
            if (id <= 0 || id != people.PersonalID)
                return BadRequest("المعرف غير متطابق.");

            try
            {
                var result = peopleService.UpdatePeople(people);
                return result > 0 ? Ok(people) : NotFound("الشخص غير موجود.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeletePerson/{id}")]
        public IActionResult DeletePerson(int id)
        {
            if (id <= 0) return BadRequest("المعرف غير صالح.");
            var person = peopleService.GetPersonById(id);
            if (person == null) return NotFound();


            // 2. حذف ملف الصورة من المجلد إذا كان موجوداً
            if (!string.IsNullOrEmpty(person.ImagePath))
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/people", person.ImagePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            int result = peopleService.DeletePeople(id);

            if (result > 0) return Ok("تم الحذف بنجاح");
            return BadRequest("فشل حذف الشخص من قاعدة البيانات");

        }

        [HttpGet("allCountries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllCountries()
        {
            var countries = peopleService.GetAllCountries();
            return (countries == null || countries.Count == 0)
                ? NotFound("No people found.")
                : Ok(countries);
        }



        [HttpPost("upload-image/{id}")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("لم يتم اختيار صورة.");

            var person = peopleService.GetPersonById(id);
            if (person == null) return NotFound("الشخص غير موجود");

            var oldFileName = person.ImagePath;
            var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "people");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // إضافة GUID بسيط لمنع مشاكل الكاش وتكرار الأسماء
            var fileName = $"{id}_{Guid.NewGuid().ToString().Substring(0, 5)}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // حذف الصورة القديمة بشكل صحيح
            if (!string.IsNullOrEmpty(oldFileName))
            {
                oldFileName = Path.GetFileName(oldFileName);
                var oldFullFilePath = Path.Combine(uploadsFolder, oldFileName);
                if (System.IO.File.Exists(oldFullFilePath))
                {
                    try { System.IO.File.Delete(oldFullFilePath); }
                    catch { /* سجل الخطأ هنا إذا فشل الحذف */ }
                }
            }

            // حفظ الملف الجديد
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // تحديث قاعدة البيانات
            var updateData = new { PersonalID = id, person.NationalNo, ImagePath = fileName };
            int result = peopleService.UpdateImage(updateData);

            if (result > 0) return Ok(new { fileName });
            return BadRequest("فشل تحديث المسار في قاعدة البيانات");
        }
        [HttpDelete("delete-image/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            // 1. جلب بيانات الشخص للتأكد من وجوده ومعرفة اسم الصورة
            var person = peopleService.GetPersonById(id);
            if (person == null) return NotFound("الشخص غير موجود");

            if (string.IsNullOrEmpty(person.ImagePath))
                return BadRequest("هذا الشخص ليس لديه صورة لحذفها.");

            // 2. حذف الملف الفعلي من السيرفر
            var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "people");
            var fileName = Path.GetFileName(person.ImagePath);
            var fullFilePath = Path.Combine(uploadsFolder, fileName);

            if (System.IO.File.Exists(fullFilePath))
            {
                try
                {
                    System.IO.File.Delete(fullFilePath);
                }
                catch (IOException ex)
                {
                    return StatusCode(500, $"خطأ في حذف الملف من السيرفر: {ex.Message}");
                }
            }

            // 3. الخطوة المفقودة: تحديث قاعدة البيانات
            // نمرر الـ id للخدمة التي تستدعي الـ SP_DeletePersonImage
            int result = peopleService.DeleteImage(id);

            if (result > 0)
                return Ok(new { message = "تم حذف الصورة من السيرفر وتحديث قاعدة البيانات بنجاح." });

            return BadRequest("تم حذف الملف ولكن فشل تحديث سجل قاعدة البيانات.");
        }

    }
}