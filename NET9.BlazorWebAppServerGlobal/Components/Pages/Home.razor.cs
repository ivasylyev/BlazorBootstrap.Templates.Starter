using Microsoft.AspNetCore.Components;
using NET9.BlazorWebAppServerGlobal.Services.Shared;

namespace NET9.BlazorWebAppServerGlobal.Components.Pages
{
    public partial class Home
    {
        [Inject]
        public ILogger<Home> Logger { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            Logger.LogInformation("Home page initializing");
            await Task.CompletedTask;
        }
    }
}
