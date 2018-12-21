﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using UltimateTicTacToe.JsonConverters.WinCheck;
using UltimateTicTacToe.Models.Game.Players;
using UltimateTicTacToe.Models.Game.WinCheck;

namespace UltimateTicTacToe.Models.Game
{
    public class TicTacToe : CompositeGame
    {
        public List<List<BoardGame>> board;
        public IWinChecker winChecker;
        public Point2D boardFilter;

        public TicTacToe(IWinChecker winChecker)
        {
            this.winChecker = winChecker;
        }

        public void makeMove(Move move)
        {
            board[move.possition.X][move.possition.Y].makeMove(move.next);
            boardFilter = move.next.possition;
        }

        public List<List<BoardGame>> getBoard()
        {
            return board;
        }


        public BoardGame getSector(Point2D point)
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
  
        public List<Move> getAvailableMoves()
        {
            List<Move> availableMoves = new List<Move>();
            if (boardFilter != null)
            {
                List<Move> subBoardMoves = getSector(boardFilter).getAvailableMoves();
                availableMoves = subBoardMoves.Count > 0 ? subBoardMoves : getMovesFromAllSubBoards();
            }
            else if (winChecker.checkForWin(this) == null)
            {
                availableMoves = getMovesFromAllSubBoards();
            }
            return availableMoves;
        }

        private List<Move> getMovesFromAllSubBoards()
        {
            List<Move> result = new List<Move>();
            for (int y = 0; y < board.Count; y++)
            {
                for (int x = 0; x < board[y].Count; x++)
                {
                    List<Move> subMoves = board[y][x].getAvailableMoves();
                    subMoves.ForEach((Move m) => result.Add(new Move
                    {
                        next = m,
                        possition = new Point2D { X = x, Y = y }
                    }));
                }
            }
            return result;
        }
    }
}
