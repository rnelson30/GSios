using System;
namespace GSios.Models
{
    public class RegistrationModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public long Latitude { get; set; }
        public long Longitude { get; set; }
        public long PhoneNumber { get; set; }
    }
}
