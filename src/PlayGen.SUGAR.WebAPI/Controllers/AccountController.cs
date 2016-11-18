﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Contracts.Shared;
using PlayGen.SUGAR.Core.EvaluationEvents;
using PlayGen.SUGAR.Core.Utilities;
using PlayGen.SUGAR.Data.Model;
using PlayGen.SUGAR.ServerAuthentication;
using PlayGen.SUGAR.WebAPI.Exceptions;
using PlayGen.SUGAR.WebAPI.Extensions;
using PlayGen.SUGAR.WebAPI.Filters;
using PlayGen.SUGAR.ServerAuthentication.Extensions;

namespace PlayGen.SUGAR.WebAPI.Controllers
{
	/// <summary>
	/// Web Controller that facilitates account specific operations.
	/// </summary>
	[Route("api/[controller]")]
	public class AccountController : Controller
	{
		private readonly JsonWebTokenUtility _jsonWebTokenUtility;
	    private readonly Core.Controllers.AccountController _accountCoreController;
	    private readonly EvaluationTracker _evaluationTracker;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="accountDbController"></param>
		/// <param name="userDbController"></param>
		/// <param name="passwordEncryption"></param>
		/// <param name="jsonWebTokenUtility"></param>
		public AccountController(Core.Controllers.AccountController accountCoreController,

            Data.EntityFramework.Controllers.UserController userDbController,
			JsonWebTokenUtility jsonWebTokenUtility,
            EvaluationTracker evaluationTracker)
		{
		    _accountCoreController = accountCoreController;
            _jsonWebTokenUtility = jsonWebTokenUtility;
		    _evaluationTracker = evaluationTracker;
		}
		
		//Todo: Move log-in into a separate controller
		/// <summary>
		/// Logs in an account based on the name and password combination.
		/// Returns a JsonWebToken used for authorization in any further calls to the API.
		/// 
		/// Example Usage: POST api/account/login
		/// </summary>
		/// <param name="accountRequest"><see cref="AccountRequest"/> object that contains the account details provided.</param>
		/// <returns>A <see cref="AccountResponse"/> containing the Account details.</returns>
		[HttpPost("login")]
		//[ResponseType(typeof(AccountResponse))]
		[ArgumentsNotNull]
		public IActionResult Login([FromBody]AccountRequest accountRequest)
		{
		    var account = accountRequest.ToModel();

            account = _accountCoreController.Login(account);

			var token = CreateToken(account);
			HttpContext.Response.SetAuthorizationToken(token);

            // todo _evaluationTracker.OnActorSessionStarted();

            var response = account.ToContract();
			return new ObjectResult(response);
		}
		
		/// <summary>
		/// Register a new account and creates an associated user.
		/// Requires the <see cref="AccountRequest.Name"/> to be unique.
		/// Returns a JsonWebToken used for authorization in any further calls to the API.
		/// 
		/// Example Usage: POST api/account/register
		/// </summary>
		/// <param name="accountRequest"><see cref="AccountRequest"/> object that contains the details of the new Account.</param>
		/// <returns>A <see cref="AccountResponse"/> containing the new Account details.</returns>
		[HttpPost("register")]
		//[ResponseType(typeof(AccountResponse))]
		[ArgumentsNotNull]
		public IActionResult Register([FromBody] AccountRequest accountRequest)
		{
		    var account = accountRequest.ToModel();

		    account = _accountCoreController.Register(account);

			var response = account.ToContract();
			if (accountRequest.AutoLogin)
			{
				var token = CreateToken(account);
				HttpContext.Response.SetAuthorizationToken(token);

                // todo _evaluationTracker.OnActorSessionStarted();
            }
			return new ObjectResult(response);
		}

		//Todo: Review if register with id is needed
		/// <summary>
		/// Register a new account for an existing user.
		/// Requires the <see cref="AccountRequest.Name"/> to be unique.
		/// Returns a JsonWebToken used for authorization in any further calls to the API.
		/// 
		/// Example Usage: POST api/account/registerwithid/1
		/// </summary>
		// <param name="userId">ID of the existing User.</param>
		/// <param name="newAccount"><see cref="AccountRequest"/> object that contains the details of the new Account.</param>
		/// <returns>A <see cref="AccountResponse"/> containing the new Account details.</returns>
		/*[HttpPost("registerwithid/{userId:int}")]
		[ResponseType(typeof(AccountResponse))]
		public IActionResult Register([FromRoute]int userId, [FromBody] AccountRequest accountRequest)
		{
			var user = _userDbController.Search(userId);

			if (string.IsNullOrWhiteSpace(accountRequest.Name) 
				|| string.IsNullOrWhiteSpace(accountRequest.Password)
				|| user != null)
			{
				throw new InvalidAccountDetailsException("Name and Password cannot be empty.");
			}

			var account = CreateAccount(accountRequest, user);

			var response = account.ToContract();
			if (accountRequest.AutoLogin)
			{
				response.Token = CreateToken(account);
			}
			return new ObjectResult(response);
		}*/

		/// <summary>
		/// Delete Account with the ID provided.
		/// 
		/// Example Usage: DELETE api/account/1
		/// </summary>
		/// <param name="id">Account ID.</param>
		[HttpDelete("{id:int}")]
		[Authorization]
		public void Delete([FromRoute]int id)
		{
            _accountCoreController.Delete(id);
		}
		
		#region Helpers
		private string CreateToken(Account account)
		{
			return _jsonWebTokenUtility.CreateToken(new Dictionary<string, object>
			{
				{ "userid", account.UserId}
			});
		}
		#endregion
	}
}