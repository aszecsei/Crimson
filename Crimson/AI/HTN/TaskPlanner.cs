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
                    CostSoFar = CostSoFar,
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
            _tasks[rootTask.Name] = rootTask;
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

                Task<T> taskToProcess = current.TasksToProcess[0];
                switch (taskToProcess)
                {
                    case PrimitiveTask<T> t:
                    {
                        if (t.IsSatisfied(current.WorkingWorldState))
                        {
                            var newState = (PlannerState)current.Clone();
                            newState.TasksToProcess.Remove(t);
                            t.Execute(newState.WorkingWorldState);
                            newState.PartialPlan.Add(t);
                            newState.CostSoFar += t.GetCost(current.WorkingWorldState);
                            
                            fringe.Enqueue(newState, newState.CostSoFar + GetHeuristic(newState.TasksToProcess, context));
                        }

                        break;
                    }
                    case CompoundTask<T> ct:
                    {
                        var applicableMethods = ct.FindSatisfiedMethods(current.WorkingWorldState);
                        for (var j = 0; j < applicableMethods.Count; ++j)
                        {
                            List<Task<T>> methodTasks = new List<Task<T>>(applicableMethods[j].Count());
                            foreach (var taskName in applicableMethods[j])
                                methodTasks.Add(_tasks[taskName]);
                            var newState = (PlannerState)current.Clone();
                            newState.TasksToProcess.RemoveAt(0);
                            newState.TasksToProcess.InsertRange(0, methodTasks);
                            int estimatedCost = newState.CostSoFar + GetHeuristic(newState.TasksToProcess, context);
                            fringe.Enqueue(newState, estimatedCost);
                        }

                        break;
                    }
                }
            }
            
            // We were unable to find a plan Q_Q
            return null;
        }

        // This is complicated and I hate it. It's also the only way to avoid infinite recursion loops. :(
        private int GetHeuristic(IReadOnlyList<Task<T>> tasksToProcess, T context)
        {
            Dictionary<string, int> heuristics = new Dictionary<string, int>();
            HashSet<string> hasChecked = new HashSet<string>();
            Stack<string> toCheck = new Stack<string>();
            
            foreach (var t in tasksToProcess)
                toCheck.Push(t.Name);

            while (toCheck.Count > 0)
            {
                var t = _tasks[toCheck.Peek()];
                if (heuristics.ContainsKey(t.Name))
                {
                    toCheck.Pop();
                    continue;
                }
                
                if (t is PrimitiveTask<T> pt)
                {
                    hasChecked.Add(pt.Name);
                    heuristics[pt.Name] = pt.GetHeuristic(context);
                    toCheck.Pop();
                }
                else if (t is CompoundTask<T> ct)
                {
                    hasChecked.Add(ct.Name);

                    int minMethod = int.MaxValue;
                    
                    bool shouldWait = false;
                    foreach (Method<T> method in ct)
                    {
                        int methodHeuristic = 0;
                        foreach (string taskName in method)
                        {
                            if (hasChecked.Contains(taskName))
                            {
                                if (heuristics.ContainsKey(taskName))
                                {
                                    methodHeuristic += heuristics[taskName];
                                }
                                else
                                {
                                    methodHeuristic = int.MaxValue;
                                    break;
                                }
                            }
                            else
                            {
                                shouldWait = true;
                                toCheck.Push(taskName);
                            }
                        }

                        minMethod = Mathf.Min(minMethod, methodHeuristic);
                    }

                    if (shouldWait)
                        continue;

                    heuristics[ct.Name] = minMethod;
                    toCheck.Pop();
                }
            }

            int result = 0;
            foreach (var t in tasksToProcess)
                result += heuristics[t.Name];

            return result;
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

    public class TaskPlanner : TaskPlanner<Blackboard>
    {
        public TaskPlanner(Task<Blackboard> rootTask) : base(rootTask)
        {
        }
    }
}