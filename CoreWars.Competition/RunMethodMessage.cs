using System;
using System.Collections.Generic;

namespace CoreWars.Competition
{
    public sealed class RunMethodMessage
    {
        public string MethodName { get; }
        public object[] MethodParams { get; } 

        public RunMethodMessage(
            string methodName
            , params object[] methodParams)
        {
            MethodName = methodName;
            MethodParams = methodParams;
        }
    }
}