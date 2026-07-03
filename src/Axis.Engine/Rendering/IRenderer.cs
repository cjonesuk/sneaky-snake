namespace Axis.Engine.Rendering;

public interface ISubRenderer
{

}

public interface IRenderer
{
    void Apply(IRenderPrepContext context);
}

