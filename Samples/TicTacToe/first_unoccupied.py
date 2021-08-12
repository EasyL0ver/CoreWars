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
        
    def find_first_unoccupied(self):
        for x in range(3):
            for y in range(3):
                if not self.tiles[x,y]:
                    return x,y
        return 0,0
          
    def place_symbol(self):
        x,y = self.find_first_unoccupied()
        return SymbolPlacement(x,y)