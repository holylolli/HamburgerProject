using System.ComponentModel.DataAnnotations;

namespace HamburgerProject.Models
{
    public class Extra
    {
        public int Id { get; set; }
        //[Required]
        //[MaxLength(50,ErrorMessage ="Must be 50 Characters!")]
        public string Name { get; set; }
        //[Required]
        public decimal Price { get; set; }
        public ICollection<OrderExtra> OrderExtras { get; set; }


    }
}
