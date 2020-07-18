namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// An <see cref="IAction{T}"/> that calls through to another <see cref="Reasoner{T}"/>
    /// </summary>
    public class ReasonerAction<T> : IAction<T>
    {
        private Reasoner<T> _reasoner;

        public ReasonerAction(Reasoner<T> reasoner)
        {
            _reasoner = reasoner;
        }

        public void Execute(T context)
        {
            _reasoner.Select(context)?.Execute(context);
        }
    }
}