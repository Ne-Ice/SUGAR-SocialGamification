﻿using System.Collections.Generic;

namespace PlayGen.SGA.Contracts.Controllers
{
    public interface IGameController
    {
        IEnumerable<GameResponse> Get();

        IEnumerable<GameResponse> Get(string[] name);

        GameResponse Create(GameRequest game);

        void Delete(int[] id);
    }
}