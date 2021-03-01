// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public class TaskDialogLink
    {
        public event EventHandler Click;

        internal TaskDialogLink(string text)
        {
            Text = text;
            Id = Guid.NewGuid().ToString();
        }

        public string Text { get; }
        internal string Id { get; }
        internal void OnClick(EventArgs e) => Click?.Invoke(this, e);
        public override string ToString() => $"<A HREF=\"{Id}\">{Text}</A>"; /* render <A HREF="Id">Text</A> with necessary escaping */
    }
}
