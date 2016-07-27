﻿using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Data.Model;

namespace PlayGen.SUGAR.WebAPI.Extensions
{
	public static class AccountExtensions
	{
		public static AccountResponse ToContract(this Account accountModel)
		{
			if (accountModel == null)
			{
				return null;
			}
			return new AccountResponse
			{
				User = accountModel.User.ToContract()
			};
		}

		public static Account ToModel(this AccountRequest accountContract)
		{
			return new Account
			{
				Name = accountContract.Name,
				Password = accountContract.Password
			};
		}
	}
}