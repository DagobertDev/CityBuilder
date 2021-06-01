using System.Collections.Generic;

namespace CityBuilder.BehaviorTree.Nodes
{
    /// <summary>
    /// Runs child nodes in sequence, until one fails.
    /// </summary>
    public class SequenceNode<T> : IParentBehaviourTreeNode<T>
    {
        /// <summary>
        /// List of child nodes.
        /// </summary>
        private readonly List<IBehaviourTreeNode<T>> _children = new(); //todo: this could be optimized as a baked array.

        public BehaviourTreeStatus Tick(T context)
        {
            foreach (var child in _children)
            {
                var childStatus = child.Tick(context);

                if (childStatus != BehaviourTreeStatus.Success)
                {
                    return childStatus;
                }
            }

            return BehaviourTreeStatus.Success;
        }

        /// <summary>
        /// Add a child to the sequence.
        /// </summary>
        public void AddChild(IBehaviourTreeNode<T> child)
        {
            _children.Add(child);
        }
    }
}
