﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  A ToolStripButton that can display a popup.
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripDropDownButton : ToolStripDropDownItem
    {
        private bool showDropDownArrow = true;
        private byte openMouseId = 0;

        /// <summary>
        ///  Constructs a ToolStripButton that can display a popup.
        /// </summary>
        public ToolStripDropDownButton()
        {
            Initialize();
        }
        public ToolStripDropDownButton(string text) : base(text, null, (EventHandler)null)
        {
            Initialize();
        }
        public ToolStripDropDownButton(Image image) : base(null, image, (EventHandler)null)
        {
            Initialize();
        }
        public ToolStripDropDownButton(string text, Image image) : base(text, image, (EventHandler)null)
        {
            Initialize();
        }
        public ToolStripDropDownButton(string text, Image image, EventHandler onClick) : base(text, image, onClick)
        {
            Initialize();
        }
        public ToolStripDropDownButton(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
        {
            Initialize();
        }
        public ToolStripDropDownButton(string text, Image image, params ToolStripItem[] dropDownItems) : base(text, image, dropDownItems)
        {
            Initialize();
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ToolStripDropDownButtonAccessibleObject(this);
        }

        [DefaultValue(true)]
        public new bool AutoToolTip
        {
            get => base.AutoToolTip;
            set => base.AutoToolTip = value;
        }

        protected override bool DefaultAutoToolTip
        {
            get
            {
                return true;
            }
        }

        [DefaultValue(true)]
        [SRDescription(nameof(SR.ToolStripDropDownButtonShowDropDownArrowDescr))]
        [SRCategory(nameof(SR.CatAppearance))]
        public bool ShowDropDownArrow
        {
            get
            {
                return showDropDownArrow;
            }
            set
            {
                if (showDropDownArrow != value)
                {
                    showDropDownArrow = value;
                    InvalidateItemLayout(PropertyNames.ShowDropDownArrow);
                }
            }
        }
        /// <summary>
        ///  Creates an instance of the object that defines how image and text
        ///  gets laid out in the ToolStripItem
        /// </summary>
        private protected override ToolStripItemInternalLayout CreateInternalLayout()
        {
            return new ToolStripDropDownButtonInternalLayout(this);
        }

        protected override ToolStripDropDown CreateDefaultDropDown()
        {
            // AutoGenerate a ToolStrip DropDown - set the property so we hook events
            return new ToolStripDropDownMenu(this, /*isAutoGenerated=*/true);
        }

        /// <summary>
        ///  Called by all constructors of ToolStripButton.
        /// </summary>
        private void Initialize()
        {
            SupportsSpaceKey = true;
        }

        /// <summary>
        ///  Overriden to invoke displaying the popup.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if ((Control.ModifierKeys != Keys.Alt) &&
                (e.Button == MouseButtons.Left))
            {
                if (DropDown.Visible)
                {
                    ToolStripManager.ModalMenuFilter.CloseActiveDropDown(DropDown, ToolStripDropDownCloseReason.AppClicked);
                }
                else
                {
                    // opening should happen on mouse down.
                    Debug.Assert(ParentInternal != null, "Parent is null here, not going to get accurate ID");
                    openMouseId = (ParentInternal == null) ? (byte)0 : ParentInternal.GetMouseId();
                    ShowDropDown(/*mousePush =*/true);
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if ((Control.ModifierKeys != Keys.Alt) &&
                (e.Button == MouseButtons.Left))
            {
                Debug.Assert(ParentInternal != null, "Parent is null here, not going to get accurate ID");
                byte closeMouseId = (ParentInternal == null) ? (byte)0 : ParentInternal.GetMouseId();
                if (closeMouseId != openMouseId)
                {
                    openMouseId = 0;  // reset the mouse id, we should never get this value from toolstrip.
                    ToolStripManager.ModalMenuFilter.CloseActiveDropDown(DropDown, ToolStripDropDownCloseReason.AppClicked);
                    Select();
                }
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            openMouseId = 0;  // reset the mouse id, we should never get this value from toolstrip.
            base.OnMouseLeave(e);
        }
        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Owner != null)
            {
                ToolStripRenderer renderer = Renderer;
                Graphics g = e.Graphics;

                renderer.DrawDropDownButtonBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));

                if ((DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image)
                {
                    renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(g, this, InternalLayout.ImageRectangle));
                }

                if ((DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text)
                {
                    renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(g, this, Text, InternalLayout.TextRectangle, ForeColor, Font, InternalLayout.TextFormat));
                }
                if (ShowDropDownArrow)
                {
                    Rectangle dropDownArrowRect = (InternalLayout is ToolStripDropDownButtonInternalLayout layout) ? layout.DropDownArrowRect : Rectangle.Empty;

                    Color arrowColor;
                    if (Selected && !Pressed && SystemInformation.HighContrast)
                    {
                        arrowColor = Enabled ? SystemColors.HighlightText : SystemColors.ControlDark;
                    }
                    else
                    {
                        arrowColor = Enabled ? SystemColors.ControlText : SystemColors.ControlDark;
                    }
                    renderer.DrawArrow(new ToolStripArrowRenderEventArgs(g, this, dropDownArrowRect, arrowColor, ArrowDirection.Down));
                }
            }
        }

        protected internal override bool ProcessMnemonic(char charCode)
        {
            // checking IsMnemonic is not necesssary - toolstrip does this for us.
            if (HasDropDownItems)
            {
                Select();
                ShowDropDown();
                return true;
            }
            return false;
        }

        /// <summary>
        ///  An implementation of Accessibleobject for use with ToolStripDropDownButton
        /// </summary>
        [Runtime.InteropServices.ComVisible(true)]
        internal class ToolStripDropDownButtonAccessibleObject : ToolStripDropDownItemAccessibleObject
        {
            private readonly ToolStripDropDownButton ownerItem = null;

            public ToolStripDropDownButtonAccessibleObject(ToolStripDropDownButton ownerItem)
                : base(ownerItem)
            {
                this.ownerItem = ownerItem;
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.ControlTypePropertyId)
                {
                    return UiaCore.UIA.ButtonControlTypeId;
                }
                else
                {
                    return base.GetPropertyValue(propertyID);
                }
            }
        }

        private protected class ToolStripDropDownButtonInternalLayout : ToolStripItemInternalLayout
        {
            private ToolStripDropDownButton    ownerItem;
            private static readonly Size       dropDownArrowSizeUnscaled = new Size(5, 3);
            private static Size                dropDownArrowSize = dropDownArrowSizeUnscaled;
            private const int                  DROP_DOWN_ARROW_PADDING = 2;
            private static Padding             dropDownArrowPadding = new Padding(DROP_DOWN_ARROW_PADDING);
            private Padding                    scaledDropDownArrowPadding = dropDownArrowPadding;
            private Rectangle                  dropDownArrowRect    = Rectangle.Empty;

            public ToolStripDropDownButtonInternalLayout(ToolStripDropDownButton ownerItem) : base(ownerItem) {
                if (DpiHelper.IsPerMonitorV2Awareness)
                {
                    dropDownArrowSize = DpiHelper.LogicalToDeviceUnits(dropDownArrowSizeUnscaled, ownerItem.DeviceDpi);
                    scaledDropDownArrowPadding = DpiHelper.LogicalToDeviceUnits(dropDownArrowPadding, ownerItem.DeviceDpi);
                }
                else if (DpiHelper.IsScalingRequired) {
                    // these 2 values are used to calculate size of the clickable drop down button
                    // on the right of the image/text
                    dropDownArrowSize = DpiHelper.LogicalToDeviceUnits(dropDownArrowSizeUnscaled);
                    scaledDropDownArrowPadding = DpiHelper.LogicalToDeviceUnits(dropDownArrowPadding);
                }
                this.ownerItem = ownerItem;
            }

            public override Size GetPreferredSize(Size constrainingSize)
            {
                Size preferredSize = base.GetPreferredSize(constrainingSize);
                if (ownerItem.ShowDropDownArrow)
                {
                    if (ownerItem.TextDirection == ToolStripTextDirection.Horizontal)
                    {
                        preferredSize.Width += DropDownArrowRect.Width + scaledDropDownArrowPadding.Horizontal;
                    }
                    else
                    {
                        preferredSize.Height += DropDownArrowRect.Height + scaledDropDownArrowPadding.Vertical;
                    }
                }
                return preferredSize;
            }

            protected override ToolStripItemLayoutOptions CommonLayoutOptions()
            {
                ToolStripItemLayoutOptions options = base.CommonLayoutOptions();

                if (ownerItem.ShowDropDownArrow)
                {
                    if (ownerItem.TextDirection == ToolStripTextDirection.Horizontal)
                    {
                        // We're rendering horizontal....  make sure to take care of RTL issues.

                        int widthOfDropDown = dropDownArrowSize.Width + scaledDropDownArrowPadding.Horizontal;
                        options.client.Width -= widthOfDropDown;

                        if (ownerItem.RightToLeft == RightToLeft.Yes)
                        {
                            // if RightToLeft.Yes: [ v | rest of drop down button ]
                            options.client.Offset(widthOfDropDown, 0);
                            dropDownArrowRect = new Rectangle(scaledDropDownArrowPadding.Left, 0, dropDownArrowSize.Width, ownerItem.Bounds.Height);
                        }
                        else
                        {
                            // if RightToLeft.No [ rest of drop down button | v ]
                            dropDownArrowRect = new Rectangle(options.client.Right, 0, dropDownArrowSize.Width, ownerItem.Bounds.Height);
                        }
                    }
                    else
                    {
                        // else we're rendering vertically.
                        int heightOfDropDown = dropDownArrowSize.Height + scaledDropDownArrowPadding.Vertical;

                        options.client.Height -= heightOfDropDown;

                        //  [ rest of button / v]
                        dropDownArrowRect = new Rectangle(0, options.client.Bottom + scaledDropDownArrowPadding.Top, ownerItem.Bounds.Width - 1, dropDownArrowSize.Height);
                    }
                }
                return options;
            }

            public Rectangle DropDownArrowRect
            {
                get
                {
                    return dropDownArrowRect;
                }
            }
        }
    }
}
