using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
  public class Company
  {
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "{0}必填")]
    [MaxLength(100)]
    [Display(Name = "名稱")]
    public string Name { get; set; }

    [MaxLength(100)]
    [Display(Name = "地址")]
    public string? Address { get; set; }

    [MaxLength(10)]
    [Display(Name = "城市")]
    public string? City { get; set; }

    [MaxLength(5)]
    [Display(Name = "地區")]
    public string? Zone { get; set; }

    [MaxLength(3)]
    [Display(Name = "郵遞區號")]
    public string? PostalCode { get; set; }

    [StringLength(10, ErrorMessage = "{0}長度至少必須為{2}個字元。", MinimumLength = 10)]
    [Display(Name = "手機")]
    public string? PhoneNumber { get; set; }

    public ICollection<ApplicationUser> ApplicationUser { get; set; }
  }
}
