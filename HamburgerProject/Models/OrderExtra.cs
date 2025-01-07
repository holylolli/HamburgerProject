using System.ComponentModel.DataAnnotations;

namespace HamburgerProject.Models
{
    public class OrderExtra
    {
        public int OrderId { get; set; }
        public int ExtraId { get; set; } 

        public int Quantity { get; set; }

        public Order Order { get; set; }
        public Extra Extra { get; set; }
    }
}
