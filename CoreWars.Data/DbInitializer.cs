using System.Collections.Generic;
using System.Linq;
using CoreWars.Common;
using CoreWars.Data.Entities;

namespace CoreWars.Data
{
    public static class DbInitializer
    {
        public static void SeedCompetitionInfo(
            this CoreWarsDataContext context
            , IEnumerable<ICompetition> competitions)
        {
            competitions.ForEach(competition =>
            {
                var name = competition.Info.Name;
                if (!context.Competitions.Any(c => c.Name == name))
                    context.Competitions.Add(new Competition()
                    {
                        Name = name
                    });
            });

            context.SaveChanges();
        }

        public static void SeedLanguageInfo(
            this CoreWarsDataContext context,
            IEnumerable<string> supportedLanguageNames)
        {
            supportedLanguageNames.ForEach(name =>
            {
                if (!context.Languages.Any(l => name == l.Name))
                    context.Languages.Add(new Language()
                    {
                        Name = name
                    });
            });

            context.SaveChanges();
        }
    
}
}