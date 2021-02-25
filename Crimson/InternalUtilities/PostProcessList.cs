using System.Collections.Generic;

namespace Crimson
{
    public class PostProcessList
    {
        private readonly List<PostProcessEffect> _adding;
        private readonly List<PostProcessEffect> _removing;
        public           List<PostProcessEffect> Effects;

        internal PostProcessList()
        {
            Effects   = new List<PostProcessEffect>();
            _adding   = new List<PostProcessEffect>();
            _removing = new List<PostProcessEffect>();
        }

        internal void UpdateLists()
        {
            if (_adding.Count > 0)
                foreach (PostProcessEffect renderer in _adding)
                    Effects.Add(renderer);

            _adding.Clear();
            if (_removing.Count > 0)
                foreach (PostProcessEffect renderer in _removing)
                    Effects.Remove(renderer);

            _removing.Clear();
        }

        public void Update()
        {
            for ( var i = 0; i < Effects.Count; ++i )
            {
                Effects[i].Update();
            }
        }

        internal void BeforeRender()
        {
            for (var i = 0; i < Effects.Count; i++)
            {
                Effects[i].BeforeRender();
            }
        }

        internal void AfterRender()
        {
            for ( var i = Effects.Count - 1; i >= 0; --i )
            {
                Effects[i].AfterRender();
            }
        }

        public void Push(PostProcessEffect effect)
        {
            _adding.Add(effect);
        }

        public void Pop()
        {
            _removing.Add(Effects.LastItem());
        }

        public void Clear()
        {
            _removing.AddRange(Effects);
        }

        public void Remove(PostProcessEffect effect)
        {
            _removing.Add(effect);
        }
    }
}
