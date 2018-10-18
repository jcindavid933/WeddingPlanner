using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace weddingplanner.Models
{
    public class Guest
    {
        [Key]
        public int GuestId {get;set;}
        public int UserId {get;set;}
        [ForeignKey("UserId")]
        public User InvitedUser {get;set;}   
        public int WeddingId {get;set;}
        [ForeignKey("WeddingId")]
        public Wedding Wedding {get;set;}     

    }
}