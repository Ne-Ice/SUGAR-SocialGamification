﻿using System.Collections.Generic;
using PlayGen.SUGAR.Data.Model.Interfaces;

namespace PlayGen.SUGAR.Data.Model
{
	public class Game : IRecord
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public virtual List<GroupAchievement> GroupAchievements { get; set; }

		public virtual List<GroupData> GroupDatas { get; set; }

		public virtual List<UserAchievement> UserAchievements { get; set; }

		public virtual List<UserData> UserDatas { get; set; }
	}
}