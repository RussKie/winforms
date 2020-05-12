// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Linq;
using System.Reflection;
using Xunit;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class PropertyDescriptorGridEntryAccessibleObjectTests
    {
        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_Ctor_Default()
        {
            using var propertyGrid = new PropertyGrid();
            var propertyDescriptorGridEntryTestEntity = new PropertyDescriptorGridEntryTestEntity(propertyGrid, null, false);
            var propertyDescriptorGridEntryAccessibleObject = propertyDescriptorGridEntryTestEntity.TestPropertyDescriptorGridEntryAccessibleObject;

            Assert.NotNull(propertyDescriptorGridEntryAccessibleObject);

            TypeInfo propertyDescriptorGridEntryAccessibleObjectTypeInfo = propertyDescriptorGridEntryAccessibleObject.GetType().GetTypeInfo();
            FieldInfo owningPropertyDescriptorGridEntryField = propertyDescriptorGridEntryAccessibleObjectTypeInfo.GetDeclaredField("_owningPropertyDescriptorGridEntry");
            var owningGridEntry = owningPropertyDescriptorGridEntryField.GetValue(propertyDescriptorGridEntryAccessibleObject);

            Assert.Equal(propertyDescriptorGridEntryTestEntity, owningGridEntry);
        }

        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_ExpandCollapseState_collapsed_by_default()
        {
            using var propertyGrid = new PropertyGrid();
            var propertyDescriptorGridEntryTestEntity = new PropertyDescriptorGridEntryTestEntity(propertyGrid, null, false);
            var propertyDescriptorGridEntryAccessibleObject = propertyDescriptorGridEntryTestEntity.TestPropertyDescriptorGridEntryAccessibleObject;

            var expandCollapseState = propertyDescriptorGridEntryAccessibleObject.ExpandCollapseState;
            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, expandCollapseState);
        }

        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_ExpandCollapseState_reflects_ExpandablePropertyState()
        {
            using Form form = new Form();
            using PropertyGrid propertyGrid = new PropertyGrid();
            var testEntity = new TestEntity();
            testEntity.FontProperty = new Font(FontFamily.GenericSansSerif, 1);
            propertyGrid.SelectedObject = testEntity;

            form.Controls.Add(propertyGrid);

            PropertyGridView propertyGridView = propertyGrid.TestAccessor().Dynamic.gridView as PropertyGridView;

            int firstPropertyIndex = 1; // Index 0 corresponds to the category grid entry.
            PropertyDescriptorGridEntry gridEntry = (PropertyDescriptorGridEntry)propertyGridView.AccessibilityGetGridEntries()[firstPropertyIndex];

            var selectedGridEntry = propertyGridView.TestAccessor().Dynamic.selectedGridEntry as PropertyDescriptorGridEntry;
            Assert.Equal(gridEntry.PropertyName, selectedGridEntry.PropertyName);

            AccessibleObject selectedGridEntryAccessibleObject = gridEntry.AccessibilityObject;

            gridEntry.InternalExpanded = false;
            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, selectedGridEntryAccessibleObject.ExpandCollapseState);

            gridEntry.InternalExpanded = true;
            Assert.Equal(UiaCore.ExpandCollapseState.Expanded, selectedGridEntryAccessibleObject.ExpandCollapseState);
        }

        internal class PropertyDescriptorGridEntryTestEntity : PropertyDescriptorGridEntry
        {
            private PropertyDescriptorGridEntryAccessibleObject _accessibleObject { get; set; }

            internal PropertyDescriptorGridEntryTestEntity(PropertyGrid ownerGrid, GridEntry peParent, bool hide)
                : base(ownerGrid, peParent, hide)
            {
                _accessibleObject = new PropertyDescriptorGridEntryAccessibleObject(this);
            }

            public GridEntryAccessibleObject TestPropertyDescriptorGridEntryAccessibleObject
            {
                get
                {
                    return _accessibleObject;
                }
            }
        }

        internal class TestEntity
        {
            public Font FontProperty
            {
                get; set;
            }
        }

        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_Navigates_to_ListBoxAccessibleObject()
        {
            using var form = new Form();
            using var propertyGrid = new PropertyGrid();
            using var button = new Button();

            propertyGrid.SelectedObject = button;
            form.Controls.Add(propertyGrid);
            form.Controls.Add(button);

            using PropertyGridView propertyGridView = propertyGrid.TestAccessor().Dynamic.gridView as PropertyGridView;

            int thirdPropertyIndex = 3; // Index of AccessibleRole property which has a ListBox as editor.
            PropertyDescriptorGridEntry gridEntry = (PropertyDescriptorGridEntry)propertyGridView.AccessibilityGetGridEntries()[thirdPropertyIndex];

            propertyGridView.TestAccessor().Dynamic.selectedGridEntry = gridEntry;

            using TestDropDownHolder dropDownHolder = new TestDropDownHolder(propertyGridView, propertyGridView.DropDownListBox);
            propertyGridView.TestAccessor().Dynamic.dropDownHolder = dropDownHolder;
            dropDownHolder.TestAccessor().Dynamic._state = 0x00000002;

            var listboxFieldAccessibleObject = gridEntry.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.Equal("GridViewListBoxAccessibleObject", listboxFieldAccessibleObject.GetType().Name);
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
