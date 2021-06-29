using System;
using System.ComponentModel.DataAnnotations;

namespace MouseCoordinatesListener.Models
{
    public class Event
    {
        public int Id { get; set; }
        
        [MaxLength(32)]
        public string EventType { get; set; }
        
        [MaxLength(10)]
        public string Coords { get; set; }
        
        public DateTime CreatedOn { get; set; }
    }
}