using DAL.DataAccessLayer.Common;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
namespace DAL.DataAccessLayer.Extensions
{
    public static class SqlCommandExtensions
    {

        public static void AddParamsFromObject(this SqlCommand cmd, object values, bool isInsert = false)
        {
            if (values == null) return;
            var type = values.GetType();
            var pkProp = FindPrimaryKeyProperty(type);
            var pkName = pkProp?.Name;

            foreach (PropertyInfo prop in type.GetProperties())
            {
                // 🚨 التعديل الجوهري: تجاهل الـ PK دائماً في هذه الدالة 
                // لأننا نضيفه يدوياً باسم @ID في الـ SqlHelper
                if (prop.Name.Equals(pkName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                object val = prop.GetValue(values) ?? DBNull.Value;
                cmd.Parameters.Add(new SqlParameter("@" + prop.Name, val));
            }
        }

        public static PropertyInfo? FindPrimaryKeyProperty(Type type)
        {
            // 1️⃣ [Key] Attribute (الأدق)
            var pk = type.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null);

            if (pk != null)
                return pk;

            // 2️⃣ Property اسمها "Id"
            pk = type.GetProperty("Id",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (pk != null)
                return pk;

            // 3️⃣ أي Property ينتهي بـ "Id"
            pk = type.GetProperties()
                .FirstOrDefault(p =>
                    p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));

            return pk;
        }

    }
}
