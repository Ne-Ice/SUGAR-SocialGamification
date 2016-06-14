﻿using System;
using System.Collections.Generic;
using PlayGen.SGA.DataModel.Interfaces;

namespace PlayGen.SGA.DataModel
{
    public class GroupAchievement : IRecord
    {
        public int Id { get; set; }

        public int GameId { get; set; }

        public string Name { get; set; }

        public string CompletionCriteria { get; set; }
    }
}
