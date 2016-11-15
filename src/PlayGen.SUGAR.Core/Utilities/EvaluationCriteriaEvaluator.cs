﻿using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Data.EntityFramework.Controllers;

using Actor = PlayGen.SUGAR.Data.Model.Actor;
using EvaluationCriteria = PlayGen.SUGAR.Data.Model.EvaluationCriteria;

namespace PlayGen.SUGAR.Core.Utilities
{
	public class EvaluationCriteriaEvaluator
	{
		protected readonly GameDataController GameDataController;
		protected readonly GroupRelationshipController GroupRelationshipController;
		protected readonly UserRelationshipController UserRelationshipController;

		// todo change all db controller usage to core controller usage
		public EvaluationCriteriaEvaluator(GameDataController gameDataController, GroupRelationshipController groupRelationshipController, UserRelationshipController userRelationshipController)
		{
			GameDataController = gameDataController;
			GroupRelationshipController = groupRelationshipController;
			UserRelationshipController = userRelationshipController;
		}

		// TODO: currently this is binary but should eventually return a progress value
		// The method of returning calculating the progress (for multiple criteria conditions) and 
		// how the progress is going to be represented (0f to 1f ?) need to be determined first.
		public float IsCriteriaSatisified(int? gameId, int? actorId, List<EvaluationCriteria> completionCriterias, ActorType actorType)
		{
			return completionCriterias.Sum(cc => Evaluate(gameId, actorId, cc, actorType)) / completionCriterias.Count;
		}


		protected float Evaluate(int? gameId, int? actorId, EvaluationCriteria completionCriteria, ActorType actorType)
		{
			if (completionCriteria.Scope == CriteriaScope.RelatedActors && actorId != null)
			{
				if (actorType != ActorType.Group)
				{
					throw new NotImplementedException("RelatedActors Scope is only implemented for groups");
				}
				var groupActors = GroupRelationshipController.GetMembers(actorId.Value);
				switch (completionCriteria.DataType)
				{
					case GameDataType.Boolean:
						return EvaluateManyBool(gameId, groupActors, completionCriteria);

					case GameDataType.String:
						return EvaluateManyString(gameId, groupActors, completionCriteria);

					case GameDataType.Float:
						return EvaluateManyFloat(gameId, groupActors, completionCriteria);

					case GameDataType.Long:
						return EvaluateManyLong(gameId, groupActors, completionCriteria);

					default:
						return 0;
				}
			}
			else
			{
				switch (completionCriteria.DataType)
				{
					case GameDataType.Boolean:
						return EvaluateBool(gameId, actorId, completionCriteria);

					case GameDataType.String:
						return EvaluateString(gameId, actorId, completionCriteria);

					case GameDataType.Float:
						return EvaluateFloat(gameId, actorId, completionCriteria);

					case GameDataType.Long:
						return EvaluateLong(gameId, actorId, completionCriteria);

					default:
						return 0;
				}
			}
		}

		protected float EvaluateLong(int? gameId, int? actorId, EvaluationCriteria completionCriteria)
		{
			switch (completionCriteria.CriteriaQueryType)
			{
				case CriteriaQueryType.Any:
					var any = GameDataController.AllLongs(gameId, actorId, completionCriteria.Key).ToList();

					return !any.Any() ? CompareValues(0, long.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.DataType) :
                                any.Max(value => CompareValues(value, long.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.DataType));

			    case CriteriaQueryType.Sum:
					var sum = GameDataController.SumLongs(gameId, actorId, completionCriteria.Key);

					return CompareValues(sum, long.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.DataType);
				case CriteriaQueryType.Latest:
					long latest;
					if (!GameDataController.TryGetLatestLong(gameId, actorId, completionCriteria.Key, out latest))
					{
						latest = 0;
					}

					return CompareValues(latest, long.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.DataType);
				default:
					return 0;
			}
		}

		protected float EvaluateFloat(int? gameId, int? actorId, EvaluationCriteria completionCriteria)
		{
			switch (completionCriteria.CriteriaQueryType)
			{
				case CriteriaQueryType.Any:
					var any = GameDataController.AllFloats(gameId, actorId, completionCriteria.Key).ToList();

					return !any.Any() ? CompareValues(0, float.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.DataType) : 
                                any.Max(value => CompareValues(value, float.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.DataType));

			    case CriteriaQueryType.Sum:
					var sum = GameDataController.SumFloats(gameId, actorId, completionCriteria.Key);

					return CompareValues(sum, float.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.DataType);
				case CriteriaQueryType.Latest:
					float latest;
					if (!GameDataController.TryGetLatestFloat(gameId, actorId, completionCriteria.Key, out latest))
					{
						latest = 0;
					}

					return CompareValues(latest, float.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.DataType);
				default:
					return 0;
			}
		}

		protected float EvaluateString(int? gameId, int? actorId, EvaluationCriteria completionCriteria)
		{
			switch (completionCriteria.CriteriaQueryType)
			{
				case CriteriaQueryType.Any:
					var any = GameDataController.AllStrings(gameId, actorId, completionCriteria.Key).ToList();

					return !any.Any() ? CompareValues("", completionCriteria.Value, completionCriteria.ComparisonType, completionCriteria.DataType) : 
                                any.Max(value => CompareValues(value, completionCriteria.Value, completionCriteria.ComparisonType, completionCriteria.DataType));

			    case CriteriaQueryType.Sum:
					return 0;
				case CriteriaQueryType.Latest:
					string latest;
					if (!GameDataController.TryGetLatestString(gameId, actorId, completionCriteria.Key, out latest))
					{
						latest = "";
					}

					return CompareValues(latest, completionCriteria.Value, completionCriteria.ComparisonType, completionCriteria.DataType);
				default:
					return 0;
			}
		}

