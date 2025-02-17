﻿using System;
using Newtonsoft.Json;
using Sedulous.Core.Data;

namespace Sedulous.Core.Tests.Data
{
    partial class DataObjectRegistryTests
    {
        public class TestDataObject : DataObject
        {
            [JsonConstructor]
            public TestDataObject(String key, Guid id)
                : base(key, id)
            { }

            public String Foo { get; protected set; }
            public String Bar { get; protected set; }
        }
    }
}
