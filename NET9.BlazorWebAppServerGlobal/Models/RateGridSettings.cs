namespace NET9.BlazorWebAppServerGlobal.Models;

public class RateGridSettings(Dictionary<string, GridColumnSetting<RateDto>> columnSettings) 
{
    public RateGridSettings() 
        : this(new Dictionary<string, GridColumnSetting<RateDto>>())
    {
    }

    public Dictionary<string, GridColumnSetting<RateDto>> ColumnSettings { get; set; } = columnSettings;
}