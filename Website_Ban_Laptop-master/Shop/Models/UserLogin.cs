using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Models
{
    [NotMapped]
    public class UserLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
 