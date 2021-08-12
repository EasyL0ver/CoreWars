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
        
        
    def get_winning_move(self, player_sign):
        #check columns and rows
        for i in range(3):
            col = []
            row = []
            for k in range(3):
                col.append(self.tiles[i,k])
                row.append(self.tiles[k,i])
            if col.count(player_sign) == 2 and col.count(None) == 1:
                return i, col.index(None)
            if row.count(player_sign) == 2 and row.count(None) == 1:
                return row.index(None), i
                
        diag1 = [self.tiles[0,0], self.tiles[1,1], self.tiles[2,2]]
        diag2 = [self.tiles[0,2], self.tiles[1,1], self.tiles[2,0]]
        
        if diag1.count(player_sign) == 2 and diag1.count(None) == 1:
                return diag1.index(None), diag1.index(None)
        if diag2.count(player_sign) == 2 and diag2.count(None) == 1:
                return diag2.index(None), 2 - diag2.index(None)
                
        
        

    def place_symbol(self):
        my_winning_move = self.get_winning_move('X')
        
        if not my_winning_move:
            x = random.randint(0,2)
            y = random.randint(0,2)
            return SymbolPlacement(x,y)
            
        return SymbolPlacement(my_winning_move[0],my_winning_move[1])