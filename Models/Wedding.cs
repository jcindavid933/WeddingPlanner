using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace weddingplanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get;set;}
        [Required]
        public int UserId{get;set;}
        [Required]
        [MinLength(2)]
        public string Wedder_One {get;set;}
        [Required]
        [MinLength(2)]
        public string Wedder_Two {get;set;}
        [Required]
        [MinLength(5)]
        public string Wedding_Address {get;set;}
        [Required]
        public DateTime created_at {get;set;}
        public List<Guest> Guest {get;set;}

    }
}