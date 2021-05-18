using System;

namespace CoreWars.Coordination
{
    public class GameUnresolvedException : Exception
    {
        public GameUnresolvedException(string messsage, Exception inner) : base(messsage, inner){}
    }
}