using System.Collections.Generic;

namespace CityBuilder.BehaviorTree.Nodes
{
    /// <summary>
    /// Runs childs nodes in parallel.
    /// </summary>
    public class ParallelNode<T> : IParentBehaviourTreeNode<T>
    {
	    /// <summary>
        /// List of child nodes.
        /// </summary>
        private readonly List<IBehaviourTreeNode<T>> _children = new();

        /// <summary>
        /// Number of child failures required to terminate with failure.
        /// </summary>
        private readonly int _numRequiredToFail;

        /// <summary>
        /// Number of child successess require to terminate with success.
        /// </summary>
        private readonly int _numRequiredToSucceed;

        public ParallelNode(int numRequiredToFail, int numRequiredToSucceed)
        {
	        _numRequiredToFail = numRequiredToFail;
            _numRequiredToSucceed = numRequiredToSucceed;
        }

        public BehaviourTreeStatus Tick(T context)
        {
            var numChildrenSucceeded = 0;
            var numChildrenFailed = 0;

            foreach (var child in _children)
            {
	            var childStatus = child.Tick(context);

	            switch (childStatus)
	            {
		            case BehaviourTreeStatus.Success:
			            ++numChildrenSucceeded;
			            break;
		            case BehaviourTreeStatus.Failure:
			            ++numChildrenFailed;
			            break;
	            }
            }

            if (_numRequiredToSucceed > 0 && numChildrenSucceeded >= _numRequiredToSucceed)
            {
                return BehaviourTreeStatus.Success;
            }

            if (_numRequiredToFail > 0 && numChildrenFailed >= _numRequiredToFail)
            {
                return BehaviourTreeStatus.Failure;
            }

            return BehaviourTreeStatus.Running;
        }

        public void AddChild(IBehaviourTreeNode<T> child)
        {
            _children.Add(child);
        }
    }
}
