using System.Numerics;
using Sedulous.Core;

namespace Sedulous.Graphics.Graphics3D
{
    /// <summary>
    /// Represents one of a <see cref="Skin"/> instance's skeletal joints.
    /// </summary>
    public class SkinJoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkinJoint"/> class.
        /// </summary>
        /// <param name="node">The joint's node.</param>
        /// <param name="inverseBindMatrix">The joint's inverse bind matrix.</param>
        public SkinJoint(ModelNode node, Matrix4x4 inverseBindMatrix)
        {
            Contract.Require(node, nameof(node));

            this.Node = node;
            this.InverseBindMatrix = inverseBindMatrix;
        }

        /// <summary>
        /// Gets the joint's node.
        /// </summary>
        public ModelNode Node { get; }

        /// <summary>
        /// Gets the joint's inverse bind matrix.
        /// </summary>
        public Matrix4x4 InverseBindMatrix { get; }
    }
}
