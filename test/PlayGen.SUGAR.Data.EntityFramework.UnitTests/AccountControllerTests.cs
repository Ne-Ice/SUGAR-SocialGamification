﻿using System.Linq;
using PlayGen.SUGAR.Data.EntityFramework.Controllers;
using PlayGen.SUGAR.Data.EntityFramework.Exceptions;
using PlayGen.SUGAR.Data.Model;
using NUnit.Framework;

namespace PlayGen.SUGAR.Data.EntityFramework.UnitTests
{
	[TestFixture]
	public class AccountControllerTests
	{
		#region Configuration
		private readonly AccountController _accountDbController;
		private readonly UserController _userDbController; 

		public AccountControllerTests()
		{
			_accountDbController = TestEnvironment.AccountController;
			_userDbController = TestEnvironment.UserController;
		}
		#endregion

		#region Tests
		[Test]
		public void CreateAndGetAccount()
		{
			var name = "CreateAndGetAccount";
			string password = $"{name}Password";

			CreateAccount(name, password);

			var accounts = _accountDbController.Get(new string[] { name });

			var matches = accounts.Count(a => a.Name == name);

			Assert.AreEqual(1, matches);
		}

		[Test]
		public void CreateDuplicateAccount()
		{
			var name = "CreateDuplicateAccount";
			string password = $"{name}Password";

			CreateAccount(name, password);

			Assert.Throws<DuplicateRecordException>(() => CreateAccount(name, password));
		}

		[Test]
		public void GetMultipleAccountsByName()
		{
			var names = new[]
			{
				"GetMultipleAccountsByName1",
				"GetMultipleAccountsByName2",
				"GetMultipleAccountsByName3",
				"GetMultipleAccountsByName4",
			};

			foreach (var name in names)
			{
				CreateAccount(name, $"{name}Password");
			}

			CreateAccount("GetMultipleAccountsByName_DontGetThis", "GetMultipleAccountsByName_DontGetThisPassword");

			var accounts = _accountDbController.Get(names);

			var matchingAccounts = accounts.Select(a => names.Contains(a.Name));

			Assert.AreEqual(names.Length, matchingAccounts.Count());
		}

		[Test]
		public void GetNonExistingAccounts()
		{
			var accounts = _accountDbController.Get(new string[] { "GetNonExsitingAccounts" });

			Assert.IsEmpty(accounts);
		}

		[Test]
		public void DeleteExistingAccount()
		{
			var name = "DeleteExistingAccount";
			string password = $"{name}Password";

			var account = CreateAccount(name, password);

			var accounts = _accountDbController.Get(new string[] { name });
			Assert.AreEqual(accounts.Count(), 1);
			Assert.AreEqual(accounts.ElementAt(0).Name, name);

			_accountDbController.Delete(account.Id);
			accounts = _accountDbController.Get(new string[] { name });

			Assert.IsEmpty(accounts);
		}

		[Test]
		public void DeleteNonExistingAccount()
		{
			Assert.Throws<MissingRecordException>(() => _accountDbController.Delete(-1));
		}
		#endregion

		#region Helpers
		private Account CreateAccount(string name, string password)
		{
			var user = CreateUser(name);

			var account = new Account
			{
				Name = name,
				Password = password,
				UserId = user.Id,
				User = user
			};
			
			return _accountDbController.Create(account);
		}

		private User CreateUser(string name)
		{
			var user = new User()
			{
				Name = name,
			};

			_userDbController.Create(user);

			return user;
		}
		#endregion
	}
}