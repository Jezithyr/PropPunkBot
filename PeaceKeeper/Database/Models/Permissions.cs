namespace PeaceKeeper.Database.Models;


public record GlobalPermissions(long UserId, GlobalPermissionLevel Permissions);

[Flags]
public enum GlobalPermissionLevel
{
    Basic = 0,
    UserManagement = 1<<0,
    TechManagement = 1<<1,
    CountryManagement = 1<<2,
    CompanyManagement = 1<<3,
    DesignManagement = 1<<4,
    SecretAccess = 1<<5,
    EconomyManagement = 1<<6,
    PermissionsManagement = 1<<7,


    User = Basic,
    Moderator = User | UserManagement | TechManagement |
                CompanyManagement | CountryManagement |
                DesignManagement | SecretAccess |
                EconomyManagement,
    Admin = Moderator | PermissionsManagement,
    Default = User,
    Developer = Admin
}
