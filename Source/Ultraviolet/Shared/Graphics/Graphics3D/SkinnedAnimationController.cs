﻿using System;
using System.Collections.Generic;
using Ultraviolet.Core;

namespace Ultraviolet.Graphics.Graphics3D
{
    /// <summary>
    /// Controls a collection of <see cref="SkinnedAnimationTrack"/> instances which are associated
    /// with a particular skinned model instance.
    /// </summary>
    internal class SkinnedAnimationController
    {
        private struct AnimationPosition { public SkinnedModelNodeAnimation Animation; public Double Time; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinnedAnimationController"/> class.
        /// </summary>
        /// <param name="model">The model instance that owns this manager.</param>
        /// <param name="tracks">The number of animation tracks to allocate for the model.</param>
        public SkinnedAnimationController(SkinnedModelInstance model, Int32 tracks)
        {
            Contract.Require(model, nameof(model));

            this.model = model;
            this.tracks = new SkinnedAnimationTrack[tracks];
            this.nodeAnimations = new AnimationPosition?[tracks];
            this.ordering = new Int64[tracks];

            for (var i = 0; i < tracks; i++)
                this.tracks[i] = new SkinnedAnimationTrack(model);
        }

        /// <summary>
        /// Updates the controller manager's state.
        /// </summary>
        /// <param name="time">Time elapsed since the last update.</param>
        public void Update(UltravioletTime time)
        {
            var wasUpdated = false;
            for (var i = 0; i < tracks.Length; i++)
            {
                if (tracks[i].Update(time))
                    wasUpdated = true;
            }

            if (wasUpdated)
                UpdateAnimationState();
        }

        /// <summary>
        /// Resets all of the manager's animations.
        /// </summary>
        public void ResetAnimations()
        {
            foreach (var controller in tracks)
                controller.Reset();
        }

        /// <summary>
        /// Plays the specified animation. If the animation is already playing,
        /// it will be restarted using the specified mode.
        /// </summary>
        /// <param name="mode">A <see cref="SkinnedAnimationMode"/> value which describes the animation mode.</param>
        /// <param name="animation">The animation to play.</param>
        /// <param name="speedMultiplier">The relative speed at which to play the animation.</param>
        /// <returns>The <see cref="SkinnedAnimationTrack"/> which is playing the animation, or <see langword="null"/> if the animation could not be played.</returns>
        public SkinnedAnimationTrack PlayAnimation(SkinnedAnimationMode mode, SkinnedAnimation animation, Single speedMultiplier)
        {
            var controllerAllocation = AllocateTrack(animation);
            var controller = controllerAllocation.Value;
            controller.Play(mode, animation, speedMultiplier);
            ordering[controllerAllocation.Key] = ++orderingCounter;
            return controller;
        }

        /// <summary>
        /// Stops the specified animation.
        /// </summary>
        /// <param name="animation">The animation to stop.</param>
        /// <returns>The <see cref="SkinnedAnimationTrack"/> which was playing the animation, or <see langword="null"/> if the animation was not being played.</returns>
        public SkinnedAnimationTrack StopAnimation(SkinnedAnimation animation)
        {
            var controller = GetTrackForAnimation(animation);
            if (controller != null)
            {
                controller.Stop();
                return controller;
            }
            return null;
        }

        /// <summary>
        /// Gets the track that is currently playing the specified animation.
        /// </summary>
        /// <param name="animation">The animation to evaluate.</param>
        /// <returns>The <see cref="SkinnedAnimationTrack"/> which is currently playing the specified 
        /// animation, or <see langword="null"/> if no track is playing the animation.</returns>
        public SkinnedAnimationTrack GetTrackForAnimation(SkinnedAnimation animation)
        {
            return GetTrackForAnimationInternal(animation).Value;
        }

