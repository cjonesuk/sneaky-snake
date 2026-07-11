namespace Remnant.PlayGame.Tools;

internal interface ITool
{
    void OnActivate(ToolContext context);
    void OnDeactivate(ToolContext context);
    void Update(ToolContext context);
}
