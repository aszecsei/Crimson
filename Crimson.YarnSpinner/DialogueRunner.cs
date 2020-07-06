using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using CsvHelper;
using Yarn;
using Crimson.Assertions;
using Color = Microsoft.Xna.Framework.Color;

namespace Crimson.YarnSpinner
{
    public class DialogueRunner : Component, ILineLocalizationProvider
    {
        /// <summary>
        /// The <see cref="YarnProgram"/> assets that should be loaded on
        /// scene start.
        /// </summary>
        public YarnProgram[] YarnScripts;

        /// <summary>
        /// The language code used to select a string table.
        /// </summary>
        /// <remarks>
        /// This must be an IETF BCP-47 language code, like "en" or "de".
        ///
        /// This value is used to select a string table from the <see cref="YarnProgram.Localizations"/>, for each of the
        /// <see cref="YarnProgram"/>s in <see cref="YarnScripts"/>.
        /// </remarks>
        public string TextLanguage;

        /// <summary>
        /// The variable storage object.
        /// </summary>
        public VariableStorageComponent VariableStorage;

        /// <summary>
        /// The object that will handle the actual display and user input.
        /// </summary>
        public DialogueUIComponent DialogueUI;

        /// <summary>The name of the node to start from.</summary>
        /// <remarks>
        /// This value is used to select a node to start from when
        /// <see cref="StartDialogue"/> is called.
        /// </remarks>
        public string StartNode = Yarn.Dialogue.DEFAULT_START;
        
        /// <summary>
        /// Whether the DialogueRunner should automatically start running
        /// dialogue after the scene loads.
        /// </summary>
        /// <remarks>
        /// The node specified by <see cref="StartNode"/> will be used.
        /// </remarks>
        public bool StartAutomatically = true;
        
        /// <summary>
        /// Gets a value that indicates if the dialogue is actively running.
        /// </summary>
        public bool IsDialogueRunning { get; set; }

        /// <summary>
        /// A delegate that is called when a node starts running.
        /// </summary>
        /// <remarks>
        /// This event receives as a parameter the name of the node that is
        /// about to start running.
        /// </remarks>
        /// <seealso cref="Yarn.Dialogue.NodeStartHandler"/>
        public Action<string> OnNodeStart;
        
        /// <summary>
        /// A delegate that is called when a node is complete.
        /// </summary>
        /// <remarks>
        /// This event receives as a parameter the name of the node that is
        /// just finished running.
        /// </remarks>
        /// <seealso cref="Yarn.Dialogue.NodeCompleteHandler"/>
        public Action<string> OnNodeComplete;

        /// <summary>
        /// A delegate that is called once the dialogue has completed.
        /// </summary>
        public Action OnDialogueComplete;

        /// <summary>
        /// Gets the name of the current node that is being run.
        /// </summary>
        /// <seealso cref="Dialogue.currentNode"/>
        public string CurrentNodeName => Dialogue.currentNode;

        /// <summary>
        /// Gets the underlying <see cref="Dialogue"/> object that runs the Yarn code.
        /// </summary>
        public Dialogue Dialogue => _dialogue ?? (_dialogue = CreateDialogueInstance());

        public DialogueRunner() : base(true, false)
        {
        }

        public void Add(YarnProgram scriptToLoad)
        {
            Dialogue.AddProgram(scriptToLoad.GetProgram());
            AddStringTable(scriptToLoad);
        }

        public void AddStringTable(YarnProgram yarnScript)
        {
            string textToLoad = null;

            if (yarnScript.Localizations != null || yarnScript.Localizations.Length > 0)
            {
                textToLoad = Array.Find(yarnScript.Localizations, element => element.LanguageName == TextLanguage)?.Text;
            }

            if (textToLoad == null || string.IsNullOrEmpty(textToLoad))
            {
                textToLoad = yarnScript.BaseLocalisationStringTable;
            }

            var configuration =
                new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);
            
