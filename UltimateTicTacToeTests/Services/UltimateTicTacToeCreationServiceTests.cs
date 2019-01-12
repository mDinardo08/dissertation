﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using UltimateTicTacToe.Models.DTOs;
using UltimateTicTacToe.Models.Game;
using UltimateTicTacToe.Models.Game.Players;
using UltimateTicTacToe.Models.Game.WinCheck;
using UltimateTicTacToe.Services;

namespace UltimateTicTacToeTests.Services
{
    [TestClass]
    public class UltimateTicTacToeCreationServiceTests
    {
        BoardCreationService service;

        [TestInitialize()]
        public void Setup()
        {
            service = new UltimateTicTacToeCreationService(null, null);
        }

        [TestMethod]
        public void WillCeateAnUltimateTicTacToeBoard()
        {
            List<List<JObject>> board = new List<List<JObject>>
            {
                new List<JObject>
                {
                    new JObject()
                }
            };

            Mock<IPlayerCreationService> mockService = new Mock<IPlayerCreationService>(MockBehavior.Loose);
            mockService.Setup(x => x.createPlayer(It.IsAny<JObject>()));
            service = new UltimateTicTacToeCreationService(null, mockService.Object);
            BoardGame game = service.createBoardGame(new BoardGameDTO { game = board });
            Assert.IsTrue(game is TicTacToe);
        }

        [TestMethod]
        public void WillSupplyAWinCheckerToTheGame()
        {
            Mock<IWinChecker> mockHandler = new Mock<IWinChecker>(MockBehavior.Loose);
            List<List<JObject>> board = new List<List<JObject>>
            {
                new List<JObject>
                {
                    new JObject()
                }
            };
            Mock<IPlayerCreationService> mockService = new Mock<IPlayerCreationService>(MockBehavior.Loose);
            mockService.Setup(x => x.createPlayer(It.IsAny<JObject>()));
            service = new UltimateTicTacToeCreationService(mockHandler.Object, mockService.Object);
            TicTacToe game = service.createBoardGame(new BoardGameDTO { game = board }) as TicTacToe;
            Assert.IsNotNull(game.winChecker);
        }

        [TestMethod]
        public void WillPopulateAOneByOneGame()
        {
            List<List<JObject>> board = new List<List<JObject>>
            {
                new List<JObject>
                {
                    new JObject()
                }
            };
            Mock<IPlayerCreationService> mockService = new Mock<IPlayerCreationService>(MockBehavior.Loose);
            mockService.Setup(x => x.createPlayer(It.IsAny<JObject>()));
            service = new UltimateTicTacToeCreationService(null, mockService.Object);
            TicTacToe game = service.createBoardGame(new BoardGameDTO { game = board }) as TicTacToe;
            Assert.IsTrue(game.getBoard()[0][0] is Tile);
        }

        [TestMethod]
        public void WillPopulateALongRows()
        {
            List<List<JObject>> board = new List<List<JObject>>
            {
                new List<JObject>
                {
                    new JObject(), new JObject()
                }
            };
            Mock<IPlayerCreationService> mockService = new Mock<IPlayerCreationService>(MockBehavior.Loose);
            mockService.Setup(x => x.createPlayer(It.IsAny<JObject>()));
            service = new UltimateTicTacToeCreationService(null, mockService.Object);
            TicTacToe game = service.createBoardGame(new BoardGameDTO { game = board }) as TicTacToe;
            Assert.IsTrue(game.getBoard()[0].Count == 2);
        }

        [TestMethod]
        public void WillPopulateAlongColumns()
        {
            List<List<JObject>> board = new List<List<JObject>>
            {
                new List<JObject>
                {
                    new JObject()
                },
                new List<JObject>
                {
                    new JObject()
                }
            };
            Mock<IPlayerCreationService> mockService = new Mock<IPlayerCreationService>(MockBehavior.Loose);
            mockService.Setup(x => x.createPlayer(It.IsAny<JObject>()));
            service = new UltimateTicTacToeCreationService(null, mockService.Object);
            TicTacToe game = service.createBoardGame(new BoardGameDTO { game = board }) as TicTacToe;
            Assert.IsTrue(game.getBoard().Count == 2);
        }

        [TestMethod]
        public void WillCorrectlyNestGamesOfTicTacToe()
        {
            JObject ticTacToe = new JObject();
            ticTacToe.Add("board", JToken.FromObject(new List<List<BoardGame>>()));
            List<List<JObject>> board = new List<List<JObject>>
            {
                new List<JObject>
                {
                    ticTacToe
                }
            };
            TicTacToe game = service.createBoardGame(new BoardGameDTO { game = board }) as TicTacToe;
            Assert.IsTrue(game.getBoard()[0][0] is TicTacToe);
        }

        [TestMethod]
        public void NestedGamesWillHaveAWinChecker()
        {
            Mock<IWinChecker> mockHandler = new Mock<IWinChecker>(MockBehavior.Loose);
            service = new UltimateTicTacToeCreationService(mockHandler.Object, null);
            JObject ticTacToe = new JObject();
            ticTacToe.Add("board", JToken.FromObject(new List<List<BoardGame>>()));
            List<List<JObject>> board = new List<List<JObject>>
            {
                new List<JObject>
                {
                    ticTacToe
                }
            };
            TicTacToe game = service.createBoardGame(new BoardGameDTO { game = board }) as TicTacToe;
            Assert.IsNotNull((game.getBoard()[0][0] as TicTacToe).winChecker);
        }

        [TestMethod]
        public void WillPassPlayerJObjectToPlayerCreationService()
        {
            Mock<IPlayerCreationService> mockService = new Mock<IPlayerCreationService>(MockBehavior.Loose);
            mockService.Setup(x => x.createPlayer(It.IsAny<JObject>())).Verifiable();
            service = new UltimateTicTacToeCreationService(null, mockService.Object);
            List<List<JObject>> board = new List<List<JObject>>
            {
                new List<JObject>
                {
                    new JObject()
                }
            };
            TicTacToe game = service.createBoardGame(new BoardGameDTO { game = board }) as TicTacToe;
            mockService.Verify();
        }

        [TestMethod]
        public void WillSetTheReturnedValueFromPlayerCreationToTheTile()
        {
            Mock<IPlayerCreationService> mockService = new Mock<IPlayerCreationService>(MockBehavior.Loose);
            Tile tile = new Tile();
            Player p = new RandomAi(null);
            tile.owner = p;
            mockService.Setup(x => x.createPlayer(It.IsAny<JObject>())).Returns(p);
            service = new UltimateTicTacToeCreationService(null, mockService.Object);
            List<List<JObject>> board = new List<List<JObject>>
            {
                new List<JObject>
                {
                    JObject.FromObject(tile)
                }
            };
            TicTacToe game = service.createBoardGame(new BoardGameDTO { game = board }) as TicTacToe;
            Assert.AreEqual(p, (game.board[0][0] as Tile).owner);
        }
    }
}
