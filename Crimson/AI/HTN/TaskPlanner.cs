using System;
using System.Collections;
using System.Collections.Generic;
using Crimson.Collections;

namespace Crimson.AI.HTN
{
    public class TaskPlanner<T> : IEnumerable<Task<T>>
    where T: class, ICloneable
    {
        private struct PlannerState : ICloneable
        {
            public List<Task<T>> TasksToProcess;
            public T WorkingWorldState;
            public List<PrimitiveTask<T>> PartialPlan;
            public int CostSoFar;

            public object Clone()
            {
                return new PlannerState
                {
                    TasksToProcess = new List<Task<T>>(TasksToProcess),
                    WorkingWorldState = (T)WorkingWorldState.Clone(),
                    PartialPlan = new List<PrimitiveTask<T>>(PartialPlan),
                    CostSoFar = 0,
                };
            }
        }

        private readonly Dictionary<string, Task<T>> _tasks = new Dictionary<string, Task<T>>();
        private readonly string _rootTask;

        public Task<T> this[string name]
        {
            get => _tasks[name];
            set => _tasks[name] = value;
        }

        public TaskPlanner(Task<T> rootTask)
        {
            _rootTask = rootTask.Name;
        }

        public List<PrimitiveTask<T>>? Plan(T context)
        {
            SimplePriorityQueue<PlannerState> fringe = new SimplePriorityQueue<PlannerState>();
            fringe.Enqueue(new PlannerState
            {
                TasksToProcess = new List<Task<T>> { _tasks[_rootTask] },
                WorkingWorldState = (T)context.Clone(),
                PartialPlan = new List<PrimitiveTask<T>>(),
                CostSoFar = 0,
            }, 0);

            // TODO: Timeout?
            while (fringe.IsNotEmpty())
            {
                var current = fringe.Dequeue();

                if (current.TasksToProcess.IsEmpty())
                    return current.PartialPlan;

                for (var i = 0; i < current.TasksToProcess.Count; ++i)
                {
                    switch (current.TasksToProcess[i])
                    {
                        case PrimitiveTask<T> t:
                        {
                            if (t.IsSatisfied(context))
                            {
                                var newState = (PlannerState)current.Clone();
                                newState.TasksToProcess.Remove(t);
                                t.Execute(newState.WorkingWorldState);
                                newState.PartialPlan.Add(t);
                                newState.CostSoFar += t.GetCost(context);
                            
                                fringe.Enqueue(newState, newState.CostSoFar + GetCostPlusHeuristic(newState.TasksToProcess, context));
                            }

                            break;
                        }
                        case CompoundTask<T> ct:
                        {
                            var applicableMethods = ct.FindSatisfiedMethods(context);
                            for (var j = 0; j < applicableMethods.Count; ++j)
                            {
                                List<Task<T>> methodTasks = new List<Task<T>>(applicableMethods[j].Count());
                                foreach (var taskName in applicableMethods[j])
                                    methodTasks.Add(_tasks[taskName]);
                                var newState = (PlannerState)current.Clone();
                                newState.TasksToProcess.RemoveAt(i);
                                newState.TasksToProcess.InsertRange(i, methodTasks);
                                fringe.Enqueue(newState, newState.CostSoFar + GetCostPlusHeuristic(newState.TasksToProcess, context));
                            }

                            break;
                        }
                    }
                }
            }
            
            // We were unable to find a plan Q_Q
            return null;
        }

        private int GetCostPlusHeuristic(IReadOnlyList<Task<T>> tasksToProcess, T context)
        {
            var res = 0;
            for (var i = 0; i < tasksToProcess.Count; ++i)
                res += tasksToProcess[i].GetHeuristic(this, context);
            return res;
        }

        public IEnumerator<Task<T>> GetEnumerator()
        {
            return _tasks.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Task<T> task)
        {
            _tasks[task.Name] = task;
        }
    }
}