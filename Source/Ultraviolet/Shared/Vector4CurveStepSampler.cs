﻿using System;

namespace Ultraviolet
{
    /// <summary>
    /// Represents an <see cref="ICurveSampler{TValue, TKey}"/> which performs step sampling on a curve of <see cref="Vector4"/> values.
    /// </summary>
    public class Vector4CurveStepSampler : ICurveSampler<Vector4, CurveKey<Vector4>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vector4CurveStepSampler"/> class.
        /// </summary>
        private Vector4CurveStepSampler() { }

        /// <inheritdoc/>
        public Vector4 InterpolateKeyframes(CurveKey<Vector4> key1, CurveKey<Vector4> key2, Single t, Vector4 offset) =>
            offset + (t >= 1 ? key2.Value : key1.Value);

        /// <inheritdoc/>
        public Vector4 CalculateLinearExtension(CurveKey<Vector4> key, Single position, CurvePositionType positionType) =>
            key.Value;

        /// <inheritdoc/>
        public Vector4 CalculateCycleOffset(Vector4 first, Vector4 last, Int32 cycle) => 
            (last - first) * cycle;

        /// <summary>
        /// Gets the singleton instance of the <see cref="Vector4CurveLinearSampler"/> class.
        /// </summary>
        public static Vector4CurveStepSampler Instance { get; } = new Vector4CurveStepSampler();
    }
}
