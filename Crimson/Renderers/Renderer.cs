using System.Collections.Generic;

namespace Crimson
{
    public abstract class Renderer
    {
        public Stack<PostProcessEffect> PostProcessStack = new Stack<PostProcessEffect>();

        public bool Visible = true;

        public virtual void Update(Scene scene)
        {
        }

        public virtual void BeforeRender(Scene scene)
        {
            var arr = PostProcessStack.ToArray();
            for (var i = 0; i < arr.Length; i++) arr[i].BeforeRender();
        }

        public virtual void Render(Scene scene)
        {
        }

        public virtual void AfterRender(Scene scene)
        {
            var arr = PostProcessStack.ToArray();
            for (var i = 0; i < arr.Length; i++) arr[i].AfterRender();
        }
    }
}