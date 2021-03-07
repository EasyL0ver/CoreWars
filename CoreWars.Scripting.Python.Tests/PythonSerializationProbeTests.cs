using System;
using System.IO;
using NUnit.Framework;

namespace CoreWars.Scripting.Python.Tests
{

    public class GetterSetterClass
    {
        public int tProperty { get; set; }
    }
    
    public class PythonSerializationProbeTests
    {
        private PythonInteroperabilityClassProxy _sut;
        private string _testScript;

        [OneTimeSetUp]
        public void LoadTestScript()
        {
            _testScript = File.ReadAllText("./Scripts/PythonSerializationProbe.py");
        }

        [SetUp]
        public void Setup()
        {
            _sut = new PythonInteroperabilityClassProxy(_testScript, "ProbePythonScript");
        }

        private string ProbeMembers(object probed)
        {
            var response = _sut.InvokeMethod("probe_members", new object[] {probed}, typeof(string));
            return response as string;
        }

        [Test]
        public void ProbeGettersSettersSerialization()
        {
            var probed = new GetterSetterClass() {tProperty = 5};
            var response = ProbeMembers(probed);
            Console.WriteLine(response);
        }
        
        [Test]
        public void ProbePersonSerialization()
        {
            var probed = new Person() {Age = 16, Name = "Daga"};
            var response = ProbeMembers(probed);
            Console.WriteLine(response);
        }


    
}

}