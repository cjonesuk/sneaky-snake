namespace Engine.Input;

public interface IInputDevice
{
    void Poll();
    void ClearContext();
}
