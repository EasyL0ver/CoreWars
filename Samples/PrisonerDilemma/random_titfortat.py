import random

class GameController:
    def __init__(self):
        self.opponent_actions = []

    ## receive information about opponent action
    def update_opponent_action(self, opponent_action):
        self.opponent_actions.append(opponent_action)
        
    def get_previous_opponent_action(self):
        if(len(self.opponent_actions) == 0):
            return True
        return self.opponent_actions[-1]

    ## implement bot logic here - return True = cooperate, False = defect
    def choose_dilemma(self):
        if(random.random() < 0.1):
            return not self.get_previous_opponent_action()
        return self.get_previous_opponent_action()