﻿using System.ComponentModel.DataAnnotations;
using PlayGen.SUGAR.Common.Shared;

namespace PlayGen.SUGAR.Contracts.Shared
{
	/// <summary>
	/// Encapsulates ActorData/EvaluationData details.
	/// </summary>
	/// <example>
	/// JSON
	/// {
	/// ActorId : 1,
	/// GameId : 1,
	/// Key : "Data Key",
	/// Value : "10",
	/// SaveDataType : "Long"
	/// }
	/// </example>
	public class EvaluationDataRequest
    {
		/// <summary>
		/// The id of the Actor which this ActorData/EvaluationData is being ensigned to. Can be left null to ensign to the system/game.
		/// </summary>
		public int? ActorId { get; set; }

		/// <summary>
		/// The id of the Game which this ActorData/EvaluationData relates to. Can be left null to relate the ActorData/EvaluationData to the wider system.
		/// </summary>
		public int? GameId { get; set; }

		/// <summary>
		/// The identifier of the data being stored.
		/// </summary>
		[Required]
		[StringLength(64)]
		public string Key { get; set; }

		/// <summary>
		/// The value of the data being stored.
		/// </summary>
		[Required]
		[StringLength(64)]
		public string Value { get; set; }

		/// <summary>
		/// The type of data which is being stored.
		/// </summary>
		[Required]
		public SaveDataType SaveDataType { get; set; }
	}
}
