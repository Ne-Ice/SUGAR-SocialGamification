﻿using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Data.Model;

namespace PlayGen.SUGAR.WebAPI.Extensions
{
	public static class AccountSourceExtensions
	{
		public static AccountSourceResponse ToContract(this AccountSource sourceModel)
		{
			if (sourceModel == null)
			{
				return null;
			}

			return new AccountSourceResponse {
				Id = sourceModel.Id,
				Description = sourceModel.Description,
				Token = sourceModel.Token,
				RequiresPassword = sourceModel.RequiresPassword,
				AutoRegister = sourceModel.AutoRegister
			};
		}

		public static IEnumerable<AccountSourceResponse> ToContractList(this IEnumerable<AccountSource> sourceModels)
		{
			return sourceModels.Select(ToContract).ToList();
		}

		public static AccountSource ToModel(this AccountSourceRequest sourceContract)
		{
			return new AccountSource {
				Description = sourceContract.Description,
				Token = sourceContract.Token,
				RequiresPassword = sourceContract.RequiresPassword,
				AutoRegister = sourceContract.AutoRegister
			};
		}

	}
}
