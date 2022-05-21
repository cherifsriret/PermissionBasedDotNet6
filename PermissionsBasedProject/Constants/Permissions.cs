namespace PermissionsBasedProject.Constants
{
    public static class Permissions
    {
        public static  string PermissionName = "Permission";
        public static List<string> GeneratePermissionList(string module)
        {
            return new List<string>()
            {
                $"Permission.{module }.Create",
                $"Permission.{module }.Read",
                $"Permission.{module }.Update",
                $"Permission.{module }.Delete",
            };
        }



        public static List<string> GenerateAllPermissions()
        {
            var allPermissions = new List<string>();
            var modules = Enum.GetValues(typeof(Modules));
            foreach (var module in modules)
            {
                allPermissions.AddRange(GeneratePermissionList(module.ToString()));
            }
            return allPermissions;
        }

        public static class Roles
        {
            public const string View = "Permission.Roles.Read";
            public const string Create = "Permission.Roles.Create";
            public const string Update = "Permission.Roles.Update";
            public const string Delete = "Permission.Roles.Delete";
        }
        public static class Users
        {
            public const string View = "Permission.Users.Read";
            public const string Create = "Permission.Users.Create";
            public const string Update = "Permission.Users.Update";
            public const string Delete = "Permission.Users.Delete";
        }

        public static class Products
        {
            public const string View = "Permission.Products.Read";
            public const string Create = "Permission.Products.Create";
            public const string Update = "Permission.Products.Update";
            public const string Delete = "Permission.Products.Delete";
        }
    }
}
