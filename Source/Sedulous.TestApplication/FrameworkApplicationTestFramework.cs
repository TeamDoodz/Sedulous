﻿using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.TestFramework;

namespace Sedulous.TestApplication
{
    /// <summary>
    /// Represents a unit test framework which hosts an instance of the Sedulous Framework.
    /// This framework is intended primarily for unit tests which test rendering.
    /// </summary>
    public abstract class FrameworkApplicationTestFramework : FrameworkTestFramework
    {
        /// <summary>
        /// Cleans up after running an Sedulous Application test.
        /// </summary>
        [TearDown]
        public void FrameworkApplicationTestFrameworkCleanup()
        {
            try
            {
                DestroyFrameworkApplication(application);
                application = null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception while tearing down {TestContext.CurrentContext.Test.MethodName}; " +
                    $"test status was {TestContext.CurrentContext.Result.Outcome.Status}", ex);
            }
        }

        /// <summary>
        /// Destroys the specified test application.
        /// </summary>
        /// <param name="application">The test application to destroy.</param>
        protected static void DestroyFrameworkApplication(IFrameworkTestApplication application)
        {
            try
            {
                if (application != null)
                {
                    application.Dispose();
                }
            }
            catch (Exception e1)
            {
                try
                {
                    var context = (FrameworkContext)typeof(FrameworkContext).GetField("current",
                        BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                    if (context != null)
                    {
                        context.Dispose();
                    }
                }
                catch (Exception e2)
                {
                    var error = new StringBuilder();
                    error.AppendLine($"An exception occurred while destroying the Sedulous application, and test framework failed to perform a clean teardown.");
                    error.AppendLine();
                    error.AppendLine($"Exception which occurred during cleanup:");
                    error.AppendLine();
                    error.AppendLine(e1.ToString());
                    error.AppendLine();
                    error.AppendLine($"Exception which occurred during teardown:");
                    error.AppendLine();
                    error.AppendLine(e2.ToString());

                    try
                    {
                        File.WriteAllText($"uv-test-error-{DateTime.Now:yyyy-MM-dd-HH-mm-ss-fff}.txt", error.ToString());
                    }
                    catch (IOException) { }
                }
                throw;
            }
        }

        /// <summary>
        /// Creates a throwaway sedulous application. The lifetime of this application
        /// must be managed manually by the caller of this method.
        /// </summary>
        /// <returns>The test application that was created.</returns>
        protected static IFrameworkTestApplication GivenAThrowawayFrameworkApplication()
        {
            return new FrameworkTestApplication();
        }

        /// <summary>
        /// Creates a throwaway Sedulous application with no window. The lifetime of this application
        /// must be managed manually by the caller of this method.
        /// </summary>
        /// <returns>The test application that was created.</returns>
        protected static IFrameworkTestApplication GivenAThrowawayFrameworkApplicationWithNoWindow()
        {
            return new FrameworkTestApplication(true);
        }

        /// <summary>
        /// Creates an Sedulous Framework test application.
        /// </summary>
        /// <returns>The test application that was created.</returns>
        protected IFrameworkTestApplication GivenAFrameworkApplication()
        {
            if (application != null)
                throw new InvalidOperationException("An application has already been created.");

            application = new FrameworkTestApplication();

            return application;
        }

        /// <summary>
        /// Creates an Sedulous Framework test application with a headless Sedulous context.
        /// </summary>
        /// <returns>The test application that was created.</returns>
        protected IFrameworkTestApplication GivenAFrameworkApplicationWithNoWindow()
        {
            if (application != null)
                throw new InvalidOperationException("An application has already been created.");

            application = new FrameworkTestApplication(headless: true);

            return application;
        }

        /// <summary>
        /// Creates an Sedulous Framework test application with an Sedulous context in service mode.
        /// </summary>
        /// <returns>The test application that was created.</returns>
        protected IFrameworkTestApplication GivenAFrameworkApplicationInServiceMode()
        {
            if (application != null)
                throw new InvalidOperationException("An application has already been created.");

            application = new FrameworkTestApplication(headless: true, serviceMode: true);

            return application;
        }

        /// <summary>
        /// Wraps the specified unit test result for evaluation.
        /// </summary>
        /// <param name="bitmap">The bitmap to wrap.</param>
        /// <returns>The wrapped value.</returns>
        protected BitmapResult TheResultingImage(Image.Image bitmap)
        {
            return new BitmapResult(bitmap);
        }

        /// <summary>
        /// Creates a copy of the specified asset which is specific to the machine that is currently
        /// executing the test.
        /// </summary>
        /// <param name="content">The content manager.</param>
        /// <param name="asset">The asset to copy.</param>
        /// <returns>The asset path of the new asset file which was created.</returns>
        protected String CreateMachineSpecificAssetCopy(ContentManager content, String asset)
        {
            Contract.Require(content, nameof(content));
            Contract.Require(asset, nameof(asset));

            var resolvedSourceFile = content.ResolveAssetFilePath(asset);
            var resolvedSourceExtension = Path.GetExtension(resolvedSourceFile);

            var copiedFileName = $"{Path.GetFileNameWithoutExtension(resolvedSourceFile)}-{Environment.MachineName}{resolvedSourceExtension}";
            var copiedFilePath = Path.Combine(Path.GetDirectoryName(resolvedSourceFile), copiedFileName);

            var copiedAssetName = Path.GetFileNameWithoutExtension(copiedFileName);
            var copiedAssetPath = Path.Combine(Path.GetDirectoryName(asset), copiedAssetName);

            File.Copy(resolvedSourceFile, copiedFilePath, true);

            return copiedAssetPath;
        }

        // State values.
        private FrameworkTestApplication application;
    }
}