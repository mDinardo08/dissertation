﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UltimateTicTacToe.Models;
using UltimateTicTacToe.Models.DTOs;
using UltimateTicTacToe.Models.Game;
using UltimateTicTacToe.Models.Game.Players;
using UltimateTicTacToe.Models.Game.WinCheck;
using UltimateTicTacToe.Services;

namespace UltimateTicTacToeTests.Services
{
    [TestClass]
    public class GameServiceTests
    {
        IGameService service;

        [TestInitialize()]
        public void Setup()
        {
            service = new GameService(null);
        }

        [TestMethod]
        public void WillSetTheWinningPlayerIfGameIsOver()
        {
            Mock<BoardGame> mockGame = new Mock<BoardGame>(MockBehavior.Strict);
            Mock<Player> p = new Mock<Player>();
            p.Setup(x => x.getPlayerType()).Returns((PlayerType) 0);
            mockGame.Setup(x => x.getWinner()).Returns(p.Object);
            BoardGameDTO result = service.processMove(mockGame.Object, p.Object);
            Assert.AreEqual(p.Object, result.Winner);
        }

        [TestMethod]
        public void WillHaveTheAiMakeAMoveIfGameIsNotOver()
        {
            Mock<BoardGame> mockGame = new Mock<BoardGame>(MockBehavior.Strict);
            Mock<Player> p = new Mock<Player>();
            p.Setup(x => x.makeMove(mockGame.Object)).Returns(mockGame.Object).Verifiable();
            mockGame.Setup(x => x.getWinner()).Throws(new NoWinnerException());
            mockGame.SetupSequence(x => x.getWinner()).Throws(new NoWinnerException()).Returns(p.Object);
            mockGame.Setup(x => x.getBoard()).Returns(new List<List<BoardGame>>());
            BoardGameDTO result = service.processMove(mockGame.Object, p.Object);
            p.Verify();
        }

        [TestMethod]
        public void WillReturnTheAiAsTheWinnerIfItsMoveWinsTheGame()
        {
            Mock<BoardGame> mockGame = new Mock<BoardGame>(MockBehavior.Loose);
            Mock<Player> p = new Mock<Player>();
            p.Setup(x => x.makeMove(mockGame.Object)).Returns(mockGame.Object);
            mockGame.SetupSequence(x => x.getWinner()).Throws(new NoWinnerException()).Returns(p.Object);
            mockGame.Setup(x => x.getBoard()).Returns(new List<List<BoardGame>>());
            BoardGameDTO result = service.processMove(mockGame.Object, p.Object);
            Assert.IsTrue(result.Winner == p.Object);
        }

        [TestMethod]
        public void WillMaintainTheAIOpponentOnTheGame()
        {
            Mock<BoardGame> mockGame = new Mock<BoardGame>(MockBehavior.Loose);
            Mock<Player> p = new Mock<Player>();
            p.Setup(x => x.makeMove(mockGame.Object)).Returns(mockGame.Object);
            p.Setup(x => x.getPlayerType()).Returns(PlayerType.RANDOM);
            mockGame.Setup(x => x.getWinner()).Throws(new NoWinnerException());
            mockGame.SetupSequence(x => x.getWinner()).Throws(new NoWinnerException()).Returns(p.Object);
            mockGame.Setup(x => x.getBoard()).Returns(new List<List<BoardGame>>());
            BoardGameDTO result = service.processMove(mockGame.Object, p.Object);
            Assert.IsTrue(result.cur["type"].ToObject<PlayerType>() == PlayerType.RANDOM);
        }

        [TestMethod]
        public void WillAddEmpty2DListOfJObject()
        {
            Mock<BoardGame> mockGame = new Mock<BoardGame>(MockBehavior.Loose);
            Mock<Player> p = new Mock<Player>();
            mockGame.Setup(x => x.getWinner()).Throws(new NoWinnerException());
            p.Setup(x => x.makeMove(mockGame.Object)).Returns(mockGame.Object);
            mockGame.Setup(x => x.getBoard()).Returns(new List<List<BoardGame>>());
            BoardGameDTO result = service.processMove(mockGame.Object, p.Object);
            Assert.IsTrue(result.game.Count is 0);
        }

        [TestMethod]
        public void WillAddOneTile()
        {
            Mock<BoardGame> mockGame = new Mock<BoardGame>(MockBehavior.Loose);
            Mock<Player> p = new Mock<Player>();
            mockGame.Setup(x => x.getWinner()).Throws(new NoWinnerException());
            p.Setup(x => x.makeMove(mockGame.Object)).Returns(mockGame.Object);
            mockGame.Setup(x => x.getBoard()).Returns(new List<List<BoardGame>> {
                new List<BoardGame>
                {
                    new Tile()
                }
            });
            BoardGameDTO result = service.processMove(mockGame.Object, p.Object);
            Assert.IsTrue(result.game[0].Count is 1);
        }

        [TestMethod]
        public void WillAddOneTileWithPropertiesPopulated()
        {
            Mock<BoardGame> mockGame = new Mock<BoardGame>(MockBehavior.Loose);
            Mock<Player> p = new Mock<Player>();
            mockGame.Setup(x => x.getWinner()).Throws(new NoWinnerException());
            p.Setup(x => x.makeMove(mockGame.Object)).Returns(mockGame.Object);
            mockGame.Setup(x => x.getBoard()).Returns(new List<List<BoardGame>> {
                new List<BoardGame>
                {
                    new Tile
                    {
                        owner = new RandomAi(null)
                    }
                }
            });
            BoardGameDTO result = service.processMove(mockGame.Object, p.Object);
            Assert.IsNotNull(result.game[0][0]["owner"]);
        }

        [TestMethod]
        public void WillAddTilesAlongWidth()
        {
            Mock<BoardGame> mockGame = new Mock<BoardGame>(MockBehavior.Loose);
            Mock<Player> p = new Mock<Player>();
            mockGame.Setup(x => x.getWinner()).Throws(new NoWinnerException());
            p.Setup(x => x.makeMove(mockGame.Object)).Returns(mockGame.Object);
            mockGame.Setup(x => x.getBoard()).Returns(new List<List<BoardGame>> {
                new List<BoardGame>
                {
                    new Tile(), new Tile()
                }
            });
            BoardGameDTO result = service.processMove(mockGame.Object, p.Object);
            Assert.IsTrue(result.game[0].Count is 2);
        }

        [TestMethod]
        public void WillPopulateOverHight()
        {
            Mock<BoardGame> mockGame = new Mock<BoardGame>(MockBehavior.Loose);
            Mock<Player> p = new Mock<Player>();
            mockGame.Setup(x => x.getWinner()).Throws(new NoWinnerException());
            p.Setup(x => x.makeMove(mockGame.Object)).Returns(mockGame.Object);
            mockGame.Setup(x => x.getBoard()).Returns(new List<List<BoardGame>> {
                new List<BoardGame>
                {
                    new Tile()
                },
                new List<BoardGame>
                {
                    new Tile()
                }
            });
            BoardGameDTO result = service.processMove(mockGame.Object, p.Object);
            Assert.IsTrue(result.game.Count is 2);
        }

    }
}
