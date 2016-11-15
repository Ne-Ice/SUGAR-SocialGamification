﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayGen.SUGAR.Authorization;
using PlayGen.SUGAR.Common.Shared.Permissions;
using PlayGen.SUGAR.Contracts.Shared;
using PlayGen.SUGAR.WebAPI.Extensions;
using PlayGen.SUGAR.WebAPI.Filters;

namespace PlayGen.SUGAR.WebAPI.Controllers
{
	/// <summary>
	/// Web Controller that facilitates GameData specific operations.
	/// </summary>
	[Route("api/[controller]")]
	public class GameDataController : Controller
	{
        private readonly IAuthorizationService _authorizationService;
        private readonly Data.EntityFramework.Controllers.GameDataController _gameDataCoreController;

		public GameDataController(Data.EntityFramework.Controllers.GameDataController gameDataCoreController,
                    IAuthorizationService authorizationService)
		{
			_gameDataCoreController = gameDataCoreController;
            _authorizationService = authorizationService;
        }

		/// <summary>
		/// Find a list of all GameData that match the <param name="actorId"/>, <param name="gameId"/> and <param name="key"/> provided.
		/// 
		/// Example Usage: GET api/gamedata?actorId=1&amp;gameId=1&amp;key=key1&amp;key=key2
		/// </summary>
		/// <param name="actorId">ID of a User/Group.</param>
		/// <param name="gameId">ID of a Game.</param>
		/// <param name="key">Array of Key names.</param>
		/// <returns>A list of <see cref="GameDataResponse"/> which match the search criteria.</returns>
		[HttpGet]
        //[ResponseType(typeof(IEnumerable<GameDataResponse>))]
        [Authorization(ClaimScope.Game, AuthorizationOperation.Get, AuthorizationOperation.GameData)]
        public IActionResult Get(int? actorId, int? gameId, string[] key)
		{
            if (_authorizationService.AuthorizeAsync(User, gameId, (AuthorizationRequirement)HttpContext.Items["Requirements"]).Result)
            {
                var data = _gameDataCoreController.Get(gameId, actorId, key);
                var dataContract = data.ToContractList();
                return new ObjectResult(dataContract);
            }
            return Unauthorized();
        }

		/// <summary>
		/// Create a new GameData record.
		/// 
		/// Example Usage: POST api/gamedata
		/// </summary>
		/// <param name="newData"><see cref="GameDataRequest"/> object that holds the details of the new GameData.</param>
		/// <returns>A <see cref="GameDataResponse"/> containing the new GameData details.</returns>
		[HttpPost]
		//[ResponseType(typeof(GameDataResponse))]
		[ArgumentsNotNull]
        [Authorization(ClaimScope.Actor, AuthorizationOperation.Create, AuthorizationOperation.GameData)]
        public IActionResult Add([FromBody]GameDataRequest newData)
		{
            if (_authorizationService.AuthorizeAsync(User, newData.GameId, (AuthorizationRequirement)HttpContext.Items["Requirements"]).Result)
            {
                var data = newData.ToModel();
                _gameDataCoreController.Create(data);
                var dataContract = data.ToContract();
                return new ObjectResult(dataContract);
            }
            return Unauthorized();
        }
	}
}