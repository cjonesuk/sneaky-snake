# Axis ECS

## Planning

WindowRenderTarget contains a list of Viewports (fullscreen, 2 player split screen)
Each viewport contains a Camera entity (and therefore linked to a world)
Rendering into the viewport will occur in layers (background, scene, ui, debug)
Each layer will consistent of a render pass which will be a sorted list of render commands (sorted indexes to render commands?)

A world renderer will be responsible for querying into the world based on the camera (TBD) and filling per thread render command lists
Each render command will be a delegate that recieves an unmanaged payload and performs the render
The render command lists will be merged into per layer render passes which commands are sorted. for 2D this will be by zindex.

When rendering, the viewport is cleared and for each pass the render command list will be iterated in order, running the delegate action that will perform rendering

## Short term tasks

- Clean up old snake game and engine code
- Rename repo?

## Medium term tasks

- Reuse removed entity Ids
- Cloning an entity
- Investigate using Id's instead of component type ids
- Handle resizable windows

## Games

### Pong

- Points tracking
- Rendering points
- End game when points reached
- Display end game screen with winner
- Start new game
