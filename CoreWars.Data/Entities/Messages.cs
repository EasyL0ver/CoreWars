namespace CoreWars.Data.Entities
{
    public class Messages
    {
        public class Add<T>
        {
            public Add(T content)
            {
                Content = content;
            }

            public T Content { get; }
        }

        public class GetAll
        {
            public static GetAll Instance => new GetAll();
        }

        public class GetAllForCompetition
        {
            public GetAllForCompetition(string competitionName)
            {
                CompetitionName = competitionName;
            }

            public string CompetitionName { get; }
        }

        public class Subscribe
        {
            public static Subscribe Instance => new Subscribe();
        }

        public class AddedEvent<T>
        {
            public AddedEvent(T addedElement)
            {
                AddedElement = addedElement;
            }

            public T AddedElement { get; }
        }
    }
}