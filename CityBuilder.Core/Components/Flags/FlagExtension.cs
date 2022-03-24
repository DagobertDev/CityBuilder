using DefaultEcs;

namespace CityBuilder.Core.Components.Flags;

public static class FlagExtension
{
	public static void AddFlag(this Entity entity, CanNotWorkReason value)
	{
		if (entity.Has<CanNotWorkReason>())
		{
			entity.Set(entity.Get<CanNotWorkReason>() | value);
		}
		else
		{
			entity.Set(value);
		}
	}

	public static void RemoveFlag(this Entity entity, CanNotWorkReason value)
	{
		if (!entity.Has<CanNotWorkReason>())
		{
			return;
		}

		var newValue = entity.Get<CanNotWorkReason>();
		newValue &= ~value;

		if (newValue == 0)
		{
			entity.Remove<CanNotWorkReason>();
		}
		else
		{
			entity.Set(newValue);
		}
	}
}
