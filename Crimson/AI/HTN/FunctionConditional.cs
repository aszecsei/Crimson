using System;

namespace Crimson.AI.HTN
{
    public class FunctionConditional<T> : IConditional<T>
    {
        private readonly Func<T, bool> _func;

        public FunctionConditional(Func<T, bool> func)
        {
            _func = func;
        }
        
        public bool Update(T context)
        {
            return _func(context);
        }
    }
}