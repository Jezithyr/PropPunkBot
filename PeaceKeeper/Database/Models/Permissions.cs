namespace PeaceKeeper.Database.Models;


public record GlobalPermissions(long UserId, GlobalPermissionLevel Permissions);

[Flags]
public enum GlobalPermissionLevel
{
    Basic = 0,
    UserManagement = 1<<0,
    TechManagement = 1<<1,
    CountryManagement = 1<<2,
    DesignManagement = 1<<3,
    SecretAccess = 1<<4,
    EconomyManagement = 1<<5,
    PermissionsManagement = 1<<6,


    User = Basic,
    Moderator = User | UserManagement | TechManagement |
                CountryManagement | DesignManagement |
                SecretAccess | EconomyManagement,
    Admin = Moderator | PermissionsManagement,
    Default = User,
    Developer = Admin
}
