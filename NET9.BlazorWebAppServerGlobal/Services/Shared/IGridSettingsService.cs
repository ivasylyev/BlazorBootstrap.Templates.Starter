using NET9.BlazorWebAppServerGlobal.Models.Grid;
using NET9.BlazorWebAppServerGlobal.Models.Page;

namespace NET9.BlazorWebAppServerGlobal.Services.Shared;

public interface IGridSettingsService<T>
{
    public PageSettings GetPageSettings();
    public Task<GridSettings<T>> GetGridSettingsAsync();
    public Task SaveGridSettingsAsync(GridSettings<T> settings);
    public Task ResetGridSettingsAsync();
    
}