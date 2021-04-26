using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CoreWars.Data.Entities
{
    public class GameScript
    {
        [Key]
        public Guid Id { get; set; }
        [Required, MinLength(1)]
        public string ScriptType { get; set; }
        [Required, MinLength(1)]
        public string CompetitionType { get; set; }
        [Required]
        public string[] ScriptFiles { get; set; }
    }
}