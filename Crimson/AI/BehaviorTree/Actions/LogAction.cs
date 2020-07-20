namespace Crimson.AI.BehaviorTree
{
    [AITag("Log")]
    public class LogAction : Behavior
    {
        public string Text;
        public bool IsError;

        public LogAction(string text, bool isError = false)
        {
            Text = text;
            IsError = isError;
        }

        public override TaskStatus Update(Blackboard context)
        {
            if (IsError)
                Utils.LogError(Text);
            else
                Utils.Log(Text);

            return TaskStatus.Success;
        }
    }
}