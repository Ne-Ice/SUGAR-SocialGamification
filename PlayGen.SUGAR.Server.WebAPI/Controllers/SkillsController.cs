﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Common.Authorization;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Server.Authorization;
using PlayGen.SUGAR.Server.Core.EvaluationEvents;
using PlayGen.SUGAR.Server.Model;
using PlayGen.SUGAR.Server.WebAPI.Attributes;
using PlayGen.SUGAR.Server.WebAPI.Extensions;

namespace PlayGen.SUGAR.Server.WebAPI.Controllers
{
	/// <summary>
	/// Web Controller that facilitates Skill specific operations.
	/// </summary>
	[Route("api/[controller]")]
	[Authorize("Bearer")]
	public class SkillsController : EvaluationsController
	{
		public SkillsController(Core.Controllers.EvaluationController evaluationCoreController, EvaluationTracker evaluationTracker, IAuthorizationService authorizationService)
			: base(evaluationCoreController, evaluationTracker, authorizationService)
		{
		}

		/// <summary>
		/// Find a Skill that matches the token and gameId provided.
		/// If global is provided instead of a gameId, get the global Skill that is not associated with a specific game with the provided token.
		/// </summary>
		/// <param name="token">Token of Skill</param>
		/// <param name="gameId">ID of the Game the Skill is for</param>
		/// <returns>Returns <see cref="EvaluationResponse"/> that holds Skill details</returns>
		[HttpGet("find/{token}/{gameId:int}")]
		[HttpGet("find/{token}/global")]
		[Authorization(ClaimScope.Game, AuthorizationAction.Get, AuthorizationEntity.Achievement)]
		public Task<IActionResult> Get([FromRoute]string token, [FromRoute]int gameId)
		{
			return Get(token, gameId, EvaluationType.Skill);
		}

		/// <summary>
		/// Find a list of Skills for the gameId provided.
		/// If global is provided instead of a gameId, get all global skills, ie. skills that are not associated with a specific game.
		/// </summary>
		/// <param name="gameId">Game ID</param>
		/// <returns>Returns multiple <see cref="EvaluationResponse"/> that hold Skill details</returns>
		[HttpGet("global/list")]
		[HttpGet("game/{gameId:int}/list")]
		[Authorization(ClaimScope.Game, AuthorizationAction.Get, AuthorizationEntity.Achievement)]
		public Task<IActionResult> Get([FromRoute]int gameId)
		{
			return Get(gameId, EvaluationType.Skill);
		}

		/// <summary>
		/// Find the current progress for all skills for a gameId for actorId.
		/// If global is provided instead of a gameId, get all progress for global skills, ie. skills that are not associated with a specific game.
		/// </summary>
		/// <param name="gameId">ID of Game</param>
		/// <param name="actorId">ID of Group/User</param>
		/// <returns>Returns multiple <see cref="EvaluationProgressResponse"/> that hold current progress toward skill.</returns>
		[HttpGet("game/{gameId:int}/evaluate/{actorId:int}")]
		[HttpGet("global/evaluate/{actorId:int}")]
		public IActionResult GetGameProgress([FromRoute]int gameId, [FromRoute]int actorId)
		{
			return GetGameProgress(gameId, actorId, EvaluationType.Skill);
		}

		/// <summary>
		/// Find the current progress for the Skill with the provided token and gameId for the actorId.
		/// If global is provided instead of a gameId, get progress for a global skill with the token provided, ie. a skill that is not associated with a specific game.
		/// </summary>
		/// <param name="token">Token of Skill</param>
		/// <param name="gameId">ID of the Game the Skill is for</param>
		/// <param name="actorId">ID of Group/User</param>
		/// <returns>Returns multiple <see cref="EvaluationProgressResponse"/> that hold current progress toward skill.</returns>
		[HttpGet("{token}/{gameId:int}/evaluate/{actorId:int}")]
		[HttpGet("{token}/global/evaluate/{actorId:int}")]
		public IActionResult GetSkillProgress([FromRoute]string token, [FromRoute]int gameId, [FromRoute]int actorId)
		{
			return GetEvaluationProgress(token, gameId, actorId, EvaluationType.Skill);
		}

		/// <summary>
		/// Create a new Skill.
		/// Requires <see cref="EvaluationCreateRequest"/> Name to be unique to that GameId.
		/// </summary>
		/// <param name="newSkill"><see cref="EvaluationCreateRequest"/> object that holds the details of the new Skill.</param>
		/// <returns>Returns a <see cref="EvaluationResponse"/> object containing details for the newly created Skill.</returns>
		[HttpPost("create")]
		[ArgumentsNotNull]
		[Authorization(ClaimScope.Game, AuthorizationAction.Create, AuthorizationEntity.Achievement)]
		public async Task<IActionResult> Create([FromBody] EvaluationCreateRequest newSkill)
		{
			if ((await _authorizationService.AuthorizeAsync(User, newSkill.GameId, HttpContext.ScopeItems(ClaimScope.Game))).Succeeded)
			{
				var skill = newSkill.ToSkillModel();
				skill = (Skill)EvaluationCoreController.Create(skill);
				var achievementContract = skill.ToContract();
				return new ObjectResult(achievementContract);
			}
			return Forbid();
		}

		/// <summary>
		/// Update an existing Skill.
		/// </summary>
		/// <param name="skill"><see cref="EvaluationCreateRequest"/> object that holds the details of the Skill.</param>
		[HttpPut("update")]
		[ArgumentsNotNull]
		[Authorization(ClaimScope.Game, AuthorizationAction.Update, AuthorizationEntity.Achievement)]
		public async Task<IActionResult> Update([FromBody] EvaluationUpdateRequest skill)
		{
			if ((await _authorizationService.AuthorizeAsync(User, skill.GameId, HttpContext.ScopeItems(ClaimScope.Game))).Succeeded)
			{
				var skillModel = skill.ToSkillModel();
				EvaluationCoreController.Update(skillModel);
				return Ok();
			}
			return Forbid();
		}

		/// <summary>
		/// Delete Skill with the token and gameId provided.
		/// </summary>
		/// <param name="token">Token of Skill</param>
		/// <param name="gameId">ID of the Game the Skill is for</param>
		[HttpDelete("{token}/global")]
		[HttpDelete("{token}/{gameId:int}")]
		[Authorization(ClaimScope.Game, AuthorizationAction.Delete, AuthorizationEntity.Achievement)]
		public Task<IActionResult> Delete([FromRoute]string token, [FromRoute]int gameId)
		{
			return base.Delete(token, gameId, EvaluationType.Skill);
		}
	}
}