namespace Engine.Input;

public interface IKeyboardAndMouseDevice : IInputDevice
{
    void BindContext(params KeyboardInputContext[] contexts);
}
