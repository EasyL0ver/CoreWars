using System.Linq;
using CoreWars.Common;

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

        public override string ToString()
        {
            return $"Calling {MethodName} with parameters: {MethodParams.Select(x => x.ToString()).FormatString(", ")}";
        }
    }
}