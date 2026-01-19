namespace Axis.ECS;

public record struct WorldSystemContext(IWorld World, float DeltaTime);

public record struct EntityEventContext(Entity Entity, float DeltaTime);