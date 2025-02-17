﻿using System;
using Sedulous.Core;
using Sedulous.Core.Messages;
using Sedulous.Input;
using Sedulous.Sdl2.Input;
using Sedulous.Sdl2.Native;
using static Sedulous.Sdl2.Native.SDLNative;

namespace Sedulous.Sdl2
{
    /// <summary>
    /// Represents the SDL2 implementation of the Sedulous Input subsystem.
    /// </summary>
    public sealed class Sdl2FrameworkInput : FrameworkResource, IInputSubsystem, IMessageSubscriber<FrameworkMessageId>
    {
        /// <summary>
        /// Initializes a new instance of the SDL2SedulousInput class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public Sdl2FrameworkInput(FrameworkContext context)
            : base(context)
        {
            this.softwareKeyboardService = context.GetFactoryMethod<SoftwareKeyboardServiceFactory>()();

            this.keyboard = new Sdl2KeyboardDevice(context);
            this.mouse = new Sdl2MouseDevice(context);
            this.gamePadInfo = new GamePadDeviceInfo(context);
            this.gamePadInfo.GamePadConnected += OnGamePadConnected;
            this.gamePadInfo.GamePadDisconnected += OnGamePadDisconnected;
            this.touchInfo = new TouchDeviceInfo(context);

            LoadGameControllerMappingDatabase(context);

            context.Messages.Subscribe(this,
                FrameworkMessages.TextInputRegionChanged);
        }

        /// <inheritdoc/>
        void IMessageSubscriber<FrameworkMessageId>.ReceiveMessage(FrameworkMessageId type, MessageData data)
        {
            if (type == FrameworkMessages.TextInputRegionChanged)
            {
                unsafe
                {
                    var region = softwareKeyboardService.TextInputRegion;
                    if (region.HasValue)
                    {
                        var rect = new SDL_Rect()
                        {
                            x = region.Value.X,
                            y = region.Value.Y,
                            w = region.Value.Width,
                            h = region.Value.Height,
                        };
                        SDL_SetTextInputRect(&rect);
                    }
                    else
                    {
                        SDL_SetTextInputRect(null);
                    }
                }
            }
        }

        /// <summary>
        /// Resets the device's state in preparation for the next frame.
        /// </summary>
        public void ResetDeviceStates()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            this.keyboard.ResetDeviceState();
            this.mouse.ResetDeviceState();
            this.gamePadInfo.ResetDeviceStates();
            this.touchInfo.ResetDeviceStates();
        }

        /// <inheritdoc/>
        public void Update(FrameworkTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            this.keyboard.Update(time);
            this.mouse.Update(time);
            this.gamePadInfo.Update(time);
            this.touchInfo.Update(time);

            Updating?.Invoke(this, time);
        }

