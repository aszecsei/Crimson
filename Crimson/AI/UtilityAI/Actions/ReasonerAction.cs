namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// An <see cref="Action"/> that calls through to another <see cref="Reasoner"/>
    /// </summary>
    public class ReasonerAction : Action
    {
        private Reasoner _reasoner;

        public ReasonerAction(Reasoner reasoner)
        {
            _reasoner = reasoner;
        }

        public override TaskStatus Update(Blackboard context)
        {
            return _reasoner.Select(context)?.Update(context) ?? TaskStatus.Failure;
        }
    }
}