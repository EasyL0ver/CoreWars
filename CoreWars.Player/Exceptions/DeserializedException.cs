using System;

namespace CoreWars.Player.Exceptions
{
    public class DeserializedException : Exception
    {
        public DeserializedException(string msg) : base(msg)
        {
            
        }
    }
}