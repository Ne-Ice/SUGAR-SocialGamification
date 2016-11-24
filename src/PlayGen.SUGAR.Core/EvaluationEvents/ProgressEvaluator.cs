﻿// todo v2.0: support group evaluations
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PlayGen.SUGAR.Data.Model;
using System.Linq;
using PlayGen.SUGAR.Core.Sessions;

namespace PlayGen.SUGAR.Core.EvaluationEvents
{
	public class ProgressEvaluator
	{
		private readonly CriteriaEvaluator _evaluationCriteriaEvaluator;

		public ProgressEvaluator(CriteriaEvaluator evaluationCriteriaEvaluator)
		{
			_evaluationCriteriaEvaluator = evaluationCriteriaEvaluator;
		}

		public ProgressCache EvaluateActor(IEnumerable<Evaluation> evaluations, Session session)
		{
			var progress = new ProgressCache();

			foreach (var evaluation in evaluations)
			{
				AddProgress(progress, evaluation, session);
			}

			return progress;
		}

		public ProgressCache EvaluateSessions(IEnumerable<Session> sessions, Evaluation evaluation)
		{
			var progress = new ProgressCache();

			foreach (var session in sessions)
			{
				AddProgress(progress, evaluation, session);
			}

			return progress;
		}

		public ProgressCache EvaluateSessions(IEnumerable<Session> sessions, IEnumerable<Evaluation> evaluations)
		{
			var progress = new ProgressCache();

			foreach (var session in sessions)
			{
				foreach (var evaluation in evaluations)
				{
					AddProgress(progress, evaluation, session);
				}
			}

			return progress;
		}

		private void AddProgress(ProgressCache progress, Evaluation evaluation, Session session)
		{
			var progressValue = _evaluationCriteriaEvaluator.IsCriteriaSatisified(evaluation.GameId, session.Actor.Id,
				evaluation.EvaluationCriterias, session.Actor.ActorType);
			progress.AddProgress(session.GameId, session.Actor.Id, evaluation, progressValue);
		}
	}
}
