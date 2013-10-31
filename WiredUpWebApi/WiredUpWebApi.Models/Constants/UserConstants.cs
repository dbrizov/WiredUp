using System;
using System.Linq;

namespace WiredUpWebApi.Models.Constants
{
    public static class UserConstants
    {
        public const int FirstNameMaxLength = 25;
        public const int LastNameMaxLength = 25;
        public const int EmailMaxLength = 100;
        public const int PasswordMinLength = 6;
        public const int PasswordMaxLength = 30;
        public const int LanguagesMaxLength = 500;
    }
}
