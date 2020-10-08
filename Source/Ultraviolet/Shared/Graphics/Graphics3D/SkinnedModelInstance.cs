﻿using System;
using System.Linq;
using Ultraviolet.Core;

namespace Ultraviolet.Graphics.Graphics3D
{
    /// <summary>
    /// Represents an animated instance of a <see cref="SkinnedModel"/>. Each instance represents a particular 
    /// skinned animation at a particular point in time.
    /// </summary>
    public class SkinnedModelInstance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkinnedModelInstance"/> class.
        /// </summary>
        /// <param name="template">The <see cref="SkinnedModel"/> which serves as this instance's template.</param>
        /// <param name="maxSimultaneousAnimations">The maximum number of animations which can simultaneously play on this instance.</param>
        public SkinnedModelInstance(SkinnedModel template, Int32 maxSimultaneousAnimations = 4)
        {
            Contract.Require(template, nameof(template));
            Contract.EnsureRange(maxSimultaneousAnimations >= 1, nameof(maxSimultaneousAnimations));

            this.Template = template;
            this.Skins = new SkinInstanceCollection(template.Skins.Select(x => new SkinInstance(x, this)));
            this.Scenes = new SkinnedModelSceneInstanceCollection(template.Scenes.Select(x => new SkinnedModelSceneInstance(x, this)), 
                template.Scenes.DefaultScene.LogicalIndex);

            this.controllerManager = new SkinnedAnimationController(this, maxSimultaneousAnimations);
            this.nodeManager = new SkinnedModelInstanceNodeManager(this);
        }

        /// <summary>
        /// Updates the model instance's state.
        /// </summary>
        /// <param name="time">Time elapsed since the last update.</param>
        public void Update(UltravioletTime time)
        {
            Contract.Require(time, nameof(time));

            controllerManager.Update(time);

            foreach (var skin in Skins)
                skin.Update();
        }

        /// <summary>
        /// Resets all of the instance's animations.
        /// </summary>
        public void ResetAnimations()
        {
            controllerManager.ResetAnimations();
        }

        /// <summary>
        /// Plays the specified animation on this instance, if it exists. If the animation is already playing,
        /// it will be restarted using the specified mode.
        /// </summary>
        /// <param name="mode">A <see cref="SkinnedAnimationMode"/> value which describes the animation mode.</param>
        /// <param name="animationName">The name of the animation to play.</param>
        /// <param name="speedMultiplier">The relative speed at which to play the animation.</param>
        /// <returns>The <see cref="SkinnedAnimationTrack"/> which is playing the animation, or <see langword="null"/> if the animation could not be played.</returns>
        public SkinnedAnimationTrack PlayAnimation(SkinnedAnimationMode mode, String animationName, Single speedMultiplier = 1.0f)
        {
            Contract.Require(animationName, nameof(animationName));

            var animation = Template.Animations.TryGetAnimationByName(animationName);
            if (animation == null)
                return null;

            return controllerManager.PlayAnimation(mode, animation, speedMultiplier);
        }

        /// <summary>
        /// Plays the specified animation on this instance, if it exists. If the animation is already playing,
        /// it will be restarted using the specified mode.
        /// </summary>
        /// <param name="mode">A <see cref="SkinnedAnimationMode"/> value which describes the animation mode.</param>
        /// <param name="animationIndex">The index of the animation to play.</param>
        /// <param name="speedMultiplier">The relative speed at which to play the animation.</param>
        /// <returns>The <see cref="SkinnedAnimationTrack"/> which is playing the animation, or <see langword="null"/> if the animation could not be played.</returns>
        public SkinnedAnimationTrack PlayAnimation(SkinnedAnimationMode mode, Int32 animationIndex, Single speedMultiplier = 1.0f)
        {
            Contract.EnsureRange(animationIndex >= 0, nameof(animationIndex));

            if (animationIndex >= Template.Animations.Count)
                return null;

            var animation = Template.Animations[animationIndex];
            return controllerManager.PlayAnimation(mode, animation, speedMultiplier);
        }

