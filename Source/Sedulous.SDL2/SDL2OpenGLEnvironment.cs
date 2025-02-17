﻿using System;
using Sedulous.Core;
using Sedulous.OpenGL;
using Sedulous.Platform;
using Sedulous.Sdl2.Platform;
using static Sedulous.Sdl2.Native.SDL_GLattr;
using static Sedulous.Sdl2.Native.SDL_GLcontextFlag;
using static Sedulous.Sdl2.Native.SDL_GLprofile;
using static Sedulous.Sdl2.Native.SDLNative;

namespace Sedulous.Sdl2
{
    /// <summary>
    /// Represents an implementation of the <see cref="OpenGLEnvironment"/> class which is implemented using SDL2.
    /// </summary>
    public sealed class Sdl2OpenGLEnvironment : OpenGLEnvironment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sdl2OpenGLEnvironment"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public Sdl2OpenGLEnvironment(FrameworkContext context)
            : base(context)
        { }

        /// <inheritdoc/>
        public override IntPtr GetProcAddress(String functionName)
        {
            Contract.Require(functionName, nameof(functionName));
            return SDL_GL_GetProcAddress(functionName);
        }

        /// <inheritdoc/>
        public override IntPtr CreateOpenGLContext()
        {
            var masterWindowPtr = ((Sdl2FrameworkWindowInfo)FrameworkContext.GetPlatform().Windows).GetMasterPointer();
            return SDL_GL_CreateContext(masterWindowPtr);
        }

        /// <inheritdoc/>
        public override void DeleteOpenGLContext(IntPtr openGLContext)
        {
            SDL_GL_DeleteContext(openGLContext);
        }

        /// <inheritdoc/>
        public override void DesignateCurrentWindow(IFrameworkWindow window, IntPtr openGLContext)
        {
            var windowInfo = ((Sdl2FrameworkWindowInfoOpenGL)FrameworkContext.GetPlatform().Windows);
            windowInfo.DesignateCurrent(window, openGLContext);
        }

        /// <inheritdoc/>
        public override void DrawFramebuffer(FrameworkTime time)
        {
            var oglwin = (Sdl2FrameworkWindow)FrameworkContext.GetPlatform().Windows.GetCurrent();
            oglwin.Draw(time);
        }

        /// <inheritdoc/>
        public override void SwapFramebuffers()
        {
            var oglwin = (Sdl2FrameworkWindow)FrameworkContext.GetPlatform().Windows.GetCurrent();
            SDL_GL_SwapWindow((IntPtr)oglwin);
        }

        /// <inheritdoc/>
        public override void ClearErrors()
        {
            SDL_ClearError();
        }

        /// <inheritdoc/>
        public override void ThrowPlatformErrorException()
        {
            throw new Sdl2Exception();
        }

        /// <inheritdoc/>
        public override Boolean RequestOpenGLProfile(Boolean isGLES)
        {
            var profile = isGLES ? SDL_GL_CONTEXT_PROFILE_ES : SDL_GL_CONTEXT_PROFILE_CORE;
            return SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, (Int32)profile) == 0;
        }

        /// <inheritdoc/>
        public override Boolean RequestOpenGLVersion(Version version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            if (SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, version.Major) < 0)
                return false;

            if (SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, version.Minor) < 0)
                return false;

            return true;
        }

        /// <inheritdoc/>
        public override Boolean RequestDebugContext(Boolean debug)
        {
            if (debug)
            {
                return SDL_GL_SetAttribute(SDL_GL_CONTEXT_FLAGS, (Int32)SDL_GL_CONTEXT_DEBUG_FLAG) == 0;
            }
            else
            {
                return SDL_GL_SetAttribute(SDL_GL_CONTEXT_FLAGS, 0) == 0;
            }
        }

        /// <inheritdoc/>
        public override Boolean RequestDepthSize(Int32 depthSize)
        {
            return SDL_GL_SetAttribute(SDL_GL_DEPTH_SIZE, depthSize) == 0;
        }

        /// <inheritdoc/>
        public override Boolean RequestStencilSize(Int32 stencilSize)
        {
            return SDL_GL_SetAttribute(SDL_GL_STENCIL_SIZE, stencilSize) == 0;
        }

        /// <inheritdoc/>
        public override Boolean Request24BitFramebuffer()
        {
            if (SDL_GL_SetAttribute(SDL_GL_RED_SIZE, 5) < 0)
                return false;

            if (SDL_GL_SetAttribute(SDL_GL_GREEN_SIZE, 6) < 0)
                return false;

            if (SDL_GL_SetAttribute(SDL_GL_BLUE_SIZE, 5) < 0)
                return false;

            return true;
        }

        /// <inheritdoc/>
        public override Boolean Request32BitFramebuffer()
        {
            if (SDL_GL_SetAttribute(SDL_GL_RED_SIZE, 8) < 0)
                return false;

            if (SDL_GL_SetAttribute(SDL_GL_GREEN_SIZE, 8) < 0)
                return false;

            if (SDL_GL_SetAttribute(SDL_GL_BLUE_SIZE, 8) < 0)
                return false;

            return true;
        }

        /// <inheritdoc/>
        public override Boolean RequestSrgbCapableFramebuffer()
        {
            return SDL_GL_SetAttribute(SDL_GL_FRAMEBUFFER_SRGB_CAPABLE, 1) == 0;
        }

        /// <inheritdoc/>
        public override Boolean SetSwapInterval(Int32 swapInterval)
        {
            return SDL_GL_SetSwapInterval(swapInterval) == 0;
        }

        /// <inheritdoc/>
        public override Boolean IsFramebufferSrgbCapable
        {
            get
            {
                unsafe
                {
                    var value = 0;
                    if (SDL_GL_GetAttribute(SDL_GL_FRAMEBUFFER_SRGB_CAPABLE, &value) < 0)
                        return false;

                    return value == 1;
                }
            }
        }
    }
}
