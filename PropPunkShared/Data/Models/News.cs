namespace PropPunkShared.Data.Models;

public record NewsRaw(
    Guid Id,
    string Organization,
    NewsType NewsType,
    string Date,
    string Text,
    string? ImageLink,
    string? OrganizationIconLink
    );

public record News(
    Guid Id,
    string Organization,
    NewsType NewsType,
    DateTime Date,
    string Text,
    string? ImageLink,
    string? OrganizationIconLink
    )
{
    public News(NewsRaw raw) : this(raw.Id, raw.Organization, raw.NewsType, DateTime.Parse(raw.Date), raw.Text,
        raw.ImageLink, raw.OrganizationIconLink)
    {
    }
}


public enum NewsType
{
    Print,
    Radio,
    Television
}
