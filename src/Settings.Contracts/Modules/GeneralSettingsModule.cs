namespace Settings.Contracts;

public record GeneralSettingsModule : BaseSettingsModule<GeneralSettingsModule>, IGeneralSettings
{
    private bool _firstTimeSetup = true;
    private int _activeAccountId;
    private bool _debugMode;
    private bool _disableAnimatedBackground;
    private bool _hideMediaFromOfflineServers;
    private bool _hideMediaFromOwnedServers;
    private bool _useLowQualityPosterImages;

    public bool FirstTimeSetup
    {
        get => _firstTimeSetup;
        set => SetProperty(ref _firstTimeSetup, value);
    }

    public int ActiveAccountId
    {
        get => _activeAccountId;
        set => SetProperty(ref _activeAccountId, value);
    }

    public bool DebugMode
    {
        get => _debugMode;
        set => SetProperty(ref _debugMode, value);
    }

    public bool DisableAnimatedBackground
    {
        get => _disableAnimatedBackground;
        set => SetProperty(ref _disableAnimatedBackground, value);
    }

    public bool HideMediaFromOfflineServers
    {
        get => _hideMediaFromOfflineServers;
        set => SetProperty(ref _hideMediaFromOfflineServers, value);
    }

    public bool HideMediaFromOwnedServers
    {
        get => _hideMediaFromOwnedServers;
        set => SetProperty(ref _hideMediaFromOwnedServers, value);
    }

    public bool UseLowQualityPosterImages
    {
        get => _useLowQualityPosterImages;
        set => SetProperty(ref _useLowQualityPosterImages, value);
    }
}