            using (var reader = new System.IO.StringReader(textToLoad))
            using (var csv = new CsvReader(reader, configuration))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    _strings.Add(csv.GetField("id"), csv.GetField("text"));
                }
            }
        }

        public void AddStringTable(IDictionary<string, string> stringTable)
        {
            foreach (var line in stringTable)
            {
                _strings.Add(line.Key, line.Value);
            }
        }

        public void StartDialogue() => StartDialogue(StartNode);

        public void StartDialogue(string startNode)
        {
            DialogueUI.StopAllCoroutines();

            IsDialogueRunning = true;
            DialogueUI.DialogueStarted();
            Dialogue.SetNode(startNode);
            ContinueDialogue();
        }

        public void ResetDialogue()
        {
            VariableStorage.ResetToDefaults();
            StartDialogue();
        }

        public void Clear()
        {
            Assert.IsFalse(IsDialogueRunning, "You cannot clear the dialogue system while a dialogue is running.");
            Dialogue.UnloadAll();
        }

        public void Stop()
        {
            IsDialogueRunning = false;
            Dialogue.Stop();
        }

        public bool NodeExists(string nodeName) => Dialogue.NodeExists(nodeName);

        public IEnumerable<string> GetTagsForNode(string nodeName) => Dialogue.GetTagsForNode(nodeName);

        public void AddCommandHandler(string commandName, CommandHandler handler)
        {
            if (_commandHandlers.ContainsKey(commandName) || _blockingCommandHandlers.ContainsKey(commandName))
            {
                Utils.LogError($"Cannot add a command handler for {commandName}: one already exists");
                return;
            }
            _commandHandlers.Add(commandName, handler);
        }

        public void AddCommandHandler(string commandName, BlockingCommandHandler handler)
        {
            if (_commandHandlers.ContainsKey(commandName) || _blockingCommandHandlers.ContainsKey(commandName))
            {
                Utils.LogError($"Cannot add a command handler for {commandName}: one already exists");
                return;
            }
            _blockingCommandHandlers.Add(commandName, handler);
        }

        public void RemoveCommandHandler(string commandName)
        {
            _commandHandlers.Remove(commandName);
            _blockingCommandHandlers.Remove(commandName);
        }

        public void AddFunction(string name, int parameterCount, ReturningFunction implementation)
        {
            if (Dialogue.library.FunctionExists(name))
            {
                Utils.LogError($"Cannot add function {name}: one already exists");
                return;
            }
            Dialogue.library.RegisterFunction(name, parameterCount, implementation);
        }
        
        public void AddFunction(string name, int parameterCount, Function implementation)
        {
            if (Dialogue.library.FunctionExists(name))
            {
                Utils.LogError($"Cannot add function {name}: one already exists");
                return;
            }
            Dialogue.library.RegisterFunction(name, parameterCount, implementation);
        }

        public void RemoveFunction(string name) => Dialogue.library.DeregisterFunction(name);


        #region Private Properties / Variables / Procedures

        private Action continueAction;

        private Action<int> selectAction;

        public delegate void CommandHandler(string[] parameters);

        public delegate void BlockingCommandHandler(string[] parameters, Action onComplete);

        private Dictionary<string, CommandHandler> _commandHandlers = new Dictionary<string, CommandHandler>();
        private Dictionary<string, BlockingCommandHandler> _blockingCommandHandlers = new Dictionary<string, BlockingCommandHandler>();
        
        private Dictionary<string, string> _strings = new Dictionary<string, string>();

        private bool _wasCompleteCalled = false;

        /// <summary>
        /// Our conversation engine
        /// </summary>
        private Dialogue _dialogue;

        public override void EntityAwake()
        {
            base.EntityAwake();
            Start();
        }

        private void Start()
        {
            Assert.IsNotNull(DialogueUI, "Implementation was not set! Can't run the dialogue!");
            Assert.IsNotNull(VariableStorage, "Variable storage was not set! Can't run the dialogue!");
            
            VariableStorage.ResetToDefaults();

            if (YarnScripts != null && YarnScripts.Length > 0)
            {
                var compiledPrograms = new List<Program>();

                foreach (var program in YarnScripts)
                {
                    compiledPrograms.Add(program.GetProgram());
                }

                var combinedProgram = Program.Combine(compiledPrograms.ToArray());
                
                Dialogue.SetProgram(combinedProgram);
            }

            if (StartAutomatically)
            {
                StartDialogue();
            }
        }

        private Dialogue CreateDialogueInstance()
        {
            var dialogue = new Yarn.Dialogue(VariableStorage)
            {
                LogDebugMessage = delegate(string message)
                {
                    Utils.Log(message);
                },
                LogErrorMessage = delegate(string message)
                {
                    Engine.Commands.Log(message, Color.Red);
                    Utils.LogError(message);
                },
                lineHandler = HandleLine,
                commandHandler = HandleCommand,
                optionsHandler = HandleOptions,
                nodeStartHandler = (node) =>
                {
                    OnNodeStart?.Invoke(node);
                    return Dialogue.HandlerExecutionType.ContinueExecution;
                },
                nodeCompleteHandler = (node) =>
                {
                    OnNodeComplete?.Invoke(node);
                    return Dialogue.HandlerExecutionType.ContinueExecution;
                },
                dialogueCompleteHandler = HandleDialogueComplete
            };
            
            AddCommandHandler("wait", HandleWaitCommand);

            foreach (var yarnScript in YarnScripts)
            {
                AddStringTable(yarnScript);
            }

            continueAction = ContinueDialogue;
            selectAction = SelectedOption;

            return dialogue;

            void HandleWaitCommand(string[] parameters, Action onComplete)
            {
                if (parameters?.Length != 1)
                {
                    Utils.LogError("<<wait>> command expects one parameter.");
                    onComplete();
                    return;
                }

                string durationString = parameters[0];

                if (float.TryParse(durationString,
                    System.Globalization.NumberStyles.AllowDecimalPoint,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var duration) == false)
                {
                    Utils.LogError($"<<wait>> failed to parse duration {durationString}");
                    onComplete();
                }

                StartCoroutine(DoHandleWait());

                IEnumerator DoHandleWait()
                {
                    yield return new WaitForSeconds(duration);
                    onComplete();
                }
            }

            void HandleOptions(OptionSet options) => DialogueUI.RunOptions(options, this, selectAction);

            void HandleDialogueComplete()
            {
                IsDialogueRunning = false;
                DialogueUI.DialogueComplete();
                OnDialogueComplete.Invoke();
            }

            Dialogue.HandlerExecutionType HandleCommand(Yarn.Command command)
            {
                bool wasValidCommand;
                Dialogue.HandlerExecutionType executionType;

                _wasCompleteCalled = false;

                (wasValidCommand, executionType) = DispatchCommandToRegisteredHandlers(command, continueAction);

                if (wasValidCommand)
                {
                    if (_wasCompleteCalled)
                    {
                        return Dialogue.HandlerExecutionType.ContinueExecution;
                    }
                    else
                    {
                        return executionType;
                    }
                }

                (wasValidCommand, executionType) = DispatchCommandToGameObject(command);

                if (wasValidCommand)
                {
                    return executionType;
                }

                return DialogueUI.RunCommand(command, continueAction);
            }

            Dialogue.HandlerExecutionType HandleLine(Line line) => DialogueUI.RunLine(line, this, continueAction);

            void SelectedOption(int obj)
            {
                Dialogue.SetSelectedOption(obj);
                ContinueDialogue();
            }

            (bool commandWasFound, Yarn.Dialogue.HandlerExecutionType executionType)
                DispatchCommandToRegisteredHandlers(Yarn.Command command, Action onComplete)
            {
                var commandTokens = command.Text.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                if (commandTokens.Length == 0)
                {
                    // Nothing to do
                    return (false, Dialogue.HandlerExecutionType.ContinueExecution);
                }

                var firstWord = commandTokens[0];

                if (_commandHandlers.ContainsKey(firstWord) == false &&
                    _blockingCommandHandlers.ContainsKey(firstWord) == false)
                {
                    // We don't have a registered handler for this command, but
                    // some other part of the game might.
                    return (false, Dialogue.HandlerExecutionType.ContinueExecution);
                }
                
                // Single-word command, e.g. <<jump>>
                if (commandTokens.Length == 1)
                {
                    if (_commandHandlers.ContainsKey(firstWord))
                    {
                        _commandHandlers[firstWord](null);
                        return (true, Dialogue.HandlerExecutionType.ContinueExecution);
                    }
                    else
                    {
                        _blockingCommandHandlers[firstWord](new string[] {}, onComplete);
                        return (true, Dialogue.HandlerExecutionType.PauseExecution);
                    }
                }
                
                // Multi-word command, e.g. <<walk Mae left>>
                var remainingWords = new string[commandTokens.Length - 1];
                System.Array.Copy(commandTokens, 1, remainingWords, 0, remainingWords.Length);

                if (_commandHandlers.ContainsKey(firstWord))
                {
                    _commandHandlers[firstWord](remainingWords);
                    return (true, Dialogue.HandlerExecutionType.ContinueExecution);
                }
                else
                {
                    _blockingCommandHandlers[firstWord](remainingWords, onComplete);
                    return (true, Dialogue.HandlerExecutionType.PauseExecution);
                }
            }

            (bool methodFound, Yarn.Dialogue.HandlerExecutionType executionType) DispatchCommandToGameObject(
                Yarn.Command command)
            {
                var words = command.Text.Split(' ');

                if (words.Length < 2)
                {
                    return (false, Dialogue.HandlerExecutionType.ContinueExecution);
                }

                var commandName = words[0];
                var objectName = words[1];
                var sceneObject = Scene.FindEntityNamed(objectName);
                if (sceneObject == null)
                {
                    return (false, Dialogue.HandlerExecutionType.ContinueExecution);
                }

                int numberOfMethodsFound = 0;
                List<string[]> errorValues = new List<string[]>();
                List<string> parameters;

                if (words.Length > 2)
                {
                    parameters = new List<string>(words);
                    parameters.RemoveRange(0, 2);
                }
                else
                {
                    parameters = new List<string>();
                }

                var startedCoroutine = false;

                foreach (var component in sceneObject.Components)
                {
                    var type = component.GetType();
                    foreach (var method in type.GetMethods())
                    {
                        var attributes =
                            (YarnCommandAttribute[]) method.GetCustomAttributes(typeof(YarnCommandAttribute), true);

                        foreach (var attribute in attributes)
                        {
                            if (attribute.CommandString == commandName)
                            {
                                var methodParameters = method.GetParameters();
                                bool paramsMatch = false;
                                if (methodParameters.Length == 1 && methodParameters[0].ParameterType
                                    .IsAssignableFrom(typeof(string[])))
                                {
                                    string[][] paramWrapper = new string[1][];
                                    paramWrapper[0] = parameters.ToArray();
                                    if (method.ReturnType == typeof(IEnumerator))
                                    {
                                        StartCoroutine(DoYarnCommandParams(component, method, paramWrapper));
                                        startedCoroutine = true;
                                    }
                                    else
                                    {
                                        method.Invoke(component, paramWrapper);
                                    }

                                    numberOfMethodsFound++;
                                    paramsMatch = true;
                                }
                                else if (methodParameters.Length == parameters.Count)
                                {
                                    paramsMatch = true;
                                    foreach (var paramInfo in methodParameters)
                                    {
                                        if (!paramInfo.ParameterType.IsAssignableFrom(typeof(string)))
                                        {
                                            Utils.LogError($"Method {method.Name} wants to respond to Yarn command {commandName}, but not all of its parameters are strings!");
                                            paramsMatch = false;
                                            break;
                                        }
                                    }

                                    if (paramsMatch)
                                    {
                                        if (method.ReturnType == typeof(IEnumerator))
                                        {
                                            StartCoroutine(DoYarnCommand(component, method,
                                                parameters.ToArray()));
                                            startedCoroutine = true;
                                        }
                                        else
                                        {
                                            method.Invoke(component, parameters.ToArray());
                                        }

                                        numberOfMethodsFound++;
                                    }
                                }

                                if (!paramsMatch)
                                {
                                    errorValues.Add(new string[] { method.Name, commandName, methodParameters.Length.ToString(), parameters.Count.ToString() });
                                }
                            }
                        }
                    }
                }

                if (numberOfMethodsFound > 1)
                {
                    Utils.Log($"The command {command} found {numberOfMethodsFound} targets. You should only have one - check your scripts.");
                }
                else if (numberOfMethodsFound == 0)
                {
                    foreach (string[] errorVal in errorValues)
                    {
                        Utils.LogError($"Method {errorVal[0]} wants to respond to Yarn command {errorVal[1]}, but it has a different number of parameters ({errorVal[2]}) to those provided ({errorVal[3]}), or is not a string array!");
                    }
                }

                var wasValidCommand = numberOfMethodsFound > 0;

                if (!wasValidCommand)
                {
                    return (false, Dialogue.HandlerExecutionType.ContinueExecution);
                }

                return (true,
                    startedCoroutine
                        ? Dialogue.HandlerExecutionType.PauseExecution
                        : Dialogue.HandlerExecutionType.ContinueExecution);

                IEnumerator DoYarnCommandParams(Component component, MethodInfo method, string[][] localParameters)
                {
                    yield return StartCoroutine((IEnumerator) method.Invoke(component, localParameters));
                    ContinueDialogue();
                }

                IEnumerator DoYarnCommand(Component component, MethodInfo method, string[] localParameters)
                {
                    yield return StartCoroutine((IEnumerator) method.Invoke(component, localParameters));
                    ContinueDialogue();
                }
            }
        }

        private void ContinueDialogue()
        {
            _wasCompleteCalled = true;
            Dialogue.Continue();
        }

        string ILineLocalizationProvider.GetLocalizedTextForLine(Line line)
        {
            if (!_strings.TryGetValue(line.ID, out var result)) return null;

            for (int i = 0; i < line.Substitutions.Length; i++)
            {
                string substitution = line.Substitutions[i];
                result = result.Replace("{" + i + "}", substitution);
            }

            result = Dialogue.ExpandFormatFunctions(result, TextLanguage);

            return result;
        }

        #endregion
        
    }
}