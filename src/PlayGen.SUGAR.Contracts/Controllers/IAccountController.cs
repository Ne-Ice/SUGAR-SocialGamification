﻿namespace PlayGen.SUGAR.Contracts.Controllers
{
	public interface IAccountController
	{
		AccountResponse Register(AccountRequest newAccount);

		AccountResponse Register(int userId, AccountRequest newAccount);

		AccountResponse Login(AccountRequest account);

		void Delete(int[] id);
	}
}