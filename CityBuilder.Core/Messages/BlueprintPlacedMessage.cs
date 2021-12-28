using CityBuilder.Core.Components;
using CityBuilder.Core.ModSupport;

namespace CityBuilder.Core.Messages;

public readonly record struct BlueprintPlacedMessage(Blueprint Blueprint, Position Position, Rotation Rotation);
