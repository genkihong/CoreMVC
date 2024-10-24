using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
  public class ApplicationUser : IdentityUser
  {
    [Required(ErrorMessage = "{0}必填")]
    public string Name { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Zone { get; set; }

    public string? PostalCode { get; set; }

    public int? CompanyId { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Company? Company { get; set; }
  }
}
