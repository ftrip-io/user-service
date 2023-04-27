namespace ftrip.io.user_service.Common
{
    public static class RegexConstants
    {
        public const string NUMERIC_REGEX = @"[0-9]";
        public const string LOWER_CASE_REGEX = @"[a-zšđžčć]";
        public const string UPPER_CASE_REGEX = @"[A-ZŠĐŽČĆ]";
        public const string ALPHA_REGEX = @"^[a-zA-ZčČćĆđĐžŽšŠ ]+$";
        public const string ALPHANUMERIC_REGEX = @"^[a-zA-ZčČćĆđĐžŽšŠ0-9 ]+$";
        public const string SPECIAL_CHARACTERS_REGEX = @"[^a-zA-Z0-9]";
        public const string PHONE_NUMBER_REGEX = @"^(\+\d{1,3})?\-?\(?\d{2,3}\)?[-]?\d{3}[-]?\d{3,4}$";
        public const string EMAIL_REGEX = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
    }
}