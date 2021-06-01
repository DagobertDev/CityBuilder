using System;

namespace CityBuilder.BehaviorTree.Nodes
{
	public static class BuilderExtensions
	{
		/// <summary>
		/// Create an action node.
		/// </summary>
		public static BehaviourTreeBuilder<T> Do<T>(this BehaviourTreeBuilder<T> builder,
			Func<T, BehaviourTreeStatus> func)
		{
			return builder.AddLeafNode(new ActionNode<T>(func));
		}

		/// <summary>
		/// Like an action node... but the function can return true/false and is mapped to success/failure.
		/// </summary>
		public static BehaviourTreeBuilder<T> Condition<T>(this BehaviourTreeBuilder<T> builder,
			Func<T, bool> condition)
		{
			return builder.Do(context => condition(context) ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure);
		}

		/// <summary>
		/// Create an inverter node that inverts the success/failure of its children.
		/// </summary>
		public static BehaviourTreeBuilder<T> Invert<T>(this BehaviourTreeBuilder<T> builder)
		{
			return builder.AddParentNode(new InverterNode<T>());
		}

		/// <summary>
		/// Create a sequence node.
		/// </summary>
		public static BehaviourTreeBuilder<T> Sequence<T>(this BehaviourTreeBuilder<T> builder)
		{
			return builder.AddParentNode(new SequenceNode<T>());
		}

		/// <summary>
		/// Create a parallel node.
		/// </summary>
		public static BehaviourTreeBuilder<T> Parallel<T>(this BehaviourTreeBuilder<T> builder, int numRequiredToFail,
			int numRequiredToSucceed)
		{
			return builder.AddParentNode(new ParallelNode<T>(numRequiredToFail, numRequiredToSucceed));
		}

		/// <summary>
		/// Create a selector node.
		/// </summary>
		public static BehaviourTreeBuilder<T> Selector<T>(this BehaviourTreeBuilder<T> builder)
		{
			return builder.AddParentNode(new SelectorNode<T>());
		}

		/// <summary>
		/// Splice a sub tree into the parent tree.
		/// </summary>
		public static BehaviourTreeBuilder<T> Splice<T>(this BehaviourTreeBuilder<T> builder,
			IBehaviourTreeNode<T> subTree)
		{
			return builder.AddLeafNode(subTree);
		}
	}
}
