using DVDL.Domain.Entities;
using DAL.DataAccessLayer.Common;
using DataAccessLayer.Dtos;
using Entites;
namespace BusinessLayer.Services
{
    public class UserService
    {
        public List<UserDto> GetAllUsers()
        {
            return SqlHelper.ExecuteQueryAll<UserDto>("SPGetAllUsers");
        }
        // في الميثود التي تستدعي الـ SqlHelper
        public UserDto GetUserById(int id)
        {
            // هنا نعرف المتغير الذي سيستقبل الـ ID من البارامتر الخارج
            int returnedId;

            var person = SqlHelper.ExecuteeQuerySingl<UserDto>(
                "SPGetUserById",
                new { ID = id }, // تأكد أن الاسم يطابق @ID في الـ Proc
                out returnedId
            );

            return person;
        }
        public int InsertUser(User User)
        {
            return SqlHelper.ExecuteNonQuery("SPInsertUpdateUser", User, true);
        }
        public int UpdateUser(User User)
        {
            return SqlHelper.ExecuteNonQuery("SPInsertUpdateUser", User, isInsert: false);
        }
        public int DeleteUser(int id)
        {
            return SqlHelper.ExecuteNonQuery("SPDeleteUser", new { PersonalID = id });
        }

        public static UserDto GetUserByUsernameAndPassword(string userName, string password)
        {
            int returnedId;


            var user = SqlHelper.ExecuteeQuerySingl<UserDto>(
                "SPGetUserByUsernameAndPassword",
                new { UserName = userName, Password = password, ID = 0 },
                out returnedId
            );

            return user;
        }

        public  int ChangePassword(int userId, string currentPassword, string newPassword)
        {
            int returnedId;

            if (currentPassword == newPassword)
                throw new Exception("يجب كتابة كلمة سر جديد غير كلمة السر الحالية");

            var result = SqlHelper.ExecuteeQuerySingl<UserDto>(
                "SPChangePassword",
                new { ID = 0, UserID = userId, CurrentPassword = currentPassword, NewPassword = newPassword },
                out returnedId
            );
            return returnedId;
        }


    }
}
