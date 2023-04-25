using System.Collections.Generic;

namespace APIProj.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string Role { get; set; }
        public string SurName { get; set; }
        public string GiveName { get; set; }

        public List<ImageModel> ImageModels { get; set; }
    }
}
