using System;

namespace CityBuilder.Core.Components;

/// <summary>
///     Rotation of an object in degree.
///     Value must be between 0 and 360
/// </summary>
public readonly record struct Rotation
{
	public Rotation(int value)
	{
		if (value is < 0 or > 360)
		{
			throw new ArgumentOutOfRangeException(nameof(value));
		}

		Value = value;
	}

	public int Value { get; }

	public static implicit operator int(Rotation rotation) => rotation.Value;
	public static implicit operator Rotation(int value) => new(value);
}
