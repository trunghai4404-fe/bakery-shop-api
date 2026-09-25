namespace Identity.Domain.Enums;

public static class PermissionEnum
{
    public static class User
    {
        public const string UserView = "User.View";
        public const string UserCreate = "User.Create";
        public const string UserUpdate = "User.Update";
        public const string UserDelete = "User.Delete";
    }
    public static class Category
    {
        public const string CategoryView = "Category.View";
        public const string CategoryCreate = "Category.Create";
        public const string CategoryUpdate = "Category.Update";
        public const string CategoryDelete = "Category.Delete";
    }
    public static class Product
    {
        public const string ProductView = "Product.View";
        public const string ProductCreate = "Product.Create";
        public const string ProductUpdate = "Product.Update";
        public const string ProductDelete = "Product.Delete";
    }
    public static class Role
    {
        public const string RoleView = "Role.View";
        public const string RoleCreate = "Role.Create";
        public const string RoleUpdate = "Role.Update";
        public const string RoleDelete = "Role.Delete";
    }
    
    public static class Permission
    {
        public const string PermissionView = "Permission.View";
        public const string PermissionUpdate = "Permission.Update";
    }
}