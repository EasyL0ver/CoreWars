using System;
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
            , IEnumerable<ICompetitionRegistration> competitions)
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

        public static void EnsureAdminExist(this CoreWarsDataContext context, string adminLogin, string adminPassword)
        {
            if(context.Users.Any(u => u.EmailAddress == adminLogin))
                return;
            
            context.Users.Add(new User()
            {
                Id = Guid.NewGuid()
                , EmailAddress = adminLogin
                , Password = adminPassword
            });

            context.SaveChanges();
        }
    
}
}