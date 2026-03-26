using Blazored.LocalStorage;
using NET9.BlazorWebAppServerGlobal.Models;

namespace NET9.BlazorWebAppServerGlobal.Services;
public abstract class GridSettingsServiceBase<T>(ILocalStorageService localStorage) : IGridSettingsService<T>
{
    protected readonly ILocalStorageService LocalStorage = localStorage;

    protected abstract string StorageKey { get; }

    /// <summary>
    /// Должен вернуть полный набор колонок (дефолт)
    /// </summary>
    protected abstract List<GridColumnSetting<T>> GetDefaultSettings();

    public async Task<GridSettings<T>> GetGridSettingsAsync()
    {
        var defaultSettings = GetDefaultSettings();

        var loaded = await LocalStorage
                         .GetItemAsync<Dictionary<string, bool>>(StorageKey)
                     ?? new Dictionary<string, bool>();

        foreach (var column in defaultSettings)
        {
            if (loaded.TryGetValue(column.Name, out var visible))
            {
                column.Visible = visible;
            }
        }

        return new GridSettings<T>(defaultSettings);
    }

    public async Task SaveGridSettingsAsync(GridSettings<T> settings)
    {
        var visibility = settings.ColumnSettings
            .ToDictionary(x => x.Name, x => x.Visible);

        await LocalStorage.SetItemAsync(StorageKey, visibility);
    }

    public async Task ResetGridSettingsAsync()
    {
        await LocalStorage.RemoveItemAsync(StorageKey);
    }
}