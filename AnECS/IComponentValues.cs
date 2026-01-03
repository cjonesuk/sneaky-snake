namespace AnECS;

internal interface IComponentValues
{
    void Add(object value);
    void Migrate(IComponentValues source, int sourceIndex);
}
