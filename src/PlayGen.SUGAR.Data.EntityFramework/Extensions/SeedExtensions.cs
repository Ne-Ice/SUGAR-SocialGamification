﻿using PlayGen.SUGAR.Data.Model;

namespace PlayGen.SUGAR.Data.EntityFramework.Extensions
{
    internal static class SUGARContextSeedExtensions
    {
        internal static void Seed(this SUGARContext context)
        {
            context.Accounts.Add(new Account()
            {
                Name = "admin",
                Password = "$2a$12$SSIgQE0cQejeH0dM61JV/eScAiHwJo/I3Gg6xZFUc0gmwh0FnMFv.",
                Id = 1,
                User = new User()
                {
                    Id = 1,
                    Name = "admin"
                },
                UserId = 1
            });

            context.SaveChanges();
        }
    }
}