        /// <summary>
        /// Gets the track that is currently playing the specified animation.
        /// </summary>
        private KeyValuePair<Int32, SkinnedAnimationTrack> GetTrackForAnimationInternal(SkinnedAnimation animation)
        {
            for (var i = 0; i < tracks.Length; i++)
            {
                var controller = tracks[i];
                if (controller.IsPlayingAnimation(animation))
                {
                    return new KeyValuePair<Int32, SkinnedAnimationTrack>(i, controller);
                }
            }
            return new KeyValuePair<Int32, SkinnedAnimationTrack>(-1, null);
        }

        /// <summary>
        /// Allocates a track to play the specified animation.
        /// </summary>
        private KeyValuePair<Int32, SkinnedAnimationTrack> AllocateTrack(SkinnedAnimation animation)
        {
            var existing = GetTrackForAnimationInternal(animation);
            if (existing.Value != null)
                return existing;

            // If no existing controller, find one that isn't playing.
            var leastRecentlyPlayed = default(SkinnedAnimationTrack);
            var leastRecentlyPlayedIndex = -1;
            var leastRecentlyPlayedOrder = Int64.MaxValue;
            for (var i = 0; i < tracks.Length; i++)
            {
                var controller = tracks[i];
                var order = ordering[i];

                if (leastRecentlyPlayed == null || order < leastRecentlyPlayedOrder)
                {
                    leastRecentlyPlayed = controller;
                    leastRecentlyPlayedIndex = i;
                    leastRecentlyPlayedOrder = order;
                }

                if (!controller.IsPlaying)
                    return new KeyValuePair<Int32, SkinnedAnimationTrack>(i, controller);
            }

            // If no stopped controller, override the one that was least recently played.
            return new KeyValuePair<Int32, SkinnedAnimationTrack>(leastRecentlyPlayedIndex, leastRecentlyPlayed);
        }

        /// <summary>
        /// Updates the animation's state and applies transforms to the model instance's nodes.
        /// </summary>
        private void UpdateAnimationState()
        {
            model.TraverseNodes((node, state) =>
            {
                var controller = (SkinnedAnimationController)state;

                var activeTracks = 0;
                for (var i = 0; i < controller.tracks.Length; i++)
                {
                    controller.nodeAnimations[i] = null;

                    var track = controller.tracks[i];
                    var trackAnimation = track.CurrentAnimation;
                    if (trackAnimation != null)
                    {
                        var nodeAnimation = trackAnimation.GetNodeAnimation(node.Template.LogicalIndex);
                        if (nodeAnimation != null)
                        {
                            controller.nodeAnimations[i] = new AnimationPosition { Animation = nodeAnimation, Time = track.Position };
                            activeTracks++;
                        }
                    }

                    controller.UpdateAnimatedNodeTransforms(node);
                }
            }, this);

            for (var i = 0; i < nodeAnimations.Length; i++)
                nodeAnimations[i] = null;
        }

        /// <summary>
        /// Updates the animated transforms for the specified node.
        /// </summary>
        private void UpdateAnimatedNodeTransforms(SkinnedModelNodeInstance node)
        {
            // TODO: Implement weighted blending

            for (var i = 0; i < nodeAnimations.Length; i++)
            {
                var nodeAnimation = nodeAnimations[i];
                if (nodeAnimation != null)
                {
                    var templatedTransform = node.Template.Transform;

                    var t = (Single)nodeAnimation.Value.Time;
                    var animation = nodeAnimation.Value.Animation;
                    var animatedTranslation = animation.Translation?.Evaluate(t, default) ?? templatedTransform.Translation;
                    var animatedRotation = animation.Rotation?.Evaluate(t, default) ?? templatedTransform.Rotation;
                    var animatedScale = animation.Scale?.Evaluate(t, default) ?? templatedTransform.Scale;

                    node.LocalTransform.UpdateFromTranslationRotationScale(animatedTranslation, animatedRotation, animatedScale);
                    return;
                }
            }
        }

        // Animation state for this model.
        private readonly SkinnedModelInstance model;
        private readonly SkinnedAnimationTrack[] tracks;
        private readonly AnimationPosition?[] nodeAnimations;
        private readonly Int64[] ordering;
        private Int32 orderingCounter;

    }
}
