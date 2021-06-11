using System;

namespace CoreWars.WebApp.Model
{
    public class Competitor
    {
        public Guid Id { get; set; }
        public string Alias { get; set; }
        public string Code { get; set; }
        public string Language { get; set; }
        public string Competition { get; set; }
        public string Exception { get; set; }
    }
}