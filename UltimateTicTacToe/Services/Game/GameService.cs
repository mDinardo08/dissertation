﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UltimateTicTacToe.Models.DTOs;
using UltimateTicTacToe.Models.Game;
using UltimateTicTacToe.Models.Game.Players;
using UltimateTicTacToe.Models.Game.WinCheck;

namespace UltimateTicTacToe.Services
{
    public class GameService : IGameService
    {
        private IWinChecker winChecker;

        public GameService(IWinChecker winChecker)
        {
            this.winChecker = winChecker;
        }

        public BoardGameDTO processMove(BoardGame game, Player cur, List<Player> players)
        {
            BoardGameDTO result = new BoardGameDTO();
            result.players = buildPlayersArray(players);
            if (game.isWon())
            {
                result.Winner = game.getWinner();
            }else
            {
                if (cur.getPlayerType() != PlayerType.HUMAN)
                {
                    HandleAiMove(cur, game, result, players);
                    if (game.isWon())
                    {
                        result.Winner = game.getWinner();
                    }
                }
                else
                {
                    result.cur = convertToJObject(cur);
                    result.game = convertToJObject(game.getBoard());
                }
            }
            result.game = convertToJObject(game.getBoard());
            result.availableMoves = game.getAvailableMoves();
            return result;
        }

        private void HandleAiMove(Player Ai, BoardGame game, BoardGameDTO result, List<Player> players)
        {
            Move move = Ai.makeMove(game.Clone() as BoardGame);
            game.makeMove(move);
            List<List<BoardGame>> board = game.getBoard();
            result.lastMove = move;
            Player next = players.Find(x => !x.getColour().Equals(Ai.getColour()));
            result.cur = convertToJObject(next);
            try
            {
                result.Winner = game.getWinner();
            }
            catch (NoWinnerException) { }

        }
    
        private List<JObject> buildPlayersArray(List<Player> players)
        {
            List<JObject> result = new List<JObject>();
            foreach(Player player in players)
            {
                result.Add(convertToJObject(player));
            }
            return result;
        }

        private JObject convertToJObject(Player player)
        {
            JObject jPlayer = new JObject();
            if (player != null)
            {
                jPlayer.Add("type", JToken.FromObject(player.getPlayerType()));
                jPlayer.Add("name", JToken.FromObject(player.getName()));
                jPlayer.Add("colour", JToken.FromObject(player.getColour()));
            }
            return jPlayer;
        }

        private List<List<JObject>> convertToJObject(List<List<BoardGame>> board)
        {
            List<List<JObject>> result = new List<List<JObject>>();
            for(int x = 0; x< board.Count; x++)
            {
                result.Add(new List<JObject>());
                for(int y = 0; y < board[x].Count; y++)
                {
                    result[x].Add(JObject.FromObject(board[x][y]));
                }
            }
            return result;

        }

        public RatingDTO rateMove(BoardGame boardGame, Move move)
        {
            throw new System.NotImplementedException();
        }
    }
}
