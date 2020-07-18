namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// An <see cref="IAction{T}"/> that logs text
    /// </summary>
    public class LogAction<T> : IAction<T>
    {
        private readonly string _text;

        public LogAction(string text)
        {
            _text = text;
        }

        public void Execute(T context)
        {
            Utils.Log(_text);
        }
    }
}