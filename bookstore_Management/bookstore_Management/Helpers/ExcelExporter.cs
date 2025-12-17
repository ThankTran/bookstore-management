using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace bookstore_Management.Helpers
{
    internal static class ExcelExporter
    {
        /// <summary>
        /// Xuất dữ liệu thành CSV đơn giản (tương thích Excel), không phụ thuộc thư viện ngoài.
        /// </summary>
        public static void ExportToCsv<T>(IEnumerable<T> items, string filePath)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));

            var list = items.ToList();
            if (!list.Any()) return;

            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // header
                writer.WriteLine(string.Join(",", props.Select(p => Escape(p.Name))));
                // rows
                foreach (var item in list)
                {
                    var values = props.Select(p => Escape(p.GetValue(item)?.ToString() ?? string.Empty));
                    writer.WriteLine(string.Join(",", values));
                }
            }
        }

        private static string Escape(string value)
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }
            return value;
        }
    }
}
