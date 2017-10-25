﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayGen.SUGAR.Common.Authorization;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Server.Authorization;
using PlayGen.SUGAR.Server.WebAPI.Attributes;
using PlayGen.SUGAR.Server.WebAPI.Extensions;

namespace PlayGen.SUGAR.Server.WebAPI.Controllers
{
	/// <summary>
	/// Web Controller that facilitates Role specific operations.
	/// </summary>
	[Route("api/[controller]")]
	[Authorize("Bearer")]
	[ValidateSession]
	public class RoleController : Controller
	{
		private readonly IAuthorizationService _authorizationService;
		private readonly Core.Controllers.RoleController _roleCoreController;

		public RoleController(Core.Controllers.RoleController roleCoreController,
					IAuthorizationService authorizationService)
		{
			_roleCoreController = roleCoreController;
			_authorizationService = authorizationService;
		}
		/// <summary>
		/// Get a list of all Roles.
		/// 
		/// Example Usage: GET api/role/list
		/// </summary>
		/// <returns>A list of <see cref="RoleResponse"/> that hold Role details.</returns>
		[HttpGet("list")]
		//[ResponseType(typeof(IEnumerable<RoleResponse>))]
		[Authorization(ClaimScope.Global, AuthorizationAction.Get, AuthorizationEntity.Role)]
		public async Task<IActionResult> Get()
		{
			if (await _authorizationService.AuthorizeAsync(User, Platform.EntityId, (AuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(ClaimScope.Global)]))
			{
				var roles = _roleCoreController.Get();
				var roleContract = roles.ToContractList();
				return new ObjectResult(roleContract);
			}
			return Forbid();
		}

		/// <summary>
		/// Get a list of all Roles for the scope with this name.
		/// 
		/// Example Usage: GET api/role/scope/game
		/// </summary>
		/// <returns>A list of <see cref="RoleResponse"/> that hold Role details.</returns>
		[HttpGet("scope/{name}")]
		//[ResponseType(typeof(IEnumerable<RoleResponse>))]
		[Authorization(ClaimScope.Global, AuthorizationAction.Get, AuthorizationEntity.Role)]
		[Authorization(ClaimScope.Group, AuthorizationAction.Get, AuthorizationEntity.Role)]
		[Authorization(ClaimScope.Game, AuthorizationAction.Get, AuthorizationEntity.Role)]
		public async Task<IActionResult> GetByScope([FromRoute]string name)
		{
			if (Enum.TryParse(name, true, out ClaimScope claimScope))
			{
				if (await _authorizationService.AuthorizeAsync(User, claimScope, (AuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(claimScope)]))
				{
					var roles = _roleCoreController.GetByScope(claimScope);
					var roleContract = roles.ToContractList();
					return new ObjectResult(roleContract);
				}
				return Forbid();
			}
			return Forbid();
		}

		/// <summary>
		/// Get default Role for the scope with this name.
		/// 
		/// Example Usage: GET api/role/scopedefault/game
		/// </summary>
		/// <returns>A <see cref="RoleResponse"/> that holds Role details.</returns>
		[HttpGet("scopedefault/{name}")]
		//[ResponseType(typeof(RoleRespons>))]
		[Authorization(ClaimScope.Global, AuthorizationAction.Get, AuthorizationEntity.Role)]
		[Authorization(ClaimScope.Group, AuthorizationAction.Get, AuthorizationEntity.Role)]
		[Authorization(ClaimScope.Game, AuthorizationAction.Get, AuthorizationEntity.Role)]
		public async Task<IActionResult> GetDefaultForScope([FromRoute]string name)
		{
			if (Enum.TryParse(name, true, out ClaimScope claimScope))
			{
				if (await _authorizationService.AuthorizeAsync(User, claimScope, (AuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(claimScope)]))
				{
					var role = _roleCoreController.GetDefaultForScope(claimScope);
					var roleContract = role.ToContract();
					return new ObjectResult(roleContract);
				}
				return Forbid();
			}
			return Forbid();
		}

		/// <summary>
		/// Create a new Role.
		/// Requires the <see cref="RoleRequest.Name"/> to be unique.
		/// 
		/// Example Usage: POST api/role
		/// </summary>
		/// <param name="newRole"><see cref="RoleRequest"/> object that contains the details of the new Role.</param>
		/// <returns>A <see cref="RoleResponse"/> containing the new Role details.</returns>
		[HttpPost]
		//[ResponseType(typeof(RoleResponse))]
		[ArgumentsNotNull]
		[Authorization(ClaimScope.Global, AuthorizationAction.Create, AuthorizationEntity.Role)]
		[Authorization(ClaimScope.Group, AuthorizationAction.Create, AuthorizationEntity.Role)]
		[Authorization(ClaimScope.Game, AuthorizationAction.Create, AuthorizationEntity.Role)]
		public async Task<IActionResult> Create([FromBody]RoleRequest newRole)
		{
			if (await _authorizationService.AuthorizeAsync(User, newRole.ClaimScope, (AuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(newRole.ClaimScope)]))
			{
				var role = newRole.ToModel();
				_roleCoreController.Create(role, int.Parse(User.Identity.Name));
				var roleContract = role.ToContract();
				return new ObjectResult(roleContract);
			}
			return Forbid();
		}

		/// <summary>
		/// Delete Role with the ID provided.
		/// 
		/// Example Usage: DELETE api/role/1
		/// </summary>
		/// <param name="id">Role ID.</param>
		[HttpDelete("{id:int}")]
		[Authorization(ClaimScope.Role, AuthorizationAction.Delete, AuthorizationEntity.Role)]
		public async Task<IActionResult> Delete([FromRoute]int id)
		{
			if (await _authorizationService.AuthorizeAsync(User, id, (AuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(ClaimScope.Role)]))
			{

				var role = _roleCoreController.GetById(id);
				if (!role.Default)
				{
					//Todo: May need to check claims don't become inaccessible due to deletion
					_roleCoreController.Delete(id);
					return Ok();
				}
			}
			return Forbid();
		}
	}
}