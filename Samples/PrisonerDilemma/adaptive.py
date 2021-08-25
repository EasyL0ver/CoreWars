class GameController:
    def __init__(self):
        self.opponent_actions = []
        self.probe_actions = [False, False, True, False, False]
        self.probe_index = 0

    ## receive information about opponent action
    def update_opponent_action(self, opponent_action):
        self.opponent_actions.append(opponent_action)
        
    def is_opponent_tit_for_tat(self):
        true_to_true = self.opponent_actions[3] == True
        false_to_false = self.opponent_actions[2] == False
        return true_to_true and false_to_false

    ## implement bot logic here - return True = cooperate, False = defect
    def choose_dilemma(self):
        if(self.probe_index < 5):
            response = self.probe_actions[self.probe_index]
            self.probe_index = self.probe_index + 1
            return response
        if(self.is_opponent_tit_for_tat()):
            return True
        return False
