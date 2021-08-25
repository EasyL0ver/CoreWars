import clr
clr.AddReference("TicTacToe")

import logging

from TicTacToe.Imports import *
from System import Array

class GameController:
    def __init__(self):
        self.tiles = Array.CreateInstance(str, 3, 3)
        logging.warning(self.tiles)

    def update_game_state(self, game_state):
        self.tiles = game_state.Board

    def find_first_unoccupied(self):
        for col in range(3):
            for row in range(3):
                logging.warning('iterating through' + str(col) + str(row))
                logging.warning('result is' + str(self.tiles[col,row]))
                if not self.tiles[col,row]:
                    return col, row

        return 0,0

    def place_symbol(self):
        logging.warning('TEST LOGOWANIA')
        logging.warning(str(self.tiles))
        x,y = self.find_first_unoccupied()
        return SymbolPlacement(x,y)

