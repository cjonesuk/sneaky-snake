using System.Numerics;
using Axis.ECS;
using Axis.Engine;
using Axis.Engine.Input;
using Raylib_cs;

namespace Remnant.PlayGame;

internal sealed class PlayGameMode : IGameMode
{
    private readonly IRemnantGame _game;
    private readonly IGameEngine _engine;
    private readonly IWorld _world;
    private readonly EntityInputReceiver _cameraInputReceiver;
    private Entity _camera;

    public PlayGameMode(IRemnantGame game)
    {
        _game = game;
        _engine = game.Engine;
        _world = game.World;
        _cameraInputReceiver = new EntityInputReceiver();
    }

    void IGameMode.Activate()
    {
        _world.RemoveAllEntities();
        _world.RemoveAllSystems();
        _world.UnregisterAllQueries();

        SpawnScene();
        BindInputs();
        SetupSystems();

        _game.SetCamera(_camera);

        Console.WriteLine("PlayGameMode activated");
    }

    void IGameMode.Deactivate()
    {
        _engine.Devices.KeyboardAndMouse.ClearContext();
        Console.WriteLine("PlayGameMode deactivated");
    }

    private void SpawnScene()
    {
        var controller = new RtsCameraController
        {
            FocusPoint = Vector3.Zero,
            Distance = 30f,
            MinDistance = 8f,
            MaxDistance = 80f,
            PitchRadians = 50f * MathF.PI / 180f,
            YawRadians = -90f * MathF.PI / 180f,
            PanSpeed = 0.5f,
            ZoomSpeed = 2f,
        };

        _camera = _world.SpawnRtsCamera(controller);
        _cameraInputReceiver.SetEntity(_camera);

        _world.SpawnFloor(new Vector3(0f, -0.01f, 0f), new Vector2(100f, 100f), Color.DarkGray);

        _world.SpawnCube(new Vector3(0f, 0.5f, 0f), Vector3.One, Color.Red);
        _world.SpawnCube(new Vector3(10f, 0.5f, 0f), Vector3.One, Color.Green);
        _world.SpawnCube(new Vector3(0f, 0.5f, 10f), Vector3.One, Color.Blue);
    }

    private void BindInputs()
    {
        var cameraContext = new KeyboardInputContext(
            _cameraInputReceiver,
            keyDown: [
                new KeyboardInputMapping(KeyboardKey.W, PlayGameActions.PanUp),
                new KeyboardInputMapping(KeyboardKey.Up, PlayGameActions.PanUp),
                new KeyboardInputMapping(KeyboardKey.S, PlayGameActions.PanDown),
                new KeyboardInputMapping(KeyboardKey.Down, PlayGameActions.PanDown),
                new KeyboardInputMapping(KeyboardKey.A, PlayGameActions.PanLeft),
                new KeyboardInputMapping(KeyboardKey.Left, PlayGameActions.PanLeft),
                new KeyboardInputMapping(KeyboardKey.D, PlayGameActions.PanRight),
                new KeyboardInputMapping(KeyboardKey.Right, PlayGameActions.PanRight),
            ],
            keyPressed: [],
            mouseWheel: [
                new MouseWheelInputMapping(MouseWheelDirection.Up, PlayGameActions.ZoomIn),
                new MouseWheelInputMapping(MouseWheelDirection.Down, PlayGameActions.ZoomOut),
            ]);

        _engine.Devices.KeyboardAndMouse.BindContext([cameraContext]);
    }

    private void SetupSystems()
    {
        _world.AddSystem(new RtsCameraInputSystem());
        _world.AddSystem(new RtsCameraSystem());
    }
}
