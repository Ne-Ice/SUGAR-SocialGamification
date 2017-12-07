﻿using System.Collections.Generic;

using PlayGen.SUGAR.Common.Authorization;
using PlayGen.SUGAR.Server.Core.EvaluationEvents;
using PlayGen.SUGAR.Server.Model;
using Xunit;

namespace PlayGen.SUGAR.Server.Core.Tests.EvaluationEvents
{
    public class EvaluationGameDataMapperEvaluationTests : EvaluationTestsBase
    {
        [Fact]
        public void CreateAndGetRelated()
        {
            // Arrange
            var gameDataMapper = new EvaluationDataMapper();
            var count = 10;
            var shouldGetEvaluations = new List<Evaluation>(count);
            var shouldntGetEvaluations = new List<Evaluation>(count);

            for (var i = 0; i < count; i++)
            {
                shouldGetEvaluations.Add(Helpers.ComposeGenericAchievement($"MapsExistingEvaluationsGameData_{i}", Platform.GlobalId));
                shouldntGetEvaluations.Add(Helpers.ComposeGenericAchievement($"MapsExistingEvaluationsGameData_Ignore_{i}", Platform.GlobalId));
            }

            // Act
            gameDataMapper.CreateMappings(shouldGetEvaluations);
            gameDataMapper.CreateMappings(shouldntGetEvaluations);

            // Assert
            foreach (var shouldGetEvaluation in shouldGetEvaluations)
            {
                foreach (var evaluationCriteria in shouldGetEvaluation.EvaluationCriterias)
                {
                    var gameData = Helpers.ComposeEvaluationData(0, evaluationCriteria, shouldGetEvaluation.GameId);

                    var didGetRelated = gameDataMapper.TryGetRelated(gameData, out var relatedEvaluations);

                    Assert.True(didGetRelated, "Should have gotten related evaluations.");
                    Assert.Contains(shouldGetEvaluation, relatedEvaluations);

                    shouldntGetEvaluations.ForEach(sge => Assert.DoesNotContain(sge, relatedEvaluations));
                }
            }
        }

        [Fact]
        public void RemovesEvaluationsGameDataMapping()
        {
            // Arrange
            var gameDataMapper = new EvaluationDataMapper();
            var count = 10;
            var removeEvaluation = Helpers.ComposeGenericAchievement($"RemovesEvaluationsGameDataMapping", Platform.GlobalId);

            var shouldntRemoveEvaluations = new List<Evaluation>(count);

            for (var i = 0; i < count; i++)
            {
                shouldntRemoveEvaluations.Add(Helpers.ComposeGenericAchievement($"RemovesEvaluationsGameDataMapping_ShouldntRemove_{i}", Platform.GlobalId));
            }

            gameDataMapper.CreateMapping(removeEvaluation);
            gameDataMapper.CreateMappings(shouldntRemoveEvaluations);

            // Act
            gameDataMapper.RemoveMapping(removeEvaluation);

            // Assert
            // Make sure removed evaluation isn't returned
            foreach (var evaluationCriteria in removeEvaluation.EvaluationCriterias)
            {
                var gameData = Helpers.ComposeEvaluationData(0, evaluationCriteria, removeEvaluation.GameId);

                var didGetRelated = gameDataMapper.TryGetRelated(gameData, out var relatedEvaluations);

                // Either shouldn't have gotten related or if did, shouldn't have returned the removed evaluation
                if (didGetRelated)
                {
                    Assert.DoesNotContain(removeEvaluation, relatedEvaluations);
                }
            }

            // Make sure other evaluations still exist
            foreach (var shouldntRemoveEvaluation in shouldntRemoveEvaluations)
            {
                foreach (var evaluationCriteria in shouldntRemoveEvaluation.EvaluationCriterias)
                {
                    var gameData = Helpers.ComposeEvaluationData(0, evaluationCriteria, shouldntRemoveEvaluation.GameId);

                    var didGetRelated = gameDataMapper.TryGetRelated(gameData, out var relatedEvaluations);

                    Assert.True(didGetRelated, "Shouldn't have removed unremoved evaluations.");
                    Assert.Contains(shouldntRemoveEvaluation, relatedEvaluations);
                }
            }
        }
    }
}