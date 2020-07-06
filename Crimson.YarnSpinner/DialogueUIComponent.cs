using System;
using Yarn;

namespace Crimson.YarnSpinner
{
    /// <summary>
    /// A <see cref="Component"/> that can display lines, options and commands to the user, and receive input regarding their choices.
    /// </summary>
    /// <remarks>
    /// The <see cref="DialogueRunner"/> uses subclasses of this type to relay information to and from the user, and to pause and resume the execution of the Yarn program.
    /// </remarks>
    /// <seealso cref="DialogueRunner.DialogueUI"/>
    /// <seealso cref="DialogueUI"/>
    public abstract class DialogueUIComponent : Component
    {
        protected DialogueUIComponent(bool active, bool visible) : base(active, visible)
        {
        }

        /// <summary>Signals that a conversation has started.</summary>
        public virtual void DialogueStarted()
        {
            // Default implementation does nothing.
        }

        /// <summary>
        /// Called by the <see cref="DialogueRunner"/> to signal that a line should be displayed to the user.
        /// </summary>
        /// <remarks>
        /// If this method returns <see
        /// cref="Dialogue.HandlerExecutionType.ContinueExecution"/>, it
        /// should not not call the <paramref name="onLineComplete"/>
        /// method.
        /// </remarks>
        /// <param name="line">The line that should be displayed to the
        /// user.</param>
        /// <param name="localizationProvider">The object that should be
        /// used to get the localised text of the line.</param>
        /// <param name="onLineComplete">A method that should be called to
        /// indicate that the line has finished being delivered.</param>
        /// <returns><see
        /// cref="Dialogue.HandlerExecutionType.PauseExecution"/> if
        /// dialogue should wait until the completion handler is
        /// called before continuing execution; <see
        /// cref="Dialogue.HandlerExecutionType.ContinueExecution"/> if
        /// dialogue should immediately continue running after calling this
        /// method.</returns>
        public abstract Dialogue.HandlerExecutionType RunLine(Line line, ILineLocalizationProvider localizationProvider,
            Action onLineComplete);
        
        /// <summary>
        /// Called by the <see cref="DialogueRunner"/> to signal that a set of options should be displayed to the user.
        /// </summary>
        /// <remarks>
        /// When this method is called, the <see cref="DialogueRunner"/>
        /// will pause execution until the `onOptionSelected` method is
        /// called.
        /// </remarks>
        /// <param name="optionSet">The set of options that should be
        /// displayed to the user.</param>
        /// <param name="localizationProvider">The object that should be
        /// used to get the localised text of each of the options.</param>
        /// <param name="onOptionSelected">A method that should be called
        /// when the user has made a selection.</param>
        public abstract void RunOptions(OptionSet optionSet, ILineLocalizationProvider localizationProvider, Action<int> onOptionSelected);
        
        /// <summary>
        /// Called by the <see cref="DialogueRunner"/> to signal that a command should be executed.
        /// </summary>
        /// <remarks>
        /// This method will only be invoked if the <see cref="Command"/>
        /// could not be handled by the <see cref="DialogueRunner"/>.
        ///
        /// If this method returns <see
        /// cref="Dialogue.HandlerExecutionType.ContinueExecution"/>, it
        /// should not call the <paramref name="onCommandComplete"/>
        /// method.
        /// </remarks>
        /// <param name="command">The command to be executed.</param>
        /// <param name="onCommandComplete">A method that should be called
        /// to indicate that the DialogueRunner should continue
        /// execution.</param>
        /// <inheritdoc cref="RunLine(Line, ILineLocalizationProvider, Action)"/>
        public abstract Dialogue.HandlerExecutionType RunCommand(Yarn.Command command, Action onCommandComplete);
        
        /// <summary>
        /// Called by the <see cref="DialogueRunner"/> to signal that the end of a node has been reached.
        /// </summary>
        /// <remarks>
        /// This method may be called multiple times before <see cref="DialogueComplete"/> is called.
        /// 
        /// If this method returns <see
        /// cref="Dialogue.HandlerExecutionType.ContinueExecution"/>, do
        /// not call the <paramref name="onComplete"/> method.
        /// </remarks>
        /// <param name="nextNode">The name of the next node that is being entered.</param>
        /// <param name="onComplete">A method that should be called to
        /// indicate that the DialogueRunner should continue executing.</param>
        /// <inheritdoc cref="RunLine(Line, ILineLocalizationProvider, Action)"/>
        public virtual Dialogue.HandlerExecutionType NodeComplete(string nextNode, Action onComplete)
        {
            // Default implementation does nothing.

            return Dialogue.HandlerExecutionType.ContinueExecution;
        }

        /// <summary>
        /// Called by the <see cref="DialogueRunner"/> to signal that the dialogue has ended.
        /// </summary>
        public virtual void DialogueComplete()
        {
            // Default implementation does nothing.
        }
    }
}