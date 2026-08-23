namespace BookHeaven.Reader.Components.Shared.ContextMenu;

public interface IContextMenuService
{
    ContextMenuModel? ActiveMenu { get; }
    void OpenMenu(int x, int y, List<ContextMenuItemModel> items);
    void OpenFixedMenu(string cssClass, List<ContextMenuItemModel> items);
    void CloseMenu();
    event Action? OnChange;
}

public class ContextMenuService : IContextMenuService
{
    public ContextMenuModel? ActiveMenu { get; private set; }

    public event Action? OnChange;

    public void OpenMenu(int x, int y, List<ContextMenuItemModel> items)
    {
        ActiveMenu = new ContextMenuModel
        {
            IsOpen = true,
            AnchorX = x,
            AnchorY = y,
            Items = items
        };
        OnChange?.Invoke();
    }
    
    public void OpenFixedMenu(string cssClass, List<ContextMenuItemModel> items)
    {
        ActiveMenu = new ContextMenuModel
        {
            IsOpen = true,
            CssClass = cssClass,
            Items = items
        };
        OnChange?.Invoke();
    }

    public void CloseMenu()
    {
        if (ActiveMenu == null) return;
        
        ActiveMenu.IsOpen = false;
        ActiveMenu = null;
        OnChange?.Invoke();
    }
}