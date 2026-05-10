using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace DAL.DataAccessLayer.Mapping
{
    public static class MappingHelper
    {
        public static T ToDto<T>(SqlDataReader reader) where T : new()
        {
            T obj = new T();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                // الحصول على اسم العمود من القارئ
                string columnName = reader.GetName(i);
                var prop = typeof(T).GetProperty(columnName);

                if (prop != null && !reader.IsDBNull(i))
                {
                    object value = reader.GetValue(i);
                    Type propType = prop.PropertyType;

                    try
                    {
                        // التحقق مما إذا كان النوع يحتاج تحويل (مثلاً من Int في DB إلى String في DTO)
                        if (value.GetType() != propType)
                        {
                            // التعامل مع أنواع Nullable
                            Type targetType = Nullable.GetUnderlyingType(propType) ?? propType;
                            value = Convert.ChangeType(value, targetType);
                        }

                        prop.SetValue(obj, value);
                    }
                    catch
                    {
                        // في حال فشل التحويل تماماً، يمكنك تركها فارغة أو تسجيل خطأ
                        continue;
                    }
                }
            }
            return obj;
        }
    }
}
