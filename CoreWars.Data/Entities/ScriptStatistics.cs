using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreWars.Common;

namespace CoreWars.Data.Entities
{
    [Table("statistics")]
    public class ScriptStatistics : ICompetitorStatistics
    {
        [Key]
        [ForeignKey(nameof(Script))]
        public Guid ScriptId { get; set; }
        
        public int Wins { get; set; }
        public int GamesPlayed { get; set; }
        
        public virtual Script Script { get; set; }
    }
}