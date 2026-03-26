using NET9.BlazorWebAppServerGlobal.Models.Grid;

namespace NET9.BlazorWebAppServerGlobal.Services.Shared;

public interface IGridSettingsService<T>
{
    public Task<GridSettings<T>> GetGridSettingsAsync();
    public Task SaveGridSettingsAsync(GridSettings<T> settings);
    public Task ResetGridSettingsAsync();
}