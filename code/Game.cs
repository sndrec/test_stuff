using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace Sandbox;



public partial class MyGame : Sandbox.Game
{	
	[Net]
	public Vector3 CurrentSpawnPos {get;set;}

	[Net]
	public Angles CurrentSpawnRot {get;set;}
	
	[Net]
	public int CurrentGameState {get;set;}

	[Net]
	public float LastGameStateChange {get;set;}

	[Net]
	public string CurrentSky{get;set;}

	[Net]
	public float BGScale {get;set;}

	[Net]
	public string DesiredSong {get;set;}

	[Net]
	public float NextGameState {get;set;}

	[Net]
	public float StageMaxTime {get;set;}

	[Net]
	public string StageName {get;set;}

	[Net]
	public int StageInCourse {get;set;}

	public bool HasFirstHit {get;set;}

	public float FirstHitTime {get;set;}

	public SMBStage CurrentStage {get;set;}

	public string PlayingSong {get;set;}

	public Sound GameMusic {get;set;}

	public int CurrentCourse {get;set;}

	public SceneWorld BGWorld {get;set;}

	public Rotation StageTilt {get;set;}

	public bool FirstFrame {get;set;}

	public int Score{get;set;}

	public MyGame()
	{
		Global.TickRate = 60;
		NextGameState = Time.Now + 30;
		CurrentCourse = 1;
		FirstFrame = false;
		if ( IsClient )
    	{
			_ = new UI_Base();
    	}
	}

	//states:
	//0 = just loaded the map.
	//1 = waiting for players...
	//2 = let's play!
	//3 = moving onto the next stage...
	//4 = game over.

	public void PlayGlobalSound(string InLine)
	{	
		Sound Line = Sound.FromWorld(InLine, Vector3.Zero);
	}

	public Vector3 BlenderPos(float x, float y, float z)
	{
		return new Vector3(y * -20, x * 20, z * 20);
	}

	public void FromEntityToAllBut(Client Avoid, string InSound, Entity InEnt)
	{	
		foreach (Client pl in Client.All)
		{
			if (pl != Avoid)
			{

				Sound Line = Sound.FromEntity(To.Single(pl), InSound, InEnt);
			}
		}
	}

	public void DestroyCurrentSMBStage()
	{
		CurrentStage.DestroyStage();
	}

	public virtual void PlayNextStageInCourse(int InCourse)
	{
		switch (InCourse)
		{
			case 1:
				course_w1.PlayNextStage();
				break;
			default:
				course_w1.PlayNextStage();
				break;
		}
	}

	public async void PlayGlobalSoundDelayed(string InLine, float InDelay)
	{
		await Task.DelaySeconds(InDelay);
		Sound Line = Sound.FromWorld(InLine, Vector3.Zero);
	}

	public async void SpawnAllBallsDelayed(float InDelayTime)
	{
		await Task.DelaySeconds(0.1f);
		foreach (Client pl in Client.All)
		{
			if (pl.Pawn != null)
			{
				pl.Pawn.Delete();
			}
			var pawn = new Pawn();
			pl.Pawn = pawn;
		}
	}

	public void SpawnAllSpectators()
	{
		foreach (Client pl in Client.All)
		{
			if (pl.Pawn is Pawn)
			{
				pl.Pawn.Delete();
				var pawn = new SpectatorPawn();
				pl.Pawn = pawn;
			}
		}
		foreach (Bot pl in Bot.All)
		{
			if (pl.Client.Pawn is Pawn)
			{
				pl.Client.Pawn.Delete();
				var pawn = new SpectatorPawn();
				pl.Client.Pawn = pawn;
			}
		}
	}

