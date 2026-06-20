using BusinessLayer.Services;
using Entites;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserService userService) : ControllerBase
    {
        private readonly UserService userService = userService;

        [HttpGet("GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public IActionResult GetAllUsers()
        {
            var Users = userService.GetAllUsers();
            if (Users == null || Users.Count == 0)
            {
                return NotFound("No User found.");
            }

            return Ok(Users);
        }

        [HttpPost("AddUser")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult InsertUser([FromBody] User user) 
        {
            try
            {
                if (user == null) return BadRequest("بيانات المستخدم مطلوبة.");

                user.ID = 0;

                var insertedId = userService.InsertUser(user);

                if (insertedId > 0)
                {
                    user.ID = insertedId;
                    return Ok(user);
                }

                return BadRequest("فشل الحفظ: تأكد من أن الـ PersonalID موجود وصحيح، وأن اسم المستخدم غير مكرر.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ تقني: {ex.Message}");
            }
        }

        [HttpPut("UpdateUser/{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateUser(int id, [FromBody] User User)
        {
            if (id <= 0 || id != User.ID)
            {
                return BadRequest("معرف الشخص غير صالح أو غير متطابق.");
            }

            try
            {
                var resultId = userService.UpdateUser(User);

                if (resultId > 0)
                {
                    return Ok(User); 
                }
                else
                {
                    return NotFound($"الشخص ذو الرقم {id} غير موجود.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteUser/{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("معرف الشخص غير صالح.");
            }
            try
            {
                var resultId = userService.DeleteUser(id);
                if (resultId > 0)
                {
                    return Ok($"تم حذف الشخص ذو الرقم {id} بنجاح.");
                }
                else
                {
                    return NotFound($"الشخص ذو الرقم {id} غير موجود.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetUserById/{id}")]
        public IActionResult GetUserById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("معرف الشخص غير صالح.");
            }
            var User = userService.GetUserById(id);
            if (User == null)
            {
                return NotFound($"الشخص ذو الرقم {id} غير موجود.");
            }
            return Ok(User);
        }


        [HttpPost("Login")]
        public IActionResult GetUserByNameAndPassword([FromForm] string UserName, [FromForm] string Password)
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                return BadRequest("اسم المستخدم وكلمة المرور مطلوبان.");
            }

            try
            {
                var user = UserService.GetUserByUsernameAndPassword(UserName, Password);

                if (user == null)
                {
                    return NotFound("اسم المستخدم أو كلمة المرور غير صحيحة.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "حدث خطأ داخلي في الخادم.");
            }
        }

        [HttpPost("ChangePassword/{userId}")]
        public IActionResult ChangePassword(int userId, [FromForm] string OldPassword, [FromForm] string NewPassword)
        {
            try
            {
              int ID =   userService.ChangePassword(userId, OldPassword, NewPassword);
               
              if(ID > 0)
                return Ok(new { Message = "تم تغيير كلمة السر بنجاح" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }


    }
}
