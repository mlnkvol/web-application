﻿namespace OnlineLibraryWebApplication.Models
{
    public static class RoleNames
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string All = nameof(Admin) + "," + nameof(User); 
    }
}
