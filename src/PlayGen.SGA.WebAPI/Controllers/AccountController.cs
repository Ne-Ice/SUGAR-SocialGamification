﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlayGen.SGA.Contracts;
using PlayGen.SGA.Contracts.Controllers;
using PlayGen.SGA.DataController;
using PlayGen.SGA.WebAPI.Exceptions;
using PlayGen.SGA.WebAPI.ExtensionMethods;

namespace PlayGen.SGA.WebAPI.Controllers
{
    /// <summary>
    /// Web Controller that facilitates account specific operations.
    /// </summary>
    [Route("api/[controller]")]
    public class AccountController : Controller, IAccountController
    {
        private readonly AccountDbController _accountDbController;

        public AccountController(AccountDbController accountDbController)
        {
            _accountDbController = accountDbController;
        }

        /// <summary>
        /// Register a new account.
        /// Requires the name to be unique.
        /// 
        /// Example Usage: POST api/account/register
        /// </summary>
        /// <param name="newAccount"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public void Register([FromBody]Account newAccount)
        {
            _accountDbController.Create(newAccount.ToModel());
        }

        /// <summary>
        /// Logs in an account based on the name and password combination.
        /// Returns a JWT used for authorization in any further calls to the API.
        /// 
        /// Example Usage: POST api/account
        /// </summary>
        [HttpPost]
        public void Login([FromBody]Account accountDetails)
        {
            var account = _accountDbController.Get(new string[] {accountDetails.Name});

            if (account == null)
            {
                throw new InvalidLoginDetailsException("Invalid Login Details.");
            }

            
            // TODO
            // get authentication to compare passwords
            // if authenticated:
            //  create jwt with claims - id specifically
            //   return the jwt

            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete accounts based on their IDs.
        /// 
        /// Example Usage: DELETE api/account?id=1&id=2
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        public void Delete(int[] id)
        {
            _accountDbController.Delete(id);
        }
    }
}
