using System;

namespace NET9.BlazorWebAppServerGlobal.Services.Shared
{
    public class PageTimingService
    {
        public TimeSpan LastRatesLoadDuration { get; private set; } = TimeSpan.Zero;

        public void SetLastRatesLoadDuration(TimeSpan duration)
        {
            LastRatesLoadDuration = duration;
            NotifyStateChanged();
        }

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}