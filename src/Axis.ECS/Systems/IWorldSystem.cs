namespace Axis.ECS;

public interface IWorldSystem
{
    void Execute(ref WorldSystemContext data);
}
