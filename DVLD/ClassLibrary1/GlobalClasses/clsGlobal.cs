using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DVLDServices.Services.UserService;

namespace DVLDServices.GlobalClasses
{
    public static class clsGlobal
    {
        public static UserDto GetUser;
       
        public static void RememberUserNameAndPassword(string UserName, string Password, bool IsActive)
        {
            string FileName = "data.txt";
            string UsersdataFolder = "Usersdata";
            string FolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UsersdataFolder);
            string fullPath = Path.Combine(FolderPath, FileName);

            try
            {
                if (IsActive)
                {
                    // تصحيح: نستخدم Directory.Exists للمجلدات
                    if (!Directory.Exists(FolderPath))
                    {
                        Directory.CreateDirectory(FolderPath);
                    }

                    // تصحيح: استخدام using لضمان كتابة البيانات وإغلاق الملف فوراً
                    using (StreamWriter writer = new StreamWriter(fullPath, false))
                    {
                        writer.WriteLine($"{UserName}#{Password}");
                    }
                }
                else
                {
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في الوصول للمسار: {ex.Message}");
            }
        }

        public static bool GetStoredUserData()
        {
            string FileName = "data.txt";
            string UsersdataFolder = "Usersdata";
            string FolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UsersdataFolder);
            string fullPath = Path.Combine(FolderPath, FileName);

            try
            {
                if (!File.Exists(fullPath))
                    return false;

                using (StreamReader reader = new StreamReader(fullPath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] Userdata = line.Split('#');

                        if (Userdata.Length == 2) // التأكد أن السطر يحتوي على جزئين
                        {
                            if (GetUser == null) GetUser = new UserDto();

                            GetUser.UserName = Userdata[0];
                            GetUser.Password = Userdata[1];
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ أثناء قراءة البيانات: {ex.Message}");
            }
        }
    }
}
