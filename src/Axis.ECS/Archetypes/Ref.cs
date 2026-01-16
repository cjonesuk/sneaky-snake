namespace Axis.ECS;

public ref struct Ref<T>
{
    public ref T Value;
    public Ref(ref T value) => Value = ref value;
}
