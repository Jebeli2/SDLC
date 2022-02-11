// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
public class SDLTextInputEventArgs : SDLHandledEventArgs
{
    private readonly string text;
    public SDLTextInputEventArgs(string text)
    {
        this.text = text;
    }

    public string Text => text;

}

public delegate void SDLTextInputEventHandler(object sender, SDLTextInputEventArgs e);
