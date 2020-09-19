﻿using System;

namespace Ultraviolet
{
    /// <summary>
    /// Represents an <see cref="ICurveSampler{TValue, TKey}"/> which performs smooth (bicubic) sampling on a curve of <see cref="Vector3"/> values.
    /// </summary>
    public class Vector3CurveSmoothSampler : ICurveSampler<Vector3, SmoothCurveKey<Vector3>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3CurveSmoothSampler"/> class.
        /// </summary>
        private Vector3CurveSmoothSampler() { }

        /// <inheritdoc/>
        public Vector3 InterpolateKeyframes(SmoothCurveKey<Vector3> key1, SmoothCurveKey<Vector3> key2, Single t, Vector3 offset)
        {
            var t2 = t * t;
            var t3 = t2 * t;
            var key1Value = key1.Value;
            var key2Value = key2.Value;
            var tangentIn = key2.TangentIn;
            var tangentOut = key1.TangentOut;

            var polynomial1 = (2.0 * t3 - 3.0 * t2 + 1.0); // (2t^3 - 3t^2 + 1)
            var polynomial2 = (t3 - 2.0 * t2 + t);         // (t3 - 2t^2 + t)  
            var polynomial3 = (-2.0 * t3 + 3.0 * t2);      // (-2t^2 + 3t^2)
            var polynomial4 = (t3 - t2);                   // (t^3 - t^2)

            return offset + (key1Value * (Single)polynomial1 + tangentOut * (Single)polynomial2 + key2Value * (Single)polynomial3 + tangentIn * polynomial4);
        }

        /// <inheritdoc/>
        public Vector3 CalculateLinearExtension(SmoothCurveKey<Vector3> key, Single position, CurvePositionType positionType)
        {
            switch (positionType)
            {
                case CurvePositionType.BeforeCurve:
                    return key.Value - key.TangentIn * (key.Position - position);

                case CurvePositionType.AfterCurve:
                    return key.Value - key.TangentOut * (key.Position - position);

                default:
                    return key.Value;
            }
        }

        /// <inheritdoc/>
        public Vector3 CalculateCycleOffset(Vector3 first, Vector3 last, Int32 cycle) => 
            (last - first) * cycle;

        /// <summary>
        /// Gets the singleton instance of the <see cref="Vector3CurveSmoothSampler"/> class.
        /// </summary>
        public static Vector3CurveSmoothSampler Instance { get; } = new Vector3CurveSmoothSampler();
    }
}
