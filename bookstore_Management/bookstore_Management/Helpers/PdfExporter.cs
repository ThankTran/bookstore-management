using System;
using System.IO;
using System.Text;

namespace bookstore_Management.Helpers
{
    /// <summary>
    /// Exporter PDF tối giản: ghi nội dung text vào file .txt (placeholder),
    /// có thể thay bằng thư viện PDF thật nếu được phép bổ sung dependency.
    /// </summary>
    internal static class PdfExporter
    {
        public static void ExportText(string content, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
            File.WriteAllText(filePath, content ?? string.Empty, Encoding.UTF8);
        }
    }
}
