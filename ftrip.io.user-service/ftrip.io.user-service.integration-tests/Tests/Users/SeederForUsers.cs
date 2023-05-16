using ftrip.io.user_service.Accounts.Domain;
using ftrip.io.user_service.Accounts.Utilities;
using ftrip.io.user_service.Users.Domain;
using System;

namespace ftrip.io.user_service.integration_tests.Tests.Users
{
    public static class UsersSeeder
    {
        public static User ExistingUser;
        public static User ChangePasswordUser;

        public static CredentialsAccount ExistingAccount;
        public static CredentialsAccount ChangePasswordAccount;

        public static void InitializeUsers()
        {
            ExistingUser = new User()
            {
                Active = true,
                CreatedAt = DateTime.Now,
                FirstName = "Milos",
                LastName = "Panic"
            };

            ChangePasswordUser = new User()
            {
                Active = true,
                CreatedAt = DateTime.Now,
                FirstName = "Dragana",
                LastName = "Filipovic"
            };
        }

        public static void InitializeCredentialsAccounts()
        {
            var salt = "RandomSalt";

            ExistingAccount = new CredentialsAccount()
            {
                Active = true,
                CreatedAt = DateTime.Now,
                UserId = ExistingUser.Id,
                Username = "panicmilos",
                Salt = salt,
                HashedPassword = PasswordHasher.Hash("password", salt)
            };

            ChangePasswordAccount = new CredentialsAccount()
            {
                Active = true,
                CreatedAt = DateTime.Now,
                UserId = ChangePasswordUser.Id,
                Username = "draganafilipovic",
                Salt = salt,
                HashedPassword = PasswordHasher.Hash("Password123", salt)
            };
        }
    }
}