using System.Collections.Generic;

namespace APIProj.Models
{
    public class ImageModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserModel UserModel { get; set; }

        public string Image { get; set; }
    }
}
