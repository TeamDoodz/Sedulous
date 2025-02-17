﻿using System;
using Sedulous.Core;
using Sedulous.Core.Messages;
using Sedulous.Input;
using Sedulous.Sdl2.Messages;
using static Sedulous.Sdl2.Native.SDL_EventType;
using static Sedulous.Sdl2.Native.SDLNative;

namespace Sedulous.Sdl2.Input
{
    /// <summary>
    /// Manages the Sedulous context's connected game pad devices.
    /// </summary>
    internal sealed class GamePadDeviceInfo : FrameworkResource,
        IMessageSubscriber<FrameworkMessageId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GamePadDeviceInfo"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public GamePadDeviceInfo(FrameworkContext context)
            : base(context)
        {
            this.devicesByPlayer = new Sdl2GamePadDevice[SDL_NumJoysticks()];

            for (int i = 0; i < this.devicesByPlayer.Length; i++)
            {
                if (SDL_IsGameController(i))
                {
                    OnControllerDeviceAdded(i);
                }
            }

            context.Messages.Subscribe(this,
                Sdl2FrameworkMessages.SdlEvent);
        }

        /// <inheritdoc/>
        void IMessageSubscriber<FrameworkMessageId>.ReceiveMessage(FrameworkMessageId type, MessageData data)
        {
            if (type != Sdl2FrameworkMessages.SdlEvent)
                return;

            var evt = ((Sdl2EventMessageData)data).Event;
            switch (evt.type)
            {
                case SDL_CONTROLLERDEVICEADDED:
                    OnControllerDeviceAdded(evt.cdevice.which);
                    break;

                case SDL_CONTROLLERDEVICEREMOVED:
                    OnControllerDeviceRemoved(evt.cdevice.which);
                    break;
            }
        }

        /// <summary>
        /// Resets the states of the connected devices in preparation for the next frame.
        /// </summary>
        public void ResetDeviceStates()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            foreach (var device in devicesByPlayer)
            {
                if (device != null)
                {
                    device.ResetDeviceState();
                }
            }
        }

        /// <summary>
        /// Updates the state of the attached game pad devices.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        public void Update(FrameworkTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            foreach (var device in devicesByPlayer)
            {
                if (device != null)
                {
                    device.Update(time);
                }
            }
        }

        /// <summary>
        /// Gets the game pad that belongs to the specified player.
        /// </summary>
        /// <param name="playerIndex">The index of the player for which to retrieve a game pad.</param>
        /// <returns>The game pad that belongs to the specified player, or <see langword="null"/> if no such game pad exists.</returns>
        public Sdl2GamePadDevice GetGamePadForPlayer(Int32 playerIndex)
        {
            Contract.EnsureRange(playerIndex >= 0, nameof(playerIndex));
            Contract.EnsureNotDisposed(this, Disposed);

            return (playerIndex >= devicesByPlayer.Length) ? null : devicesByPlayer[playerIndex];
        }

        /// <summary>
        /// Gets the first connected game pad device.
        /// </summary>
        /// <returns>The first connected game pad device, or <see langword="null"/> if no game pads are connected.</returns>
        public Sdl2GamePadDevice GetFirstConnectedGamePad()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            for (int i = 0; i < devicesByPlayer.Length; i++)
            {
                if (devicesByPlayer[i] != null)
                {
                    return devicesByPlayer[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the first registered game pad device.
        /// </summary>
        /// <returns>The first registered game pad device, or <see langword="null"/> if no game pads are registered.</returns>
        public Sdl2GamePadDevice GetFirstRegisteredGamePad()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            for (int i = 0; i < devicesByPlayer.Length; i++)
            {
                if (devicesByPlayer[i]?.IsRegistered ?? false)
                {
                    return devicesByPlayer[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the number of attached game pads.
        /// </summary>
        public Int32 Count => count;

        /// <inheritdoc/>
        public event GamePadConnectionEventHandler GamePadConnected;

        /// <inheritdoc/>
        public event GamePadConnectionEventHandler GamePadDisconnected;

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                foreach (var device in devicesByPlayer)
                {
                    if (device != null)
                    {
                        device.Dispose();
                    }
                }
                if (!FrameworkContext.Disposed)
                {
                    FrameworkContext.Messages.Unsubscribe(this);
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Called when a controller device is added.
        /// </summary>
        /// <param name="joystickIndex">The index of the device to add.</param>
        private void OnControllerDeviceAdded(Int32 joystickIndex)
        {
            var gamecontroller = SDL_GameControllerOpen(joystickIndex);
            var joystick       = SDL_GameControllerGetJoystick(gamecontroller);
            var joystickID     = SDL_JoystickInstanceID(joystick);

            for (int i = 0; i < devicesByPlayer.Length; i++)
            {
                if (devicesByPlayer[i] != null && devicesByPlayer[i].InstanceID == joystickID)
                {
                    return;
                }
            }

            var playerIndex = GetFirstAvailablePlayerIndex();
            var device = new Sdl2GamePadDevice(FrameworkContext, joystickIndex, playerIndex);

            devicesByPlayer[playerIndex] = device;
            count++;

            OnGamePadConnected(device, playerIndex);
        }

        /// <summary>
        /// Called when a controller device is removed.
        /// </summary>
        /// <param name="instanceID">The instance identifier of the device to remove.</param>
        private void OnControllerDeviceRemoved(Int32 instanceID)
        {
            for (int i = 0; i < devicesByPlayer.Length; i++)
            {
                var device = devicesByPlayer[i];
                if (device != null && device.InstanceID == instanceID)
                {
                    OnGamePadDisconnected(device, i);

                    devicesByPlayer[i] = null;
                    count--;

                    return;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="GamePadConnected"/> event.
        /// </summary>
        /// <param name="device">The device that was connected.</param>
        /// <param name="playerIndex">The player index associated with the game pad.</param>
        private void OnGamePadConnected(GamePadDevice device, Int32 playerIndex) =>
            GamePadConnected?.Invoke(device, playerIndex);

        /// <summary>
        /// Raises the <see cref="GamePadDisconnected"/> event.
        /// </summary>
        /// <param name="device">The device that was disconnected.</param>
        /// <param name="playerIndex">The player index associated with the game pad.</param>
        private void OnGamePadDisconnected(GamePadDevice device, Int32 playerIndex) =>
            GamePadDisconnected?.Invoke(device, playerIndex);

        /// <summary>
        /// Gets the index of the first player which does not have an associated game pad.
        /// </summary>
        /// <returns>The index of the first player which does not have an associated game pad.</returns>
        private Int32 GetFirstAvailablePlayerIndex()
        {
            for (int i = 0; i < devicesByPlayer.Length; i++)
            {
                if (devicesByPlayer[i] == null)
                {
                    return i;
                }
            }
        
            var devicesOld = devicesByPlayer;
            var devicesNew = new Sdl2GamePadDevice[devicesOld.Length + 1];
            Array.Copy(devicesOld, devicesNew, devicesOld.Length);

            devicesByPlayer = devicesNew;

            return devicesByPlayer.Length - 1;
        }

        // State values.
        private Sdl2GamePadDevice[] devicesByPlayer;
        private Int32 count;
    }
}
