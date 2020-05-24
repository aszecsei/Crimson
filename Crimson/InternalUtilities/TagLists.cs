using System.Collections.Generic;

namespace Crimson
{
    public class TagLists
    {
        private readonly List<Entity>[] lists;
        private readonly bool[] unsorted;
        private bool areAnyUnsorted;

        internal TagLists()
        {
            lists = new List<Entity>[BitTag.TotalTags];
            unsorted = new bool[BitTag.TotalTags];
            for (var i = 0; i < lists.Length; i++) lists[i] = new List<Entity>();
        }

        public List<Entity> this[int index] => lists[index];

        internal void MarkUnsorted(int index)
        {
            areAnyUnsorted = true;
            unsorted[index] = true;
        }

        internal void UpdateLists()
        {
            if (areAnyUnsorted)
            {
                for (var i = 0; i < lists.Length; i++)
                    if (unsorted[i])
                    {
                        lists[i].Sort(EntityList.CompareDepth);
                        unsorted[i] = false;
                    }

                areAnyUnsorted = false;
            }
        }

        internal void EntityAdded(Entity entity)
        {
            for (var i = 0; i < BitTag.TotalTags; i++)
                if (entity.TagCheck(1 << i))
                {
                    this[i].Add(entity);
                    areAnyUnsorted = true;
                    unsorted[i] = true;
                }
        }

        internal void EntityRemoved(Entity entity)
        {
            for (var i = 0; i < BitTag.TotalTags; i++)
                if (entity.TagCheck(1 << i))
                    lists[i].Remove(entity);
        }
    }
}