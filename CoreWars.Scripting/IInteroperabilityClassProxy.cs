using System;

namespace CoreWars.Scripting
{
    public interface IInteroperabilityClassProxy : IDisposable
    {
        object InvokeMethod(string methodName, object[] methodParameters, Type expectedResponseType = null);
    }
}