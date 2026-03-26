using NET9.BlazorWebAppServerGlobal.Models;

namespace NET9.BlazorWebAppServerGlobal.Services;

public interface IGridSettingsService<T>
{
    public Task<GridSettings<T>> GetGridSettingsAsync();
    public Task SaveGridSettingsAsync(GridSettings<T> settings);
    public Task ResetGridSettingsAsync();
}