using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
  public class ShoppingCart
  {
    [Key]
    public int Id { get; set; }

    [Range(1, 1000, ErrorMessage = "數量必須在{0}到{1}之間")]
    [Display(Name = "數量")]
    public int Quantity { get; set; }    

    [Display(Name = "商品編號")]
    public int ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    [Display(Name = "商品")]
    public Product Product { get; set; }

    [Display(Name = "用戶編號")]
    public string ApplicationUserId { get; set; }

    [ForeignKey(nameof(ApplicationUserId))]
    [Display(Name = "用戶")]
    public ApplicationUser? ApplicationUser { get; set; }

    [NotMapped]
    [Display(Name = "金額")]
    public int Price { get; set; }

    [Display(Name = "小計")]
    public int SubTotal => Quantity * Price;//Expression-bodied members
  }
}
