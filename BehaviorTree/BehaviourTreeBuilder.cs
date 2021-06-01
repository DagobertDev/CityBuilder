using System;
using System.Collections.Generic;

namespace CityBuilder.BehaviorTree
{
    /// <summary>
    /// Fluent API for building a behaviour tree.
    /// </summary>
    public class BehaviourTreeBuilder<T>
    {
        /// <summary>
        /// Last node created.
        /// </summary>
        private IBehaviourTreeNode<T>? _currentNode;

        /// <summary>
        /// Stack node nodes that we are build via the fluent API.
        /// </summary>
        private readonly Stack<IParentBehaviourTreeNode<T>> _parentNodeStack = new();

        public BehaviourTreeBuilder<T> AddLeafNode(IBehaviourTreeNode<T> node)
        {
	        if (_parentNodeStack.Count == 0)
	        {
		        throw new ApplicationException("Can't create an unnested leaf node.");
	        }

	        _parentNodeStack.Peek().AddChild(node);
	        return this;
        }

        public BehaviourTreeBuilder<T> AddParentNode(IParentBehaviourTreeNode<T> node)
        {
	        if (_parentNodeStack.Count > 0)
	        {
		        _parentNodeStack.Peek().AddChild(node);
	        }

	        _parentNodeStack.Push(node);
	        return this;
        }

        /// <summary>
        /// Build the actual tree.
        /// </summary>
        public IBehaviourTreeNode<T> Build()
        {
            if (_currentNode == null)
            {
                throw new ApplicationException("Can't create a behaviour tree with zero nodes");
            }

            return _currentNode;
        }

        /// <summary>
        /// Ends a sequence of children.
        /// </summary>
        public BehaviourTreeBuilder<T> End()
        {
            _currentNode = _parentNodeStack.Pop();
            return this;
        }
    }
}
