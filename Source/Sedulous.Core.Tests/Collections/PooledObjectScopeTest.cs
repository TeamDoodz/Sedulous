﻿using System;
using NUnit.Framework;
using Sedulous.Core.Collections;
using Sedulous.Core.TestFramework;

namespace Sedulous.Core.Tests.IO
{
    [TestFixture]
    public class PooledObjectScopeTest : CoreTestFramework
    {
        private class PoolableDataObject { public String Data { get; set; } }

        [Test]
        public void PooledObjectScope_ScopeRetrievesAndReleasesObject()
        {
            var pool = new ExpandingPool<PoolableDataObject>(1);

            using (var scope = pool.RetrieveScoped())
            {
                TheResultingValue(pool.Capacity).ShouldBe(1);
                TheResultingValue(pool.Count).ShouldBe(1);

                scope.Object.Data = "Hello, world!";
            }

            TheResultingValue(pool.Capacity).ShouldBe(1);
            TheResultingValue(pool.Count).ShouldBe(0);
        }
    }
}
