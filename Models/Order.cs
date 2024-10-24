using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
  public class Order
  {
    [Key]
    public int Id { get; set; }
    [Display(Name = "用戶編號")]
    public string? ApplicationUserId { get; set; }

    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser? ApplicationUser { get; set; }

    [Display(Name = "建立日期")]
    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Display(Name = "出貨日期")]
    public DateTime ShippingDate { get; set; }

    [Display(Name = "總計")]
    public int OrderTotal { get; set; }

    [Display(Name = "訂單狀態")]
    public string? OrderStatus { get; set; }

    [Display(Name = "付款狀態")]
    public string? PaymentStatus { get; set; }

    [Display(Name = "流水號")]
    public string? TrackingNumber { get; set; }

    [Display(Name = "送貨員")]
    public string? Carrier { get; set; }

    [Display(Name = "付款日期")]
    public DateTime PaymentDate { get; set; }

    [Display(Name = "付款截止日期")]
    public DateTime PaymentDueDate { get; set; }
    
    [Display(Name = "付款編號")]
    public string? PaymentIntentId { get; set; }

    public string? SessionId { get; set; }
    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "姓名")]
    public string Name { get; set; }

    [StringLength(10, ErrorMessage = "{0}長度至少必須為{2}個字元。", MinimumLength = 10)]
    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "手機")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "城市")]
    public string City { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "地區")]
    public string Zone { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "郵遞區號")]
    public string PostalCode { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [Display(Name = "地址")]
    public string Address { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
  }
}
