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
        
    def find_unoccupied(self):
        unoccupied = []
        for x in range(3):
            for y in range(3):
                if not self.tiles[x,y]:
                    unoccupied.append((x,y))
        return unoccupied
          
    def place_symbol(self):
        if not self.tiles[1,1]:
            return SymbolPlacement(1,1)
        unoccupied = self.find_unoccupied()
        if(len(unoccupied) == 0):
            return SymbolPlacement(0,0)
        x,y = random.choice(unoccupied)
        return SymbolPlacement(x,y)