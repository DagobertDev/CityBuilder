using System;
using System.Collections.Generic;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Inventory;
using CityBuilder.Core.Messages;
using CityBuilder.Core.Systems;
using DefaultEcs;
using Godot;

namespace CityBuilder.GUI
{
	public class InventoryInspector : ItemList
	{
		private readonly IDictionary<int, Action> _actions = new Dictionary<int, Action>();

		public override void _Ready()
		{
			Connect("item_activated", this, nameof(OnItemActivated));

			GetParent().GetParent<EntityInspector>().EntityUpdated += (_, entity) =>
			{
				Clear();
				_actions.Clear();

				AddWorker(entity);
				AddHousing(entity);
				AddWorkplace(entity);
				AddTransport(entity);
				AddInventory(entity);
			};
		}

		private void AddItem(string text, Texture? icon = null)
		{
			AddItem(text, icon, false);
		}

		private void AddEmptyLine()
		{
			if (GetItemCount() > 0)
			{
				AddItem(" ", selectable: false);
			}
		}

		private void AddHeading(string text)
		{
			AddEmptyLine();
			AddItem(text, selectable: false);
			var index = GetItemCount() - 1;
			SetItemCustomBgColor(index, Colors.DarkSlateGray);
		}

		private void AddAction(string text, Action action)
		{
			AddItem(text, selectable: true);
			_actions[GetItemCount() - 1] = action;
		}

		private void OnItemActivated(int index)
		{
			if (_actions.TryGetValue(index, out var action))
			{
				action();
			}
		}

		private void AddWorker(Entity entity)
		{
			if (entity.Has<Agent>() && entity.Get<Agent>().Type == AIType.Worker)
			{
				AddHeading("Needs");
				AddItem($"Hunger: {entity.Get<Hunger>().Value:N0}");
				AddItem($"Sleep: {entity.Get<Tiredness>().Value:N0}");

				if (entity.Has<Resident>())
				{
					var resident = entity.Get<Resident>();
					AddEmptyLine();
					AddAction($"House: {resident.Home}",
						() => Game.World.Publish(new EntitySelected(resident.Home)));
				}
				else
				{
					AddItem("Homeless");
				}

				if (entity.Has<Employee>())
				{
					var employee = entity.Get<Employee>();
					AddEmptyLine();
					AddAction($"Workplace: {employee.Workplace}",
						() => Game.World.Publish(new EntitySelected(employee.Workplace)));
				}
				else
				{
					AddItem("Unemployed");
				}
			}
		}

		private void AddHousing(Entity entity)
		{
			if (entity.Has<Housing>())
			{
				var housing = entity.Get<Housing>();
				AddHeading("Housing");
				AddItem($"Used beds: {housing.UsedBeds}, Total beds: {housing.MaxBeds}");

				if (housing.UsedBeds > 0)
				{
					var housingSystem = Game.World.Get<HousingSystem>();
					var residents = housingSystem.GetResidents(entity);

					AddItem("Residents:");

					foreach (var resident in residents)
					{
						AddAction($"{resident}", () => Game.World.Publish(new EntitySelected(resident)));
					}
				}
			}
		}

		private void AddWorkplace(Entity entity)
		{
			if (entity.Has<Workplace>())
			{
				var workplace = entity.Get<Workplace>();
				AddHeading("Workplace");
				AddItem($"Workers: {workplace.CurrentEmployees}, Total workplaces: {workplace.MaxEmployees}");

				if (workplace.CurrentEmployees > 0)
				{
					var workSystem = Game.World.Get<WorkSystem>();
					var employees = workSystem.GetEmployees(entity);

					AddEmptyLine();
					AddItem("Workers:");

					foreach (var employee in employees)
					{
						AddAction($"{employee}", () => Game.World.Publish(new EntitySelected(employee)));
					}
				}
			}
		}

		private void AddTransport(Entity entity)
		{
			if (entity.Has<TransportCapacity>())
			{
				AddHeading("Transporter");
				AddItem($"Max capacity: {entity.Get<TransportCapacity>().Value}");
			}

			if (entity.Has<Transport>())
			{
				var (from, to, good, amount, delivering) = entity.Get<Transport>();
				AddHeading("Transport");
				AddItem($"Transporting {amount.Value} {good.Name} from {from} to {to}.");
				AddItem($"Currently going to the {(delivering ? "end" : "start")} of the route.");
			}
		}

		private void AddInventory(Entity entity)
		{
			var inventorySystem = Game.World.Get<IInventorySystem>();

			var goods = inventorySystem.GetGoods(entity);

			if (goods is { Count: > 0 })
			{
				AddHeading("Inventory");

				foreach (var good in goods)
				{
					AddItem($"{good.Get<Good>().Name}: {good.Get<Amount>().Value}");
				}
			}
		}
	}
}
