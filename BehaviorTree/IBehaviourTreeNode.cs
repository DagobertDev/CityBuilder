namespace CityBuilder.BehaviorTree
{
    /// <summary>
    /// Interface for behaviour tree nodes.
    /// </summary>
    public interface IBehaviourTreeNode<in T>
    {
        /// <summary>
        /// Update the context of the behaviour tree.
        /// </summary>
        BehaviourTreeStatus Tick(T context);
    }
}
