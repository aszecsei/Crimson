using System;
using System.Collections;
using System.Text;
using Yarn;

namespace Crimson.YarnSpinner
{
    public class DialogueUI : DialogueUIComponent
    {
        /// <summary>
        /// The object that contains the dialogue and the options.
        /// </summary>
        public Entity DialogueContainer;

        /// <summary>
        /// How quickly to show the text, in seconds per character
        /// </summary>
        public float TextSpeed = 0.025f;

        /// <summary>
        /// When true, the user has indicated that they want to proceed to the next line.
        /// </summary>
        private bool _userRequestedNextLine = false;

        /// <summary>
        /// The method that we should call when the user has chosen an option. Externally provided
        /// by the DialogueRunner.
        /// </summary>
        private Action<int> _currentOptionSelectionHandler;

        /// <summary>
        /// When true, the DialogueRunner is waiting for the user to press one of the option buttons.
        /// </summary>
        private bool _waitingForOptionSelection = false;

        /// <summary>
        /// An event that is called when the dialogue starts.
        /// </summary>
        /// <remarks>
        /// Use this event to enable any dialogue-related UI and gameplay elements,
        /// and disable any non-dialogue UI and gameplay elements.
        /// </remarks>
        public Action OnDialogueStart;

        /// <summary>
        /// An event that is called when the dialogue ends.
        /// </summary>
        /// <remarks>
        /// Use this event to disable any dialogue-related UI and gameplay elements,
        /// and enable any non-dialogue UI and gameplay elements.
        /// </remarks>
        public Action OnDialogueEnd;

        /// <summary>
        /// An event that is called when a <see cref="Line"/> has been delivered.
        /// </summary>
        /// <remarks>
        /// This event is called before <see cref="OnLineUpdate"/> is called. Use
        /// this event to prepare the scene to deliver a line.
        /// </remarks>
        public Action OnLineStart;

        /// <summary>
        /// An event that is called when a line has finished being delivered.
        /// </summary>
        /// <remarks>
        /// This method is called after <see cref="OnLineUpdate"/>. Use this method
        /// to display UI elements like a "continue" button.
        ///
        /// When this method has been called, the Dialogue UI will wait for the
        /// <see cref="MarkLineComplete"/> method to be called, which signals that
        /// the line should be dismissed.
        /// </remarks>
        /// <seealso cref="OnLineUpdate"/>
        /// <seealso cref="MarkLineComplete"/>
        public Action OnLineFinishDisplaying;

        /// <summary>
        /// An event that is called when the visible part of the line's localized
        /// text changes.
        /// </summary>
        /// <remarks>
        /// The <see cref="string"/> parameter that this event receives is
        /// the text that should be displayed to the user. Use this method to
        /// display line text to the user.
        /// </remarks>
        public Action<string> OnLineUpdate;

        /// <summary>
        /// An event that is called when a line has finished displaying, and should
        /// be removed from the screen.
        /// </summary>
        /// <remarks>
        /// This method is called after the <see cref="MarkLineComplete"/> function
        /// has been called. Use this method to dismiss the line's UI elements.
        ///
        /// After this method is called, the next piece of dialogue content will
        /// be presented, or the dialogue will end.
        /// </remarks>
        public Action OnLineEnd;

        /// <summary>
        /// An event that is called when an <see cref="OptionSet"/> has been displayed
        /// to the user.
        /// </summary>
        public Action OnOptionsStart;

        /// <summary>
        /// An event that is called when an option has been selected, and the
        /// option UI elements should be hidden.
        /// </summary>
        public Action OnOptionsEnd;

        /// <summary>
        /// An event that is called when a <see cref="Yarn.Command"/> is received.
        /// </summary>
        /// <remarks>
        /// Use this method to dispatch a command to other parts of your game.
        ///
        /// This method is only called if the <see cref="Yarn.Command"/> has not
        /// been handled by a command handler that has been added to the <see cref="DialogueRunner"/>,
        /// or by a method on a <see cref="Component"/> in the scene with the attribute
        /// <see cref="YarnCommandAttribute"/>.
        /// </remarks>
        public Action<string> OnCommand;
        
        /// <summary>
        /// A delegate that is called to transform each line before it is displayed to the user. No transformation is
        /// applied by default.
        /// </summary>
        public Func<string, string> LineTransformer;

        public DialogueUI() : base(true, false)
        {
        }
        
        public override void EntityAdded(Scene scene)
        {
            base.EntityAdded(scene);

            DialogueContainer?.SetEnabled(false);
            
            Engine.Commands.Log("Awake!");
            
            // TODO: Hide buttons
        }

        public override Dialogue.HandlerExecutionType RunLine(Line line, ILineLocalizationProvider localizationProvider, Action onLineComplete)
        {
            StartCoroutine(DoRunLine(line, localizationProvider, onLineComplete));
            return Dialogue.HandlerExecutionType.PauseExecution;
        }

        private IEnumerator DoRunLine(Yarn.Line line, ILineLocalizationProvider localizationProvider, Action onComplete)
        {
            OnLineStart?.Invoke();

            _userRequestedNextLine = false;

            string text = localizationProvider.GetLocalizedTextForLine(line);

            if (text == null)
            {
                Utils.Log($"Line {line.ID} doesn't have any localized text.");
                text = line.ID;
            }

            if (LineTransformer != null)
            {
                text = LineTransformer(text);
            }

            if (TextSpeed > 0f)
            {
                var stringBuilder = new StringBuilder();

                foreach (char c in text)
                {
                    stringBuilder.Append(c);
                    OnLineUpdate?.Invoke(stringBuilder.ToString());
                    if (_userRequestedNextLine)
                    {
                        OnLineUpdate?.Invoke(text);
                        break;
                    }

                    yield return new WaitForSeconds(TextSpeed);
                }
            }
            else
            {
                OnLineUpdate?.Invoke(text);
            }

            _userRequestedNextLine = false;
            
            OnLineFinishDisplaying?.Invoke();
            
            yield return new WaitUntil(() => _userRequestedNextLine);
            
            Engine.Commands.Log("Done waiting");

            yield return new WaitForEndOfFrame();

            OnLineEnd?.Invoke();

            onComplete();
        }

        public override void RunOptions(OptionSet optionSet, ILineLocalizationProvider localizationProvider, Action<int> onOptionSelected)
        {
            throw new NotImplementedException();
        }

        public override Dialogue.HandlerExecutionType RunCommand(Yarn.Command command, Action onCommandComplete)
        {
            OnCommand?.Invoke(command.Text);

            return Dialogue.HandlerExecutionType.ContinueExecution;
        }

        public override void DialogueStarted()
        {
            DialogueContainer?.SetEnabled(true);
            OnDialogueStart?.Invoke();
        }

        public override void DialogueComplete()
        {
            OnDialogueEnd?.Invoke();
            DialogueContainer?.SetEnabled(false);
        }

        public void MarkLineComplete()
        {
            _userRequestedNextLine = true;
        }

        public void SelectOption(int optionID)
        {
            if (!_waitingForOptionSelection)
            {
                Utils.Log("An option was selected, but the dialogue UI was not expecting it.");
                return;
            }

            _waitingForOptionSelection = false;
            _currentOptionSelectionHandler?.Invoke(optionID);
        }
    }
}