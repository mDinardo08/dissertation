﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltimateTicTacToe.DataAccess;
using UltimateTicTacToe.Models.Game.Players;

namespace UltimateTicTacToe.Services
{
    public class RandomPlayerClassHandler : AbstractPlayerClassHandler
    {
        public RandomPlayerClassHandler(IRandomService randomService, IDatabaseProvider provider) : base(randomService, provider)
        {
            type = PlayerType.RANDOM;
            successor = new HumanPlayerClassHandler(randomService, provider);
        }

        protected override Player buildPlayer()
        {
            return new RandomAi(randomService, provider);
        }
    }
}
