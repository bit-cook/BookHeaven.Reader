using Microsoft.AspNetCore.Components;

namespace BookHeaven.Reader.Components.Shared.ContextMenu;

public class ContextMenuModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public bool IsOpen { get; set; }
    public string? CssClass { get; set; }
    public int AnchorX { get; set; }
    public int AnchorY { get; set; }
    public List<ContextMenuItemModel> Items { get; set; } = new();
}

public class ContextMenuItemModel
{
    public string? Icon { get; set; }
    public string? Text { get; set; }
    public bool IsSeparator { get; set; }
    public bool IsDisabled { get; set; }
    public Func<Task>? OnClick { get; set; }
    public List<ContextMenuItemModel> SubItems { get; set; } = [];
    public string? CssClass { get; set; }
}