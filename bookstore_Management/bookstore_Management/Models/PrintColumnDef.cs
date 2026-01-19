using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bookstore_Management.Models
{
    public class PrintColumnDef
    {       
        public string Header { get; set; }       // Tên hiển thị trên đầu bảng (VD: "Họ tên")
        public string PropertyPath { get; set; } // Tên biến trong Code (VD: "Name")
        public double WidthStar { get; set; } = 1; // Độ rộng tỉ lệ (1*, 2*...)
        
    }
}

