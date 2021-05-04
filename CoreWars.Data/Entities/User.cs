using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreWars.Common;

namespace CoreWars.Data.Entities
{
    [Table("users")]
    public class User : IUser
    {
        [Key]
        public Guid Id { get; set; }
        
        public string EmailAddress { get; set; }
        
        public string Password { get; set; }
        
        public ICollection<Script> Scripts { get; set; }
    }
}