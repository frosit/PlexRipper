namespace Settings.Contracts;

public class GeneralSettingsDTO : IGeneralSettings
{
    public required bool FirstTimeSetup { get; set; }

    public required int ActiveAccountId { get; set; }

    public required bool DisableAnimatedBackground { get; set; }

    public bool HideMediaFromOfflineServers { get; set; }

    public bool HideMediaFromOwnedServers { get; set; }

    public bool UseLowQualityPosterImages { get; set; }
}
