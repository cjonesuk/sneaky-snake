# Sneaky Snake

This repo is for making a very simple blocky snake based game. The purpose is to build knowledge of how to make simple games in C#

The run the game using the following commands

```bash
cd cs
dotnet run
```

## Functionality

### Entity Component System (ECS)

#### ECS Todo

- Fixed size arrays for components within an archetype instead of using `List` and `CollectionsMarshal.AsSpan`
- Defer removal of entities until start of a frame to avoid references to components becoming invalid
- `RemoveEntity()` by swapping components with last entry in the array to avoid gaps
- Find entities by component (e.g. all entities with the `PlayerTag` component)

### Rendering

#### Rendering Todo

- Add a debug view to display fps and allow toggling it on and off
- Pooling for RenderPass's
- Resizable windows

### Input

#### Input Todo

- Device abstraction and KeyboardDevice implementation
- GameEngine provides list of available devices
- InputContext concept that can have devices attached
- InputContext can be enabled causing the engine to trigger processing at the start of each frame
- InputContext has mappings from a device input to an InputAction which is emitted (for now logging the action)
- GamePadDevice implementation

## General Todo

- Refactor into Game and Engine projects moving everything into a `src` directory
- Expand test coverage

## Planning

### Definitions

- Game Engine - manages everything and runs the game loop
- Game Instance - Single instance of the Game that oversees all of the games high level states (start menu, play)
- World - Manages entities and world specific systems such as Physics. The engine can manage 1 or more worlds simulateously
- ECS - Entity Component System is a way of managing everything in the world by composing small components together
- Entities - An int ID representing something in the world. An entity is composed of one or more components
- Components - A typed piece of state attached to an entity. Systems can process these in bulk.
- System - A service updated each frame that can perform operations on components in an efficient way.
- Render Passes - A set of render command specific render queues
- Render Queue - A queue of type specific render commands
- Render Command - A set of data for something to be rendered (e.g. a sprite, mesh, text)
- Renderer - Processes a render queue's commands to render the actual pixels
- Viewport
- Camera
- World Renderer - Given a world and a camera, adds render commands to queues in the render pass

### World

GameEngine -> Game Instance
