using System;

namespace CoreWars.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var application = new CoreWarsApp();
            application.Initialize();
            Console.ReadLine();
        }
    }
}