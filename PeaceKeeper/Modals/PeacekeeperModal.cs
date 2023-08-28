using Discord.Interactions;

namespace PeaceKeeper.Modals;

public sealed class PeacekeeperModal : IModal
{
    public string Title { get; init; } = "PeacekeeperModal";
    public string CustomId { get; init; } = "PeacekeeperModalId";
}
