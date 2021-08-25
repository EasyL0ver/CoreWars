class GameController:
    def __init__(self):
        self.opponent_actions = []
        self.previous_move = True

    ## receive information about opponent action
    def update_opponent_action(self, opponent_action):
        self.opponent_actions.append(opponent_action)
        
    def get_previous_opponent_action(self):
        if(len(self.opponent_actions) == 0):
            return True
        return self.opponent_actions[-1]

    ## implement bot logic here - return True = cooperate, False = defect
    def choose_dilemma(self):
        move = self.previous_move == self.get_previous_opponent_action()
        self.previous_move = move
        return move
