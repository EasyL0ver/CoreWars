using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreWars.Common;

namespace CoreWars.Data.Entities
{
    [Table("languages")]
    public class Language 
    {
        [Key]
        public string Name { get; set; }
        
        public ICollection<Script> Scripts { get; set; }
    }
}