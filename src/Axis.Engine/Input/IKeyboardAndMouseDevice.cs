namespace Axis.Engine.Input;

public interface IKeyboardAndMouseDevice : IInputDevice
{
    /// <summary>Live mouse position and button-edge state. Updated every poll.</summary>
    MouseState Mouse { get; }

    void BindContext(params KeyboardInputContext[] contexts);
}
