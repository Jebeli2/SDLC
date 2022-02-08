﻿namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLObjectTracker<T> : IDisposable where T : SDLObject
    {
        private readonly string name;
        private readonly LogCategory category;
        private readonly Dictionary<string, T> tracked = new();
        private bool disposedValue;

        public SDLObjectTracker(LogCategory category, string name)
        {
            this.category = category;
            this.name = name;
        }

        public string Name => name;
        public void Track(T obj)
        {
            if (tracked.ContainsKey(obj.Name))
            {
                SDLLog.Warn(category, $"Tracking {name} '{obj.Name}' again");
            }
            tracked[obj.Name] = obj;
        }

        public void Untrack(T obj)
        {
            if (tracked.Remove(obj.Name))
            {
                SDLLog.Info(category, $"{name} '{obj.Name}' removed");
            }
            else
            {
                SDLLog.Error(category, $"Tried to remove untracked {name} '{obj.Name}'");
            }
        }

        private void ClearTrackedObjects()
        {
            if (tracked.Count > 0)
            {
                if (tracked.Count == 1)
                {
                    SDLLog.Warn(category, $"Clearing {tracked.Count} leaked {name}");
                }
                else
                {
                    SDLLog.Warn(category, $"Clearing {tracked.Count} leaked {name}s");
                }
                List<T> objs = tracked.Values.ToList();
                foreach (T obj in objs)
                {
                    obj.Dispose();
                }
                tracked.Clear();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }
                ClearTrackedObjects();
                disposedValue = true;
            }
        }

        ~SDLObjectTracker()
        {

            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
