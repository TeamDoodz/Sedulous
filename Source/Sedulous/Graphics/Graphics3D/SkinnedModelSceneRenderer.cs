﻿using System.Numerics;
using Sedulous.Core;

namespace Sedulous.Graphics.Graphics3D
{
    /// <summary>
    /// Represents an service which can render instances of the <see cref="SkinnedModelSceneInstance"/> class.
    /// </summary>
    public class SkinnedModelSceneRenderer : ModelSceneRendererBase<SkinnedModelSceneInstance, SkinnedModelNodeInstance>
    {
        /// <inheritdoc/>
        protected override void OnDrawingModelScene(SkinnedModelSceneInstance scene, Camera camera, ref Matrix4x4 transform)
        {
            currentSkinInstance = null;
            base.OnDrawingModelScene(scene, camera, ref transform);
        }

        /// <inheritdoc/>
        protected override void OnDrawnModelScene()
        {
            currentSkinInstance = null;
            base.OnDrawnModelScene();
        }

        /// <inheritdoc/>
        protected override void OnDrawingModelNode(SkinnedModelNodeInstance node, Camera camera, Effect effect, ref Matrix4x4 transform)
        {
            if (node.Skin != null && currentSkinInstance != node.Skin)
            {
                currentSkinInstance = node.Skin;
                node.Skin.GetBoneTransforms(skinBoneTransforms);
                UpdateSkinnedEffect(effect);
            }

            base.OnDrawingModelNode(node, camera, effect, ref transform);
        }

        /// <inheritdoc/>
        protected override void OnEffectChanged(Effect effect, Camera camera)
        {
            UpdateSkinnedEffect(effect);
            base.OnEffectChanged(effect, camera);
        }

        /// <summary>
        /// Updates the bone transforms on the specified effect.
        /// </summary>
        private void UpdateSkinnedEffect(Effect effect)
        {
            if (effect is SkinnedEffect skinnedEffect)
                skinnedEffect.SetBoneTransforms(skinBoneTransforms);
        }

        // Bone transforms for the current skin.
        private readonly Matrix4x4[] skinBoneTransforms = new Matrix4x4[SkinnedEffect.MaxBoneCount];
        private SkinInstance currentSkinInstance;
    }
}
