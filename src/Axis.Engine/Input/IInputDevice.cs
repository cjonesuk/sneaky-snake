namespace Axis.Engine.Input;

public interface IInputDevice
{
    void Poll();
    void ClearContext();
}
