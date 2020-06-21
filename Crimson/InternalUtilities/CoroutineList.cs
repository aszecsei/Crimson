using System.Collections;
using System.Collections.Generic;

namespace Crimson
{
    public class CoroutineList
    {
        private readonly List<Coroutine> _coroutineList;
        private readonly HashSet<Coroutine> _toRemove;

        public CoroutineList()
        {
            _coroutineList = new List<Coroutine>();
            _toRemove = new HashSet<Coroutine>();
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            Coroutine result = new Coroutine(this, routine);
            _coroutineList.Add(result);
            return result;
        }

        public void StopAllCoroutines()
        {
            for (int i = 0; i < _coroutineList.Count; i++)
            {
                _toRemove.Add(_coroutineList[i]);
            }
        }

        public void StopCoroutine(IEnumerator routine)
        {
            foreach (Coroutine c in _coroutineList)
            {
                if (c.FunctionCall == routine)
                {
                    _toRemove.Add(c);
                }
            }
        }

        public void StopCoroutine(Coroutine routine)
        {
            _toRemove.Add(routine);
        }

        private void Clean()
        {
            // Remove all dead coroutines
            for (int i = 0; i < _coroutineList.Count; i++)
            {
                Coroutine data = _coroutineList[i];
                if (!data.Active)
                {
                    _toRemove.Add(data);
                }
            }
            
            // Remove all to-be-removed coroutines
            foreach (Coroutine data in _toRemove)
            {
                _coroutineList.Remove(data);
            }
        }

        public void HandleUpdate()
        {
            Clean();
            
            for (int i = 0; i < _coroutineList.Count; i++)
                _coroutineList[i].HandleUpdate();
        }

        public void HandleEndOfFrame()
        {
            for (int i = 0; i < _coroutineList.Count; i++)
                _coroutineList[i].HandleEndOfFrame();
        }
    }
}