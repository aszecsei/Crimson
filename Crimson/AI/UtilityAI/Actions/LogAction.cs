namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// An <see cref="IAction{T}"/> that logs text
    /// </summary>
    public class LogAction : IAction
    {
        private readonly string _text;

        public LogAction(string text)
        {
            _text = text;
        }

        public void Execute(Blackboard context)
        {
            Utils.Log(_text);
        }
    }
}