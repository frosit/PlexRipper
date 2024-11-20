namespace Settings.Contracts;

public record GeneralSettingsModule : BaseSettingsModule<GeneralSettingsModule>, IGeneralSettings
{
    private bool _firstTimeSetup = true;
    private int _activeAccountId;
    private bool _disableAnimatedBackground;
    private bool _hideMediaFromOfflineServers;
    private bool _hideMediaFromOwnedServers;
    private bool _useLowQualityPosterImages;

    public static GeneralSettingsModule Create() =>
        new()
        {
            FirstTimeSetup = true,
            ActiveAccountId = 0,
            DisableAnimatedBackground = false,
            HideMediaFromOfflineServers = false,
            HideMediaFromOwnedServers = false,
            UseLowQualityPosterImages = false,
        };

    public required bool FirstTimeSetup
    {
        get => _firstTimeSetup;
        set => SetProperty(ref _firstTimeSetup, value);
    }

    public required int ActiveAccountId
    {
        get => _activeAccountId;
        set => SetProperty(ref _activeAccountId, value);
    }

    public required bool DisableAnimatedBackground
    {
        get => _disableAnimatedBackground;
        set => SetProperty(ref _disableAnimatedBackground, value);
    }

    public required bool HideMediaFromOfflineServers
    {
        get => _hideMediaFromOfflineServers;
        set => SetProperty(ref _hideMediaFromOfflineServers, value);
    }

    public required bool HideMediaFromOwnedServers
    {
        get => _hideMediaFromOwnedServers;
        set => SetProperty(ref _hideMediaFromOwnedServers, value);
    }

    public required bool UseLowQualityPosterImages
    {
        get => _useLowQualityPosterImages;
        set => SetProperty(ref _useLowQualityPosterImages, value);
    }
}
