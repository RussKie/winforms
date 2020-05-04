// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Accessibility;
using static Interop;

namespace System.Windows.Forms
{
    public partial class AccessibleObject
    {
        /// <summary>
        /// This is a helper class that is used to wrap system IAccessible object
        /// and to perform calls to system accessibility methods with check wrapped
        /// IAccessible object for null and with COM exception handling to prevent
        /// application crash in case system IAccessible object is not found.
        /// </summary>
        private class SystemIAccessibleWrapper : IAccessible
        {
            private IAccessible _systemIAccessible;

            public SystemIAccessibleWrapper(IAccessible systemIAccessible)
                => _systemIAccessible = systemIAccessible;

            public void accSelect(int flagsSelect, object varChild)
                => Execute(() => _systemIAccessible.accSelect(flagsSelect, varChild));

            public void accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, object varChild)
            {
                int left = 0;
                int top = 0;
                int width = 0;
                int height = 0;

                Execute(() =>
                {
                    _systemIAccessible.accLocation(out left, out top, out width, out height, varChild);

                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccLocation: Setting " +
                        left.ToString(CultureInfo.InvariantCulture) + ", " +
                        top.ToString(CultureInfo.InvariantCulture) + ", " +
                        width.ToString(CultureInfo.InvariantCulture) + ", " +
                        height.ToString(CultureInfo.InvariantCulture));
                });

                pxLeft = left;
                pyTop = top;
                pcxWidth = width;
                pcyHeight = height;
            }

            public object accNavigate(int navDir, object varStart)
                => GetValue(() => _systemIAccessible.accNavigate(navDir, varStart));

            public object accHitTest(int xLeft, int yTop)
                => GetValue(() => _systemIAccessible.accHitTest(xLeft, yTop));

            public void accDoDefaultAction(object varChild)
                => Execute(() => _systemIAccessible.accDoDefaultAction(varChild));

            public object accParent
                => GetValue(() => _systemIAccessible.accParent);

            public int accChildCount
                => GetValue(() => _systemIAccessible.accChildCount);

            public object get_accChild(object childID)
                => GetValue(() => _systemIAccessible.get_accChild(childID));

            public string get_accName(object childID)
                => GetValue(() => _systemIAccessible.get_accName(childID));

            public void set_accName(object childID, string newName)
                => Execute(() => _systemIAccessible.set_accName(childID, newName));

            public string accName
            {
                get => GetValue(() => _systemIAccessible.accName);
                set => Execute(() => _systemIAccessible.accName = value);
            }

            public string accValue
            {
                get => GetValue(() => _systemIAccessible.accValue);
                set => Execute(() => _systemIAccessible.accValue = value);
            }

            public string get_accValue(object childID)
                => GetValue(() => _systemIAccessible.get_accValue(childID));

            public void set_accValue(object childID, string newValue)
                => Execute(() => _systemIAccessible.set_accValue(childID, newValue));

            public string accDescription
                => GetValue(() => _systemIAccessible.accDescription);

            public string get_accDescription(object childID)
                => GetValue(() => _systemIAccessible.get_accDescription(childID));

            public object accRole
                => GetValue(() => _systemIAccessible.accRole);

            public object get_accRole(object childID)
                => GetValue(() => _systemIAccessible.get_accRole(childID));

            public object accState
                => GetValue(() => _systemIAccessible.accState);

            public object get_accState(object childID)
                => GetValue(() => _systemIAccessible.get_accState(childID));

            public string accHelp
                => GetValue(() => _systemIAccessible.accHelp);

            public string get_accHelp(object childID)
                => GetValue(() => _systemIAccessible.get_accHelp(childID));

            public int get_accHelpTopic(out string pszHelpFile, object childID)
            {
                string helpFile = null;
                int result = GetValue(() => _systemIAccessible.get_accHelpTopic(out helpFile, childID), -1);
                pszHelpFile = helpFile;
                return result;
            }

            public string accKeyboardShortcut
                => GetValue(() => _systemIAccessible.accKeyboardShortcut);

            public string get_accKeyboardShortcut(object childID)
                => GetValue(() => _systemIAccessible.get_accKeyboardShortcut(childID));

            public object accFocus
                => GetValue(() => _systemIAccessible.accFocus);

            public object accSelection
                => GetValue(() => _systemIAccessible.accSelection);

            public string accDefaultAction
                => GetValue(() => _systemIAccessible.accDefaultAction);

            public string get_accDefaultAction(object childID)
                => GetValue(() => _systemIAccessible.get_accDefaultAction(childID));

            private TReturn GetValue<TReturn>(Func<TReturn> getFunction) where TReturn : class
            {
                if (_systemIAccessible == null || getFunction == null)
                {
                    return null;
                }

                try
                {
                    return getFunction();
                }
                catch (COMException e) when (e.ErrorCode == (int)HRESULT.DISP_E_MEMBERNOTFOUND)
                {
                    // System IAccessible is not found.
                }
                catch (ArgumentException)
                {
                    // Argument exception can be thrown in case main system IAccessible cannot be gotten
                    // with MEMBERNOTFOUND and then all children (ChildId > 0) also cannot be gotten.
                }

                return null;
            }

            private TReturn GetValue<TReturn>(
                Func<TReturn> func,
                TReturn defaultReturnValue = default(TReturn)) where TReturn : struct
            {
                if (_systemIAccessible == null || func == null)
                {
                    return defaultReturnValue;
                }

                try
                {
                    return func();
                }
                catch (COMException e) when (e.ErrorCode == (int)HRESULT.DISP_E_MEMBERNOTFOUND)
                {
                    // System IAccessible is not found.
                }
                catch (ArgumentException)
                {
                    // Argument exception can be thrown in case main system IAccessible cannot be gotten
                    // with MEMBERNOTFOUND and then all children (ChildId > 0) also cannot be gotten.
                }

                return defaultReturnValue;
            }

            private void Execute(Action action)
            {
                if (_systemIAccessible == null || action == null)
                {
                    return;
                }

                try
                {
                    action();
                }
                catch (COMException e) when (e.ErrorCode == (int)HRESULT.DISP_E_MEMBERNOTFOUND)
                {
                    // System IAccessible is not found.
                }
                catch (ArgumentException)
                {
                    // Argument exception can be thrown in case main system IAccessible cannot be gotten
                    // with MEMBERNOTFOUND and then all children (ChildId > 0) also cannot be gotten.
                }
            }
        }
    }
}
