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
                
        
        

    def place_symbol(self):
        my_winning_move = self.get_winning_move('X')
        
        if my_winning_move:
            return SymbolPlacement(my_winning_move[0],my_winning_move[1])
            
        block_opp_move = self.get_winning_move('O')
        
        if block_opp_move:
            return SymbolPlacement(block_opp_move[0],block_opp_move[1])    
        
        x = random.randint(0,2)
        y = random.randint(0,2)
        return SymbolPlacement(x,y)
