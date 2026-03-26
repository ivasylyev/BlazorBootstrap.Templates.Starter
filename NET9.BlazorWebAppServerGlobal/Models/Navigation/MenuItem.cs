using BlazorBootstrap;

namespace NET9.BlazorWebAppServerGlobal.Models.Navigation;

public class MenuItem
{
    public string Url { get; set; } = "/";
    public string Text { get; set; } = "";
    public IconName Icon { get; set; }
}