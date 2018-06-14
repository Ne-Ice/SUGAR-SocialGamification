﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Logging;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Server.Core.Controllers;
using PlayGen.SUGAR.Server.EntityFramework;
using PlayGen.SUGAR.Server.Model;
using EvaluationCriteria = PlayGen.SUGAR.Server.Model.EvaluationCriteria;

namespace PlayGen.SUGAR.Server.Core.EvaluationEvents
{
	/// <summary>
	/// Evaluates evaluation criteria.
	/// </summary>
	// Values ensured to not be nulled by model validation
	[SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
	public class CriteriaEvaluator
	{
		protected readonly RelationshipController RelationshipCoreController;
		protected readonly SUGARContextFactory ContextFactory;

		protected ILogger<EvaluationDataController> EvaluationDataLogger;

		public CriteriaEvaluator(
			ILogger<EvaluationDataController> evaluationDataLogger,
			SUGARContextFactory contextFactory, 
			RelationshipController relationshipCoreController)
		{
			EvaluationDataLogger = evaluationDataLogger;
			ContextFactory = contextFactory;
			RelationshipCoreController = relationshipCoreController;
		}

		// The method of returning calculating the progress (for multiple criteria conditions) and 
		// how the progress is going to be represented (0f to 1f ?) need to be determined first.
		public float IsCriteriaSatisified(int gameId, int actorId, List<EvaluationCriteria> completionCriterias, ActorType actorType, EvaluationType evaluationType)
		{
			return completionCriterias.Sum(cc => Evaluate(gameId, actorId, cc, actorType, evaluationType)) / completionCriterias.Count;
		}

		protected float Evaluate(int gameId, int actorId, EvaluationCriteria completionCriteria, ActorType actorType, EvaluationType evaluationType)
		{
			switch (completionCriteria.Scope)
			{
				case CriteriaScope.Actor:
					switch (completionCriteria.EvaluationDataType)
					{
						case EvaluationDataType.Boolean:
							return EvaluateBool(gameId, actorId, completionCriteria);

						case EvaluationDataType.String:
							return EvaluateString(gameId, actorId, completionCriteria);

						case EvaluationDataType.Float:
							return EvaluateFloat(gameId, actorId, completionCriteria);

						case EvaluationDataType.Long:
							return EvaluateLong(gameId, actorId, completionCriteria);

						default:
							return 0;
					}
				case CriteriaScope.RelatedUsers:
					var relatedUsers = RelationshipCoreController.GetRelationships(actorId, ActorType.User).Select(a => a.Id).ToList();
					if (actorType == ActorType.User)
					{
						relatedUsers.Add(actorId);
						relatedUsers = relatedUsers.Distinct().ToList();
					}
					switch (completionCriteria.EvaluationDataType)
					{
						case EvaluationDataType.Boolean:
							return EvaluateManyBool(gameId, relatedUsers, completionCriteria);

						case EvaluationDataType.String:
							return EvaluateManyString(gameId, relatedUsers, completionCriteria);

						case EvaluationDataType.Float:
							return EvaluateManyFloat(gameId, relatedUsers, completionCriteria);

						case EvaluationDataType.Long:
							return EvaluateManyLong(gameId, relatedUsers, completionCriteria);

						default:
							return 0;
					}
				case CriteriaScope.RelatedGroups:
					if (actorType == ActorType.User)
					{
						throw new NotImplementedException("RelatedGroups Scope is only implemented for groups");
					}
					var relatedGroups = RelationshipCoreController.GetRelationships(actorId, ActorType.Group).Select(a => a.Id).ToList();
					relatedGroups.Add(actorId);
					relatedGroups = relatedGroups.Distinct().ToList();
					switch (completionCriteria.EvaluationDataType)
					{
						case EvaluationDataType.Boolean:
							return EvaluateManyBool(gameId, relatedGroups, completionCriteria);

						case EvaluationDataType.String:
							return EvaluateManyString(gameId, relatedGroups, completionCriteria);

						case EvaluationDataType.Float:
							return EvaluateManyFloat(gameId, relatedGroups, completionCriteria);

						case EvaluationDataType.Long:
							return EvaluateManyLong(gameId, relatedGroups, completionCriteria);

						default:
							return 0;
					}
				case CriteriaScope.RelatedGroupUsers:
					if (actorType == ActorType.User)
					{
						throw new NotImplementedException("RelatedGroupUsers Scope is only implemented for groups");
					}
					var groups = RelationshipCoreController.GetRelationships(actorId, ActorType.Group).Select(a => a.Id).ToList();
					groups.Add(actorId);
					groups = groups.Distinct().ToList();
					var relatedGroupUsers = groups.SelectMany(g => RelationshipCoreController.GetRelationships(g, ActorType.User).Select(a => a.Id)).Distinct().ToList();
					switch (completionCriteria.EvaluationDataType)
					{
						case EvaluationDataType.Boolean:
							return EvaluateManyBool(gameId, relatedGroupUsers, completionCriteria);

						case EvaluationDataType.String:
							return EvaluateManyString(gameId, relatedGroupUsers, completionCriteria);

						case EvaluationDataType.Float:
							return EvaluateManyFloat(gameId, relatedGroupUsers, completionCriteria);

						case EvaluationDataType.Long:
							return EvaluateManyLong(gameId, relatedGroupUsers, completionCriteria);

						default:
							return 0;
					}
				default:
					return 0;
			}
		}

		protected float EvaluateLong(int gameId, int actorId, EvaluationCriteria completionCriteria)
		{
			var evaluationDataController = new EvaluationDataController(EvaluationDataLogger, ContextFactory, completionCriteria.EvaluationDataCategory);

			switch (completionCriteria.CriteriaQueryType)
			{
				case CriteriaQueryType.Any:
					var any = evaluationDataController.List(gameId, actorId, completionCriteria.EvaluationDataKey, EvaluationDataType.Long);
					return !any.Any() ? 0 : any.Max(value => CompareValues(long.Parse(value.Value), long.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.EvaluationDataType));

				case CriteriaQueryType.Sum:
					var sum = evaluationDataController.SumLong(gameId, actorId, completionCriteria.EvaluationDataKey, EvaluationDataType.Long);
					return CompareValues(sum, long.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.EvaluationDataType);

				case CriteriaQueryType.Latest:
					if (!evaluationDataController.TryGetLatest(gameId, actorId, completionCriteria.EvaluationDataKey, out var latest, EvaluationDataType.Long))
					{
						return 0;
					}

					return CompareValues(long.Parse(latest.Value), long.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.EvaluationDataType);
				default:
					return 0;
			}
		}

		protected float EvaluateFloat(int gameId, int actorId, EvaluationCriteria completionCriteria)
		{
			var evaluationDataController = new EvaluationDataController(EvaluationDataLogger, ContextFactory, completionCriteria.EvaluationDataCategory);

			switch (completionCriteria.CriteriaQueryType)
			{
				case CriteriaQueryType.Any:
					var any = evaluationDataController.List(gameId, actorId, completionCriteria.EvaluationDataKey, EvaluationDataType.Float);

					return !any.Any() ? 0 : any.Max(value => CompareValues(float.Parse(value.Value), float.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.EvaluationDataType));

				case CriteriaQueryType.Sum:
					var sum = evaluationDataController.SumFloat(gameId, actorId, completionCriteria.EvaluationDataKey, EvaluationDataType.Float);
					return CompareValues(sum, float.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.EvaluationDataType);

				case CriteriaQueryType.Latest:
					if (!evaluationDataController.TryGetLatest(gameId, actorId, completionCriteria.EvaluationDataKey, out var latest, EvaluationDataType.Float))
					{
						return 0;
					}

					return CompareValues(float.Parse(latest.Value), float.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.EvaluationDataType);
				default:
					return 0;
			}
		}

		protected float EvaluateString(int gameId, int actorId, EvaluationCriteria completionCriteria)
		{
			var evaluationDataController = new EvaluationDataController(EvaluationDataLogger, ContextFactory, completionCriteria.EvaluationDataCategory);

			switch (completionCriteria.CriteriaQueryType)
			{
				case CriteriaQueryType.Any:
					var any = evaluationDataController.List(gameId, actorId, completionCriteria.EvaluationDataKey, EvaluationDataType.String);

					return !any.Any() ? 0 : any.Max(value => CompareValues(value.Value, completionCriteria.Value, completionCriteria.ComparisonType, completionCriteria.EvaluationDataType));
				case CriteriaQueryType.Latest:
					if (!evaluationDataController.TryGetLatest(gameId, actorId, completionCriteria.EvaluationDataKey, out var latest, EvaluationDataType.String))
					{
						return 0;
					}

					return CompareValues(latest.Value, completionCriteria.Value, completionCriteria.ComparisonType, completionCriteria.EvaluationDataType);
				default:
					return 0;
			}
		}

		protected float EvaluateBool(int gameId, int actorId, EvaluationCriteria completionCriteria)
		{
			var evaluationDataController = new EvaluationDataController(EvaluationDataLogger, ContextFactory, completionCriteria.EvaluationDataCategory);

			switch (completionCriteria.CriteriaQueryType)
			{
				case CriteriaQueryType.Any:
					var any = evaluationDataController.List(gameId, actorId, completionCriteria.EvaluationDataKey, EvaluationDataType.Boolean);

					return !any.Any() ? 0 : any.Max(value => CompareValues(bool.Parse(value.Value), bool.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.EvaluationDataType));
				case CriteriaQueryType.Latest:
					if (!evaluationDataController.TryGetLatest(gameId, actorId, completionCriteria.EvaluationDataKey, out var latest, EvaluationDataType.Boolean))
					{
						return 0;
					}

					return CompareValues(bool.Parse(latest.Value), bool.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.EvaluationDataType);
				default:
					return 0;
			}
		}

		protected float EvaluateManyLong(int gameId, List<int> actors, EvaluationCriteria completionCriteria)
		{
			var evaluationDataController = new EvaluationDataController(EvaluationDataLogger, ContextFactory, completionCriteria.EvaluationDataCategory);

			switch (completionCriteria.CriteriaQueryType)
			{
				case CriteriaQueryType.Sum:
					var totalSum = actors.Sum(a => evaluationDataController.SumLong(gameId, a, completionCriteria.EvaluationDataKey, EvaluationDataType.Long));

					return CompareValues(totalSum, long.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.EvaluationDataType);
				default:
					return 0;
			}
		}

		protected float EvaluateManyFloat(int gameId, List<int> actors, EvaluationCriteria completionCriteria)
		{
			var evaluationDataController = new EvaluationDataController(EvaluationDataLogger, ContextFactory, completionCriteria.EvaluationDataCategory);
			
			switch (completionCriteria.CriteriaQueryType)
			{
				case CriteriaQueryType.Sum:
					var totalSum = actors.Sum(a => evaluationDataController.SumFloat(gameId, a, completionCriteria.EvaluationDataKey, EvaluationDataType.Float));

					return CompareValues(totalSum, float.Parse(completionCriteria.Value), completionCriteria.ComparisonType, completionCriteria.EvaluationDataType);
				default:
					return 0;
			}
		}

		protected float EvaluateManyString(int gameId, List<int> actors, EvaluationCriteria completionCriteria)
		{
			return 0;
		}

		protected float EvaluateManyBool(int gameId, List<int> actors, EvaluationCriteria completionCriteria)
		{
			return 0;
		}

		protected static float CompareValues<T>(T value, T expected, ComparisonType comparisonType, EvaluationDataType dataType) where T : IComparable
		{
			var comparisonResult = value.CompareTo(expected);

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
					if (!(comparisonResult > 0) && (dataType == EvaluationDataType.String || dataType == EvaluationDataType.Boolean))
					{
						return 0;
					}
					if (float.TryParse(expected.ToString(), out var expectedGreaterNum)) {
						if (dataType == EvaluationDataType.Long)
						{
							expectedGreaterNum += 1;
						} else
						{
							expectedGreaterNum += 0.000001f;
						}
						return float.Parse(value.ToString()) / expectedGreaterNum;
					}
					return 0;

				case ComparisonType.GreaterOrEqual:
					if (comparisonResult >= 0)
					{
						return 1;
					}
					if (!(comparisonResult >= 0) && (dataType == EvaluationDataType.String || dataType == EvaluationDataType.Boolean))
					{
						return 0;
					}
					if (float.TryParse(expected.ToString(), out var expectedGreaterEqualNum))
					{
						return float.Parse(value.ToString()) / expectedGreaterEqualNum;
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
