using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;

namespace DVLDServices.Commons
{
    public static class clsFormHelper
    {
        public static async Task<bool> ExecuteSafeDeleteAsync(Func<Task<HttpResponseMessage>> deleteAction, string entityName)
        {
            try
            {
                var response = await deleteAction();

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"تم حذف {entityName} بنجاح.", "نجاح العملية", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    string rawTableName = ExtractDependentTableDirectly(jsonString);

                    string friendlyTableName = MapTechnicalTableToArabic(rawTableName);

                    MessageBox.Show(
                        $"عذراً، لا يمكن حذف ({entityName}) حالياً.\n\n" +
                        $"السبب: هذا السجل مرتبط ببيانات حيوية في شاشة [{friendlyTableName}].\n\n" +
                        $"الإجراء المطلوب: يجب عليك الذهاب إلى شاشة ({friendlyTableName}) وحذف السجلات المرتبطة به هناك أولاً، ثم المحاولة مجدداً.",
                        "فشل الحذف - تضارب بيانات",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return false;
                }

                var error = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"خطأ من السيرفر: {error}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء الاتصال بالسيرفر: {ex.Message}", "خطأ نظام", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private static string ExtractDependentTableDirectly(string json)
        {
            try
            {
                if (string.IsNullOrEmpty(json)) return "جداول أخرى";

                string key = "\"dependentTable\":\"";
                int startIndex = json.IndexOf(key);
                if (startIndex != -1)
                {
                    startIndex += key.Length;
                    int endIndex = json.IndexOf("\"", startIndex);
                    if (endIndex != -1)
                    {
                        return json.Substring(startIndex, endIndex - startIndex);
                    }
                }
            }
            catch { }
            return "جداول أخرى";
        }

        private static string MapTechnicalTableToArabic(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return "جداول أخرى";

            switch (tableName.ToLower())
            {
                case "drivers": return "السائقين";
                case "users": return "المستخدمين";
                case "licenses": return "الرخص الصادرة";
                case "detainedlicenses": return "الرخص المحتجزة";
                case "people": return "الأشخاص";
                default: return tableName;
            }
        }
    }
}
