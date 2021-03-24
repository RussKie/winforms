// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ProjectConfiguration.Initialize();

            Application.EnableVisualStyles();

            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            ////Application.SetDefaultFont(new Font(....));

            //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException); //UnhandledExceptionMode.ThrowException
            //Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            //try
            //{
            //}
            //catch (System.Exception)
            //{
            //    Environment.Exit(-1);
            //}

            //Environment.Exit(0);
        }
    }
}
