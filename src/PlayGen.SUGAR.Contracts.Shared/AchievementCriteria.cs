﻿using System.ComponentModel.DataAnnotations;

namespace PlayGen.SUGAR.Contracts
{
	/// <summary>
	/// Encapsulates requirements for completing an achievement or skill.
	/// </summary>
	/// <example>
	/// JSON
	/// {
	/// Key : "GameData Key",
	/// DataType : "String",
	/// CriteriaQueryType : "Any",
	/// ComparisonType : "Equals",
	/// Scope : "Actor",
	/// Value : "GameData Key Value"
	/// }
	/// </example>
	public class AchievementCriteria
	{
		/// <summary>
		/// The key which will be queried against to check if criteria is met.
		/// </summary>
		[Required]
		public string Key { get; set; }

		/// <summary>
		/// GameDataType of the key which is being queried.
		/// </summary>
		[Required]
		public GameDataType DataType { get; set; }

		/// <summary>
		/// Which stored GameData will be queried.
		/// </summary>
		[Required]
		public CriteriaQueryType CriteriaQueryType { get; set; }

		/// <summary>
		/// How the target value and the actual value will be compared.
		/// </summary>
		[Required]
		public ComparisonType ComparisonType { get; set; }

		/// <summary>
		/// Whether the criteria will be checked against the actor or related actors (i.e. group members, user friends).
		/// </summary>
		[Required]
		public CriteriaScope Scope { get; set; }

		/// <summary>
		/// The value which will compared against in order to see if the criteria has been met.
		/// </summary>
		[Required]
		public string Value { get; set; }
	}
}