		protected float EvaluateBool(int? gameId, int? actorId, EvaluationCriteria completionCriteria)
		{
			switch (completionCriteria.CriteriaQueryType)
			{
				case CriteriaQueryType.Any:
					var any = GameDataController.AllBools(gameId, actorId, completionCriteria.Key).ToList();

					return !any.Any() ? CompareValues(false, bool.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.DataType) : 
                                any.Max(value => CompareValues(value, bool.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.DataType));

			    case CriteriaQueryType.Sum:
					return 0;
				case CriteriaQueryType.Latest:
					bool latest;
					if (!GameDataController.TryGetLatestBool(gameId, actorId, completionCriteria.Key, out latest))
					{
						latest = false;
					}

					return CompareValues(latest, bool.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.DataType);
				default:
					return 0;
			}
		}

		protected float EvaluateManyLong(int? gameId, IEnumerable<Actor> actor, EvaluationCriteria completionCriteria)
		{
		    var actorList = actor as List<Actor> ?? actor.ToList();
		    switch (completionCriteria.CriteriaQueryType)
			{
				case CriteriaQueryType.Any:
					return actorList.Sum(a => EvaluateLong(gameId, a.Id, completionCriteria)) / actorList.Count;
				case CriteriaQueryType.Sum:
					var sum = actorList.Sum(a => GameDataController.SumLongs(gameId, a.Id, completionCriteria.Key));

					return CompareValues(sum, long.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.DataType);
				case CriteriaQueryType.Latest:
					return actorList.Sum(a => EvaluateLong(gameId, a.Id, completionCriteria)) / actorList.Count;
				default:
					return 0;
			}
		}

		protected float EvaluateManyFloat(int? gameId, IEnumerable<Actor> actor, EvaluationCriteria completionCriteria)
		{
            var actorList = actor as List<Actor> ?? actor.ToList();
            switch (completionCriteria.CriteriaQueryType)
			{
				case CriteriaQueryType.Any:
					return actorList.Sum(a => EvaluateFloat(gameId, a.Id, completionCriteria)) / actorList.Count;
				case CriteriaQueryType.Sum:
					var sum = actorList.Sum(a => GameDataController.SumFloats(gameId, a.Id, completionCriteria.Key));

					return CompareValues(sum, float.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.DataType);
				case CriteriaQueryType.Latest:
					return actorList.Sum(a => EvaluateFloat(gameId, a.Id, completionCriteria)) / actorList.Count;
				default:
					return 0;
			}
		}

		protected float EvaluateManyString(int? gameId, IEnumerable<Actor> actor, EvaluationCriteria completionCriteria)
		{
            var actorList = actor as List<Actor> ?? actor.ToList();
            return actorList.Sum(a => EvaluateString(gameId, a.Id, completionCriteria)) / actorList.Count;
		}

		protected float EvaluateManyBool(int? gameId, IEnumerable<Actor> actor, EvaluationCriteria completionCriteria)
		{
            var actorList = actor as List<Actor> ?? actor.ToList();
            return actorList.Sum(a => EvaluateBool(gameId, a.Id, completionCriteria)) / actorList.Count;
		}

		protected static float CompareValues<T>(T value, T expected, ComparisonType comparisonType, GameDataType dataType) where T : IComparable
		{
			var comparisonResult = value.CompareTo(expected);

		    float expectedNum;
		    switch (comparisonType)
			{
				case ComparisonType.Equals:
					return comparisonResult == 0 ? 1 : 0;

				case ComparisonType.NotEqual:
					return comparisonResult != 0 ? 1 : 0;

				case ComparisonType.Greater:
					if (comparisonResult > 0)
					{
						return 1;
					}
			        if (!(comparisonResult > 0) && (dataType == GameDataType.String || dataType == GameDataType.Boolean))
			        {
			            return 0;
			        }
			        if ((float.TryParse(expected.ToString(), out expectedNum))) {
			            if (dataType == GameDataType.Long)
			            {
			                expectedNum += 1;
			            } else
			            {
			                expectedNum += 0.000001f;
			            }
			            return (float.Parse(value.ToString()) / expectedNum);
			        }
			        return 0;

			    case ComparisonType.GreaterOrEqual:
					if (comparisonResult >= 0)
					{
						return 1;
					}
			        if (!(comparisonResult >= 0) && (dataType == GameDataType.String || dataType == GameDataType.Boolean))
			        {
			            return 0;
			        }
			        if ((float.TryParse(expected.ToString(), out expectedNum)))
			        {
			            return (float.Parse(value.ToString()) / expectedNum);
			        }
			        return 0;

			    case ComparisonType.Less:
					return comparisonResult < 0 ? 1 : 0;

				case ComparisonType.LessOrEqual:
					return comparisonResult <= 0 ? 1 : 0;

				default:
					throw new NotImplementedException($"There is no case for the comparison type: {comparisonType}.");
			}
		}
	}
}
