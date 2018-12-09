﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace UltimateTicTacToe.Models.GameClasses
{
    public class TicTacToe : BoardGame
    {
        public List<List<BoardGame>> board;
        public List<Player> players;
        private IWinChecker winChecker;

        public TicTacToe(IWinChecker winChecker)
        {
            this.winChecker = winChecker;
        }

        public void makeMove(Move move)
        {
            throw new NotImplementedException();
        }

        public List<List<BoardGame>> getBoard()
        {
            return board;
        }


        public BoardGame getSector(Point point)
        {
            return board[point.X][point.Y];
        }

        public Player getWinner()
        {
            Player result = winChecker.checkForWin(this);
            if (result == null)
            {
                throw new NoWinnerException();
            }
            return result;
        }

        public void setBoard(List<List<BoardGame>> board)
        {
            this.board = board;
        }

        public bool isLeaf()
        {
            throw new NotImplementedException();
        }
    }
}
