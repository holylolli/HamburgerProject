using HamburgerProject.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace HamburgerProject.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }=DateTime.Now;
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal TotalPrice { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int MenuId { get; set; }
        public Menu Menu { get; set; }
        public ICollection<OrderExtra> OrderExtras { get; set; }

    }
}
