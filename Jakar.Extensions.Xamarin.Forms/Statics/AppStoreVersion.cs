using Plugin.LatestVersion;


namespace Jakar.Extensions.Xamarin.Forms.Statics;


public static class AppStoreVersion
{
    // https://github.com/edsnider/latestversionplugin

    public static async Task<bool>   IsLatest()             => await CrossLatestVersion.Current.IsUsingLatestVersion().ConfigureAwait(false);
    public static async Task<string> LatestVersionNumber()  => await CrossLatestVersion.Current.GetLatestVersionNumber().ConfigureAwait(false);
    public static       string       InstalledVersionNumber => CrossLatestVersion.Current.InstalledVersionNumber;
    public static async Task         OpenAppInStore()       => await CrossLatestVersion.Current.OpenAppInStore().ConfigureAwait(false);


    public static async Task<bool> VerifyAppStoreVersion<TDeviceID, TViewPage>( this Prompts<TDeviceID, TViewPage> prompts, string newVersionAvailable, string newVersionUpdateNowOrLater, CancellationToken token = default )
    {
        bool isLatest = await IsLatest().ConfigureAwait(false);
        if ( isLatest ) { return false; }

        bool update = await prompts.ConfirmAsync(newVersionAvailable, newVersionUpdateNowOrLater, token).ConfigureAwait(false);
        if ( !update ) { return false; }

        await OpenAppInStore().ConfigureAwait(false);
        return true;
    }

    public static async Task<bool> VerifyAppStoreVersion<TDeviceID, TViewPage>( this Prompts<TDeviceID, TViewPage> prompts, string newVersionAvailable, string newVersionUpdateNowOrLater, string yes, string no, CancellationToken token = default )
    {
        bool isLatest = await IsLatest().ConfigureAwait(false);

        if ( isLatest ) { return false; }

        bool update = await prompts.ConfirmAsync(newVersionAvailable, newVersionUpdateNowOrLater, yes, no, token).ConfigureAwait(false);

        if ( !update ) { return false; }

        await OpenAppInStore().ConfigureAwait(false);
        return true;
    }
}
