using System.Collections.Generic;

namespace Crimson
{
    public class RendererList
    {
        private readonly List<Renderer> adding;
        private readonly List<Renderer> removing;
        private readonly Scene scene;
        public List<Renderer> Renderers;

        internal RendererList(Scene scene)
        {
            this.scene = scene;

            Renderers = new List<Renderer>();
            adding = new List<Renderer>();
            removing = new List<Renderer>();
        }

        internal void UpdateLists()
        {
            if (adding.Count > 0)
                foreach (Renderer renderer in adding)
                    Renderers.Add(renderer);

            adding.Clear();
            if (removing.Count > 0)
                foreach (Renderer renderer in removing)
                    Renderers.Remove(renderer);

            removing.Clear();
        }

        public void Update()
        {
            foreach (Renderer renderer in Renderers) renderer.Update(scene);
        }

        internal void BeforeRender()
        {
            for (var i = 0; i < Renderers.Count; i++)
            {
                if (!Renderers[i].Visible) continue;

                Draw.Renderer = Renderers[i];
                Renderers[i].BeforeRender(scene);
            }
        }

        internal void Render()
        {
            for (var i = 0; i < Renderers.Count; i++)
            {
                if (!Renderers[i].Visible) continue;

                Draw.Renderer = Renderers[i];
                Renderers[i].Render(scene);
            }
        }

        internal void AfterRender()
        {
            for (var i = 0; i < Renderers.Count; i++)
            {
                if (!Renderers[i].Visible) continue;

                Draw.Renderer = Renderers[i];
                Renderers[i].AfterRender(scene);
            }
        }

        public void MoveToFront(Renderer renderer)
        {
            Renderers.Remove(renderer);
            Renderers.Add(renderer);
        }

        public void Add(Renderer renderer)
        {
            adding.Add(renderer);
        }

        public void Remove(Renderer renderer)
        {
            removing.Add(renderer);
        }
    }
}
