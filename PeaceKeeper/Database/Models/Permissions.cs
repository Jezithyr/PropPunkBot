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
    SendNews = 1<<8,
    NewsManagement = 1<<9,
    MakeTrusted = 1<<10,
    RemoveTrusted = 1<<11,
    CanRp = 1<<12,

    User = Basic | CanRp,
    TrustedUser = Basic | SendNews,
    Verifier = TrustedUser | MakeTrusted,
    Moderator = Verifier | UserManagement | TechManagement | NewsManagement | CompanyManagement | CountryManagement
                | DesignManagement | SecretAccess | EconomyManagement | RemoveTrusted,
    Admin = Moderator | PermissionsManagement,
    Default = User,
    Developer = Admin
}
