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
-

## General Todo

- Refactor into Game and Engine projects moving everything into a `src` directory
- Expand test coverage
