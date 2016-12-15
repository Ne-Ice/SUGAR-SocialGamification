﻿using System;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Data.Model.Interfaces;

namespace PlayGen.SUGAR.Data.Model
{
	public class EvaluationData : IModificationHistory
	{
		public int Id { get; set; }

		public int? ActorId { get; set; }

        public Actor Actor { get; set; }

		public int? GameId { get; set; }

        public Game Game { get; set; }

		public GameDataCategory Category { get; set; }

		public string Key { get; set; }

		public string Value { get; set; }

		public SaveDataType SaveDataType { get; set; }

		public DateTime DateCreated { get; set; }

		public DateTime DateModified { get; set; }
	}
}