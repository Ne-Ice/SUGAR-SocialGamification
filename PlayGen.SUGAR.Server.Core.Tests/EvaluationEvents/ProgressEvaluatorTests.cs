﻿using System.Collections.Generic;
using PlayGen.SUGAR.Server.Core.EvaluationEvents;
using PlayGen.SUGAR.Server.Core.Sessions;
using PlayGen.SUGAR.Server.Model;
using Xunit;

namespace PlayGen.SUGAR.Server.Core.Tests.EvaluationEvents
{
    public class ProgressEvaluatorTests
	{
		private readonly ProgressEvaluator _progressEvaluator;

		public ProgressEvaluatorTests()
		{
			_progressEvaluator = new ProgressEvaluator(ControllerLocator.EvaluationController);
		}

		[Fact]
		public void CanEvaluateActor()
        {
            // Assign
            var evaluationCount = 2;
            var session = new Session(null, Helpers.GetOrCreateUser("CanEvaluateActor").Id);

            var evaluations = new List<Evaluation>();

            for (var i = 0; i < evaluationCount; i++)
            {
                var evaluation = Helpers.CreateGenericAchievement($"CanEvaluateActorComplete_{i}", session.GameId);
                Helpers.CompleteGenericAchievement(evaluation, session.ActorId);
                evaluations.Add(evaluation);
            }

            // Act
            var progress = _progressEvaluator.EvaluateActor(evaluations, session);

            // Assert
            var actorProgress = progress.GetActorProgress(session.GameId, session.ActorId);
            Assert.True(evaluations.Count <= actorProgress.Count); // Should have a progress value for each evaluation

            foreach (var evaluation in evaluations)
            {
                Assert.Contains(evaluation, actorProgress.Keys); // Each evaluation should be returned in the progress
                Assert.True(actorProgress[evaluation] > 0);      // Make sure actually processed progress
            }
        }
	}
}