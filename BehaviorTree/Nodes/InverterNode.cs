using System;

namespace CityBuilder.BehaviorTree.Nodes
{
    /// <summary>
    /// Decorator node that inverts the success/failure of its child.
    /// </summary>
    public class InverterNode<T> : IParentBehaviourTreeNode<T>
    {
	    /// <summary>
        /// The child to be inverted.
        /// </summary>
        private IBehaviourTreeNode<T>? _childNode;

        public BehaviourTreeStatus Tick(T context)
        {
            if (_childNode == null)
            {
                throw new ApplicationException("InverterNode must have a child node!");
            }

            var result = _childNode.Tick(context);

            return result switch
            {
	            BehaviourTreeStatus.Failure => BehaviourTreeStatus.Success,
	            BehaviourTreeStatus.Success => BehaviourTreeStatus.Failure,
	            _ => result
            };
        }

        /// <summary>
        /// Add a child to the parent node.
        /// </summary>
        public void AddChild(IBehaviourTreeNode<T> child)
        {
            if (_childNode != null)
            {
                throw new ApplicationException("Can't add more than a single child to InverterNode!");
            }

            _childNode = child;
        }
    }
}
