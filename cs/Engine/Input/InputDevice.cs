namespace Engine.Input;

public abstract class InputDevice<TContext> : IInputDevice
{
    protected TContext[] _contexts;

    protected InputDevice()
    {
        _contexts = Array.Empty<TContext>();
    }

    public virtual void ClearContext()
    {
        _contexts = Array.Empty<TContext>();
    }

    public virtual void BindContext(params TContext[] contexts)
    {
        _contexts = contexts;
    }

    public virtual void Poll()
    {
        // Default implementation does nothing.
    }
}
