using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace weddingplanner.Models
{
    public class DashboardModels
    {
        public List<Wedding> allWeddings {get; set;}
        public User User {get; set;}
        public List<Wedding> JoinedWeddings {get; set;}
        public List<Wedding> NotJoinedWeddings {get; set;}
    }
}