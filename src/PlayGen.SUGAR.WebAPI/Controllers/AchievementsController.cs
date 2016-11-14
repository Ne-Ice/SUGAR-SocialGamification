﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PlayGen.SUGAR.Authorization;
using PlayGen.SUGAR.Common.Shared.Permissions;
using PlayGen.SUGAR.Contracts.Shared;
using PlayGen.SUGAR.Core.Controllers;
using PlayGen.SUGAR.Core.Utilities;
using PlayGen.SUGAR.Data.Model;
using PlayGen.SUGAR.WebAPI.Extensions;
using PlayGen.SUGAR.WebAPI.Filters;

namespace PlayGen.SUGAR.WebAPI.Controllers
{
	/// <summary>
	/// Web Controller that facilitates Achievement specific operations.
	/// </summary>
	[Route("api/[controller]")]
	public class AchievementsController : Controller
	{
        private readonly IAuthorizationService _authorizationService;
        private readonly EvaluationController _evaluationCoreController;

		public AchievementsController(EvaluationController evaluationController,
                    IAuthorizationService authorizationService)
		{
            _evaluationCoreController = evaluationController;
            _authorizationService = authorizationService;
        }

		/// <summary>
		/// Find an Achievement that matches <param name="token"/> and <param name="gameId"/>.
		/// 
		/// Example Usage: GET api/achievements/find/ACHIEVEMENT_TOKEN/1
		/// </summary>
		/// <param name="token">Token of Achievement</param>
		/// <param name="gameId">ID of the Game the Achievement is for</param>
		/// <returns>Returns <see cref="EvaluationResponse"/> that holds Achievement details</returns>
		[HttpGet("find/{token}/{gameId:int}")]
		[HttpGet("find/{token}/global")]
        //[ResponseType(typeof(EvaluationResponse))]
        [Authorization(ClaimScope.Game, AuthorizationOperation.Get, AuthorizationOperation.Achievement)]
        public IActionResult Get([FromRoute]string token, [FromRoute]int? gameId)
		{
            if (_authorizationService.AuthorizeAsync(User, gameId, (AuthorizationRequirement)HttpContext.Items["Requirements"]).Result)
            {
                var achievement = _evaluationCoreController.Get(token, gameId);
                var achievementContract = achievement.ToContract();
                return new ObjectResult(achievementContract);
            }
            return Unauthorized();
		}

		/// <summary>
		/// Find a list of Achievements that match <param name="gameId"/>.
		/// If global is provided instead of a gameId, get all global achievements, ie. achievements that are not associated with a specific game.
		/// 
		/// Example Usage: GET api/achievements/game/1/list
		/// </summary>
		/// <param name="gameId">Game ID</param>
		/// <returns>Returns multiple <see cref="EvaluationResponse"/> that hold Achievement details</returns>
		[HttpGet("global/list")]
		[HttpGet("game/{gameId:int}/list")]
        //[ResponseType(typeof(IEnumerable<EvaluationResponse>))]
        [Authorization(ClaimScope.Game, AuthorizationOperation.Get, AuthorizationOperation.Achievement)]
        public IActionResult Get([FromRoute]int? gameId)
		{
            if (_authorizationService.AuthorizeAsync(User, gameId, (AuthorizationRequirement)HttpContext.Items["Requirements"]).Result)
            {
                var achievement = _evaluationCoreController.GetByGame(gameId);
                var achievementContract = achievement.ToContractList();
                return new ObjectResult(achievementContract);
            }
            return Unauthorized();
        }

		/// <summary>
		/// Find the current progress for all achievements for a <param name="gameId"/> for <param name="actorId"/>.
		/// 
		/// Example Usage: GET api/achievements/game/1/evaluate/1
		/// </summary>
		/// <param name="gameId">ID of Game</param>
		/// <param name="actorId">ID of Group/User</param>
		/// <returns>Returns multiple <see cref="EvaluationProgressResponse"/> that hold current progress toward achievement.</returns>
		[HttpGet("game/{gameId:int}/evaluate")]
		[HttpGet("global/evaluate")]
		[HttpGet("game/{gameId:int}/evaluate/{actorId:int}")]
		[HttpGet("global/evaluate/{actorId:int}")]
		//[ResponseType(typeof(IEnumerable<EvaluationProgressResponse>))]
		public IActionResult GetGameProgress([FromRoute]int gameId, [FromRoute]int? actorId)
		{
			var achievementsProgress = _evaluationCoreController.GetGameProgress(gameId, actorId);
		    var achievementsProgressResponses = achievementsProgress.ToContractList();
            return new ObjectResult(achievementsProgressResponses);
		}

