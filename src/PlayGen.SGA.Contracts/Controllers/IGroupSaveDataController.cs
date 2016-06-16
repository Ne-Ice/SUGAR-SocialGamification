﻿using System.Collections.Generic;

namespace PlayGen.SGA.Contracts.Controllers
{
    public interface IGroupSaveDataController
    {
        void Add(SaveData data);

        IEnumerable<SaveData> Get(int actorId, int gameId, string[] keys);
    }
}
