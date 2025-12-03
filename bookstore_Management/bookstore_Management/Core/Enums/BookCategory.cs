using System.ComponentModel.DataAnnotations;

namespace bookstore_Management.Core.Enums
{
    public enum BookCategory
    {
        [Display(Name = "Sách thiếu nhi")]
        Children = 1,
        
        [Display(Name = "Văn học")]
        Literature = 2,
        
        [Display(Name = "Tâm lý")]
        Psychology = 3,
        
        [Display(Name = "Kinh tế")]
        Economics = 4,
        
        [Display(Name = "Sách giáo khoa")]
        Textbook = 5,
        
        [Display(Name = "Tiểu sử")]
        Biography = 6,
        
        [Display(Name = "Sách ngoại ngữ")]
        ForeignLanguage = 7,
        
        [Display(Name = "Tiểu thuyết")]
        Novel = 8
    }
}