	public void ChangeGameState(int inState)
	{
		CurrentGameState = inState;
		LastGameStateChange = Time.Now;
		if (inState == 0)
		{
			foreach (Client pl in Client.All)
			{
				pl.Pawn.Delete();
				var pawn = new SpectatorPawn();
				pl.Pawn = pawn;
			}
			DestroyCurrentSMBStage();
			stwfp.CreateStage();
			SkyGenerator.CreateBackground(CurrentSky);
			NextGameState = Time.Now + 6;
			SpawnAllBallsDelayed(0.1f);
		}else
		if (inState == 1)
		{
			SpawnAllSpectators();
			NextGameState = Time.Now + 2;
		}else
		if (inState == 2)
		{
			DestroyCurrentSMBStage();
			PlayNextStageInCourse(CurrentCourse);
			NextGameState = Time.Now + (StageMaxTime + 5.6f);
			SkyGenerator.CreateBackground(CurrentSky);
			SpawnAllBallsDelayed(0.1f);
		}else
		if (inState == 3)
		{
			SpawnAllSpectators();
			NextGameState = Time.Now + 0.5f;
		}else
		if (inState == 4)
		{
			foreach (Client pl in Client.All)
			{
				pl.Pawn.Delete();
				var pawn = new SpectatorPawn();
				pl.Pawn = pawn;
			}
			DestroyCurrentSMBStage();
			stwfp.CreateStage();
			NextGameState = Time.Now + 10;
		}
	}

	public void EndCourse()
	{
		ChangeGameState(4);
	}

	[Event.Entity.PostSpawn]
	public void StartGame()
	{
		ChangeGameState(0);
		ToneMappingEntity Tonemapper = new ToneMappingEntity();
		Tonemapper.Enabled = true;
		Tonemapper.MaxExposure = 1;
		Tonemapper.MinExposure = 1;
		Tonemapper.Enable();
	}

	[Event.Tick.Server]
	public void ServerTick()
	{
		if (Time.Now > NextGameState)
		{
			if (CurrentGameState == 0)
			{
				ChangeGameState(1);
			}else
			if (CurrentGameState == 1)
			{
				ChangeGameState(2);
			}else
			if (CurrentGameState == 2)
			{
				ChangeGameState(3);
			}else
			if (CurrentGameState == 3)
			{
				ChangeGameState(2);
			}
			if (CurrentGameState == 4)
			{
				ChangeGameState(1);
			}
		}

		if (CurrentGameState == 2 && Time.Now > LastGameStateChange + 2)
		{
			bool SkipStage = true;
			foreach ( Client pl in Client.All )
			{
				if (pl.Pawn is Pawn)
				{
					SkipStage = false;
				}
			}
			if ( SkipStage )
			{
				ChangeGameState(3);
			}
		}
		//Log.Info(NextGameState - Time.Now);
	}

	[Event.Tick.Client]
	public void ManageGameMusic()
	{
		if (PlayingSong != DesiredSong)
		{
			PlayingSong = DesiredSong;
			GameMusic.Stop();
			GameMusic = Sound.FromWorld(PlayingSong, new Vector3(0,0,0));
		}
		else
		{
			if (GameMusic.Finished)
			{
				GameMusic.Stop();
				GameMusic = Sound.FromWorld(PlayingSong, new Vector3(0,0,0));
			}
		}
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		if (CurrentGameState == 1 | CurrentGameState == 3)
		{
			var pawn = new SpectatorPawn();
			client.Pawn = pawn;
		}else
		if (CurrentGameState == 0 | CurrentGameState == 2)
		{
			var pawn = new Pawn();
			client.Pawn = pawn;
		}
	}

	public Transform StageTiltTransform(Transform InTransform)
	{
		if (!(Local.Pawn is Pawn))
		{
			return InTransform;
		}
		Pawn OurBall = Local.Pawn as Pawn;
		Vector3 Pivot = OurBall.ClientPosition;
		Vector3 Dir = InTransform.Position - Pivot;
		Dir = StageTilt * Dir;
		Vector3 NewPosition = Dir + Pivot;
		Rotation NewRotation = StageTilt * InTransform.Rotation;
		return new Transform(NewPosition, NewRotation, InTransform.Scale);
	}

	[Event.PreRender]
	public void HandleStageTilt()
	{
		if (!FirstFrame)
		{
			StageTilt = Rotation.Identity;
			FirstFrame = true;
		}
		foreach (Entity element in Entity.All)
		{
			if (!element.Tags.Has("BGObject"))
			{
				continue;
			}
			if (element is ModelEntity)
			{
				ModelEntity Ent = element as ModelEntity;
				SceneObject CurObj = Ent.SceneObject;
				if (CurObj != null)
				{
					CurObj.Transform = StageTiltTransform(new Transform(Ent.Position, Ent.Rotation, Ent.Scale));
				}
			}
		}
	}
}
