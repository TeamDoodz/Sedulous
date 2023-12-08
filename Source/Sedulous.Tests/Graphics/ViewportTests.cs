﻿using System;
using System.Numerics;
using NUnit.Framework;
using Sedulous.Graphics;
using Sedulous.TestFramework;

namespace Sedulous.Tests.Graphics
{
    [TestFixture]
    public class ViewportTests : FrameworkTestFramework
    {
        [Test]
        public void Viewport_CalculatesProjectCorrectly()
        {
            var viewport = new Viewport(0, 0, 1024, 768);

            var source = new Vector3(12f, 23f, 34f);
            var world = Matrix4x4.Identity;
            var view = Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.UnitY);
            var proj = Matrix4x4.CreatePerspectiveFieldOfView((float)Math.PI / 4f, 4f / 3f, 1f, 1000f);

            var result = viewport.Project(source, proj, view, world);

            TheResultingValue(result).WithinDelta(0.0001f)
                .ShouldBe(128.3898f, 1119.2529f, 1.0355f);
        }

        [Test]
        public void Viewport_CalculatesProjectCorrectly_WithOutParam()
        {
            var viewport = new Viewport(0, 0, 1024, 768);

            var source = new Vector3(12f, 23f, 34f);
            var world = Matrix4x4.Identity;
            var view = Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.UnitY);
            var proj = Matrix4x4.CreatePerspectiveFieldOfView((float)Math.PI / 4f, 4f / 3f, 1f, 1000f);

            viewport.Project(ref source, ref proj, ref view, ref world, out var result);

            TheResultingValue(result).WithinDelta(0.0001f)
                .ShouldBe(128.3898f, 1119.2529f, 1.0355f);
        }

        [Test]
        public void Viewport_CalculatesUnprojectCorrectly()
        {
            var viewport = new Viewport(0, 0, 1024, 768);

            var source = new Vector3(128.3898f, 1119.2529f, 1.0355f);
            var world = Matrix4x4.Identity;
            var view = Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.UnitY);
            var proj = Matrix4x4.CreatePerspectiveFieldOfView((float)Math.PI / 4f, 4f / 3f, 1f, 1000f);

            var result = viewport.Project(source, proj, view, world);

            TheResultingValue(result).WithinDelta(0.01f)
                .ShouldBe(30534.64f, -261341.9f, 0.7485f);
        }

        [Test]
        public void Viewport_CalculatesUnprojectCorrectly_WithOutParam()
        {
            var viewport = new Viewport(0, 0, 1024, 768);

            var source = new Vector3(128.3898f, 1119.2529f, 1.0355f);
            var world = Matrix4x4.Identity;
            var view = Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.UnitY);
            var proj = Matrix4x4.CreatePerspectiveFieldOfView((float)Math.PI / 4f, 4f / 3f, 1f, 1000f);

            viewport.Project(ref source, ref proj, ref view, ref world, out var result);

            TheResultingValue(result).WithinDelta(0.01f)
                .ShouldBe(30534.64f, -261341.9f, 0.7485f);
        }
    }
}
