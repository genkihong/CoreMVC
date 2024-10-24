using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models
{
  public class Category
  {
    [Key]
    public int Id { get; set; }

    [DisplayName("類別名稱")]
    [StringLength(50)]
    [Required(ErrorMessage = "{0}必填")]
    public string Name { get; set; }

    [DisplayName("訂購數量")]
    [Required(ErrorMessage = "{0}必填")]
    [Range(1, 100, ErrorMessage = "必須介於{0}至{1}")]
    public int DisplayOrder { get; set; }
   
    [Display(Name = "建立日期")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    //public ICollection<Product> Products { get; set; } = new List<Product>();
  }
}
