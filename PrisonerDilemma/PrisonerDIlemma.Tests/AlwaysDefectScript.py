class GameController:
    def __init__(self):
        self.opponent_actions = []
        
    def choose_dilemma(self):
        return False

    def update_opponent_action(self, opponent_action):
        self.opponent_actions.append(opponent_action)