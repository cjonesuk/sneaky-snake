namespace Axis.ECS;

public interface IWorldSystem
{
    void Execute(ref WorldSystemData data);
}
