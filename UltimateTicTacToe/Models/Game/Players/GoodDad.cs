﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltimateTicTacToe.DataAccess;
using UltimateTicTacToe.Models.DTOs;
using UltimateTicTacToe.Models.MCTS;
using UltimateTicTacToe.Services;

namespace UltimateTicTacToe.Models.Game.Players
{
    public class GoodDad : AbstractPlayer
    {
        public GoodDad(IRandomService random, IDatabaseProvider provider) : base(random, provider)
        {
            type = PlayerType.GOODDAD;
            userId = (int)PlayerType.GOODDAD;
        }

        protected override INode decideMove(BoardGame game, List<INode> nodes, RatingDTO opponentRating)
        {
            double diff = int.MaxValue;
            INode best = nodes[0];
            double targetScore = 0.8 * opponentRating.average + 0.2 * opponentRating.latest;
            foreach (INode node in nodes)
            {
                double nodeReward = node.getReward();
                double nodeDiff = Math.Abs(targetScore - nodeReward);
                if (nodeDiff < diff)
                {
                    diff = nodeDiff;
                    best = node;
                }
                if(nodeReward == 1)
                {
                    best = node;
                    targetScore = 10;
                }
            }
            return best; 
        }
    }
}
