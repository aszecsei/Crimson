using System.Collections;
using System.Collections.Generic;

namespace Crimson.AI.BehaviorTree
{
    public abstract class Node : Operator, IEnumerable<Decorator>
    {
        public readonly List<Decorator> Decorators = new List<Decorator>();
        private readonly List<Service> _services = new List<Service>();

        private TaskInstance? _decoratorInstance;
        private TaskInstance[] _serviceInstances = new TaskInstance[0];
        private bool _needsRunDecorators = true;

        public void Add(Decorator decorator)
        {
            Decorators.Add(decorator);
        }

        public void Add(Service service)
        {
            _services.Add(service);
        }
        
        public IEnumerator<Decorator> GetEnumerator()
        {
            return Decorators.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override void OnStart()
        {
            base.OnStart();

            if (Decorators.Count > 0)
            {
                for (var i = 0; i < Decorators.Count - 1; ++i)
                {
                    Decorators[i].Child = Decorators[i + 1];
                }

                Decorators.LastItem().Child = this;

                _decoratorInstance = Decorators[0].Instance();
            }

            _serviceInstances = new TaskInstance[_services.Count];
            for (var i = 0; i < _services.Count; ++i)
            {
                _serviceInstances[i] = _services[i].Instance();
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            
            _decoratorInstance?.Invalidate();
            _decoratorInstance = null;

            for (var i = 0; i < _serviceInstances.Length; ++i)
            {
                _serviceInstances[i].Invalidate();
            }
            _serviceInstances = new TaskInstance[0];
        }

        protected abstract TaskStatus Tick(Blackboard context);

        public override TaskStatus Update(Blackboard context)
        {
            if (_decoratorInstance != null && _needsRunDecorators)
            {
                _needsRunDecorators = false;
                var status = _decoratorInstance.Tick(context);
                _needsRunDecorators = true;
                return status;
            }

            for (var i = 0; i < _serviceInstances.Length; ++i)
            {
                _serviceInstances[i].Tick(context);
            }
                
            return Tick(context);
        }
    }
}