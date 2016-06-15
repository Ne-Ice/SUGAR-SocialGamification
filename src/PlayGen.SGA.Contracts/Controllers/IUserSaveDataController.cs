﻿using System.Collections.Generic;

namespace PlayGen.SGA.Contracts.Controllers
{
    public interface IUserSaveDataController
    {
        void Add(int id, SaveData data);

        IEnumerable<SaveData> Get(int actorId, int gameId, IEnumerable<string> keys);
    }
}
