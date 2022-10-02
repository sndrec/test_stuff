using Sandbox.UI;

//namespace ExampleUI;

[UseTemplate]
public class UI_Base : RootPanel
{
    public string UI_ScoreDigits { get; set; }
    public string UI_TimeSecond { get; set; }
    public string UI_TimeMili { get; set; }
    public string UI_LevelNumber { get; set; }
    public string UI_LevelName { get; set; }
}
