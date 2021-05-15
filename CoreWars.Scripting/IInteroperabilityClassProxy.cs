namespace CoreWars.Scripting
{
    public interface IInteroperabilityClassProxy 
    {
        object InvokeMethod(string methodName, object[] methodParameters);

        void Initialize();
    }
}