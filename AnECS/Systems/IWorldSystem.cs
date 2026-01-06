namespace AnECS;

internal interface IWorldSystem
{
    void Execute(ref WorldSystemData data);
}
