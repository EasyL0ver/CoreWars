import clr
clr.AddReference("TicTacToe")
import random

from TicTacToe.Imports import *
from System import Array

class GameController:
    def __init__(self):
        self.tiles = Array.CreateInstance(str, 3, 3)

    def update_game_state(self, game_state):
        self.tiles = game_state.Board
        

    def place_symbol(self):
        x = random.randint(0,2)
        y = random.randint(0,2)
        return SymbolPlacement(x,y)