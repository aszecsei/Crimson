using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public abstract class Scene : IEnumerable<Entity>, IEnumerable
    {
        private readonly Dictionary<int, double> _actualDepthLookup;

        public bool HasFocus = true;
        public bool Paused;
        public float RawTimeActive;
        public float TimeActive;

        public Scene()
        {
            Tracker = new Tracker();
            Entities = new EntityList(this);
            TagLists = new TagLists();
            RendererList = new RendererList(this);

            _actualDepthLookup = new Dictionary<int, double>();
        }

        public bool Focused { get; private set; }

        public EntityList Entities { get; }
        public TagLists TagLists { get; }
        public RendererList RendererList { get; }
        public Tracker Tracker { get; }

        public event Action OnEndOfFrame;

        public virtual void Begin()
        {
            Focused = true;
            foreach (Entity entity in Entities) entity.SceneBegin(this);
        }

        public virtual void End()
        {
            Focused = false;
            foreach (Entity entity in Entities) entity.SceneEnd(this);
        }

        public virtual void BeforeUpdate()
        {
            if (!Paused) TimeActive += Time.DeltaTime;

            RawTimeActive += Time.RawDeltaTime;

            Entities.UpdateLists();
            TagLists.UpdateLists();
            RendererList.UpdateLists();
        }

        public virtual void Update()
        {
            if (!Paused)
            {
                Entities.Update();
                RendererList.Update();
            }
        }

        public virtual void AfterUpdate()
        {
            if (!Paused) Entities.LateUpdate();

            if (OnEndOfFrame != null)
            {
                OnEndOfFrame();
                OnEndOfFrame = null;
            }
        }

        public virtual void BeforeRender()
        {
            RendererList.BeforeRender();
        }

        public virtual void Render()
        {
            RendererList.Render();
        }

        public virtual void AfterRender()
        {
            RendererList.AfterRender();
        }

        public virtual void HandleGraphicsReset()
        {
            Entities.HandleGraphicsReset();
        }

        public virtual void HandleGraphicsCreate()
        {
            Entities.HandleGraphicsCreate();
        }

        public virtual void GainFocus()
        {
            HasFocus = true;
        }

        public virtual void LoseFocus()
        {
            HasFocus = false;
        }

        #region Utils

        internal void SetActualDepth(Entity entity)
        {
            const double theta = 0.000001f;

            if (_actualDepthLookup.TryGetValue(entity.Depth, out var add))
                _actualDepthLookup[entity.Depth] += theta;
            else
                _actualDepthLookup.Add(entity.Depth, theta);

            entity.ActualDepth = entity.Depth - add;

            // Mark lists unsorted
            Entities.MarkUnsorted();
            for (var i = 0; i < BitTag.TotalTags; i++)
                if (entity.TagCheck(1 << i))
                    TagLists.MarkUnsorted(i);
        }

        #endregion

        #region Interval

        /// <summary>
        ///     Returns whether the Scene timer has passed the given time interval since the last frame. Ex: given 2.0f, this will
        ///     return true once every 2 seconds.
        /// </summary>
        /// <param name="interval">The time interval to check for</param>
        /// <returns></returns>
        public bool OnInterval(float interval)
        {
            return (int) ((TimeActive - Time.DeltaTime) / interval) < (int) (TimeActive / interval);
        }

        /// <summary>
        ///     Returns whether the Scene timer has passed the given time interval since the last frame. Ex: given 2.0f, this will
        ///     return true once every 2 seconds.
        /// </summary>
        /// <param name="interval">The time interval to check for</param>
        /// <param name="offset">An offset time interval</param>
        /// <returns></returns>
        public bool OnInterval(float interval, float offset)
        {
            return Mathf.Floor((TimeActive - offset - Time.DeltaTime) / interval) <
                   Mathf.Floor((TimeActive - offset) / interval);
        }

        public bool BetweenInterval(float interval)
        {
            return Mathf.BetweenInterval(TimeActive, interval);
        }

        public bool OnRawInterval(float interval)
        {
            return (int) ((RawTimeActive - Time.RawDeltaTime) / interval) < (int) (RawTimeActive / interval);
        }

        public bool OnRawInterval(float interval, float offset)
        {
            return Mathf.Floor((RawTimeActive - offset - Time.RawDeltaTime) / interval) <
                   Mathf.Floor((RawTimeActive - offset) / interval);
        }

        public bool BetweenRawInterval(float interval)
        {
            return Mathf.BetweenInterval(RawTimeActive, interval);
        }

        #endregion

        #region Entity Shortcuts

        public List<Entity> this[BitTag tag] => TagLists[tag.ID];

        public void Add(Entity entity)
        {
            Entities.Add(entity);
        }

        public void Remove(Entity entity)
        {
            Entities.Remove(entity);
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return Entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<Entity> GetEntitiesByTagMask(int mask)
        {
            var list = new List<Entity>();
            foreach (Entity entity in Entities)
                if ((entity.Tag & mask) != 0)
                    list.Add(entity);

            return list;
        }

        public List<Entity> GetEntitiesExcludingTagMask(int mask)
        {
            var list = new List<Entity>();
            foreach (Entity entity in Entities)
                if ((entity.Tag & mask) == 0)
                    list.Add(entity);

            return list;
        }

        #endregion
    }
}