using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Routing;
using NET9.BlazorWebAppServerGlobal.Models;

namespace NET9.BlazorWebAppServerGlobal.Components.Layout;

public partial class HeaderMenu
{
    private List<MenuItem> MenuItems = new()
    {
        new() { Url = "/", Text = "Домашняя", Icon = IconName.HouseDoorFill },
        new() { Url = "/rates", Text = "Ставки", Icon = IconName.Table }
    };

    protected override void OnInitialized()
    {
        Nav.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        StateHasChanged(); // перерисовать меню, чтобы обновилась подсветка
    }

    private bool IsActive(string url)
    {
         var relative = "/" + Nav.ToBaseRelativePath(Nav.Uri).Trim('/');
         return relative == url;
    }

    public void Dispose()
    {
        Nav.LocationChanged -= OnLocationChanged;
    }
}