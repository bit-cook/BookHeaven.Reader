using Microsoft.AspNetCore.Components;

namespace BookHeaven.Reader.Components.Shared.ContextMenu;

public class ContextMenuModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public double AnchorX { get; set; }
    public double AnchorY { get; set; }
    public RenderFragment? Content { get; set; }
}