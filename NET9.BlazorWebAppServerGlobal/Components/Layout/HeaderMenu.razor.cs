using BlazorBootstrap;

namespace NET9.BlazorWebAppServerGlobal.Components.Layout;

public partial class HeaderMenu
{

    public class MenuItem
    {
        public string Url { get; set; } = "/";
        public string Text { get; set; } = "";
        public IconName Icon { get; set; }
    }

    private List<MenuItem> MenuItems = new List<MenuItem>
    {
        new MenuItem { Url = "/", Text = "Домашняя", Icon = IconName.HouseDoorFill },
        new MenuItem { Url = "/rates", Text = "Ставки", Icon = IconName.Table }
    };

    protected override void OnInitialized()
    {
        Nav.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
    {
        StateHasChanged(); // перерисовать меню, чтобы обновилась подсветка
    }

    private bool IsActive(string url)
    {
        var relative = "/" + Nav.ToBaseRelativePath(Nav.Uri).TrimEnd('/');
        if (url == "/")
            return relative == "/" || string.IsNullOrEmpty(relative.Trim('/'));
        return relative.StartsWith(url.TrimEnd('/'));

        // var relative = "/" + Nav.ToBaseRelativePath(Nav.Uri).Trim('/');
        // return relative == url;
    }

    public void Dispose()
    {
        Nav.LocationChanged -= OnLocationChanged;
    }
}