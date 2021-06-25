using System;
using System.ComponentModel.DataAnnotations;

namespace MouseCoordinatesListener.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [MaxLength(32)]
        public string Email { get; set; }
       
        [MaxLength(32)]
        public string Password { get; set; }
        
        [MaxLength(32)]
        public string Skype { get; set; }
        
        [MaxLength(32)]
        public string WhatsApp { get; set; }
        
        [MaxLength(32)]
        public string PhoneNumber { get; set; }
        
        [MaxLength(32)]
        public string Message { get; set; }
        
        [MaxLength(32)]
        public string Name { get; set; }
        
        public int Age { get; set; }
    }
}