namespace Crimson.AI
{
    [AITag("Log")]
    public class LogOperator : Operator
    {
        public string Text;
        public bool IsError;
        
        public LogOperator(string text, bool isError = false)
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