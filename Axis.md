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

- Create Axis.Engine project
- Move game engine code into Axis.Engine project
- Create pong application project

## Medium term tasks

- Reuse removed entity Ids
- Expand Add entity with more values
- Expand Query entities with more selections
- Replace old ECS in snake game with Axis.ECS
- Cloning an entity
- Investigate using Id's instead of component type ids
- Handle resizable windows

## Games

### Pong

- Create project
- Bootstrap engine and show window
- Start menu page - press button to start
- 2 Player game mode
- Add 2 paddles
- Add ball
- Add bounds
- Player input controller -> actions
- Paddle movement
- Ball movement
- Ball collision
- Goal collision
- Reset ball position on goal
- Points tracking
- Rendering points
- End game when points reached
- Display end game screen with winner
- Start new game
