using System;
using System.Reflection.Emit;
using DefaultEcs;

namespace CityBuilder.Core.Components.Flags;

public static class FlagExtension
{
	public static void AddFlag<T>(this Entity entity, T value) where T : struct, Enum
	{
		if (entity.Has<T>())
		{
			var toLong = Converter<T>.ToLong;
			entity.Set((T)(object)(toLong(entity.Get<T>()) | toLong(value)));
		}
		else
		{
			entity.Set(value);
		}
	}

	public static void RemoveFlag<T>(this Entity entity, T removedValue) where T : struct, Enum
	{
		var toLong = Converter<T>.ToLong;

		var value = toLong(entity.Get<T>());
		value &= ~toLong(removedValue);

		if (value == 0)
		{
			entity.Remove<T>();
		}
		else
		{
			entity.Set((T)(object)value);
		}
	}

	private static class Converter<TEnum> where TEnum : struct, Enum
	{
		public static readonly Func<TEnum, long> ToLong = CreateConvertToLong<TEnum>();

		private static Func<T, long> CreateConvertToLong<T>() where T : struct, Enum
		{
			var method = new DynamicMethod(
				"ConvertToLong",
				typeof(long),
				new[] { typeof(T) },
				typeof(FlagExtension).Module,
				true);

			var ilGen = method.GetILGenerator();

			ilGen.Emit(OpCodes.Ldarg_0);
			ilGen.Emit(OpCodes.Conv_I8);
			ilGen.Emit(OpCodes.Ret);
			return (Func<T, long>)method.CreateDelegate(typeof(Func<T, long>));
		}
	}
}
