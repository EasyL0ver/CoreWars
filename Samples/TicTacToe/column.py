import clr
clr.AddReference("TicTacToe")
import random

from TicTacToe.Imports import *
from System import Array

class GameController:
    def __init__(self):
        self.tiles = Array.CreateInstance(str, 3, 3)
        self.column_index = random.randint(0,2)
        self.current_row = 0

    def update_game_state(self, game_state):
        self.tiles = game_state.Board
        

          
    def place_symbol(self):
        x = self.column_index
        y = self.current_row
        self.current_row = self.current_row + 1
        return SymbolPlacement(x,y)