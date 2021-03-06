﻿using System;
using System.Collections;
using System.Collections.Generic;
using Crimson.Collections;
using Priority_Queue;

namespace Crimson.AI.HTN
{
    public class TaskPlanner : IEnumerable<ITask>
    {
        private const int MAX_NODES = 1000;
        private class PlannerState : FastPriorityQueueNode, ICloneable
        {
            public List<ITask> TasksToProcess;
            public Blackboard WorkingWorldState;
            public List<PrimitiveTask> PartialPlan;
            public int CostSoFar;

            public object Clone()
            {
                return new PlannerState
                {
                    TasksToProcess = new List<ITask>(TasksToProcess),
                    WorkingWorldState = (Blackboard)WorkingWorldState.Clone(),
                    PartialPlan = new List<PrimitiveTask>(PartialPlan),
                    CostSoFar = CostSoFar,
                };
            }
        }

        private readonly Dictionary<string, ITask> _tasks = new Dictionary<string, ITask>();
        private readonly string _rootTask;

        public ITask this[string name]
        {
            get => _tasks[name];
            set => _tasks[name] = value;
        }

        public TaskPlanner(ITask rootTask)
        {
            _rootTask = rootTask.Name;
            _tasks[_rootTask] = rootTask;
        }

        public Plan? Plan(Blackboard context)
        {
            FastPriorityQueue<PlannerState> fringe = new FastPriorityQueue<PlannerState>(MAX_NODES);
            fringe.Enqueue(new PlannerState
            {
                TasksToProcess = new List<ITask> { _tasks[_rootTask] },
                WorkingWorldState = (Blackboard)context.Clone(),
                PartialPlan = new List<PrimitiveTask>(),
                CostSoFar = 0,
            }, 0);

            // TODO: Timeout?
            while (fringe.IsNotEmpty())
            {
                var current = fringe.Dequeue();

                if (current.TasksToProcess.IsEmpty())
                    return new Plan(current.PartialPlan.ToArray());

                ITask taskToProcess = current.TasksToProcess[0];
                switch (taskToProcess)
                {
                    case PrimitiveTask t:
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
                    case CompoundTask ct:
                    {
                        var applicableMethods = ct.FindSatisfiedMethods(current.WorkingWorldState);
                        for (var j = 0; j < applicableMethods.Count; ++j)
                        {
                            List<ITask> methodTasks = ListPool<ITask>.Obtain();
                            foreach (var taskName in applicableMethods[j])
                                methodTasks.Add(_tasks[taskName]);
                            var newState = (PlannerState)current.Clone();
                            newState.TasksToProcess.RemoveAt(0);
                            newState.TasksToProcess.InsertRange(0, methodTasks);
                            ListPool<ITask>.Free(methodTasks);
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
        private int GetHeuristic(IReadOnlyList<ITask> tasksToProcess, Blackboard context)
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
                
                if (t is PrimitiveTask pt)
                {
                    hasChecked.Add(pt.Name);
                    heuristics[pt.Name] = pt.GetHeuristic(context);
                    toCheck.Pop();
                }
                else if (t is CompoundTask ct)
                {
                    hasChecked.Add(ct.Name);

                    int minMethod = int.MaxValue;
                    
                    bool shouldWait = false;
                    foreach (Method method in ct)
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

        public IEnumerator<ITask> GetEnumerator()
        {
            return _tasks.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ITask task)
        {
            _tasks[task.Name] = task;
        }
    }
}