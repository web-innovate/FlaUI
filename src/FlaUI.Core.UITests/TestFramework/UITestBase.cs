﻿using System;
using System.IO;
using FlaUI.UIA2;
using FlaUI.UIA3;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace FlaUI.Core.UITests.TestFramework
{
    /// <summary>
    /// Base class for ui test with some helper methods
    /// </summary>
    public abstract class UITestBase
    {
        /// <summary>
        /// Flag which indicates if any test was run on a new instance of the app
        /// </summary>
        private bool _wasTestRun;

        protected AutomationType AutomationType { get; }

        protected TestApplicationType ApplicationType { get; }

        /// <summary>
        /// Path of the directory for the screenshots
        /// </summary>
        protected string ScreenshotDir { get; }

        /// <summary>
        /// Instance of the current running application
        /// </summary>
        protected Application App { get; private set; }

        protected AutomationBase Automation { get; }

        protected UITestBase(AutomationType automationType, TestApplicationType appType)
        {
            AutomationType = automationType;
            ApplicationType = appType;
            ScreenshotDir = @"c:\FailedTestsScreenshots";
            _wasTestRun = false;
            Automation = AutomationType == AutomationType.UIA2 ? (AutomationBase)new UIA2Automation() : new UIA3Automation();
        }

        /// <summary>
        /// Setup which starts the application (once per test-class)
        /// </summary>
        [OneTimeSetUp]
        public void BaseSetup()
        {
            switch (ApplicationType)
            {
                case TestApplicationType.Custom:
                    App = StartApplication();
                    return;
                case TestApplicationType.WinForms:
                    App = Application.Launch(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\TestApplications\WinFormsApplication\bin\WinFormsApplication.exe"));
                    return;
                case TestApplicationType.Wpf:
                    App = Application.Launch(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\TestApplications\WpfApplication\bin\WpfApplication.exe"));
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Closes the application after all tests were run
        /// </summary>
        [OneTimeTearDown]
        public void BaseTeardown()
        {
            Automation.Dispose();
            App.Dispose();
            App = null;
        }

        /// <summary>
        /// Takes screenshot on failed tests
        /// </summary>
        [TearDown]
        public void BaseTestTeardown()
        {
            // Mark that a test was run on this application
            _wasTestRun = true;
            // Make a screenshot if the test failed
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                TakeScreenshot(TestContext.CurrentContext.Test.FullName);
            }
        }

        /// <summary>
        /// Method which starts the custom application to test
        /// </summary>
        protected virtual Application StartApplication()
        {
            return null;
        }

        /// <summary>
        /// Restarts the application to test
        /// </summary>
        protected void RestartApp()
        {
            // Don't restart if no test was already run on it
            if (!_wasTestRun) return;
            // Restart the application
            BaseTeardown();
            BaseSetup();
            _wasTestRun = false;
        }

        private void TakeScreenshot(string screenshotName)
        {
            var imagename = screenshotName + ".png";
            imagename = imagename.Replace("\"", "");
            var imagePath = Path.Combine(ScreenshotDir, imagename);
            try
            {
                Directory.CreateDirectory(ScreenshotDir);
                ScreenCapture.CaptureScreenToFile(imagePath);
                Console.WriteLine("Screenshot taken: {0}", imagePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save screenshot to directory: {0}, filename: {1}, Ex: {2}", ScreenshotDir, imagePath, ex);
            }
        }
    }
}
