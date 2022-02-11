﻿// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.GUI;

using System;

public interface IGUISystem
{
    Screen OpenScreen(bool keepOldScreens = false);
    void CloseScreen(Screen screen);


    Window OpenWindow(Screen screen,
        int leftEdge = 0,
        int topEdge = 0,
        int width = 256,
        int height = 256,
        string title = "",
        int minWidth = 0,
        int minHeight = 0,
        bool borderless = false,
        bool backdrop = false,
        bool sizing = true,
        bool dragging = true,
        bool zooming = true,
        bool closing = true,
        bool depth = true);
    void CloseWindow(Window window);

    Gadget AddGadget(Window window,
        int leftEdge = 0,
        int topEdge = 0,
        int width = 100,
        int height = 50,
        GadgetFlags flags = GadgetFlags.None,
        GadgetActivation activation = GadgetActivation.Immediate | GadgetActivation.RelVerify,
        GadgetType type = GadgetType.BoolGadget,
        string? text = null,
        Icons icon = Icons.NONE,
        bool disabled = false,
        bool selected = false,
        bool toggleSelect = false,
        Action? clickAction = null,
        string? buffer = null,
        int gadgetId = -1);

    void RemoveGadget(Window window, Gadget gadget);

    void WindowToFront(Window window);
    void WindowToBack(Window window);
    void ActivateWindow(Window window);

    void ModifyProp(Gadget gadget, PropFlags flags, int horizPot, int vertPot, int horizBody, int vertBody);
}
