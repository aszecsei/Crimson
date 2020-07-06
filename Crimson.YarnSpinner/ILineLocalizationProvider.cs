using Yarn;

namespace Crimson.YarnSpinner
{
    /// <summary>
    /// Provides a mechanism for determining the user-facing localized content for a <see cref="Line"/>.
    /// </summary>
    public interface ILineLocalizationProvider
    {
        /// <summary>
        /// Returns the user-facing text for a given <see cref="Line"/>.
        /// </summary>
        /// <remarks>
        /// This method determines the final text to deliver to the user,
        /// given a <see cref="Line"/>. Classes that implement this method
        /// should use the Line's <see cref="Line.ID"/> to look up the
        /// user-facing text in a string table, replace any substitutions
        /// in the text, and then expand any format functions by calling
        /// <see cref="Dialogue.ExpandFormatFunctions"/>.
        ///
        /// See the <seealso cref="Line"/> class's documentation for more
        /// information on how to prepare a Line for delivery to the user.
        /// </remarks>
        /// <param name="line">The <see cref="Line"/> to get the text for.</param>
        /// <returns>The text to show the user, or `null` if the
        /// user-facing text cannot be found.</returns>
        /// <seealso cref="Line"/>
        /// <seealso cref="Dialogue.ExpandFormatFunctions(string, string)"/>
        string GetLocalizedTextForLine(Line line);
    }
}