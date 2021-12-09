using System.Collections;
using Godot;
using Godot.Collections;

namespace CityBuilder.GUI
{
	public class JsonTree : Tree
	{
		public void SetData(object data)
		{
			Clear();
			var root = CreateItem();
			CreateFromValue(data, root);
		}

		private void CreateFromValue(object data, TreeItem parent)
		{
			switch (data)
			{
				case IDictionary dictionary:
					CreateFromDictionary(dictionary, parent);
					break;
				
				case IList list:
					CreateFromList(list, parent);
					break;
				
				default:
					var child = CreateItem(parent);
					child.SetText(0, data.ToString());
					break;
			}
		}

		private void CreateFromDictionary(IDictionary dictionary, TreeItem parent)
		{
			foreach (var key in dictionary.Keys)
			{
				var child = CreateItem(parent);

				var value = dictionary[key];
				
				switch (value)
				{
					case IDictionary dict:
						child.SetText(0, key.ToString());
						CreateFromDictionary(dict, child);
						break;
				
					case Array array:
						child.SetText(0, key.ToString());
						CreateFromList(array, child);
						break;
					
					default:
						child.SetText(0, key.ToString());
						child.SetText(1, value?.ToString());
						break;
				}
			}
		}

		private void CreateFromList(IList list, TreeItem parent)
		{
			foreach (var value in list)
			{
				CreateFromValue(value, parent);
			}
		}
	}
}
