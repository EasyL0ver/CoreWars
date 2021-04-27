using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using CoreWars.Common;

namespace CoreWars.Data.Entities
{
    [Table("languages")]
    public class Language : IScriptingLanguage
    {
        [Key]
        public string Name { get; set; }
        
        public ICollection<Script> Scripts { get; set; }
    }
}