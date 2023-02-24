﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripDropDownMenuTests
    {
        [WinFormsFact]
        public void ToolStripDropDownMenu_Constructor()
        {
            using var menu = new ToolStripDropDownMenu();

            Assert.NotNull(menu);
        }

        [WinFormsFact]
        public void ToolStripDropDownMenu_ConstructorOwnerItemBool()
        {
            using var owner = new ToolStripButton();
            var isAutoGenerated = true;

            using var menu = new ToolStripDropDownMenu(owner, isAutoGenerated);

            Assert.NotNull(menu);
            Assert.Equal(owner, menu.OwnerItem);
            Assert.True(menu.IsAutoGenerated);
        }
    }
}
