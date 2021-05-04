using System;

namespace CoreWars.Common
{
    public interface IUser
    {
        public Guid Id { get; }
        public string EmailAddress { get; }
    }
}