namespace Crimson.AI.BehaviorTree
{
    [AITag("Log", "text", "isError")]
    public class LogAction<T> : Behavior<T>
    {
        public string Text;
        public bool IsError;

        public LogAction(string text, bool isError = false)
        {
            Text = text;
            IsError = isError;
        }

        public override TaskStatus Update(T context)
        {
            if (IsError)
                Utils.LogError(Text);
            else
                Utils.Log(Text);

            return TaskStatus.Success;
        }
    }
}