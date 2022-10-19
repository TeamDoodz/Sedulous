﻿using System;
using Sedulous.Core.Data;

namespace Sedulous.Core.Tests.Data
{
    /// <summary>
    /// Represents a model with a single-index indexer used by the object loader tests.
    /// </summary>
    public class ObjectLoader_IndexerModel : DataObject
    {
        public ObjectLoader_IndexerModel(String key, Guid globalID)
            : base(key, globalID)
        {

        }

        public Int32 this[Int32 ix]
        {
            get { return values[ix]; }
            set { values[ix] = value; }
        }

        private Int32[] values = new Int32[100];
    }
}