        /// <summary>
        /// Stops the specified animation on this instance, if it is playing.
        /// </summary>
        /// <param name="animationName">The name of the animation to stop playing.</param>
        /// <returns><see langword="true"/> if the animation was stopped; otherwise, <see langword="false"/>.</returns>
        public Boolean StopAnimation(String animationName)
        {
            Contract.Require(animationName, nameof(animationName));

            var animation = Template.Animations.TryGetAnimationByName(animationName);
            if (animation == null)
                return false;

            return controllerManager.StopAnimation(animation) != null;
        }

        /// <summary>
        /// Stops the specified animation on this instance, if it is playing.
        /// </summary>
        /// <param name="animationIndex">The index of the animation to stop playing.</param>
        /// <returns><see langword="true"/> if the animation was stopped; otherwise, <see langword="false"/>.</returns>
        public Boolean StopAnimation(Int32 animationIndex)
        {
            Contract.EnsureRange(animationIndex >= 0, nameof(animationIndex));

            if (animationIndex >= Template.Animations.Count)
                return false;

            var animation = Template.Animations[animationIndex];
            return controllerManager.StopAnimation(animation) != null;
        }

        /// <summary>
        /// Gets the <see cref="SkinnedAnimationTrack"/> that is playing the specified animation,
        /// if that animation is currently being played.
        /// </summary>
        /// <param name="animationName">The name of the animation for which to retrieve a controller.</param>
        /// <returns>The <see cref="SkinnedAnimationTrack"/> which is playing the specified animation,
        /// or <see langword="null"/> if no controller is playing the animation.</returns>
        public SkinnedAnimationTrack GetAnimationController(String animationName)
        {
            Contract.Require(animationName, nameof(animationName));

            var animation = Template.Animations.TryGetAnimationByName(animationName);
            if (animation == null)
                return null;

            return controllerManager.GetTrackForAnimation(animation);
        }

        /// <summary>
        /// Gets the <see cref="SkinnedAnimationTrack"/> that is playing the specified animation,
        /// if that animation is currently being played.
        /// </summary>
        /// <param name="animationIndex">The index of the animation for which to retrieve a controller.</param>
        /// <returns>The <see cref="SkinnedAnimationTrack"/> which is playing the specified animation,
        /// or <see langword="null"/> if no controller is playing the animation.</returns>
        public SkinnedAnimationTrack GetAnimationController(Int32 animationIndex)
        {
            Contract.EnsureRange(animationIndex >= 0, nameof(animationIndex));

            if (animationIndex >= Template.Animations.Count)
                return null;

            var animation = Template.Animations[animationIndex];
            return controllerManager.GetTrackForAnimation(animation);
        }

        /// <summary>
        /// Gets the node instance with the specified logical index.
        /// </summary>
        /// <param name="logicalIndex">The logical index of the node instance to retrieve.</param>
        /// <returns>The <see cref="SkinnedModelNodeInstance"/> with the specified logical index.</returns>
        public SkinnedModelNodeInstance GetNodeInstanceByLogicalIndex(Int32 logicalIndex) => nodeManager.GetNodeInstanceByLogicalIndex(logicalIndex);

        /// <summary>
        /// Performs an action on all nodes in the model.
        /// </summary>
        /// <param name="action">The action to perform on each node.</param>
        /// <param name="state">An arbitrary state objece to pass to <paramref name="action"/>.</param>
        public void TraverseNodes(Action<SkinnedModelNodeInstance, Object> action, Object state)
        {
            Contract.Require(action, nameof(action));

            foreach (var scene in Scenes)
                scene.TraverseNodes(action, state);
        }

        /// <summary>
        /// Gets the <see cref="SkinnedModel"/> which serves as this instance's template.
        /// </summary>
        public SkinnedModel Template { get; }

        /// <summary>
        /// Gets the instance's collection of skins.
        /// </summary>
        public SkinInstanceCollection Skins { get; }

        /// <summary>
        /// Gets the instance's collection of scenes.
        /// </summary>
        public SkinnedModelSceneInstanceCollection Scenes { get; }

        // Internal state trackers.
        private readonly SkinnedAnimationController controllerManager;
        private readonly SkinnedModelInstanceNodeManager nodeManager;
    }
}
