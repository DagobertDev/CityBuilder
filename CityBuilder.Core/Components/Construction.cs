using DefaultEcs;

namespace CityBuilder.Core.Components;

/// <summary>
///     Flags an entity as being responsible for the construction of a building
/// </summary>
public readonly record struct Construction;

/// <summary>
///     References the entity, that contains describes the construction
/// </summary>
/// <param name="Value"></param>
public readonly record struct ConstructionReference(Entity Value);
