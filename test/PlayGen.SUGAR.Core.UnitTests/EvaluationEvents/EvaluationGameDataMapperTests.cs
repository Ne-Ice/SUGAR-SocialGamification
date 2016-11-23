﻿using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Data.Model;
using Xunit;

namespace PlayGen.SUGAR.Core.UnitTests.EvaluationEvents
{
    //[Collection("Project Fixture Collection")]
    public class EvaluationGameDataMapperTests : TestsBase
    {
        [Fact]
        public void CreateAndGetRelated()
        {
            // Arrange
            var count = 10;
            var shouldGetEvaluations = new List<Evaluation>(count);
            var shouldntGetEvaluations = new List<Evaluation>(count);

            for (var i = 0; i < count; i++)
            {
               shouldGetEvaluations.Add(Helpers.ComposeEvaluation($"MapsExistingEvaluationsGameData_{i}"));
                shouldntGetEvaluations.Add(Helpers.ComposeEvaluation($"MapsExistingEvaluationsGameData_Ignore_{i}"));
            }

            // Act
            GameDataMapper.CreateMappings(shouldGetEvaluations);
            GameDataMapper.CreateMappings(shouldntGetEvaluations);

            // Assert
            foreach (var shouldGetEvaluation in shouldGetEvaluations)
            {
                foreach (var evaluationCriteria in shouldGetEvaluation.EvaluationCriterias)
                {
                    var gameData = Helpers.ComposeGameData(evaluationCriteria, shouldGetEvaluation.GameId);

                    HashSet<Evaluation> relatedEvaluations;
                    var didGetRelated = GameDataMapper.TryGetRelated(gameData, out relatedEvaluations);

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
            var count = 10;
            var removeEvaluation = Helpers.ComposeEvaluation($"RemovesEvaluationsGameDataMapping");

            var shouldntRemoveEvaluations = new List<Evaluation>(count);

            for (var i = 0; i < count; i++)
            {
                shouldntRemoveEvaluations.Add(Helpers.ComposeEvaluation($"RemovesEvaluationsGameDataMapping_ShouldntRemove_{i}"));
            }

            GameDataMapper.CreateMapping(removeEvaluation);
            GameDataMapper.CreateMappings(shouldntRemoveEvaluations);

            // Act
            GameDataMapper.RemoveMapping(removeEvaluation);

            // Assert
            // Make sure removed evaluation isn't returned
            foreach (var evaluationCriteria in removeEvaluation.EvaluationCriterias)
            {
                var gameData = Helpers.ComposeGameData(evaluationCriteria, removeEvaluation.GameId);

                HashSet<Evaluation> relatedEvaluations;
                var didGetRelated = GameDataMapper.TryGetRelated(gameData, out relatedEvaluations);

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
                    var gameData = Helpers.ComposeGameData(evaluationCriteria, shouldntRemoveEvaluation.GameId);

                    HashSet<Evaluation> relatedEvaluations;
                    var didGetRelated = GameDataMapper.TryGetRelated(gameData, out relatedEvaluations);

                    Assert.True(didGetRelated, "Shouldn't have removed unremoved evaluations.");
                    Assert.Contains(shouldntRemoveEvaluation, relatedEvaluations);
                }
            }
        }
    }
}