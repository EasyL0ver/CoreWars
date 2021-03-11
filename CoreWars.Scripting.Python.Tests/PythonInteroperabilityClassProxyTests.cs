using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace CoreWars.Scripting.Python.Tests
{
    public enum TestEnum : short
    {
        First
        , Second
    }
    
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        
        public Person() {}
        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
    
    public class PythonInteroperabilityClassProxyTests
    {
        private PythonInteroperabilityClassProxy _sut;
        private string _testScript;

        [OneTimeSetUp]
        public void LoadTestScript()
        {
            _testScript = File.ReadAllText("./Scripts/BasicPythonTestScript.py");
        }
        
        [SetUp]
        public void Setup()
        {
            _sut = new PythonInteroperabilityClassProxy(_testScript, "TestAgentBehaviour");
        }
        
        [Test]
        public void InvokeBasicMethod_HelloMessageIsReturned()
        {
            var response = _sut.InvokeMethod("say_hello", System.Array.Empty<object>());
            
            Assert.IsInstanceOf<string>(response);
            Assert.AreEqual("hello", response);
        }

        [Test]
        public void IncrementIntegerWithScript_TypePersistAndNumberIncremented()
        {
            const int number = 6;
            var response = _sut.InvokeMethod("increment_number", new object[1] {number});
            
            Assert.IsInstanceOf<int>(response);
            Assert.AreEqual(number + 1, response);
        }
        
        [Test]
        public void ConcatStringFromWithinScript_TypePersistAndStringCorrect()
        {
            const string sentence = "hello";
            var response = _sut.InvokeMethod("concat_dot", new object[1] {sentence});
            
            Assert.IsInstanceOf<string>(response);
            Assert.AreEqual(sentence + ".", response);
        }

        [Test]
        public void DoesntExpectResponse_ResponseTypeIsNull()
        {
            const int number = 6;
            var response = _sut.InvokeMethod("ret_none", Array.Empty<object>());
            
            Assert.IsNull(response);
        }
        
        [Test]
        public void IncrementListOfNumbers_TypePersistAndNumbersIncremented()
        {
            var numbers = new List<int>() {1, 2, 3};
            var expected = new List<int>() {2, 3, 4};
            
            _sut.InvokeMethod("increment_each_number", new object[1] {numbers});
            
            Assert.That(numbers, Is.EquivalentTo(expected));
        }
        
        [Test]
        public void ModifySimpleObject_TypePersistAndObjectModified()
        {
            var person = new Person() {Age = 18, Name = "Pawel"};
            
            _sut.InvokeMethod("modify_person", new object[1] {person});
            
            Assert.AreEqual(26, person.Age);
            Assert.AreEqual("Gawel", person.Name);
        }
        
        [Test]
        public void CreateSimpleObject_TypePersistAndValuesCorrect()
        {
            var age = 18;
            
            var result = _sut.InvokeMethod("create_person", new object[1] {age});
            
            Assert.IsInstanceOf<Person>(result);

            var personResult = (Person) result;
            
            Assert.AreEqual(age, personResult.Age);
            Assert.AreEqual("mati", personResult.Name);
        }

        [Test]
        public void InvokedMethodReturnsEnum_CorrectValueIsReturned()
        {
            var response = _sut.InvokeMethod("ret_enum", Array.Empty<object>());
            Assert.AreEqual(TestEnum.Second, response);
        }
        
        [Test]
        public void InvokedMethodTakesEnum_CorrectValueIsReturned()
        {
            var response = _sut.InvokeMethod("take_enum", new object[1] {TestEnum.Second});
            Assert.AreEqual("correct", response);
        }
        
        
        
    }
}