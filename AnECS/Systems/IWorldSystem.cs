namespace AnECS;

public interface IWorldSystem
{
    void Execute(ref WorldSystemData data);
}
