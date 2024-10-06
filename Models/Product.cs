using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
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
    [Display(Name = "作者")]
    public string Author { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "List Price")]
    [Range(1, 1000)]
    public double ListPrice { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "Price for 1-50")]
    [Range(1, 1000)]
    public double Price { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "Price for 50+")]
    [Range(1, 1000)]
    public double Price50 { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "Price for 100+")]
    [Range(1, 1000)]
    public double Price100 { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "產品類別")]
    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    //[ValidateNever]
    public Category? Category { get; set; }

    [DataType(DataType.ImageUrl)]
    [Display(Name = "圖片連結")]
    public string ImageUrl { get; set; }

    //[ValidateNever]
    //public ICollection<ProductImage> ProductImages { get; set; }
  }
}
