using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Crimson.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Crimson
{
    public class Commands
    {
        private const float UNDERSCORE_TIME = .5f;
        private const float REPEAT_DELAY = .5f;
        private const float REPEAT_EVERY = 1 / 30f;
        private const float OPACITY = .8f;
        private readonly List<string> _commandHistory;

        private readonly Dictionary<string, CommandInfo> _commands;
        private readonly List<Line> _drawCommands;
        private readonly List<string> _sorted;
        private bool _canOpen;
        private KeyboardState _currentState;
        private string _currentText = "";

        public bool Enabled = true;

        private KeyboardState _oldState;
        public bool Open;
        private float _repeatCounter;
        private Keys? _repeatKey;
        private int _seekIndex = -1;
        private int _tabIndex = -1;
        private string _tabSearch = "";
        private bool _underscore;
        private float _underscoreCounter;

        public Commands()
        {
            _commandHistory = new List<string>();
            _drawCommands = new List<Line>();
            _commands = new Dictionary<string, CommandInfo>();
            _sorted = new List<string>();
            FunctionKeyActions = new Action[12];

            BuildCommandsList();
        }

        public Action[] FunctionKeyActions { get; }

        public void Log(object obj, Color color)
        {
            var str = obj.ToString();

            //Newline splits
            if (str.Contains("\n"))
            {
                var all = str.Split('\n');
                foreach (var line in all) Log(line, color);

                return;
            }

            //Split the string if you overlow horizontally
            var maxWidth = Engine.Instance.Window.ClientBounds.Width - 40;
            while (Draw.DefaultFont.MeasureString(str).X > maxWidth)
            {
                var split = -1;
                for (var i = 0; i < str.Length; i++)
                    if (str[i] == ' ')
                    {
                        if (Draw.DefaultFont.MeasureString(str.Substring(0, i)).X <= maxWidth)
                            split = i;
                        else
                            break;
                    }

                if (split == -1) break;

                _drawCommands.Insert(0, new Line(str.Substring(0, split), color));
                str = str.Substring(split + 1);
            }

            _drawCommands.Insert(0, new Line(str, color));

            //Don't overflow top of window
            var maxCommands = (Engine.Instance.Window.ClientBounds.Height - 100) / 30;
            while (_drawCommands.Count > maxCommands) _drawCommands.RemoveAt(_drawCommands.Count - 1);
        }

        public void Log(object obj)
        {
            Log(obj, Color.White);
        }

        private struct Line
        {
            public readonly string Text;
            public readonly Color Color;

            public Line(string text)
            {
                Text = text;
                Color = Color.White;
            }

            public Line(string text, Color color)
            {
                Text = text;
                Color = color;
            }
        }

        #region Updating and Rendering

        internal void UpdateClosed()
        {
            if (!_canOpen)
            {
                _canOpen = true;
            }
            else if (CInput.Keyboard.Pressed(Keys.OemTilde, Keys.Oem8))
            {
                Open = true;
                if (Engine.Scene != null) Engine.Scene.Paused = true;
                _currentState = Keyboard.GetState();
            }

            for (var i = 0; i < FunctionKeyActions.Length; i++)
                if (CInput.Keyboard.Pressed(Keys.F1 + i))
                    ExecuteFunctionKeyAction(i);
        }

        internal void UpdateOpen()
        {
            _oldState = _currentState;
            _currentState = Keyboard.GetState();

            _underscoreCounter += Time.DeltaTime;
            while (_underscoreCounter >= UNDERSCORE_TIME)
            {
                _underscoreCounter -= UNDERSCORE_TIME;
                _underscore = !_underscore;
            }

            if (_repeatKey.HasValue)
            {
                if (_currentState[_repeatKey.Value] == KeyState.Down)
                {
                    _repeatCounter += Time.DeltaTime;

                    while (_repeatCounter >= REPEAT_DELAY)
                    {
                        HandleKey(_repeatKey.Value);
                        _repeatCounter -= REPEAT_EVERY;
                    }
                }
                else
                {
                    _repeatKey = null;
                }
            }

            foreach (var key in _currentState.GetPressedKeys())
                if (_oldState[key] == KeyState.Up)
                {
                    HandleKey(key);
                    break;
                }
        }

        private void HandleKey(Keys key)
        {
            if (key != Keys.Tab && key != Keys.LeftShift && key != Keys.RightShift && key != Keys.RightAlt &&
                key != Keys.LeftAlt && key != Keys.RightControl && key != Keys.LeftControl)
                _tabIndex = -1;

            if (key != Keys.OemTilde && key != Keys.Oem8 && key != Keys.Enter && _repeatKey != key)
            {
                _repeatKey = key;
                _repeatCounter = 0;
            }

            switch (key)
            {
                default:
                    if (key.ToString().Length == 1)
                    {
                        if (_currentState[Keys.LeftShift] == KeyState.Down ||
                            _currentState[Keys.RightShift] == KeyState.Down)
                            _currentText += key.ToString();
                        else
                            _currentText += key.ToString().ToLower();
                    }

                    break;

                case Keys.D1:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '!';
                    else
                        _currentText += '1';

                    break;
                case Keys.D2:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '@';
                    else
                        _currentText += '2';

                    break;
                case Keys.D3:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '#';
                    else
                        _currentText += '3';

                    break;
                case Keys.D4:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '$';
                    else
                        _currentText += '4';

                    break;
                case Keys.D5:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '%';
                    else
                        _currentText += '5';

                    break;
                case Keys.D6:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '^';
                    else
                        _currentText += '6';

                    break;
                case Keys.D7:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '&';
                    else
                        _currentText += '7';

                    break;
                case Keys.D8:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '*';
                    else
                        _currentText += '8';

                    break;
                case Keys.D9:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '(';
                    else
                        _currentText += '9';

                    break;
                case Keys.D0:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += ')';
                    else
                        _currentText += '0';

                    break;
                case Keys.OemComma:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '<';
                    else
                        _currentText += ',';

                    break;
                case Keys.OemPeriod:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '>';
                    else
                        _currentText += '.';

                    break;
                case Keys.OemQuestion:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '?';
                    else
                        _currentText += '/';

                    break;
                case Keys.OemSemicolon:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += ':';
                    else
                        _currentText += ';';

                    break;
                case Keys.OemQuotes:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '"';
                    else
                        _currentText += '\'';

                    break;
                case Keys.OemBackslash:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '|';
                    else
                        _currentText += '\\';

                    break;
                case Keys.OemOpenBrackets:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '{';
                    else
                        _currentText += '[';

                    break;
                case Keys.OemCloseBrackets:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '}';
                    else
                        _currentText += ']';

                    break;
                case Keys.OemMinus:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '_';
                    else
                        _currentText += '-';

                    break;
                case Keys.OemPlus:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                        _currentText += '+';
                    else
                        _currentText += '=';

                    break;

                case Keys.Space:
                    _currentText += " ";
                    break;
                case Keys.Back:
                    if (_currentText.Length > 0) _currentText = _currentText.Substring(0, _currentText.Length - 1);

                    break;
                case Keys.Delete:
                    _currentText = "";
                    break;

                case Keys.Up:
                    if (_seekIndex < _commandHistory.Count - 1)
                    {
                        _seekIndex++;
                        _currentText = string.Join(" ", _commandHistory[_seekIndex]);
                    }

                    break;
                case Keys.Down:
                    if (_seekIndex > -1)
                    {
                        _seekIndex--;
                        if (_seekIndex == -1)
                            _currentText = "";
                        else
                            _currentText = string.Join(" ", _commandHistory[_seekIndex]);
                    }

                    break;

                case Keys.Tab:
                    if (_currentState[Keys.LeftShift] == KeyState.Down || _currentState[Keys.RightShift] == KeyState.Down)
                    {
                        if (_tabIndex == -1)
                        {
                            _tabSearch = _currentText;
                            FindLastTab();
                        }
                        else
                        {
                            _tabIndex--;
                            if (_tabIndex < 0 || _tabSearch != "" && _sorted[_tabIndex].IndexOf(_tabSearch, StringComparison.Ordinal) != 0)
                                FindLastTab();
                        }
                    }
                    else
                    {
                        if (_tabIndex == -1)
                        {
                            _tabSearch = _currentText;
                            FindFirstTab();
                        }
                        else
                        {
                            _tabIndex++;
                            if (_tabIndex >= _sorted.Count || _tabSearch != "" && _sorted[_tabIndex].IndexOf(_tabSearch, StringComparison.Ordinal) != 0)
                                FindFirstTab();
                        }
                    }

                    if (_tabIndex != -1) _currentText = _sorted[_tabIndex];

                    break;

                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                    ExecuteFunctionKeyAction(key - Keys.F1);
                    break;

                case Keys.Enter:
                    if (_currentText.Length > 0) EnterCommand();

                    break;

                case Keys.Oem8:
                case Keys.OemTilde:
                    Open = _canOpen = false;
                    if (Engine.Scene != null) Engine.Scene.Paused = false;
                    break;
            }
        }

        private void EnterCommand()
        {
            var data = _currentText.Split(new[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries);
            if (_commandHistory.Count == 0 || _commandHistory[0] != _currentText) _commandHistory.Insert(0, _currentText);

            _drawCommands.Insert(0, new Line(_currentText, Color.Aqua));
            _currentText = "";
            _seekIndex = -1;

            var args = new string[data.Length - 1];
            for (var i = 1; i < data.Length; i++) args[i - 1] = data[i];

            ExecuteCommand(data[0].ToLower(), args);
        }

        private void FindFirstTab()
        {
            for (var i = 0; i < _sorted.Count; i++)
                if (_tabSearch == "" || _sorted[i].IndexOf(_tabSearch, StringComparison.Ordinal) == 0)
                {
                    _tabIndex = i;
                    break;
                }
        }

        private void FindLastTab()
        {
            for (var i = 0; i < _sorted.Count; i++)
                if (_tabSearch == "" || _sorted[i].IndexOf(_tabSearch, StringComparison.Ordinal) == 0)
                    _tabIndex = i;
        }

        internal void Render()
        {
            var screenWidth = Engine.ViewWidth;
            var screenHeight = Engine.ViewHeight;

            Draw.SpriteBatch.Begin();

            Draw.Rect(10, screenHeight - 50, screenWidth - 20, 40, Color.Black * OPACITY);
            if (_underscore)
                Draw.SpriteBatch.DrawString(Draw.DefaultFont, ">" + _currentText + "_",
                    new Vector2(20, screenHeight - 42), Color.White);
            else
                Draw.SpriteBatch.DrawString(Draw.DefaultFont, ">" + _currentText, new Vector2(20, screenHeight - 42),
                    Color.White);

            if (_drawCommands.Count > 0)
            {
                var height = 10 + 30 * _drawCommands.Count;
                Draw.Rect(10, screenHeight - height - 60, screenWidth - 20, height, Color.Black * OPACITY);
                for (var i = 0; i < _drawCommands.Count; i++)
                    Draw.SpriteBatch.DrawString(Draw.DefaultFont, _drawCommands[i].Text,
                        new Vector2(20, screenHeight - 92 - 30 * i), _drawCommands[i].Color);
            }

            Draw.SpriteBatch.End();
        }

        #endregion

        #region Execute

        public void ExecuteCommand(string command, string[] args)
        {
            if (_commands.ContainsKey(command))
                _commands[command].Action(args);
            else
                Log("Command '" + command + "' not found! Type 'help' for list of commands", Color.Yellow);
        }

        public void ExecuteFunctionKeyAction(int num)
        {
            if (FunctionKeyActions[num] != null) FunctionKeyActions[num]();
        }

        #endregion

        #region Parse Commands

        private void BuildCommandsList()
        {
#if !CONSOLE
            AppDomain currentDomain = AppDomain.CurrentDomain;

            foreach ( var assembly in currentDomain.GetAssemblies() )
            {
                // Check for Commands
                foreach (var type in assembly.GetTypes())
                    foreach (var method in type.GetMethods(
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                        ProcessMethod(method);
            }

            //Maintain the sorted command list
            foreach (var command in _commands) _sorted.Add(command.Key);

            _sorted.Sort();
#endif
        }

        private void ProcessMethod(MethodInfo method)
        {
            Command? attr = null;
            {
                var attrs = method.GetCustomAttributes(typeof(Command), false);
                if (attrs.Length > 0) attr = attrs[0] as Command;
            }

            if (attr != null)
            {
                if (!method.IsStatic)
                    throw new Exception(method.DeclaringType.Name + "." + method.Name +
                                        " is marked as a command, but is not static");

                var info = new CommandInfo {Help = attr.Help};

                var parameters = method.GetParameters();
                var defaults = new object?[parameters.Length];
                var usage = new string[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                {
                    var p = parameters[i];
                    usage[i] = p.Name + ":";

                    if (p.ParameterType == typeof(string))
                        usage[i] += "string";
                    else if (p.ParameterType == typeof(int))
                        usage[i] += "int";
                    else if (p.ParameterType == typeof(float))
                        usage[i] += "float";
                    else if (p.ParameterType == typeof(bool))
                        usage[i] += "bool";
                    else
                        throw new Exception(method.DeclaringType.Name + "." + method.Name +
                                            " is marked as a command, but has an invalid parameter type. Allowed types are: string, int, float, and bool");

                    if (p.DefaultValue == DBNull.Value)
                    {
                        defaults[i] = null;
                    }
                    else if (p.DefaultValue != null)
                    {
                        defaults[i] = p.DefaultValue;
                        if (p.ParameterType == typeof(string))
                            usage[i] += "=\"" + p.DefaultValue + "\"";
                        else
                            usage[i] += "=" + p.DefaultValue;
                    }
                    else
                    {
                        defaults[i] = null;
                    }
                }

                if (usage.Length == 0)
                    info.Usage = "";
                else
                    info.Usage = "[" + string.Join(" ", usage) + "]";

                info.Action = args =>
                {
                    if (parameters.Length == 0)
                    {
                        InvokeMethod(method);
                    }
                    else
                    {
                        var param = (object[]) defaults.Clone();

                        for (var i = 0; i < param.Length && i < args.Length; i++)
                            if (parameters[i].ParameterType == typeof(string))
                                param[i] = ArgString(args[i]);
                            else if (parameters[i].ParameterType == typeof(int))
                                param[i] = ArgInt(args[i]);
                            else if (parameters[i].ParameterType == typeof(float))
                                param[i] = ArgFloat(args[i]);
                            else if (parameters[i].ParameterType == typeof(bool)) param[i] = ArgBool(args[i]);

                        InvokeMethod(method, param);
                    }
                };

                _commands[attr.Name] = info;
            }
        }

        private void InvokeMethod(MethodInfo method, object[] param = null)
        {
            try
            {
                method.Invoke(null, param);
            }
            catch (Exception e)
            {
                Engine.Commands.Log(e.InnerException.Message, Color.Yellow);
                LogStackTrace(e.InnerException.StackTrace);
            }
        }

        private void LogStackTrace(string stackTrace)
        {
            foreach (var call in stackTrace.Split('\n'))
            {
                var log = call;

                //Remove File Path
                {
                    var from = log.LastIndexOf(" in ", StringComparison.Ordinal) + 4;
                    var to = log.LastIndexOf('\\') + 1;
                    if (from != -1 && to != -1) log = log.Substring(0, from) + log.Substring(to);
                }

                //Remove arguments list
                {
                    var from = log.IndexOf('(') + 1;
                    var to = log.IndexOf(')');
                    if (from != -1 && to != -1) log = log.Substring(0, from) + log.Substring(to);
                }

                //Space out the colon line number
                var colon = log.LastIndexOf(':');
                if (colon != -1) log = log.Insert(colon + 1, " ").Insert(colon, " ");

                log = log.TrimStart();
                log = "-> " + log;

                Engine.Commands.Log(log, Color.White);
            }
        }

        private struct CommandInfo
        {
            public Action<string[]> Action;
            public string Help;
            public string Usage;
        }

        #region Parsing Arguments

        private static string ArgString(string arg)
        {
            if (arg == null) return "";

            return arg;
        }

        private static bool ArgBool(string arg)
        {
            if (arg != null) return !(arg == "0" || arg.ToLower() == "false" || arg.ToLower() == "f");

            return false;
        }

        private static int ArgInt(string arg)
        {
            try
            {
                return Convert.ToInt32(arg);
            }
            catch
            {
                return 0;
            }
        }

        private static float ArgFloat(string arg)
        {
            try
            {
                return Convert.ToSingle(arg);
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        #endregion

        #region Built-In Commands

#if !CONSOLE
        [Command("clear", "Clears the terminal")]
        public static void Clear()
        {
            Engine.Commands._drawCommands.Clear();
        }

        [Command("exit", "Exits the game")]
        private static void Exit()
        {
            Engine.Instance.Exit();
        }

        [Command("vsync", "Enables or disables vertical sync")]
        private static void Vsync(bool enabled = true)
        {
            Engine.Graphics.SynchronizeWithVerticalRetrace = enabled;
            Engine.Graphics.ApplyChanges();
            Engine.Commands.Log("Vertical Sync " + (enabled ? "Enabled" : "Disabled"));
        }

        [Command("fixed", "Enables or disables fixed time step")]
        private static void Fixed(bool enabled = true)
        {
            Engine.Instance.IsFixedTimeStep = enabled;
            Engine.Commands.Log("Fixed Time Step " + (enabled ? "Enabled" : "Disabled"));
        }

        [Command("framerate", "Sets the target framerate")]
        private static void Framerate(float target)
        {
            Engine.Instance.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / target);
        }

        [Command("count", "Logs amount of Entities in the Scene. Pass a tagIndex to count only Entities with that tag")]
        private static void Count(int tagIndex = -1)
        {
            if (Engine.Scene == null)
            {
                Engine.Commands.Log("Current Scene is null!");
                return;
            }

            if (tagIndex < 0)
                Engine.Commands.Log(Engine.Scene.Entities.Count.ToString());
            else
                Engine.Commands.Log(Engine.Scene.TagLists[tagIndex].Count.ToString());
        }

        [Command("tracker",
            "Logs all tracked objects in the scene. Set mode to 'e' for just entities, 'c' for just components, or 'cc' for just collidable components")]
        private static void Tracker(string mode)
        {
            if (Engine.Scene == null)
            {
                Engine.Commands.Log("Current scene is null!");
                return;
            }

            switch (mode)
            {
                default:
                    Engine.Commands.Log("-- Entities --");
                    Engine.Scene.Tracker.LogEntities();
                    Engine.Commands.Log("-- Components --");
                    Engine.Scene.Tracker.LogComponents();
                    Engine.Commands.Log("-- Collidable Components --");
                    Engine.Scene.Tracker.LogCollidableComponents();
                    break;

                case "e":
                    Engine.Scene.Tracker.LogEntities();
                    break;

                case "c":
                    Engine.Scene.Tracker.LogComponents();
                    break;

                case "cc":
                    Engine.Scene.Tracker.LogCollidableComponents();
                    break;
            }
        }

        [Command("fullscreen", "Switches to fullscreen mode")]
        private static void Fullscreen()
        {
            Engine.SetFullscreen();
        }

        [Command("pooler", "Logs the pooled Entity counts")]
        private static void Pooler()
        {
            Engine.Pooler.Log();
        }

        [Command("window", "Switches to window mode")]
        private static void Window(int scale = 1)
        {
            Engine.SetWindowed(Engine.Width * scale, Engine.Height * scale);
        }

        [Command("help", "Shows usage help for a given command")]
        private static void Help(string command)
        {
            if (Engine.Commands._sorted.Contains(command))
            {
                var c = Engine.Commands._commands[command];
                var str = new StringBuilder();

                //Title
                str.Append(":: ");
                str.Append(command);

                //Usage
                if (!string.IsNullOrEmpty(c.Usage))
                {
                    str.Append(" ");
                    str.Append(c.Usage);
                }

                Engine.Commands.Log(str.ToString());

                //Help
                if (string.IsNullOrEmpty(c.Help))
                    Engine.Commands.Log("No help info set");
                else
                    Engine.Commands.Log(c.Help);
            }
            else
            {
                var str = new StringBuilder();
                str.Append("Commands list: ");
                str.Append(string.Join(", ", Engine.Commands._sorted));
                Engine.Commands.Log(str.ToString());
                Engine.Commands.Log("Type 'help command' for more info on that command!");
            }
        }
#endif

        #endregion
    }

    public class Command : Attribute
    {
        public string Help;
        public string Name;

        public Command(string name, string help)
        {
            Name = name;
            Help = help;
        }
    }
}
