using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bookstore_Management.Core.Utils
{
    internal static class FileHelper
    {
        public static string Combine(params string[] parts)
        {
            return System.IO.Path.Combine(parts);
        }

        public static void EnsureDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }
    }
}
