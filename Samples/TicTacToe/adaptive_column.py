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
        
        free_columns = self.free_column_indexes()
        
        if self.column_index not in free_columns:
            if len(free_columns) != 0:
                self.column_index = random.choice(free_columns)
                self.current_row = 0
        

    def free_column_indexes(self):
        free_columns = []
        for x in range(3):
            for y in range(3):
                if self.tiles[x,y] == 'O':
                    break
            else:
                free_columns.append(x)
        return free_columns
            
    def place_symbol(self):
        x = self.column_index
        y = self.current_row
        self.current_row = self.current_row + 1
        return SymbolPlacement(x,y)