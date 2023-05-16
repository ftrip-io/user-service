using System;

namespace ftrip.io.user_service.integration_tests.Tests.Users
{
    public static class UserContants
    {
        public static string AuthenticateUserApi() => "api/auth/authenticate";

        public static string CreateUserApi() => "api/users";

        public static string UpdateUserApi(Guid userId) => $"api/users/{userId}";

        public static string ChanngePasswordForUserApi(Guid userId) => $"api/users/{userId}/account/password";
    }
}