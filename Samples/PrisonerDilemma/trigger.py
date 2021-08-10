class GameController:
    def __init__(self):
        self.opponent_actions = []
        self.triggered = False

    ## receive information about opponent action
    def update_opponent_action(self, opponent_action):
        if not opponent_action:
            self.triggered = True
            
        self.opponent_actions.append(opponent_action)

    ## implement bot logic here - return True = cooperate, False = defect
    def choose_dilemma(self):
        return not self.triggered