﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Represents an expander button and the associated expanded area
    ///   of a task dialog.
    /// </summary>
    public sealed class TaskDialogExpander : TaskDialogControl
    {
        private string? _text;

        private string? _expandedButtonText;

        private string? _collapsedButtonText;

        private bool _expandFooterArea;

        private bool _expanded;

        /// <summary>
        ///   Occurs when the value of the <see cref="Expanded"/> property changes while
        ///   this control is shown in a task dialog.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This event will only occur when the expanded state is changed by the user,
        ///   because it isn't possible to programmatically change the <see cref="Expanded"/>
        ///   property while this control is shown in a task dialog.
        /// </para>
        /// </remarks>
        public event EventHandler? ExpandedChanged;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogExpander"/> class.
        /// </summary>
        public TaskDialogExpander()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogExpander"/> class
        ///   using the given text.
        /// </summary>
        /// <param name="text">The text to be displayed in the dialog's expanded area.</param>
        public TaskDialogExpander(string? text)
            : this()
        {
            _text = text;
        }

        /// <summary>
        ///   Gets or sets the text to be displayed in the dialog's expanded area.
        /// </summary>
        /// <value>
        ///   The text to be displayed in the dialog's expanded area. The default value is
        ///   <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   This control will only be shown if this property is not <see langword="null"/> or an empty string.
        /// </para>
        /// <para>
        ///   This property can be set while the dialog is shown.</para>
        /// </remarks>
        public string? Text
        {
            get => _text;

            set
            {
                DenyIfBoundAndNotCreated();
                DenyIfWaitingForInitialization();

                // Update the text if we are bound.
                BoundTaskDialog?.UpdateTextElement(
                    ComCtl32.TDE.EXPANDED_INFORMATION, value);

                _text = value;
            }
        }

        /// <summary>
        ///   Gets or sets the text to be displayed in the expander button when it
        ///   is in the expanded state.
        /// </summary>
        /// <value>
        ///   The text that is to be displayed in the expander button when it
        ///   is in the expanded state, or <see langword="null"/> or an empty string to use a
        ///   text provided by the operating system. The default value is <see langword="null"/>.
        /// </value>
        public string? ExpandedButtonText
        {
            get => _expandedButtonText;

            set
            {
                DenyIfBound();

                _expandedButtonText = value;
            }
        }

        /// <summary>
        ///   Gets or sets the text to be displayed in the expander button when it
        ///   is in the collapsed state.
        /// </summary>
        /// <value>
        ///   The text that is to be displayed in the expander button when it
        ///   is in the collapsed state, or <see langword="null"/> or an empty string to use a
        ///   text provided by the operating system. The default value is <see langword="null"/>.
        /// </value>
        public string? CollapsedButtonText
        {
            get => _collapsedButtonText;

            set
            {
                DenyIfBound();

                _collapsedButtonText = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that indicates whether the expander button is in the
        ///   expanded state (so that the dialog's expanded area is visible).
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the expander button is in the expanded state; <see langword="false"/> if
        ///   it's in the collapsed state. The default value is <see langword="false"/>.
        /// </value>
        public bool Expanded
        {
            get => _expanded;

            set
            {
                // The Task Dialog doesn't provide a message type to click the expando
                // button, so we don't allow to change this property (it will however
                // be updated when we receive an ExpandoButtonClicked notification).
                // TODO: Should we throw only if the new value is different than the
                // old one?
                DenyIfBound();

                _expanded = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that indicates whether the expanded area is displayed at the bottom
        ///   of the task dialog's footer area, instead of immediately after the dialog's
        ///   <see cref="TaskDialogPage.Text"/>.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the expanded area is displayed at the bottom of the task dialog's
        ///   footer area; <see langword="false"/> if the expanded area is displayed immediately after the
        ///   dialog's <see cref="TaskDialogPage.Text"/>. The default value is <see langword="false"/>.
        /// </value>
        public bool ExpandFooterArea
        {
            get => _expandFooterArea;

            set
            {
                DenyIfBound();

                _expandFooterArea = value;
            }
        }

        internal override bool IsCreatable => base.IsCreatable && !TaskDialogPage.IsNativeStringNullOrEmpty(_text);

        /// <summary>
        ///   Returns a string that represents the current <see cref="TaskDialogExpander"/> control.
        /// </summary>
        /// <returns>A string that contains the control text.</returns>
        public override string ToString() => _text ?? base.ToString() ?? string.Empty;

        internal void HandleExpandoButtonClicked(bool expanded)
        {
            _expanded = expanded;
            OnExpandedChanged(EventArgs.Empty);
        }

        private protected override ComCtl32.TDF BindCore()
        {
            ComCtl32.TDF flags = base.BindCore();

            if (_expanded)
            {
                flags |= ComCtl32.TDF.EXPANDED_BY_DEFAULT;
            }
            if (_expandFooterArea)
            {
                flags |= ComCtl32.TDF.EXPAND_FOOTER_AREA;
            }

            return flags;
        }

        private void OnExpandedChanged(EventArgs e) => ExpandedChanged?.Invoke(this, e);
    }
}
