using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreWars.Data.Entities
{
    [Table("failures")]
    public class ScriptFailure
    {
        [Key]
        [ForeignKey(nameof(Script))]
        public Guid ScriptId { get; set; }
        
        public DateTime FailureDateTime { get; set; }
        public string Exception { get; set; }
        
        public virtual Script Script { get; set; }
    }
}