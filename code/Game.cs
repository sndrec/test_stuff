using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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

	[Net]
	public BBox StageBounds {get;set;}

	[Net]
	public float FalloutHeight { get; set; } = 0;

	[ConVar.ClientData( "smb_stagetilteffect_maxangle" )]
	public float BallMaxVisualTilt {get;set;} = 13.2f;

	[ConVar.ClientData( "smb_manualcamera" )]
	public bool ManualCamera {get;set;} = false;

	[ConVar.Replicated("smb_playercollision")]
	public bool PlayerCollision {get;set;} = true;

	[ConVar.Replicated("smb_ball_restitution")]
	public float BallRestitution {get;set;} = 0.5f;

	[ConVar.Replicated("smb_ball_friction")]
	public float BallFriction {get;set;} = 0.54f;

	[ConVar.Replicated("smb_ball_gravity")]
	public float BallGravity {get;set;} = 588f;

	[ConVar.Replicated("smb_ball_gravity_tilt")]
	public float BallGravityTilt {get;set;} = 23f;

	public bool AllPlayersReady {get;set;} = false;

	public bool HasFirstHit {get;set;}

	public float FirstHitTime {get;set;}

	public SMBStage CurrentStage {get;set;}

	public string PlayingSong {get;set;}

	public List<Sound> GameMusic {get;set;}

	[Net]
	public int CurrentCourse {get;set;}

	public SceneWorld BGWorld {get;set;}

	public Rotation StageTilt {get;set;}

	public bool FirstFrame {get;set;}

	public Rotation LightAngles {get;set;}

	public bool HasLightEnvironment {get;set;} = false;


	public MyGame()
	{
		Global.TickRate = 60;
		NextGameState = Time.Now + 30;
		CurrentCourse = 1;
		FirstFrame = false;
		GameMusic = new List<Sound>{};
		StageBounds = new BBox(Vector3.Zero, Vector3.Zero);
		if ( IsClient )
    	{
			UI_Base UIBase = new UI_Base();
			//UIBase.AddChild<VoiceList>();
			//UIBase.AddChild<VoiceSpeaker>();
		}
	}

	public void SetTimescale(float InTimescale)
	{
		Global.TimeScale = InTimescale;
	}
	public async void SetTimescaleDelayed(float InDelay, float InTimescale )
	{
		await Task.DelaySeconds( InDelay );
		Global.TimeScale = InTimescale;
	}

	public Vector3 ApplyCollisionResponseGeneric(Vector3 InVelocity, Vector3 HitNormal, Entity HitEntity, Vector3 HitPosition, float RealDelta, bool DoParticlesAndSounds)
	{
		SMBObject HitEntitySMB = HitEntity as SMBObject;
		Vector3 FinalVel = InVelocity;
		Vector3 RelativeVel = InVelocity;
		Vector3 VelAtPos = new Vector3(0,0,0);
		if (HitEntitySMB != null)
		{
			VelAtPos = HitEntitySMB.GetVelocityAtPoint(HitPosition, Global.TickInterval);
			RelativeVel = InVelocity - VelAtPos;
		}
		float RelativeComponent = Vector3.Dot(RelativeVel, HitNormal);
		if (DoParticlesAndSounds)
		{
			if (RelativeComponent > 0)
			{
				return InVelocity;
			}
			if (RelativeComponent < -240)
			{	
				Sound BumpSound = Sound.FromEntity("ball_impact_hard", this);
			}else
			if (RelativeComponent < -150)
			{	
				Sound BumpSound = Sound.FromEntity("ball_impact_medium", this);
			}else
			if (RelativeComponent < -100)
			{	
				Sound BumpSound = Sound.FromEntity("ball_impact_soft", this);
			}
		}

		RelativeComponent = Math.Abs(RelativeComponent);
		float ResultSpeed = RelativeComponent * 1f;
		float AddSpeed = RelativeComponent * 0.5f;
		AddSpeed = Math.Max(0, AddSpeed - 10);
		FinalVel += HitNormal * (ResultSpeed + AddSpeed);
		if (DoParticlesAndSounds)
		{
			if (RelativeComponent > 150)
			{
				//2.x = amount of particles
				//1 = position
				//0 = direction/power
				Particles ImpactParticle = Particles.Create("particles/collisionstars.vpcf");
				ImpactParticle.SetPosition(2, new Vector3(RelativeComponent / 64, 0, 0));
				ImpactParticle.SetPosition(1, HitPosition);
				ImpactParticle.SetPosition(0, (HitNormal * RelativeComponent * -0.125f) + (FinalVel * 0.75f));
				ImpactParticle.Simulating = true;
				ImpactParticle.EnableDrawing = true;
			}
		}

		return FinalVel;
	}

	public (Vector3, Vector3) TryBallCollisionDiscreteGenericServer(float BallRadius, Vector3 BallPosition, Vector3 BallVelocity, float RealDelta, bool DoCollisionResponse, bool DoEffects, string[] IgnoreTags)
	{
		DebugOverlay.Sphere(BallPosition, BallRadius, new Color(255,0,0), Time.Delta, false);
		Vector3 ModifiedPosition = BallPosition;
		Vector3 ModifiedVelocity = BallVelocity;
		for (int i = 0; i < 4; i++)
		{
			TraceResult MoveTrace = Trace.Sphere(BallRadius, ModifiedPosition, ModifiedPosition).WithTag("solid").WithoutTags(IgnoreTags).Run();
			if (MoveTrace.Hit)
			{	
				TraceResult DepenTrace;
				Vector3 UseNormal = MoveTrace.Normal;
				if (MoveTrace.StartedSolid)
				{
					DepenTrace = Trace.Ray(ModifiedPosition, MoveTrace.HitPosition).WithTag("solid").WithoutTags(IgnoreTags).Run();
					if (DepenTrace.Hit)
					{
						ModifiedPosition = DepenTrace.HitPosition + (MoveTrace.Normal * 10.001f);
						UseNormal = DepenTrace.Normal;
					}
				}
				TraceResult FinalTrace = Trace.Sphere(9, ModifiedPosition, ModifiedPosition - (MoveTrace.Normal * 2)).WithTag("solid").WithoutTags(IgnoreTags).Run();
				if (DoCollisionResponse)
				{
					ModifiedVelocity = ApplyCollisionResponseGeneric(ModifiedVelocity, FinalTrace.Normal, MoveTrace.Entity, MoveTrace.HitPosition, RealDelta, DoEffects);
				}
			}else
			{
				break;
			}
		}
		return (ModifiedPosition, ModifiedVelocity);
	}

	public (Vector3, Vector3) TryBallCollisionDiscreteGenericClient(float BallRadius, Vector3 BallPosition, Vector3 BallVelocity, float RealDelta, bool DoCollisionResponse, bool DoEffects)
	{
		for (int i = 0; i < 4; i++)
		{
			TraceResult MoveTrace = Trace.Sphere(BallRadius, BallPosition, BallPosition).WithTag("solid").WithoutTags("smbtrigger").IncludeClientside(true).Run();
			if (MoveTrace.Hit)
			{	
				TraceResult DepenTrace;
				Vector3 UseNormal = MoveTrace.Normal;
				if (MoveTrace.StartedSolid)
				{
					DepenTrace = Trace.Ray(BallPosition, MoveTrace.HitPosition).WithTag("solid").WithoutTags("smbtrigger").IncludeClientside(true).Run();
					if (DepenTrace.Hit)
					{
						BallPosition = DepenTrace.HitPosition + (MoveTrace.Normal * 10.001f);
						UseNormal = DepenTrace.Normal;
					}
				}
				TraceResult FinalTrace = Trace.Sphere(9, BallPosition, BallPosition - (MoveTrace.Normal * 2)).WithTag("solid").WithoutTags("smbtrigger").IncludeClientside(true).Run();
				if (DoCollisionResponse)
				{
					BallVelocity = ApplyCollisionResponseGeneric(BallVelocity, FinalTrace.Normal, MoveTrace.Entity, MoveTrace.HitPosition, RealDelta, DoEffects);
				}
			}else
			{
				break;
			}
		}
		return (BallPosition, BallVelocity);
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

	public Vector3 ConfigPos(float x, float y, float z)
	{
		return new Vector3(y * -0.2f, x * -0.2f, z * 0.2f);
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

	[ConCmd.Server]
	public static void SendKillfeedEntry(string lsteamid, string left, string right, string method)
	{
		MyGame GameEnt = Game.Current as MyGame;
		GameEnt.ReceiveKillfeedEntry(Convert.ToInt64(lsteamid), left, right, method);
	}

	[ClientRpc]
	public virtual void ReceiveKillfeedEntry(long lsteamid, string left, string right, string method)
	{
		KillFeed.Current?.AddEntry(lsteamid, left, right, method);
	}

	public void DestroyCurrentSMBStage()
	{
		if (CurrentStage != null)
		{
			CurrentStage.DestroyStage();
		}
	}

	public virtual void PlayNextStageInCourse(int InCourse)
	{
		SetTimescale( 1 );
		switch (InCourse)
		{
			case 1:
				course_w1.PlayNextStage();
				break;
			default:
				course_test.PlayNextStage();
				break;
		}
	}

	public virtual void PlaySpecificStageInCourse(int InCourse, int InIndex)
	{
		Log.Info( "Playing specific stage!" );
		Log.Info( "Course: " + InCourse );
		Log.Info( "Stage: " + InIndex );
		DestroyCurrentSMBStage();
		switch (InCourse)
		{
			case 1:
				course_w1.PlayDesiredStage(InIndex);
				break;
			default:
				course_test.PlayNextStage();
				break;
		}
		NextGameState = Time.Now + (StageMaxTime + 5.6f);
		SkyGenerator.CreateBackground( CurrentSky );
		LastGameStateChange = Time.Now;
	}

	public async void PlayGlobalSoundDelayed(string InLine, float InDelay)
	{
		await Task.DelaySeconds(InDelay);
		Sound Line = Sound.FromWorld(InLine, Vector3.Zero);
	}

	public async void SpawnAllBallsDelayed(float InDelayTime)
	{
		await Task.DelaySeconds(0.01f);
		foreach (Client pl in Client.All)
		{
			if (pl.Pawn != null)
			{
				pl.Pawn.Delete();
			}
			Pawn pawn = new Pawn();
			pawn.Owner = pl as Entity;
			pawn.GetPlayerStateManager();
			pl.Pawn = pawn;
		}
	}

	public void SpawnAllBalls()
	{
		foreach (Client pl in Client.All)
		{
			if (pl.Pawn != null)
			{
				pl.Pawn.Delete();
			}
			Pawn pawn = new Pawn();
			pawn.Owner = pl as Entity;
			pawn.GetPlayerStateManager();
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
			AllPlayersReady = false;
			NextGameState = Time.Now + 6000;
			SpawnAllBalls();
		}else
		if (inState == 1)
		{
			SpawnAllSpectators();
			NextGameState = Time.Now + 2;
		}else
		if (inState == 2)
		{
			DestroyCurrentSMBStage();
			PlayNextStageInCourse( CurrentCourse );
			NextGameState = Time.Now + (StageMaxTime + 5.6f);
			SkyGenerator.CreateBackground(CurrentSky);
			SpawnAllBalls();
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

		foreach (Entity element in Entity.All)
		{
			if (element is EnvironmentLightEntity)
			{
				EnvironmentLightEntity Ent = element as EnvironmentLightEntity;
				Ent.Delete();
			}
		}
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

		if (CurrentGameState == 0 && Client.All.Count > 0 && !AllPlayersReady && Time.Now > 10)
		{
			bool AllReady = true;
			foreach (Client pl in Client.All)
			{
				Pawn Ball = pl.Pawn as Pawn;
				if (Ball != null && Ball.Ready == false)
				{
					AllReady = false;
				}
			}
			if (AllReady)
			{
				NextGameState = Time.Now + 5;
				AllPlayersReady = true;
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

	public async void StartMusicDelayed()
	{
		foreach (Sound Song in GameMusic)
		{
			Song.SetVolume(0);
		}
		await Task.DelaySeconds(0.3f);
		foreach (Sound Song in GameMusic)
		{
			Song.Stop();
		}
		GameMusic.Clear();
		Sound NewSong = Sound.FromWorld(PlayingSong, new Vector3(0,0,0));
		GameMusic.Add(NewSong);
	}

	[Event.Tick.Client]
	public void ManageGameMusic()
	{
		if (Time.Delta > 0.02f)
		{
			return;
		}
		if (GameMusic.Count > 1)
		{
			foreach (Sound Song in GameMusic)
			{
				Song.Stop();
			}
			GameMusic.Clear();
		}
		if (PlayingSong != DesiredSong)
		{
			PlayingSong = DesiredSong;
			foreach (Sound Song in GameMusic)
			{
				Song.SetVolume(0);
			}
			StartMusicDelayed();
		}
		else
		{
			if (GameMusic.Count >= 1 && GameMusic[0].Finished)
			{
				foreach (Sound Song in GameMusic)
				{
					Song.Stop();
				}
				GameMusic.Clear();
				Sound NewSong = Sound.FromWorld(PlayingSong, new Vector3(0,0,0));
				GameMusic.Add(NewSong);
			}
		}
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		PlayerStateManager NewManager = new PlayerStateManager();
		NewManager.Owner = client as Entity;

		if (CurrentGameState == 1 | CurrentGameState == 3)
		{
			var pawn = new SpectatorPawn();
			client.Pawn = pawn;
		}else
		if (CurrentGameState == 0 | CurrentGameState == 2)
		{
			var pawn = new Pawn();
			pawn.Owner = client as Entity;
			pawn.GetPlayerStateManager();
			client.Pawn = pawn;
		}

		SkyGenerator.CreateBackground(To.Single(client), CurrentSky);

	}

	public override bool CanHearPlayerVoice( Client source, Client dest ) => true;

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

	[Event.Frame]
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
				if (element is EnvironmentLightEntity)
				{
					EnvironmentLightEntity Ent = element as EnvironmentLightEntity;
					if (Ent != null)
					{
						Transform NewTransform = StageTiltTransform(new Transform(Vector3.Zero, LightAngles, 1));
						Ent.Rotation = NewTransform.Rotation;
					}
				}else
				{
					continue;
				}
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
