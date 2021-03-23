import clr
clr.AddReference("CoreWars.Scripting.Python.Tests")

from CoreWars.Scripting.Python.Tests import *


class TestAgentBehaviour:
    def __init__(self):
        self.dot_char = '.'
        self.hello_message = 'hello'
        self.default_name = 'mati'
        self.inner_list = []
        
    def say_hello(self):
        return self.hello_message
        
    def increment_number(self, number):
        return number + 1
    
    def concat_dot(self, sentence):
        return sentence + self.dot_char
    
    def increment_each_number(self, numbers):
        for i in range(len(numbers)):
            numbers[i] = numbers[i] + 1
            
    def modify_person(self, person):
        person.Age = 26
        person.Name = 'Gawel'
        
    def create_person(self, age):
        instance = Person(self.default_name, age)
        return instance
    
    def ret_enum(self):
        return TestEnum.Second
    
    def take_enum(self, enum):
        if enum == TestEnum.Second:
            return "correct"
        
    def ret_none(self):
        pass
    
    def append_to_inner_list(self, object):
        self.inner_list.append(object)
    
    def get_inner_list(self):
        return self.inner_list
        
