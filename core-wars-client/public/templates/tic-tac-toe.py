import clr
clr.AddReference("TicTacToe")


from TicTacToe.Imports import *
from System import Array

class GameController:
    def __init__(self):
        self.tiles = Array.CreateInstance(str, 3, 3)

    # update game state contained within self.tiles
    # 3x3 array with string inside 
    # X means claimed by you, O means claimed by opoonent, None is unclaimed
    def update_game_state(self, game_state):
        self.tiles = game_state.Board

    # make a choice where to put your next symbol - constructor takes X,Y coordinates
    # invalid move (index out of bounds, field already occupied)
    # causes player to lose instantly
    def place_symbol(self):
        return SymbolPlacement(0,0)