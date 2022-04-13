using System;
using System.Collections.Generic;
using System.Linq;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Flags;
using CityBuilder.Core.Components.Needs;
using CityBuilder.Core.Components.Production;
using DefaultEcs;
using Godot;
using Input = CityBuilder.Core.Components.Production.Input;
using Resource = CityBuilder.Core.Components.Production.Resource;

namespace CityBuilder.ModSupport;

public record Blueprint
(
	string Name,
	Texture Texture,
	string? Category,
	Housing? Housing,
	Workplace? Workplace,
	string? ResourceCollector,
	Resource? Resource,
	Blueprint.SerializedGoods? Input,
	Blueprint.SerializedGoods? Output,
	Agent? Agent,
	float? Speed,
	int? TransportCapacity,
	Blueprint.SerializedConstruction? Construction,
	RemoveRequest? RemoveRequest,
	string? CollectResource,
	float? HungerRate,
	float? TirednessRate,
	WantsHousing? WantsHousing,
	WantsRecreation? WantsRecreation,
	WantsWork? WantsWork,
	Market? Market,
	float? GrowthRate
)
{
	public void Populate(Entity entity)
	{
		if (Housing is { } housing)
		{
			entity.Set(housing);
		}

		if (Workplace is { } workplace)
		{
			entity.Set(workplace);
		}

		if (ResourceCollector is { } resourceCollector)
		{
			entity.Set(new ResourceCollector(resourceCollector));
		}

		if (Resource is { } resource)
		{
			entity.Set(resource);
		}

		if (Input is { } input)
		{
			entity.Set(new Input(input));
		}

		if (Output is { Count: 1 } output)
		{
			var value = output.Single();
			entity.Set(new Output(value.Key, value.Value));
		}

		if (Agent is { } agent)
		{
			entity.Set(agent);
		}

		if (Speed is { } speed)
		{
			entity.Set<Speed>(speed);
		}

		if (TransportCapacity is { } transportCapacity)
		{
			entity.Set<TransportCapacity>(transportCapacity);
		}

		if (HungerRate is { } hungerRate)
		{
			entity.Set<HungerRate>(hungerRate);
		}

		if (TirednessRate is { } tirednessRate)
		{
			entity.Set<TirednessRate>(tirednessRate);
		}

		if (WantsHousing is { } wantsHousing)
		{
			entity.Set(wantsHousing);
		}

		if (WantsRecreation is { } wantsRecreation)
		{
			entity.Set(wantsRecreation);
		}

		if (WantsWork is { } wantsWork)
		{
			entity.Set(wantsWork);
		}

		if (Market is { } market)
		{
			entity.Set(market);
		}

		if (RemoveRequest is { } removeRequest)
		{
			entity.Set(removeRequest);
		}

		if (CollectResource is { } collectResource)
		{
			entity.Set(new CollectResource(collectResource));
		}

		if (GrowthRate is { } growthRate)
		{
			entity.Set<GrowthRate>(growthRate);
		}

		entity.Set(Texture);
	}

	public void PopulateConstruction(Entity entity)
	{
		entity.Set<Construction>();
		entity.Set(Construction?.Workplace ?? throw new ApplicationException("No Workplace provided"));

		if (Construction.Input is { } input)
		{
			entity.Set(new Input(input));
		}

		entity.Set(this);
		entity.Set(Texture);
	}

	public record SerializedConstruction(Workplace Workplace, SerializedGoods? Input);

	public class SerializedGoods : Dictionary<string, int> { }
}
