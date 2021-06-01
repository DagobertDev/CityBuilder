using System;

namespace CityBuilder.BehaviorTree.Nodes
{
    /// <summary>
    /// A behaviour tree leaf node for running an action.
    /// </summary>
    public class ActionNode<T> : IBehaviourTreeNode<T>
    {
        /// <summary>
        /// Function to invoke for the action.
        /// </summary>
        private readonly Func<T, BehaviourTreeStatus> _func;

        public ActionNode(Func<T, BehaviourTreeStatus> func)
        {
	        _func = func;
        }

        public BehaviourTreeStatus Tick(T context) => _func(context);
    }
}
