using System.Numerics;
using Axis.ECS;
using Axis.Engine;
using Axis.Engine.Input;
using Raylib_cs;
using Remnant.PlayGame.Collision;
using Remnant.PlayGame.Systems;
using Remnant.PlayGame.Tools;

namespace Remnant.PlayGame;

internal sealed class PlayGameMode : IGameMode
{
    private static float CollisionCellSize = 10f;
    private static int CollisionColumns = 10;
    private static int CollisionRows = 10;
    private static Vector2 WorldSize = new Vector2(CollisionCellSize * CollisionColumns, CollisionCellSize * CollisionRows);
    private readonly IRemnantGame _game;
    private readonly IGameEngine _engine;
    private readonly IWorld _world;
    private readonly ICollisionWorld _collisionWorld;
    private readonly EntityInputReceiver _cameraInputReceiver;
    private readonly RemnantPlayHud _hud;
    private Entity _camera;
    private readonly ToolContext _toolContext;
    private readonly SelectTool _selectTool;

    public PlayGameMode(IRemnantGame game)
    {
        _game = game;
        _engine = game.Engine;
        _world = game.World;
        _collisionWorld = InitialiseCollisionWorld();
        _cameraInputReceiver = new EntityInputReceiver();
        _hud = new RemnantPlayHud();

        _selectTool = new SelectTool();
        _toolContext = new ToolContext(
            _world,
            _collisionWorld,
            Entity.Invalid,
            _engine.Mouse,
            _selectTool);
    }

    private ICollisionWorld InitialiseCollisionWorld()
    {
        var broadphase = new UniformGridBroadphase(
            Vector2.Zero,
            CollisionCellSize,
            CollisionColumns,
            CollisionRows);
        return new CollisionWorld(broadphase);
    }

    void IGameMode.Activate()
    {
        _world.RemoveAllEntities();
        _world.RemoveAllSystems();

        SpawnScene();
        BindInputs();
        SetupSystems();

        _game.SetCamera(_camera);
        _toolContext.SetCamera(_camera);

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

        _world.SpawnFloor(new Vector3(0f, -0.01f, 0f), WorldSize, Color.DarkGray);

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
        // Input systems
        _world.AddSystem(new RemnantUiSystem(_game.Ui, _hud, _engine.Mouse));
        _world.AddSystem(new RtsCameraInputSystem());

        // Simulation systems
        _world.AddSystem(new RtsCameraSystem());
        _world.AddSystem(new MovementSystem());

        // Collision systems
        _world.AddSystem(new CollisionSyncSystem(_collisionWorld));

        // Action systems
        _world.AddSystem(new ToolSystem(_toolContext));
    }
}
