using BookHeaven.Core.Features.ProfileSettingss;
using BookHeaven.Reader.Components.Shared.ContextMenu;
using BookHeaven.Reader.Constants;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.AspNetCore.Components;

namespace BookHeaven.Reader.Components.Layout;

public partial class BooksLayout
{
    [Inject] private AppStateService AppStateService { get; set; } = null!;
    [Inject] private IServerService ServerService { get; set; } = null!;
    [Inject] private ISender Sender { get; set; } = null!;
    [Inject] private IContextMenuService ContextMenuService { get; set; } = null!;
    
    private void OpenMenu()
    {
        ContextMenuService.OpenFixedMenu("border-t-0 border-r-0 border-s border-b rounded-none border-black bg-white !top-[40px] !left-auto !right-0", [
            new ContextMenuItemModel
            {
                Text = Translations.SETTINGS,
                OnClick = () =>
                {
                    NavigationManager.NavigateTo(Urls.Settings);
                    return Task.CompletedTask;
                }
            },
            new ContextMenuItemModel
            {
                Text = "Backup Profile",
                OnClick = BackupProfile
            },
            new ContextMenuItemModel
            {
                Text = Translations.SYNC_PROGRESS,
                OnClick = SyncProgress
            }
        ]);
    }
    

    private async Task<bool> CheckConnection()
    {
        if ((await ServerService.CanConnect()).IsFailure)
        {
            await Toast.Make(Translations.CONNECTION_FAILED, ToastDuration.Long).Show();
            return false;
        }

        return true;
    }

    private async Task SyncProgress()
    {
        if(!await CheckConnection())
            return;

        await ServerService.UpdateProgressByProfile(AppStateService.ProfileId);
        await Toast.Make(Translations.SYNC_COMPLETED).Show();
    }

    private async Task BackupProfile()
    {
        if(!await CheckConnection())
            return;
        
        var getSettings = await Sender.Send(new GetProfileSettings.Query(AppStateService.ProfileId));
        if(getSettings.IsFailure) return;
        
        await ServerService.UpdateProfileSettings(getSettings.Value);
        await Toast.Make("Backup completed").Show();
    }
}