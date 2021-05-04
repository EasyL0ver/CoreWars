using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreWars.Data.Entities
{
    [Table("competitions")]
    public class Competition
    {
        [Key]
        public string Name { get; set; }
        
        public ICollection<Script> Scripts { get; set; }
    }
}