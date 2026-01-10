using Axis.ECS;
using Axis.Engine.Components;
using Engine.Components;

namespace Axis.Engine.Rendering;



public readonly record struct LayerId(int Value);

public sealed class RenderPassManager
{
    private readonly Dictionary<LayerId, RenderPass> _passes;

    public RenderPassManager()
    {
        _passes = new Dictionary<LayerId, RenderPass>();
    }

    public RenderPass GetOrCreatePass(LayerId layer)
    {
        if (!_passes.TryGetValue(layer, out var pass))
        {
            pass = new RenderPass();
            _passes[layer] = pass;
        }

        return pass;
    }
}

/// <summary>
/// 
/// </summary>
public sealed class RenderPass
{

}

internal sealed class WorldRenderManager
{
    public WorldRenderManager()
    {
    }

    public void Render(IWorld world, Camera2dRenderView view, RenderPassManager renderPasses)
    {
        // Problems
        // 1. Need access to context within queries
        // 2. Need to define render passes
        // 3. Need to define render queues within passes
        // 4. Need to make render commands

        world.QueryEach((ref Id id, ref Transform2d transform, ref BasicShape shape) =>
        {


        });
    }
}