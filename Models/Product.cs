using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Models
{
  public class Product
  {
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "名稱")]
    public string Title { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "描述")]
    public string Description { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "編號")]
    public string ISBN { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "建立者")]
    public string Author { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Range(1, 1000)]
    [Display(Name = "價格")]
    public int ListPrice { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Range(1, 1000)]
    [Display(Name = "價格 1-50")]
    public int Price { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Range(1, 1000)]
    [Display(Name = "價格 50+")]
    public int Price50 { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "價格 100+")]
    [Range(1, 1000)]
    public int Price100 { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "商品類別")]
    public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }

    [Display(Name = "圖片")]
    [DataType(DataType.ImageUrl)]
    [RegularExpression(@".*\.(jpg|jpeg|png|gif|bmp)$", ErrorMessage = "僅允許上傳圖片格式 (jpg, jpeg, png, gif, bmp)。")]
    [NotMapped]
    public List<IFormFile>? Images { get; set; }

    //[MaxLength(100)]
    //[Display(Name = "圖片連結")]
    //public string? ImageUrl { get; set; }
    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
  }
}
