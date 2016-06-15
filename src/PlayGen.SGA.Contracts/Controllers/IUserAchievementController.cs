﻿using System.Collections.Generic;

namespace PlayGen.SGA.Contracts.Controllers
{
    public interface IUserAchievementController
    {
        void Create(Achievement achievement);

        IEnumerable<Achievement> Get(string name, int gameId);

        void Delete(int achievementId);

        IEnumerable<AchievementProgress> GetProgress(int userId, int gameId);

        IEnumerable<AchievementProgress> GetProgress(int achievementId, List<int> actorIds);
    }
}