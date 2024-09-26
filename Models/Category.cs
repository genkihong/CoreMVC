using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        //[Display(Name = "名稱")]
        [DisplayName("類型名稱")]
        [StringLength(50)]
        [Required(ErrorMessage = "此欄位必填")]
        public string Name { get; set; }

        [DisplayName("訂購數量")]
        [Required(ErrorMessage = "此欄位必填")]
        [Range(1, 100, ErrorMessage = "必須介於1至100")]
        public int DisplayOrder { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
