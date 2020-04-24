// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Xunit;
using System.Reflection;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class PropertyDescriptorGridEntryAccessibleObjectTests
    {
        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_Navigates_to_ListBoxAccessibleObject()
        {
            using var form = new Form();
            using var propertyGrid = new PropertyGrid();
            using var button = new Button();

            propertyGrid.SelectedObject = button;
            form.Controls.Add(propertyGrid);
            form.Controls.Add(button);

            Type propertyGridType = typeof(PropertyGrid);
            FieldInfo[] fields = propertyGridType.GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Instance);
            FieldInfo gridViewField = fields.First(f => f.Name == "gridView");
            using PropertyGridView propertyGridView = gridViewField.GetValue(propertyGrid) as PropertyGridView;

            int thirdPropertyIndex = 3; // Index of AccessibleRole property which has a ListBox as editor.
            PropertyDescriptorGridEntry gridEntry = (PropertyDescriptorGridEntry)propertyGridView.AccessibilityGetGridEntries()[thirdPropertyIndex];

            Type propertyGridViewType = typeof(PropertyGridView);
            FieldInfo[] propertyGridViewFields = propertyGridViewType.GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Instance);
            FieldInfo selectedGridEntryField = propertyGridViewFields.First(f => f.Name == "selectedGridEntry");
            selectedGridEntryField.SetValue(propertyGridView, gridEntry);

            using TestDropDownHolder dropDownHolder = new TestDropDownHolder(propertyGridView, propertyGridView.DropDownListBox);
            FieldInfo dropDownHolderField = propertyGridViewFields.First(f => f.Name == "dropDownHolder");
            dropDownHolderField.SetValue(propertyGridView, dropDownHolder);
            Type controlType = typeof(Control);
            FieldInfo[] controlFields = controlType.GetFields(
                         BindingFlags.NonPublic |
                         BindingFlags.Instance);
            FieldInfo stateField = controlFields.First(f => f.Name == "_state");
            stateField.SetValue(dropDownHolder, 0x00000002);

            var listboxFieldAccessibleObject = gridEntry.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.Equal("GridViewListBoxAccessibleObject", listboxFieldAccessibleObject.GetType().Name);
        }

        private class TestGridEntry : GridEntry
        {
            PropertyGridView _propertyGridView;

            public TestGridEntry(PropertyGrid ownerGrid, GridEntry peParent, PropertyGridView propertyGridView)
                : base(ownerGrid, peParent)
            {
                _propertyGridView = propertyGridView;
            }

            internal override PropertyGridView GridEntryHost
            {
                get
                {
                    return _propertyGridView;
                }

                set
                {
                    base.GridEntryHost = value;
                }
            }
        }

        private class TestPropertyDescriptorGridEntry : PropertyDescriptorGridEntry
        {
            public TestPropertyDescriptorGridEntry(PropertyGrid ownerGrid, GridEntry peParent, bool hide)
                : base(ownerGrid, peParent, hide)
            {
            }

            public override GridEntryCollection Children
            {
                get
                {
                    GridEntryCollection collection = new GridEntryCollection(this, new GridEntry[0]);
                    return collection;
                }
            }

            internal override bool Enumerable => false;
        }

        private class TestPropertyGridView : PropertyGridView
        {
            private Control _parent;

            public TestPropertyGridView(IServiceProvider serviceProvider, PropertyGrid propertyGrid)
                : base(serviceProvider, propertyGrid)
            {
                _parent = propertyGrid;
            }

            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new TestPropertyGridViewAccessibleObject(this, null);
            }

            internal override Control ParentInternal
            {
                get
                {
                    return _parent;
                }
                set
                {
                    _parent = value;
                }
            }
        }

        private class TestPropertyGridViewAccessibleObject : PropertyGridView.PropertyGridViewAccessibleObject
        {
            public TestPropertyGridViewAccessibleObject(PropertyGridView owner, PropertyGrid parentPropertyGrid)
                : base(owner, parentPropertyGrid)
            {
            }
        }

        private class TestDropDownHolder : PropertyGridView.DropDownHolder
        {
            private Control _component;

            public TestDropDownHolder(PropertyGridView psheet, Control component)
                : base(psheet)
            {
                _component = component;
            }

            public override Control Component => _component;
        }
    }
}
