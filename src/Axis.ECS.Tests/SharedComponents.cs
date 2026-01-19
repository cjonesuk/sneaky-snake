namespace Axis.ECS.Tests;

internal record struct Position(float X, float Y);
internal record struct Velocity(float DX, float DY);
internal record struct PlayerTag();

internal record struct Health(int Value);
internal record struct Healing(int Amount);

// Additional components used for higher-arity creation and system tests
internal record struct Armor(int Value);
internal record struct Mana(int Value);
internal record struct Stamina(int Value);
