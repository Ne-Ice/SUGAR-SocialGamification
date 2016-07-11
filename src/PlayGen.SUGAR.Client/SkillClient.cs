﻿using System.Collections.Generic;
using PlayGen.SUGAR.Client.Extensions;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Contracts.Controllers;

namespace PlayGen.SUGAR.Client
{
	/// <summary>
	/// Controller that facilitates Skill specific operations.
	/// </summary>
	public class SkillClient : ClientBase
	{
		public SkillClient(string baseAddress, Credentials credentials) : base(baseAddress, credentials)
		{
		}

		/// <summary>
		/// Get all global skills, ie. skills that are not associated with a specific game
		/// </summary>
		/// <returns>Returns multiple <see cref="AchievementResponse"/> that hold Skill details</returns>
		public IEnumerable<AchievementResponse> GetGlobal()
		{
			var query = GetUriBuilder("api/skills/global/list").ToString();
			return Get<IEnumerable<AchievementResponse>>(query);
		}

		/// <summary>
		/// Find a list of Skills that match <param name="gameId"/>.
		/// 
		/// Example Usage: GET api/skills/game/1/list
		/// </summary>
		/// <param name="gameId">Game ID</param>
		/// <returns>Returns multiple <see cref="AchievementResponse"/> that hold Skill details</returns>
		public IEnumerable<AchievementResponse> Get(int gameId)
		{
			var query = GetUriBuilder($"api/skills/game/{gameId}/list").ToString();
			return Get<IEnumerable<AchievementResponse>>(query);
		}

		/// <summary>
		/// Find the current progress for all global skills for <param name="actorId"/>.
		/// </summary>
		/// <param name="actorId">ID of Group/User</param>
		/// <returns>Returns multiple <see cref="AchievementProgressResponse"/> that hold Skill progress details</returns>
		public IEnumerable<AchievementProgressResponse> GetGlobalProgress(int actorId)
		{
			var query = GetUriBuilder($"api/skills/global/evaluate/{actorId}").ToString();
			return Get<IEnumerable<AchievementProgressResponse>>(query);
		}

		/// <summary>
		/// Find the current progress for all skills for a <param name="gameId"/> for <param name="actorId"/>.
		/// 
		/// Example Usage: GET api/skills/game/1/evaluate/1
		/// </summary>
		/// <param name="gameId">ID of Game</param>
		/// <param name="actorId">ID of Group/User</param>
		/// <returns>Returns multiple <see cref="AchievementProgressResponse"/> that hold current progress toward skill.</returns>
		public IEnumerable<AchievementProgressResponse> GetGameProgress(int actorId, int gameId)
		{
			var query = GetUriBuilder($"api/skills/game/{gameId}/evaluate/{actorId}").ToString();
			return Get<IEnumerable<AchievementProgressResponse>>(query);
		}

		/// <summary>
		/// Find the current progress for an Skill for <param name="actorId"/>.
		/// </summary>
		/// <param name="token">Token of Skill</param>
		/// <param name="actorId">ID of actor/User</param>
		/// <returns>Returns multiple <see cref="AchievementProgressResponse"/> that hold current progress toward skill.</returns>
		public IEnumerable<AchievementProgressResponse> GetGlobalAchievementProgress(string token, int actorId)
		{
			var query = GetUriBuilder($"api/skills/{token}/global/evaluate/{actorId}").ToString();
			return Get<IEnumerable<AchievementProgressResponse>>(query);
		}

		/// <summary>
		/// Find the current progress for a Skill for <param name="actorId"/>.
		/// </summary>
		/// <param name="token">Token of Skill</param>
		/// <param name="gameId">ID of the Game the Skill is for</param>
		/// <param name="actorId">ID of Group/User</param>
		/// <returns>Returns multiple <see cref="AchievementProgressResponse"/> that hold current progress toward skill.</returns>
		public IEnumerable<AchievementProgressResponse> GetAchievementProgress(string token, int gameId, int actorId)
		{
			var query = GetUriBuilder($"api/skills/{token}/{gameId}/evaluate/{actorId}").ToString();
			return Get<IEnumerable<AchievementProgressResponse>>(query);
		}

		/// <summary>
		/// Create a new Skill.
		/// Requires <see cref="AchievementRequest.Name"/> to be unique to that <see cref="AchievementRequest.GameId"/>.
		/// </summary>
		/// <param name="newSkill"><see cref="AchievementRequest"/> object that holds the details of the new Skill.</param>
		/// <returns>Returns a <see cref="AchievementResponse"/> object containing details for the newly created Skill.</returns>
		public AchievementResponse Create(AchievementRequest newSkill)
		{
			var query = GetUriBuilder("api/skills/create").ToString();
			return Post<AchievementRequest, AchievementResponse>(query, newSkill);
		}

		/// <summary>
		/// Update an existing Skill.
		/// </summary>
		/// <param name="skill"><see cref="AchievementRequest"/> object that holds the details of the Skill.</param>
		public void Update(AchievementRequest skill)
		{
			var query = GetUriBuilder($"api/skills/update").ToString();
			Put(query, skill);
		}

		/// <summary>
		/// Delete a global skill, ie. a skill that is not associated with a specific game
		/// </summary>
		/// <param name="token">Token of Skill</param>
		public void DeleteGlobal(string token)
		{
			var query = GetUriBuilder($"api/skills/{token}/global").ToString();
			Delete(query);
		}

		/// <summary>
		/// Delete Skill with the  <param name="token"/> and <param name="gameId"/> provided.
		/// </summary>
		/// <param name="token">Token of Skill</param>
		/// <param name="gameId">ID of the Game the Skill is for</param>
		public void Delete(string token, int gameId)
		{
			var query = GetUriBuilder($"api/skills/{token}/{gameId}").ToString();
			Delete(query);
		}

	}
}
