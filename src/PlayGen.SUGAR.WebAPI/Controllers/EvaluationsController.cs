﻿using Microsoft.AspNetCore.Mvc;
using PlayGen.SUGAR.Contracts.Shared;
using PlayGen.SUGAR.Core.EvaluationEvents;
using PlayGen.SUGAR.WebAPI.Extensions;
using PlayGen.SUGAR.WebAPI.Filters;

namespace PlayGen.SUGAR.WebAPI.Controllers
{
    // todo replace the skill and achievement controllers with this one and just specify 2 api routes for this class?
    public abstract class EvaluationsController : Controller
    {
        protected readonly Core.Controllers.EvaluationController EvaluationCoreController;
        private readonly EvaluationTracker _evaluationTracker;

        protected EvaluationsController(Core.Controllers.EvaluationController evaluationCoreController, EvaluationTracker evaluationTracker)
        {
            EvaluationCoreController = evaluationCoreController;
            _evaluationTracker = evaluationTracker;
        }

        protected IActionResult Get(string token, int? gameId)
        {
            var evaluation = EvaluationCoreController.Get(token, gameId);
            var evaluationContract = evaluation.ToContract();
            return new ObjectResult(evaluationContract);
        }

        protected IActionResult Get(int? gameId)
        {
            var evaluation = EvaluationCoreController.GetByGame(gameId);
            var evaluationContract = evaluation.ToContractList();
            return new ObjectResult(evaluationContract);
        }

        protected IActionResult GetGameProgress(int gameId, int? actorId)
        {
            // todo: should this be taken from the progress cache?
            var evaluationsProgress = EvaluationCoreController.GetGameProgress(gameId, actorId);
            var evaluationsProgressResponses = evaluationsProgress.ToContractList();
            return new ObjectResult(evaluationsProgressResponses);
        }

        protected IActionResult GetEvaluationProgress(string token, int? gameId, int? actorId)
        {
            // todo: should this be taken from the progress cache?
            var evaluation = EvaluationCoreController.Get(token, gameId);
            var progress = EvaluationCoreController.EvaluateProgress(evaluation, actorId);
            return new ObjectResult(new EvaluationProgressResponse
            {
                Name = evaluation.Name,
                Progress = progress,
            });
        }

        protected void Delete(string token, int? gameId)
        {
            EvaluationCoreController.Delete(token, gameId);
        }
    }
}