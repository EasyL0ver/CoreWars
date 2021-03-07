class Person:
    def __init__(self, name, age):
        self.name = name
        self.age = age

class TestAgentBehaviour:
    def __init__(self):
        self.dot_char = '.'
        self.hello_message = 'hello'
        self.default_name = 'mati'
        
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
        return Person(self.default_name, age)
        
