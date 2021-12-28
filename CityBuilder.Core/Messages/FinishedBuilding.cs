using CityBuilder.Core.Components;
using CityBuilder.Core.ModSupport;

namespace CityBuilder.Core.Messages;

public readonly record struct FinishedBuilding(Blueprint Blueprint, Position Position, Rotation Rotation);
