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
    public float ScoreInterp {get;set;}
    public float TimeInSeconds {get;set;}
    public float Milliseconds {get;set;}

	[Event.Frame]
	public void GetVars()
	{
		MyGame GameEnt = Game.Current as MyGame;
		Pawn Ball = Local.Client.Pawn as Pawn;
		if (GameEnt.CurrentGameState != 2)
		{
			GameEnt.HasFirstHit = false;
		}
		ScoreInterp = MathX.Lerp(ScoreInterp, GameEnt.Score, Time.Delta * 10, true);
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