		/// <summary>
		/// Find the current progress for an Achievement for <param name="actorId"/>.
		/// 
		/// Example Usage: GET api/achievements/ACHIEVEMENT_TOKEN/1/evaluate/1
		/// </summary>
		/// <param name="token">Token of Achievement</param>
		/// <param name="gameId">ID of the Game the Achievement is for</param>
		/// <param name="actorId">ID of Group/User</param>
		/// <returns>Returns multiple <see cref="EvaluationProgressResponse"/> that hold current progress toward achievement.</returns>
		[HttpGet("{token}/{gameId:int}/evaluate")]
		[HttpGet("{token}/global/evaluate")]
		[HttpGet("{token}/{gameId:int}/evaluate/{actorId:int}")]
		[HttpGet("{token}/global/evaluate/{actorId:int}")]
		//[ResponseType(typeof(EvaluationProgressResponse))]
		public IActionResult GetAchievementProgress([FromRoute]string token, [FromRoute]int? gameId, [FromRoute]int? actorId)
		{
			var achievement = _evaluationCoreController.Get(token, gameId);
			var progress = _evaluationCoreController.EvaluateProgress(achievement, actorId);
			return new ObjectResult(new EvaluationProgressResponse
			{
				Name = achievement.Name,
				Progress = progress,
			});
		}

		/// <summary>
		/// Create a new Achievement.
		/// Requires <see cref="EvaluationRequest.Name"/> to be unique to that <see cref="EvaluationRequest.GameId"/>.
		/// 
		/// Example Usage: POST api/achievements/create
		/// </summary>
		/// <param name="newAchievement"><see cref="EvaluationRequest"/> object that holds the details of the new Achievement.</param>
		/// <returns>Returns a <see cref="EvaluationResponse"/> object containing details for the newly created Achievement.</returns>
		[HttpPost("create")]
		//[ResponseType(typeof(EvaluationResponse))]
		[ArgumentsNotNull]
        [Authorization(ClaimScope.Game, AuthorizationOperation.Create, AuthorizationOperation.Achievement)]
        public IActionResult Create([FromBody] EvaluationCreateRequest newAchievement)
		{
            if (_authorizationService.AuthorizeAsync(User, newAchievement.GameId, (AuthorizationRequirement)HttpContext.Items["Requirements"]).Result)
            {
                var achievement = newAchievement.ToAchievementModel();
                achievement = (Achievement)_evaluationCoreController.Create(achievement);
                var achievementContract = achievement.ToContract();
                return new ObjectResult(achievementContract);
            }
            return Unauthorized();
        }

		/// <summary>
		/// Update an existing Achievement.
		/// 
		/// Example Usage: PUT api/achievements/update
		/// </summary>
		/// <param name="achievement"><see cref="EvaluationRequest"/> object that holds the details of the Achievement.</param>
		[HttpPut("update")]
		[ArgumentsNotNull]
        [Authorization(ClaimScope.Game, AuthorizationOperation.Update, AuthorizationOperation.Achievement)]
        public void Update([FromBody] EvaluationUpdateRequest achievement)
		{
            if (_authorizationService.AuthorizeAsync(User, achievement.GameId, (AuthorizationRequirement)HttpContext.Items["Requirements"]).Result)
            {
                var achievementModel = achievement.ToAchievementModel();
                _evaluationCoreController.Update(achievementModel);
            }
		}

		/// <summary>
		/// Delete Achievement with the <param name="token"/> and <param name="gameId"/> provided.
		/// 
		/// Example Usage: DELETE api/achievements/ACHIEVEMENT_TOKEN/1
		/// </summary>
		/// <param name="token">Token of Achievement</param>
		/// <param name="gameId">ID of the Game the Achievement is for</param>
		[HttpDelete("{token}/global")]
		[HttpDelete("{token}/{gameId:int}")]
        [Authorization(ClaimScope.Game, AuthorizationOperation.Delete, AuthorizationOperation.Achievement)]
        public void Delete([FromRoute]string token, [FromRoute]int? gameId)
		{
            if (_authorizationService.AuthorizeAsync(User, gameId, (AuthorizationRequirement)HttpContext.Items["Requirements"]).Result)
            {
                _evaluationCoreController.Delete(token, gameId);
            }
		}
	}
}