        /// <inheritdoc/>
        public void ShowSoftwareKeyboard()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            softwareKeyboardService.ShowSoftwareKeyboard(KeyboardMode.Text);
        }

        /// <inheritdoc/>
        public void ShowSoftwareKeyboard(KeyboardMode mode)
        {
            Contract.EnsureNotDisposed(this, Disposed);
            
            softwareKeyboardService.ShowSoftwareKeyboard(mode);
        }

        /// <inheritdoc/>
        public void HideSoftwareKeyboard()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            softwareKeyboardService.HideSoftwareKeyboard();
        }

        /// <inheritdoc/>
        public Boolean IsKeyboardSupported()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return true;
        }

        /// <inheritdoc/>
        public Boolean IsKeyboardRegistered()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return keyboard.IsRegistered;
        }

        /// <inheritdoc/>
        public KeyboardDevice GetKeyboard()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return keyboard;
        }

        /// <inheritdoc/>
        public Boolean IsMouseSupported()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return true;
        }

        /// <inheritdoc/>
        public Boolean IsMouseRegistered()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return mouse.IsRegistered;
        }

        /// <inheritdoc/>
        public MouseDevice GetMouse()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return mouse;
        }

        /// <inheritdoc/>
        public Int32 GetGamePadCount()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return gamePadInfo.Count;
        }

        /// <inheritdoc/>
        public Boolean IsGamePadSupported()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return true;
        }

        /// <inheritdoc/>
        public Boolean IsGamePadConnected()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return gamePadInfo.Count > 0;
        }

        /// <inheritdoc/>
        public Boolean IsGamePadConnected(Int32 playerIndex)
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.EnsureRange(playerIndex >= 0, nameof(playerIndex));

            return gamePadInfo.GetGamePadForPlayer(playerIndex) != null;
        }
        
        /// <inheritdoc/>
        public Boolean IsGamePadRegistered()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            for (int i = 0; i < gamePadInfo.Count; i++)
            {
                if (gamePadInfo.GetGamePadForPlayer(i)?.IsRegistered ?? false)
                    return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public Boolean IsGamePadRegistered(Int32 playerIndex)
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.EnsureRange(playerIndex >= 0, nameof(playerIndex));

            return gamePadInfo.GetGamePadForPlayer(playerIndex)?.IsRegistered ?? false;
        }

        /// <inheritdoc/>
        public GamePadDevice GetGamePadForPlayer(Int32 playerIndex)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return gamePadInfo.GetGamePadForPlayer(playerIndex);
        }

        /// <inheritdoc/>
        public GamePadDevice GetFirstConnectedGamePad()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return gamePadInfo.GetFirstConnectedGamePad();
        }

        /// <inheritdoc/>
        public GamePadDevice GetFirstRegisteredGamePad()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return gamePadInfo.GetFirstRegisteredGamePad();
        }

        /// <inheritdoc/>
        public GamePadDevice GetPrimaryGamePad()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return primaryGamePad;
        }

        /// <inheritdoc/>
        public Boolean IsTouchSupported()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return touchInfo.Count > 0;
        }

        /// <inheritdoc/>
        public Boolean IsTouchDeviceConnected()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return touchInfo.Count > 0;
        }

        /// <inheritdoc/>
        public Boolean IsTouchDeviceConnected(Int32 index)
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.EnsureRange(index >= 0, nameof(index));

            return touchInfo.Count > index;
        }

        /// <inheritdoc/>
        public Boolean IsTouchDeviceRegistered()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            for (int i = 0; i < touchInfo.Count; i++)
            {
                if (touchInfo.GetTouchDeviceByIndex(i).IsRegistered)
                    return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public Boolean IsTouchDeviceRegistered(Int32 index)
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.EnsureRange(index >= 0, nameof(index));

            return touchInfo.GetTouchDeviceByIndex(index).IsRegistered;
        }
        
        /// <inheritdoc/>
        public TouchDevice GetTouchDevice(Int32 index)
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.EnsureRange(index >= 0, nameof(index));

            return index >= touchInfo.Count ? null : touchInfo.GetTouchDeviceByIndex(index);
        }

        /// <inheritdoc/>
        public TouchDevice GetFirstConnectedTouchDevice()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return touchInfo.Count > 0 ? touchInfo.GetTouchDeviceByIndex(0) : null;
        }

        /// <inheritdoc/>
        public TouchDevice GetFirstRegisteredTouchDevice()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            for (int i = 0; i < touchInfo.Count; i++)
            {
                var device = touchInfo.GetTouchDeviceByIndex(i);
                if (device.IsRegistered)
                    return device;
            }

            return null;
        }

        /// <inheritdoc/>
        public TouchDevice GetPrimaryTouchDevice()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return primaryTouchDevice;
        }

        /// <inheritdoc/>
        public Boolean EmulateMouseWithTouchInput { get; set; } = true;

        /// <inheritdoc/>
        public Boolean IsMouseCursorAvailable => mouse.IsRegistered || (EmulateMouseWithTouchInput && IsTouchDeviceRegistered());

        /// <inheritdoc/>
        public event FrameworkSubsystemUpdateEventHandler Updating;

        /// <inheritdoc/>
        public event KeyboardRegistrationEventHandler KeyboardRegistered;

        /// <inheritdoc/>
        public event MouseRegistrationEventHandler MouseRegistered;

        /// <inheritdoc/>
        public event GamePadConnectionEventHandler GamePadConnected;

        /// <inheritdoc/>
        public event GamePadConnectionEventHandler GamePadDisconnected;

        /// <inheritdoc/>
        public event GamePadRegistrationEventHandler GamePadRegistered;

        /// <inheritdoc/>
        public event TouchDeviceRegistrationEventHandler TouchDeviceRegistered;

        /// <summary>
        /// Registers the specified device as having received user input.
        /// </summary>
        /// <param name="device">The device to register.</param>
        /// <returns><see langword="true"/> if the device was registered; otherwise, <see langword="false"/>.</returns>
        internal Boolean RegisterKeyboardDevice(Sdl2KeyboardDevice device)
        {
            if (device.IsRegistered)
                return false;

            KeyboardRegistered?.Invoke(device);
            return true;
        }

        /// <summary>
        /// Registers the specified device as having received user input.
        /// </summary>
        /// <param name="device">The device to register.</param>
        /// <returns><see langword="true"/> if the device was registered; otherwise, <see langword="false"/>.</returns>
        internal Boolean RegisterMouseDevice(Sdl2MouseDevice device)
        {
            if (device.IsRegistered)
                return false;

            MouseRegistered?.Invoke(device);            
            return true;
        }

        /// <summary>
        /// Registers the specified device as having received user input.
        /// </summary>
        /// <param name="device">The device to register.</param>
        /// <returns><see langword="true"/> if the device was registered; otherwise, <see langword="false"/>.</returns>
        internal Boolean RegisterGamePadDevice(Sdl2GamePadDevice device)
        {
            if (primaryGamePad == null)
                primaryGamePad = device;

            if (device.IsRegistered)
                return false;

            GamePadRegistered?.Invoke(device, device.PlayerIndex);
            return true;
        }

        /// <summary>
        /// Registers the specified device as having received user input.
        /// </summary>
        /// <param name="device">The device to register.</param>
        /// <returns><see langword="true"/> if the device was registered; otherwise, <see langword="false"/>.</returns>
        internal Boolean RegisterTouchDevice(Sdl2TouchDevice device)
        {
            if (primaryTouchDevice == null)
                primaryTouchDevice = device;

            if (device.IsRegistered)
                return false;

            TouchDeviceRegistered?.Invoke(device);
            return true;
        }

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                if (FrameworkContext != null && !FrameworkContext.Disposed)
                    FrameworkContext.Messages.Unsubscribe(this);

                SafeDispose.DisposeRef(ref keyboard);
                SafeDispose.DisposeRef(ref mouse);
                SafeDispose.DisposeRef(ref gamePadInfo);
                SafeDispose.DisposeRef(ref touchInfo);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Attempts to load gamecontrollerdb.txt, if it is located in the application's root directory. 
        /// </summary>
        private void LoadGameControllerMappingDatabase(FrameworkContext context)
        {
            const string DatabasePath = "gamecontrollerdb.txt";

            var fss = new Sedulous.Platform.FileSystemService();
            if (fss.FileExists(DatabasePath))
            {
                using (var stream = fss.OpenRead(DatabasePath))
                using (var wrapper = new Sdl2StreamWrapper(stream))
                {
                    if (SDL_GameControllerAddMappingsFromRW(wrapper.ToIntPtr(), 0) < 0)
                        throw new Sdl2Exception();
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="GamePadConnected"/> event.
        /// </summary>
        /// <param name="device">The device that was connected.</param>
        /// <param name="playerIndex">The player index associated with the game pad.</param>
        private void OnGamePadConnected(GamePadDevice device, Int32 playerIndex)
        {
            GamePadConnected?.Invoke(device, playerIndex);
        }

        /// <summary>
        /// Raises the <see cref="GamePadDisconnected"/> event.
        /// </summary>
        /// <param name="device">The device that was disconnected.</param>
        /// <param name="playerIndex">The player index associated with the game pad.</param>
        private void OnGamePadDisconnected(GamePadDevice device, Int32 playerIndex)
        {
            if (primaryGamePad == device)
                primaryGamePad = null;

            GamePadDisconnected?.Invoke(device, playerIndex);
        }
        
        // Platform services.
        private readonly SoftwareKeyboardService softwareKeyboardService;

        // Input devices.
        private Sdl2KeyboardDevice keyboard;
        private Sdl2MouseDevice mouse;
        private GamePadDeviceInfo gamePadInfo;
        private Sdl2GamePadDevice primaryGamePad;
        private TouchDeviceInfo touchInfo;
        private Sdl2TouchDevice primaryTouchDevice;        
    }
}
