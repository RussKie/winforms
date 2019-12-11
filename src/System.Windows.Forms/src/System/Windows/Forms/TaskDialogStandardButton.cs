﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a standard ("common") button of a task dialog whose text and function
    /// is defined by a <see cref="TaskDialogResult"/> value.
    /// </summary>
    /// <remarks>
    /// The text of a <see cref="TaskDialogStandardButton"/> is provided by the operating system,
    /// depending on the <see cref="TaskDialogResult"/> that the button uses.
    /// 
    /// In contrast to a <see cref="TaskDialogCustomButton"/> that can be shown as regular button
    /// or as command link, a <see cref="TaskDialogStandardButton"/> can only be shown as
    /// regular button.
    /// 
    /// Showing a <see cref="TaskDialogButton"/> with a <see cref="TaskDialogResult.Cancel"/>
    /// result in a task dialog will add a close button to the task dialog title bar and will
    /// allow to close the dialog by pressing ESC or Alt+F4 (just as if
    /// <see cref="TaskDialogPage.AllowCancel"/> was set to <see langword="true"/>).
    /// </remarks>
    public sealed class TaskDialogStandardButton : TaskDialogButton
    {
        private TaskDialogResult _result;

        private bool _visible = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogStandardButton"/> class.
        /// </summary>
        public TaskDialogStandardButton()
            // Use 'OK' by default instead of 'None' (which would not be a valid
            // standard button).
            : this(TaskDialogResult.OK)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="TaskDialogStandardButton"/> class
        ///  using the specified result.
        /// </summary>
        /// <param name="result">The <see cref="TaskDialogResult"/> that is represent by this 
        ///   <see cref="TaskDialogStandardButton"/>.
        /// </param>
        /// <param name="enabled">A value indicating whether the button can respond to user interaction.</param>
        /// <param name="defaultButton">A value that indicates whether this button is the default button
        ///   in the task dialog.
        /// </param>
        /// <param name="allowCloseDialog">A value that indicates whether the task dialog should close
        ///   when this button is clicked.
        /// </param>
        public TaskDialogStandardButton(TaskDialogResult result, bool enabled = true, bool defaultButton = false, bool allowCloseDialog = true)
        {
            if (!IsValidStandardButtonResult(result))
            {
                throw new ArgumentOutOfRangeException(nameof(result));
            }

            _result = result;
            Enabled = enabled;
            DefaultButton = defaultButton;
            AllowCloseDialog = allowCloseDialog;
        }

        /// <summary>
        /// Gets or sets the <see cref="TaskDialogResult"/> which is represented by
        /// this <see cref="TaskDialogStandardButton"/>.
        /// </summary>
        public TaskDialogResult Result
        {
            get => _result;

            set
            {
                if (!IsValidStandardButtonResult(value))
                {
                    // Note: This shouldn't be an InvalidEnumArgumentException because we actually
                    // don't allow all values of the enum (TaskDialogResult.None is not a valid
                    // standard button result).
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                DenyIfBound();

                // If we are part of a StandardButtonCollection, we must now notify it
                // that we changed our result.
                Collection?.HandleKeyChange(this, value);

                // If this was successful or we are not part of a collection,
                // we can now set the result.
                _result = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if this
        /// <see cref="TaskDialogStandardButton"/> should be shown when displaying
        /// the task dialog.
        /// </summary>
        /// <remarks>
        /// Setting this property to <see langword="false"/> allows you to still receive the
        /// <see cref="TaskDialogButton.Click"/> event (e.g. for the
        /// <see cref="TaskDialogResult.Cancel"/> button when
        /// <see cref="TaskDialogPage.AllowCancel"/> is set), or to call the
        /// <see cref="TaskDialogButton.PerformClick"/> method even if the button
        /// is not shown.
        /// </remarks>
        public bool Visible
        {
            get => _visible;

            set
            {
                DenyIfBound();

                _visible = value;
            }
        }

        internal override bool IsCreatable => base.IsCreatable && _visible;

        internal override int ButtonID => (int)_result;

        internal new TaskDialogStandardButtonCollection? Collection
        {
            get => (TaskDialogStandardButtonCollection?)base.Collection;
            set => base.Collection = value;
        }

        private static TaskDialogButtons GetButtonFlagForResult(TaskDialogResult result) => result switch
        {
            TaskDialogResult.OK => TaskDialogButtons.OK,
            TaskDialogResult.Cancel => TaskDialogButtons.Cancel,
            TaskDialogResult.Abort => TaskDialogButtons.Abort,
            TaskDialogResult.Retry => TaskDialogButtons.Retry,
            TaskDialogResult.Ignore => TaskDialogButtons.Ignore,
            TaskDialogResult.Yes => TaskDialogButtons.Yes,
            TaskDialogResult.No => TaskDialogButtons.No,
            TaskDialogResult.Close => TaskDialogButtons.Close,
            TaskDialogResult.Help => TaskDialogButtons.Help,
            TaskDialogResult.TryAgain => TaskDialogButtons.TryAgain,
            TaskDialogResult.Continue => TaskDialogButtons.Continue,
            _ => default
        };

        private static bool IsValidStandardButtonResult(TaskDialogResult result) =>
            GetButtonFlagForResult(result) != default;

        /// <summary>
        /// Returns a string that represents the current <see cref="TaskDialogRadioButton"/> control.
        /// </summary>
        /// <returns>A string that contains the name of the <see cref="TaskDialogResult"/> value.</returns>
        public override string ToString() => _result.ToString();

        internal TaskDialogButtons GetButtonFlag() => GetButtonFlagForResult(_result);
    }
}
