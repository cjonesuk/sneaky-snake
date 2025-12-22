namespace Engine.Input;

public interface IDeviceManager
{
    IKeyboardAndMouseDevice KeyboardAndMouse { get; }

    void Poll();
}
