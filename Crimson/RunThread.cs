using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Crimson
{
    public static class RunThread
    {
        private static List<Thread>   _threads     = new List<Thread>();
        private static List<DateTime> _threadTimes = new List<DateTime>();
        private static List<string>   _threadInfos = new List<string>();

        [ThreadStatic] public static WeakReference<Thread> Current;

        public static void Start(Action method, string name, bool highPriority = false)
        {
            Thread thread = new Thread(() => RunThreadWithLogging(method))
            {
                Name         = name,
                IsBackground = true,
                Priority     = highPriority ? ThreadPriority.Highest : ThreadPriority.Normal,
            };
            lock ( _threads )
            {
                _threads.Add(thread);
                _threadTimes.Add(DateTime.UtcNow);
                _threadInfos.Add($"Name: {name}\nAction: {method?.Method?.ToString() ?? method.ToString()}\nStarter:\n{new StackTrace(1)}");
            }
            Current = new WeakReference<Thread>(thread);
            thread.Start();
        }

        private static void RunThreadWithLogging(Action method)
        {
            try
            {
                method();
            }
            catch ( ThreadAbortException e )
            {
                Logger.Log(LogLevel.Warn, "RunThread",
                           $"Background thread {Thread.CurrentThread?.Name ?? "???"} aborted");
                e.LogDetailed();
            }
            catch ( Exception e )
            {
                ErrorLog.Write(e);
                ErrorLog.Open();
                Engine.Instance.Exit();
            }
            finally
            {
                lock ( _threads )
                {
                    int index = _threads.IndexOf(Thread.CurrentThread);
                    if ( index != -1 )
                    {
                        _threads.RemoveAt(index);
                        _threadTimes.RemoveAt(index);
                        _threadInfos.RemoveAt(0);
                    }
                }
            }
        }

        public static void WaitAll()
        {
            while ( _threads.Count > 0 )
            {
                Thread thread;
                DateTime start;
                DateTime? timeout = null;
                lock ( _threads )
                {
                    thread = _threads[0];
                    start = _threadTimes[0];
                }

                while ( thread.IsAlive )
                {
                    if ( (DateTime.UtcNow - start).TotalSeconds >= 90 )
                    {
                        if ( timeout == null )
                        {
                            timeout = DateTime.UtcNow + TimeSpan.FromSeconds(5);
                        }

                        if ( (DateTime.UtcNow - timeout.Value).Ticks >= 0 )
                        {
                            lock ( _threads )
                            {
                                Logger.Log("RunThread.WaitAll", $"Background thread taking too long, discarding it.\n{_threadInfos[0]}");
                                _threads.RemoveAt(0);
                                _threadTimes.RemoveAt(0);
                                _threadInfos.RemoveAt(0);
                                break;
                            }
                        }
                    }

                    try
                    {
                        Engine.Instance.GraphicsDevice.Present();
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}
