﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltimateTicTacToe.Models.DTOs;
using UltimateTicTacToe.Models.Game;
using UltimateTicTacToe.Models.Game.Players;
using UltimateTicTacToe.Models.Game.WinCheck;

namespace UltimateTicTacToe.Services
{
    public class UltimateTicTacToeCreationService : BoardCreationService
    {
        public IWinChecker winChecker;
        public UltimateTicTacToeCreationService(IWinChecker winChecker)
        {
            this.winChecker = winChecker;
        }

        public BoardGame createBoardGame(BoardGameDTO gameDto)
        {
            BoardGame result = createBoardGame(gameDto.game);
            result.validateBoard();
            result.registerMove(gameDto.lastMove);
            return result;
        }

        public BoardGame createBoardGame(int size)
        {
            TicTacToe result = new TicTacToe(winChecker);
            List<List<BoardGame>> board = new List<List<BoardGame>>();
            for (int y = 0; y < size; y++)
            {
                board.Add(new List<BoardGame>());
                for (int x = 0; x < size; x++)
                {
                    board[y].Add(createNewTicTacToe(size));
                }
            }
            result.board = board;
            return result;
        }

        private TicTacToe createNewTicTacToe(int size)
        {
            TicTacToe result = new TicTacToe(winChecker);
            List<List<BoardGame>> board = new List<List<BoardGame>>();
            for (int y = 0; y < size; y++)
            {
                board.Add(new List<BoardGame>());
                for (int x = 0; x < size; x++)
                {
                    board[y].Add(new Tile());
                }
            }
            result.board = board;
            return result;
        }

        public BoardGame createBoardGame(List<List<JObject>> JObjectBoard)
        {
            TicTacToe result = new TicTacToe(this.winChecker);
            List<List<BoardGame>> board = new List<List<BoardGame>>();
            for (int row = 0; row < JObjectBoard.Count; row++)
            {
                board.Add(new List<BoardGame>());
                for (int col = 0; col < JObjectBoard[row].Count; col++)
                {
                    JObject space = JObjectBoard[row][col];
                    if (space["board"] == null)
                    {
                        board[row].Add(createTile(space));   
                    }
                    else
                    {
                        TicTacToe innerGame = createBoardGame(space["board"].ToObject<List<List<JObject>>>()) as TicTacToe;
                        innerGame.owner = space["owner"].ToObject<PlayerColour?>();
                        board[row].Add(innerGame);
                    }
                }
            }
            result.board = board;
            return result;
        }

        private Tile createTile(JObject jObject)
        {
            Tile t = new Tile();
            t.owner = jObject["owner"].ToObject<PlayerColour?>();
            return t;
        }
    }
}
