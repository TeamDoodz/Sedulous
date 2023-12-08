using System;
using System.Security;
using System.Runtime.InteropServices;

namespace Sedulous.Sdl2.Native
{
#pragma warning disable 1591
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate Int32 SDL_EventFilter(IntPtr userdata, SDL_Event* @event);
    
    [SuppressUnmanagedCodeSecurity]
    public abstract unsafe class SDLNativeImpl
    {
        public abstract String SDL_GetError();
        public abstract void SDL_ClearError();
        public abstract Int32 SDL_Init(SDL_Init flags);
        public abstract void SDL_Quit();
        public abstract void SDL_PumpEvents();
        public abstract Int32 SDL_PollEvent(out SDL_Event @event);
        public abstract void SDL_SetEventFilter(IntPtr filter, IntPtr userdata);
        public abstract IntPtr SDL_CreateWindow(String title, Int32 x, Int32 y, Int32 w, Int32 h, SDL_WindowFlags flags);
        public abstract IntPtr SDL_CreateWindowFrom(IntPtr data);
        public abstract void SDL_DestroyWindow(IntPtr window);
        public abstract UInt32 SDL_GetWindowID(IntPtr window);
        public abstract String SDL_GetWindowTitle(IntPtr window);
        public abstract void SDL_SetWindowTitle(IntPtr window, String title);
        public abstract void SDL_SetWindowIcon(IntPtr window, IntPtr icon);
        public abstract void SDL_GetWindowPosition(IntPtr window, out Int32 x, out Int32 y);
        public abstract void SDL_SetWindowPosition(IntPtr window, Int32 x, Int32 y);
        public abstract void SDL_GetWindowSize(IntPtr window, out Int32 w, out Int32 h);
        public abstract void SDL_SetWindowSize(IntPtr window, Int32 w, Int32 h);
        public abstract void SDL_GetWindowMinimumSize(IntPtr window, out Int32 w, out Int32 h);
        public abstract void SDL_SetWindowMinimumSize(IntPtr window, Int32 w, Int32 h);
        public abstract void SDL_GetWindowMaximumSize(IntPtr window, out Int32 w, out Int32 h);
        public abstract void SDL_SetWindowMaximumSize(IntPtr window, Int32 w, Int32 h);
        public abstract Boolean SDL_GetWindowGrab(IntPtr window);
        public abstract void SDL_SetWindowGrab(IntPtr window, Boolean grabbed);
        public abstract Int32 SDL_SetWindowBordered(IntPtr window, Boolean bordered);
        public abstract Int32 SDL_SetWindowFullscreen(IntPtr window, UInt32 flags);
        public abstract Int32 SDL_SetWindowDisplayMode(IntPtr window, SDL_DisplayMode* mode);
        public abstract Int32 SDL_GetWindowDisplayMode(IntPtr window, SDL_DisplayMode* mode);
        public abstract Int32 SDL_GetWindowDisplayIndex(IntPtr window);
        public abstract SDL_WindowFlags SDL_GetWindowFlags(IntPtr window);
        public abstract void SDL_ShowWindow(IntPtr window);
        public abstract void SDL_HideWindow(IntPtr window);
        public abstract void SDL_MaximizeWindow(IntPtr window);
        public abstract void SDL_MinimizeWindow(IntPtr window);
        public abstract void SDL_RestoreWindow(IntPtr window);
        public abstract Boolean SDL_GetWindowWMInfo(IntPtr window, SDL_SysWMinfo* info);
        public abstract IntPtr SDL_RWFromFile(String file, String mode);
        public abstract IntPtr SDL_RWFromMem(IntPtr mem, Int32 size);
        public abstract IntPtr SDL_AllocRW();
        public abstract void SDL_FreeRW(IntPtr area);
        public abstract SDL_Surface* SDL_LoadBMP_RW(IntPtr src, Int32 freesrc);
        public abstract Int32 SDL_SaveBMP_RW(SDL_Surface* surface, IntPtr dst, Int32 freedst);
        public abstract UInt32 SDL_GetMouseState(out Int32 x, out Int32 y);
        public abstract IntPtr SDL_GetKeyboardState(out Int32 numkeys);
        public abstract SDL_Scancode SDL_GetScancodeFromKey(SDL_Keycode keycode);
        public abstract SDL_Keymod SDL_GetModState();
        public abstract Boolean SDL_SetHint(String name, String value);
        public abstract SDL_Surface* SDL_CreateRGBSurface(UInt32 flags, Int32 width, Int32 height, Int32 depth, UInt32 Rmask, UInt32 Gmask, UInt32 Bmask, UInt32 AMask);
        public abstract void SDL_FreeSurface(SDL_Surface* surface);
        public abstract Int32 SDL_LockSurface(SDL_Surface* surface);
        public abstract void SDL_UnlockSurface(SDL_Surface* surface);
        public abstract Int32 SDL_BlitSurface(SDL_Surface* src, SDL_Rect* srcrect, SDL_Surface* dst, SDL_Rect* dstrect);
        public abstract Int32 SDL_BlitScaled(SDL_Surface* src, SDL_Rect* srcrect, SDL_Surface* dst, SDL_Rect* dstrect);
        public abstract Int32 SDL_SetSurfaceBlendMode(SDL_Surface* surface, SDL_BlendMode blendMode);
        public abstract Int32 SDL_GetSurfaceBlendMode(SDL_Surface* surface, SDL_BlendMode* blendMode);
        public abstract Int32 SDL_FillRect(SDL_Surface* surface, SDL_Rect* rect, UInt32 color);
        public abstract Int32 SDL_FillRects(SDL_Surface* dst, SDL_Rect* rects, Int32 count, UInt32 colors);
        public abstract SDL_Cursor* SDL_CreateColorCursor(SDL_Surface* surface, Int32 hot_x, Int32 hot_y);
        public abstract void SDL_FreeCursor(SDL_Cursor* cursor);
        public abstract Int32 SDL_ShowCursor(Int32 toggle);
        public abstract SDL_Cursor* SDL_GetCursor();
        public abstract void SDL_SetCursor(SDL_Cursor* cursor);
        public abstract SDL_Cursor* SDL_GetDefaultCursor();
        public abstract Int32 SDL_GetNumVideoDisplays();
        public abstract String SDL_GetDisplayName(Int32 displayIndex);
        public abstract Int32 SDL_GetDisplayBounds(Int32 displayIndex, SDL_Rect* rect);
        public abstract Int32 SDL_GetNumDisplayModes(Int32 displayIndex);
        public abstract Int32 SDL_GetDisplayMode(Int32 displayIndex, Int32 modeIndex, SDL_DisplayMode* mode);
        public abstract Int32 SDL_GetCurrentDisplayMode(Int32 displayIndex, SDL_DisplayMode* mode);
        public abstract Int32 SDL_GetDesktopDisplayMode(Int32 displayIndex, SDL_DisplayMode* mode);
        public abstract SDL_DisplayMode* SDL_GetClosestDisplayMode(Int32 displayIndex, SDL_DisplayMode* mode, SDL_DisplayMode* closest);
        public abstract Boolean SDL_PixelFormatEnumToMasks(UInt32 format, Int32* bpp, UInt32* Rmask, UInt32* Gmask, UInt32* Bmask, UInt32* Amask);
        public abstract IntPtr SDL_GL_GetProcAddress(String proc);
        public abstract IntPtr SDL_GL_CreateContext(IntPtr window);
        public abstract void SDL_GL_DeleteContext(IntPtr context);
        public abstract IntPtr SDL_GL_GetCurrentContext(IntPtr context);
        public abstract Int32 SDL_GL_MakeCurrent(IntPtr window, IntPtr context);
        public abstract Int32 SDL_GL_SetAttribute(SDL_GLattr attr, Int32 value);
        public abstract Int32 SDL_GL_GetAttribute(SDL_GLattr attr, Int32* value);
        public abstract void SDL_GL_SwapWindow(IntPtr window);
        public abstract Int32 SDL_GL_SetSwapInterval(Int32 interval);
        public abstract void SDL_GL_GetDrawableSize(IntPtr window, out Int32 w, out Int32 h);
        public abstract Int32 SDL_NumJoysticks();
        public abstract Boolean SDL_IsGameController(Int32 joystick_index);
        public abstract IntPtr SDL_GameControllerOpen(Int32 index);
        public abstract void SDL_GameControllerClose(IntPtr gamecontroller);
        public abstract String SDL_GameControllerNameForIndex(Int32 joystick_index);
        public abstract Boolean SDL_GameControllerGetButton(IntPtr gamecontroller, SDL_GameControllerButton button);
        public abstract IntPtr SDL_GameControllerGetJoystick(IntPtr gamecontroller);
        public abstract Int32 SDL_JoystickInstanceID(IntPtr joystick);
        public abstract Int32 SDL_GetNumTouchDevices();
        public abstract Int64 SDL_GetTouchDevice(Int32 index);
        public abstract Int32 SDL_GetNumTouchFingers(Int64 touchID);
        public abstract SDL_Finger* SDL_GetTouchFinger(Int64 touchID, Int32 index);
        public abstract Int32 SDL_RecordGesture(Int64 touchID);
        public abstract Int32 SDL_SaveAllDollarTemplates(IntPtr dst);
        public abstract Int32 SDL_SaveDollarTemplate(Int64 gestureID, IntPtr dst);
        public abstract Int32 SDL_LoadDollarTemplates(Int64 touchID, IntPtr src);
        public abstract void SDL_StartTextInput();
        public abstract void SDL_StopTextInput();
        public abstract void SDL_SetTextInputRect(SDL_Rect* rect);
        public abstract Boolean SDL_HasClipboardText();
        public abstract IntPtr SDL_GetClipboardText();
        public abstract void SDL_SetClipboardText(IntPtr text);
        public abstract SDL_PowerState SDL_GetPowerInfo(Int32* secs, Int32* pct);
        public abstract Int32 SDL_ShowSimpleMessageBox(UInt32 flags, String title, String message, IntPtr window);
        public abstract Int32 SDL_SetWindowOpacity(IntPtr window, Single opacity);
        public abstract Int32 SDL_GetWindowOpacity(IntPtr window, Single* opacity);
        public abstract Int32 SDL_GameControllerAddMapping(String mappingString);
        public abstract Int32 SDL_GameControllerAddMappingsFromRW(IntPtr rw, Int32 freerw);
        public abstract String SDL_GameControllerMapping(IntPtr gamecontroller);
        public abstract String SDL_GameControllerMappingForGUID(Guid guid);
        public abstract Guid SDL_JoystickGetGUID(String pchGUID);
        public abstract Int32 SDL_GetDisplayDPI(Int32 displayIndex, Single* ddpi, Single* hdpi, Single* vdpi);
        public abstract void SDL_free(IntPtr mem);
        public abstract Boolean SDL_GetRelativeMouseMode();
        public abstract Int32 SDL_SetRelativeMouseMode(Boolean enabled);
        public abstract void SDL_WarpMouseInWindow(IntPtr window, Int32 x, Int32 y);
    }
#pragma warning restore 1591
}
