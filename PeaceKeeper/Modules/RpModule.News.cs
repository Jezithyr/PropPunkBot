using Dapper;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PropPunkShared.Database.Models;

namespace PeaceKeeper.Modules;

public partial class RpModule
{
    private const string NewsModalId = "NewsCreationForm";
    private const string OrgId = "org";
    private const string OrgIconId = "orgIcon";
    private const string ImageId = "imgUrl";
    private const string TextId = "text";


    [SlashCommand("createnewspaper", "Create a newspaper entry, you must be trusted to use this command!")]
    public async Task CreateNewsPrintArticle()
    {
        await CreateNewsModal(NewsType.Print);
    }

    [SlashCommand("createnewsradio", "Create a news radio broadcast entry, you must be trusted to use this command!")]
    public async Task CreateNewsRadioBroadcast()
    {
        await CreateNewsModal(NewsType.Radio);
    }
    [SlashCommand("createnewstv", "Create a news tv broadcast entry, you must be trusted to use this command!")]
    public async Task CreateNewsTVBroadcast()
    {
        await CreateNewsModal(NewsType.Television);
    }

    public async Task CreateNewsModal(NewsType newsType)
    {
        if (! await CheckPermissions(GlobalPermissionLevel.SendNews))
            return;

        string newsTitle;
        string orgPlaceholder;

        switch (newsType)
        {
            case NewsType.Print:
                newsTitle = "Create Newspaper article";
                orgPlaceholder = "Newspaper";
                break;
            case NewsType.Radio:
                newsTitle = "Create Radio Transcript";
                orgPlaceholder = "RadioShow";
                break;
            case NewsType.Television:
                newsTitle = "Create TV Transcript";
                orgPlaceholder = "NewsBroadcast";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newsType), newsType, null);
        }

        var mb = new Discord.ModalBuilder(newsTitle, NewsModalId,
                new ModalComponentBuilder())
            .AddTextInput(
                "Organization",
                OrgId,
                TextInputStyle.Short,
                orgPlaceholder,
                1,
                128,
                true
            )
            .AddTextInput(
                "IconUrl",
                OrgIconId,
                TextInputStyle.Short,
                null,
                null,
                null,
                false
            );

        if (newsType != NewsType.Radio)
        {
            mb.AddTextInput(
                "ImageUrl",
                ImageId,
                TextInputStyle.Short,
                null,
                null,
                null,
                false
            );
        }
        mb.AddTextInput(
            "Text",
            TextId,
            TextInputStyle.Paragraph,
            "This is where you put your news! This must be written in character!",
            null,
            null,
            true
        );
        await Context.Interaction.RespondWithModalAsync(mb.Build());
    }

    private async Task OnNewsModalSubmitted(SocketModal arg)
    {
        var (currentYear, currentQuarter) = await _worldstate.GetCurrentTurnDateQuarters();
        if (arg.Data.CustomId != NewsModalId) return;
        string?[] newsData = new string[4];

        NewsType newsType;
        if (!Enum.TryParse(arg.Data.CustomId, out newsType))
            newsType = NewsType.Print;
        foreach (var socketMessageData in arg.Data.Components)
        {
            if (socketMessageData == null) continue;
            switch (socketMessageData.CustomId)
            {
                case OrgId:
                {
                    newsData[0] = socketMessageData.Value;
                    break;
                }
                case TextId:
                {
                    newsData[1] = socketMessageData.Value;
                    break;
                }
                case ImageId:
                {
                    newsData[2] = socketMessageData.Value;
                    break;
                }
                case OrgIconId:
                {
                    newsData[3] = socketMessageData.Value;
                    break;
                }
            }
        }
        var embed = new EmbedBuilder();
        embed.WithAuthor(newsData[0], newsData[3]);
        embed.Title = "Breaking News:";
        embed.Description = $"Dated: {currentYear} Q{currentQuarter}";
        if (newsData[2] != null)
            embed.ImageUrl = newsData[2];
        embed.AddField("", newsData[1]);
        await arg.RespondAsync(embed: embed.Build());
        await using var connection = await _db.Get();
        await connection.QueryAsync(
            "INSERT INTO news(organization, newstype, text, imagelink, organizationiconlink, dateyear, datequarter) " +
            "VALUES(@org,@newstype,@text,@imglink,@orgiconlink,@dateyear,@datequarter) " +
            "ON CONFLICT DO NOTHING",
            new
            {
                org = newsData[0], newstype = newsType, text = newsData[1],
                imglink = newsData[2], orgiconlink = newsData[3], dateyear = currentYear, datequarter = currentQuarter
            });
    }
}
