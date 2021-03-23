class GameController:
    def __init__(self):
        self.opponent_actions = []

    def choose_dilemma(self):
        if len(self.opponent_actions) == 0:
            return True
        
        return self.opponent_actions[-1]

    def update_opponent_action(self, opponent_action):
        self.opponent_actions.append(opponent_action)