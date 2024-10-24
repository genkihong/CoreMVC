using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModel
{
  public class OrderViewModel
  {
    public Order Order { get; set; }
    public IEnumerable<OrderDetail> OrderDetailList { get; set; }
  }
}
