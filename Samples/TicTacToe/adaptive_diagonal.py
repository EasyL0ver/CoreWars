import clr
clr.AddReference("TicTacToe")
import random

from TicTacToe.Imports import *
from System import Array

class GameController:
    def __init__(self):
        self.tiles = Array.CreateInstance(str, 3, 3)
        self.rising = random.choice([True,False])
        self.current_row = 0

    def update_game_state(self, game_state):
        self.tiles = game_state.Board
        
        free_slopes = self.free_diagonal_slopes()
        
        if self.rising not in free_slopes:
            if len(free_slopes) != 0:
                self.rising = random.choice(free_slopes)
                self.current_row = 0
        

    def free_diagonal_slopes(self):
        slopes = []
        diag1 = [self.tiles[0,0], self.tiles[1,1], self.tiles[2,2]]
        diag2 = [self.tiles[0,2], self.tiles[1,1], self.tiles[2,0]]
        
        if 'O' not in diag1:
            slopes.append(True)
        if 'O' not in diag2:
            slopes.append(False)
        
        return slopes
            
    def place_symbol(self):
        x = self.current_row
        y = self.current_row
        
        if not self.rising:
            y = 2 - self.current_row
        self.current_row = self.current_row + 1
        return SymbolPlacement(x,y)