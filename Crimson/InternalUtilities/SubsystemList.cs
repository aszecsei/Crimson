using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Crimson
{
    public class SubsystemList : IEnumerable<Subsystem>
    {
        private readonly List<Type> _registered = new List<Type>();
        private readonly List<Subsystem?> _subsystems = new List<Subsystem?>();
        private readonly Dictionary<Type, Subsystem> _subsystemsByType = new Dictionary<Type, Subsystem>();
        private bool _immediateInit;
        private bool _immediateStart;

        /// <summary>
        /// Registers a subsystem
        /// </summary>
        public void Register<T>() where T : Subsystem
        {
            Register(typeof(T));
        }

        /// <summary>
        /// Registers a subsystem
        /// </summary>
        public void Register(Type type)
        {
            if (_immediateInit)
            {
                var subsystem = Instantiate(type);

                if (_immediateStart)
                    StartupSubsystem(subsystem);
            }
            else
            {
                _registered.Add(type);
            }
        }

        /// <summary>
        /// Registers a Subsystem
        /// </summary>
        private Subsystem Instantiate(Type type)
        {
            if (!(Activator.CreateInstance(type) is Subsystem subsystem))
                throw new Exception("Type must inherit from Subsystem");

            // add Subsystem to lookup
            while (type != typeof(Subsystem))
            {
                if (!_subsystemsByType.ContainsKey(type))
                    _subsystemsByType[type] = subsystem;

                if (type.BaseType == null)
                    break;

                type = type.BaseType;
            }

            // insert in order
            var insert = 0;
            while (insert < _subsystems.Count && (_subsystems[insert]?.Priority ?? int.MinValue) <= subsystem.Priority)
                insert++;
            _subsystems.Insert(insert, subsystem);

            // registered
            subsystem.IsRegistered = true;
            subsystem.MainThreadId = Thread.CurrentThread.ManagedThreadId;
            return subsystem;
        }

        /// <summary>
        /// Removes a Subsystem
        /// Note: Removing core subsystems (such as System) will make everything break
        /// </summary>
        public void Remove(Subsystem subsystem)
        {
            if (!subsystem.IsRegistered)
                throw new Exception("Subsystem is not already registered");

            subsystem.Shutdown();
            subsystem.Disposed();

            var index = _subsystems.IndexOf(subsystem);
            _subsystems[index] = null;

            var type = subsystem.GetType();
            while (type != typeof(Subsystem))
            {
                if (_subsystemsByType[type] == subsystem)
                    _subsystemsByType.Remove(type);

                if (type.BaseType == null)
                    break;

                type = type.BaseType;
            }

            subsystem.IsRegistered = false;
        }

        /// <summary>
        /// Tries to get the First Subsystem of the given type
        /// </summary>
        public bool TryGet<T>(out T? subsystem) where T : Subsystem
        {
            if (_subsystemsByType.TryGetValue(typeof(T), out var m))
            {
                subsystem = (T)m;
                return true;
            }

            subsystem = null;
            return false;
        }

        /// <summary>
        /// Tries to get the First Subsystem of the given type
        /// </summary>
        public bool TryGet(Type type, out Subsystem? subsystem)
        {
            if (_subsystemsByType.TryGetValue(type, out var m))
            {
                subsystem = m;
                return true;
            }

            subsystem = null;
            return false;
        }

        /// <summary>
        /// Gets the First Subsystem of the given type, if it exists, or throws an Exception
        /// </summary>
        public T Get<T>() where T : Subsystem
        {
            if (!_subsystemsByType.TryGetValue(typeof(T), out var subsystem))
                throw new Exception($"App is does not have a {typeof(T).Name} Subsystem registered");

            return (T)subsystem;
        }

        /// <summary>
        /// Gets the First Subsystem of the given type, if it exists, or throws an Exception
        /// </summary>
        public Subsystem Get(Type type)
        {
            if (!_subsystemsByType.TryGetValue(type, out var subsystem))
                throw new Exception($"App is does not have a {type.Name} Subsystem registered");

            return subsystem;
        }

        /// <summary>
        /// Checks if a Subsystem of the given type exists
        /// </summary>
        public bool Has<T>() where T : Subsystem
        {
            return _subsystemsByType.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Checks if a Subsystem of the given type exists
        /// </summary>
        public bool Has(Type type)
        {
            return _subsystemsByType.ContainsKey(type);
        }

        internal void Startup()
        {
            // instantiate subsystems that are registered
            for (var i = 0; i < _registered.Count; i++)
                Instantiate(_registered[i]);

            // further subsystems will be instantiated immediately
            _immediateInit = true;

            // call started on all subsystems
            for (var i = 0; i < _subsystems.Count; i++)
                StartupSubsystem(_subsystems[i]);

            // further subsystems will have Startup called immediately
            _immediateStart = true;
        }

        private static void StartupSubsystem(Subsystem? subsystem)
        {
            if (subsystem != null && !subsystem.IsStarted)
            {
                subsystem.IsStarted = true;
                subsystem.Startup();
            }
        }

        internal void Shutdown()
        {
            for (var i = _subsystems.Count - 1; i >= 0; i--)
                _subsystems[i]?.Shutdown();

            for (var i = _subsystems.Count - 1; i >= 0; i--)
                _subsystems[i]?.Disposed();

            _registered.Clear();
            _subsystems.Clear();
            _subsystemsByType.Clear();
        }

        internal void BeforeUpdate()
        {
            for (var i = 0; i < _subsystems.Count; i++)
                _subsystems[i]?.BeforeUpdate();
        }

        internal void Update()
        {
            for (var i = 0; i < _subsystems.Count; i++)
                _subsystems[i]?.Update();
        }

        internal void AfterUpdate()
        {
            for (var i = 0; i < _subsystems.Count; i++)
                _subsystems[i]?.AfterUpdate();
        }

        internal void BeforeRender()
        {
            for (var i = 0; i < _subsystems.Count; i++)
                _subsystems[i]?.BeforeRender();
        }

        internal void AfterRender()
        {
            for (var i = 0; i < _subsystems.Count; i++)
                _subsystems[i]?.AfterRender();
        }

        internal void Tick()
        {
            for (var i = 0; i < _subsystems.Count; i++)
                _subsystems[i]?.Tick();

            // remove null subsystem entries
            int toRemove;
            while ((toRemove = _subsystems.IndexOf(null)) >= 0)
                _subsystems.RemoveAt(toRemove);
        }
        
        internal void ViewUpdated()
        {
            for (var i = 0; i < _subsystems.Count; i++)
                _subsystems[i]?.ViewUpdated();
        }

        public IEnumerator<Subsystem> GetEnumerator()
        {
            for (var i = 0; i < _subsystems.Count; i++)
            {
                var subsystem = _subsystems[i];
                if (subsystem != null)
                    yield return subsystem;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (var i = 0; i < _subsystems.Count; i++)
            {
                var subsystem = _subsystems[i];
                if (subsystem != null)
                    yield return subsystem;
            }
        }
    }
}