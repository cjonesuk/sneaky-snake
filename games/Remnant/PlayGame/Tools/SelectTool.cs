using System.Numerics;
using Axis.ECS;
using Axis.ECS.Queries;
using Axis.Engine.Components;
using Raylib_cs;

namespace Remnant.PlayGame.Tools;

internal sealed class SelectTool : ITool
{
    private static readonly Color OutlineColor = new(255, 230, 50, 255);
    private readonly List<Entity> _toDeselect = new();
    private readonly List<Entity> _toCommand = new();
    private Query<Transform3d, BasicShape3d>? _pickableQuery;
    private Query? _selectedQuery;

    void ITool.OnActivate(ToolContext context)
    {
        Console.WriteLine("SelectTool activated");
    }

    void ITool.OnDeactivate(ToolContext context)
    {
        Console.WriteLine("SelectTool deactivated");
    }


    private Query SelectedQuery(ToolContext context)
    {
        _selectedQuery ??= DefineQuery.For(context.World).With<Selected>().Build();
        return _selectedQuery.Value;
    }

    void ITool.Update(ToolContext context)
    {

        if (context.MouseState.WasPressed(MouseButton.Left))
        {
            HandleSelection(context);
            return;
        }
        else if (context.MouseState.WasPressed(MouseButton.Right))
        {
            HandleMoveCommand(context);
        }
    }

    private void HandleSelection(ToolContext context)
    {
        Ray ray = context.PointerRay.Ray;
        Entity bestHit = Entity.Invalid;
        float bestDistance = float.MaxValue;

        _pickableQuery ??= DefineQuery.For<Transform3d, BasicShape3d>(context.World).With<Selectable>().Build();
        _pickableQuery.Value.ForEach((Entity entity, ref Transform3d transform, ref BasicShape3d shape) =>
        {
            var halfSize = shape.Size * 0.5f;
            var box = new BoundingBox(transform.Position - halfSize, transform.Position + halfSize);
            var collision = Raylib.GetRayCollisionBox(ray, box);
            if (collision.Hit && collision.Distance < bestDistance)
            {
                bestDistance = collision.Distance;
                bestHit = entity;
            }
        });

        _toDeselect.Clear();
        SelectedQuery(context).ForEach(_toDeselect.Add);

        foreach (var entity in _toDeselect)
        {
            entity.Remove<Selected>();
            entity.Remove<Outline>();
        }

        if (bestHit.IsValid())
        {
            bestHit.Add<Selected>();
            bestHit.Set(new Outline(OutlineColor));
        }
    }

    private void HandleMoveCommand(ToolContext context)
    {
        if (!context.PointerRay.HasGroundHit)
        {
            return;
        }

        Vector3 groundHit = context.PointerRay.GroundHitPoint;

        _toCommand.Clear();

        SelectedQuery(context).ForEach(_toCommand.Add);

        foreach (var entity in _toCommand)
        {
            float currentY = entity.GetRef<Transform3d>().Position.Y;
            var target = new Vector3(groundHit.X, currentY, groundHit.Z);
            entity.Set(new MoveTarget(target));
        }
    }

}
