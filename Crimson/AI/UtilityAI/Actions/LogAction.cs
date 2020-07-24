namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// An <see cref="IAction{T}"/> that logs text
    /// </summary>
    public class LogAction : Action
    {
        private readonly string _text;

        public LogAction(string text)
        {
            _text = text;
        }

        public override TaskStatus Update(Blackboard context)
        {
            Utils.Log(_text);
            return TaskStatus.Success;
        }
    }
}