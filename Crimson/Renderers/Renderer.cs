using System.Collections.Generic;

namespace Crimson
{
    public abstract class Renderer
    {
        public PostProcessList PostProcessStack = new PostProcessList();

        public bool Visible = true;

        public virtual void BeforeUpdate(Scene scene)
        {
            PostProcessStack.UpdateLists();
        }

        public virtual void Update(Scene scene)
        {
            PostProcessStack.Update();
        }

        public virtual void BeforeRender(Scene scene)
        {
        }

        public virtual void Render(Scene scene)
        {
        }

        public virtual void AfterRender(Scene scene)
        {
        }
    }
}
