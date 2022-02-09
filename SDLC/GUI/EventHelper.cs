namespace SDLC.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal static class EventHelper
    {
        public static void Raise<T>(object sender, EventHandler<T>? handler, T e) where T : EventArgs
        {
            handler?.Invoke(sender, e);
        }

        public static int GetExpirationTime(double time, int millis)
        {
            return (int)(time + millis);
        }

        public static bool HasExpired(double time, int millis)
        {
            return time > millis;
        }
    }
}
