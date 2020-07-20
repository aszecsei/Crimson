namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// An <see cref="IAction{T}"/> that calls through to another <see cref="Reasoner{T}"/>
    /// </summary>
    public class ReasonerAction : IAction
    {
        private Reasoner _reasoner;

        public ReasonerAction(Reasoner reasoner)
        {
            _reasoner = reasoner;
        }

        public void Execute(Blackboard context)
        {
            _reasoner.Select(context)?.Execute(context);
        }
    }
}