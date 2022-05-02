using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyCoreMVC_20220327.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        //[Display(Name = "名稱")]
        [DisplayName("名稱")]
        [Required(ErrorMessage = "此欄位必填")]
        public string Name { get; set; }
        [DisplayName("數量")]
        [Required(ErrorMessage = "此欄位必填")]
        public int DisplayOrder { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
