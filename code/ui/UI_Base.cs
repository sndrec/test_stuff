using Sandbox.UI;
using Sandbox;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

//namespace ExampleUI;

[UseTemplate]
public class UI_Base : RootPanel
{
    public string UI_ScoreDigits { get; set; }
    public string UI_TimeSecond { get; set; }
    public string UI_TimeMili { get; set; }
    public string UI_LevelNumber { get; set; }
    public string UI_LevelName { get; set; }
    public double UI_ScaleFactor {get;set;}
    public float ScoreInterp {get;set;}
    public float TimeInSeconds {get;set;}
    public float Milliseconds {get;set;}

    public Panel UIE_HudScalableOne {get;set;}
    public Panel UIE_HudScalableTwo {get;set;}
    public Panel UIE_HudScalableThr {get;set;}
    public Panel UIE_HudScalableFou {get;set;}
    public Panel UIE_HudScalableFiv {get;set;}
    public Panel UIE_HudScalableSix {get;set;}
    public string ScaleString {get;set;}

	[Event.Frame]
	public void GetVars()
	{
		MyGame GameEnt = Game.Current as MyGame;
		Pawn Ball = Local.Client.Pawn as Pawn;

    	UI_ScaleFactor = 2.0/3; //REPLACE 2.0 WITH CONSOLE VARIABLE
    	ScaleString = "scale(" + UI_ScaleFactor.ToString() + ")";
	
    	UIE_HudScalableOne.Style.Set("transform",ScaleString);
    	UIE_HudScalableTwo.Style.Set("transform",ScaleString);
    	UIE_HudScalableThr.Style.Set("transform",ScaleString);
    	UIE_HudScalableFou.Style.Set("transform",ScaleString);
    	UIE_HudScalableFiv.Style.Set("transform",ScaleString);
    	UIE_HudScalableSix.Style.Set("transform",ScaleString);
    	//Log.Info(ScaleString);
    	Local.Client.SetInt("FakeScore", Local.Client.GetInt("FakeScore", 0) + 1);

		if (GameEnt.CurrentGameState != 2)
		{
			GameEnt.HasFirstHit = false;
		}
		if (Ball != null)
		{
			ScoreInterp = MathX.Lerp(ScoreInterp, Ball.OurManager.Score, Time.Delta * 10, true);
		}
		if (Ball != null && Ball.BallState == 0)
		{
			TimeInSeconds = (float)Math.Floor(GameEnt.StageMaxTime - (Time.Now - GameEnt.FirstHitTime));
			Milliseconds = (float)(1 - (Math.Floor(((Time.Now - GameEnt.FirstHitTime) % 1) * 1000) / 1000));
		}
		string MiliString = Milliseconds.ToString();
		if (MiliString.Length > 1)
		{
			MiliString = MiliString.Substring(1, MiliString.Length - 1);
		}
		if (MiliString.Length > 4)
		{
			MiliString = MiliString.Substring(0, 4);
		}
		if (MiliString.Length < 4)
		{
			MiliString = MiliString + "0";
		}
		if (TimeInSeconds < 0 | !GameEnt.HasFirstHit)
		{
			MiliString = ".000";
		}
		if (!GameEnt.HasFirstHit)
		{
			TimeInSeconds = GameEnt.StageMaxTime;
		}
		UI_ScoreDigits = Math.Ceiling(ScoreInterp).ToString();
		UI_TimeSecond = Math.Max(TimeInSeconds, 0).ToString();
		UI_TimeMili = MiliString;
		UI_LevelName = GameEnt.StageName;
		UI_LevelNumber = GameEnt.StageInCourse.ToString();
	}
}
