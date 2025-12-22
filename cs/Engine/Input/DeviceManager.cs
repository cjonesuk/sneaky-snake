namespace Engine.Input;

internal sealed class DeviceManager : IDeviceManager
{
    public IKeyboardAndMouseDevice KeyboardAndMouse { get; }

    public DeviceManager()
    {
        KeyboardAndMouse = new KeyboardAndMouseDevice();
    }

    public void Poll()
    {
        KeyboardAndMouse.Poll();
    }
}
