using System.Collections.Generic;

namespace CityBuilder.BehaviorTree.Nodes
{
    /// <summary>
    /// Selects the first node that succeeds. Tries successive nodes until it finds one that doesn't fail.
    /// </summary>
    public class SelectorNode<T> : IParentBehaviourTreeNode<T>
    {
	    /// <summary>
        /// List of child nodes.
        /// </summary>
        private readonly List<IBehaviourTreeNode<T>> _children = new(); //todo: optimization, bake this to an array.

	    public BehaviourTreeStatus Tick(T context)
        {
            foreach (var child in _children)
            {
                var childStatus = child.Tick(context);
                if (childStatus != BehaviourTreeStatus.Failure)
                {
                    return childStatus;
                }
            }

            return BehaviourTreeStatus.Failure;
        }

        /// <summary>
        /// Add a child node to the selector.
        /// </summary>
        public void AddChild(IBehaviourTreeNode<T> child)
        {
            _children.Add(child);
        }
    }
}
