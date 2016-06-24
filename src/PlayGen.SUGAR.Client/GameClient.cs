﻿using System.Collections.Generic;
using PlayGen.SUGAR.Client.Extensions;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Contracts.Controllers;

namespace PlayGen.SUGAR.Client
{
	/// <summary>
	/// Controller that facilitates Game specific operations.
	/// </summary>
	public class GameClient : ClientBase, IGameController
	{
		public GameClient(string baseAddress) : base(baseAddress)
		{
		}

		/// <summary>
		/// Get a list of all Games.
		/// </summary>
		/// <returns>A list of <see cref="GameResponse"/> that hold Games details.</returns>
		public IEnumerable<GameResponse> Get()
		{
			var query = GetUriBuilder("api/game/all").ToString();
			return Get<IEnumerable<GameResponse>>(query);
		}

		/// <summary>
		/// Get a list of Games that match <param name="name"/> provided.
		/// </summary>
		/// <param name="name">Array of Game names</param>
		/// <returns>A list of <see cref="GameResponse"/> which match the search criteria.</returns>
		public IEnumerable<GameResponse> Get(string[] name)
		{
			var query = GetUriBuilder("api/game")
				.AppendQueryParameters(name, "name={0}")
				.ToString();
			return Get<IEnumerable<GameResponse>>(query);
		}

		/// <summary>
		/// Create a new Game.
		/// Requires the <see cref="GameRequest.Name"/> to be unique.
		/// </summary>
		/// <param name="game"><see cref="GameRequest"/> object that contains the details of the new Game.</param>
		/// <returns>A <see cref="GameResponse"/> containing the new Game details.</returns>
		public GameResponse Create(GameRequest game)
		{
			var query = GetUriBuilder("api/game").ToString();
			return Post<GameRequest, GameResponse>(query, game);
		}

		/// <summary>
		/// Delete Games with the IDs provided.
		/// </summary>
		/// <param name="id">Array of Game IDs.</param>
		public void Delete(int[] id)
		{
			var query = GetUriBuilder("api/game")
				.AppendQueryParameters(id, "id={0}")
				.ToString();
			Delete(query);
		}
	}
}
