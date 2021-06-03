using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using CoreWars.Common;

namespace CoreWars.Data.Entities
{
    [Table("scripts")]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class Script : IScript
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        [ForeignKey("Language")]
        public string ScriptType { get; set; }
        [ForeignKey("Competition")]
        public string CompetitionName { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        
        //todo create file entity ?
        [Required]
        public string[] ScriptFiles { get; set; }
        
        public DateTime DateTimeCreated { get; set; }
        public DateTime DateTimeUpdated { get; set; }
        
        public virtual User User { get; set; }
        public virtual Competition Competition { get; set; }
        public virtual Language Language { get; set; }
        public virtual ScriptStatistics Stats { get; set; }
        public virtual ScriptFailure FailureInfo { get; set; }
    }
}