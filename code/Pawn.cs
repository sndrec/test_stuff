using Sandbox;
using System;
using System.Linq;

namespace Sandbox;

partial class Pawn : ModelEntity
{
	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	[Net]
	public ModelEntity ServerRenderBall {get;set;}
	
	[Net]
	public float LatestNow {get;set;}

	public Vector3 LastGroundNormal {get;set;}

	public Vector3 LastGroundVel {get;set;}

	public Vector3 LastVelAtPos {get;set;}

	public Vector3 OldVelocity {get;set;}

	public Vector3 OldPosition {get;set;}

	public float AirTime {get;set;}

	public float GroundTime {get;set;}

	public float LastHit {get;set;}

	public float YawTilt {get;set;}

	public float PitchTilt {get;set;}

	public float LastStateChange {get;set;}

	public float SpawnTime {get;set;}

	public ModelEntity RenderBall {get;set;}

	public Rotation RenderBallAng {get;set;}

	public Angles BallCamAng {get;set;}

	public Angles SpawnAngles {get;set;}

	public Vector3 SpawnPos {get;set;}

	public int BallState {get;set;}

	public Vector3 ClientVelocity {get;set;}

	public Vector3 ClientPosition {get;set;}

	public bool ControlEnabled {get;set;}

	public float FirstHit {get;set;}

	public Vector3 CameraPosition {get;set;}

	public Rotation CameraRotation {get;set;}

	public Vector3 CameraVelocity {get;set;}

	public Vector3 CameraGoalDesiredPosition {get;set;}

	public Vector3 BlastUpZLock {get;set;}

	public bool BlastingUp {get;set;}

	public Sound RollingWoop {get;set;}

	public Sound RollingGrind {get;set;}

	public float LastServerMessage {get;set;}
	
	public MyGame GameEnt {get;set;}

	public bool AboutToGainControl {get;set;}

	public static float InOutQuad(float t, float b, float c, float d)
	{
		t = t / d * 2;
		if (t < 1)
		{
		  return c / 2 * (float)Math.Pow(t, 2) + b;
		}
		else
		{
		  return -c / 2 * ((t - 1) * (t - 3) - 1) + b;
		}
	}

	public static float InOutSine(float t, float b, float c, float d)
	{
		return -c / 2 * ((float)Math.Cos(3.141592f * t / d) - 1) + b;
	}

	public static float inSine(float t, float b, float c, float d)
	{
		return -c * (float)Math.Cos(t / d * (3.141592 / 2)) + c + b;
	}

	public static float outSine(float t, float b, float c, float d)
	{
		return c * (float)Math.Sin(t / d * (3.141592 / 2)) + b;
	}

	public static float AngleDifference(float a, float b)
	{
		float diff = MathX.NormalizeDegrees( a - b );
	
		if ( diff < 180f )
		{
			return diff;
		}
		return diff - 360f;
	}

	public static float ApproachAngle(float cur, float target, float inc)
	{
		float diff = AngleDifference( target, cur );

		return MathX.Approach( cur, cur + diff, inc );
	}

	[ClientRpc]
	public static void CreateClientCollisionParticles(float RelativeComponent, Vector3 HitPosition, Vector3 HitNormal, Vector3 FinalVel)
	{
		if (RelativeComponent > 150)
		{
			Particles ImpactParticle = Particles.Create("particles/collisionstars.vpcf");
			ImpactParticle.SetPosition(2, new Vector3(RelativeComponent / 64, 0, 0));
			ImpactParticle.SetPosition(1, HitPosition);
			ImpactParticle.SetPosition(0, (HitNormal * RelativeComponent * -0.125f) + (FinalVel * 0.75f));
			ImpactParticle.Simulating = true;
			ImpactParticle.EnableDrawing = true;
		}
	}

	[ConCmd.Server]
	public static void SendServerCollision(Vector3 InVelocity, Vector3 HitNormal, int InNetworkIdent, Vector3 HitPosition, float InNow)
	{
		MyGame GameEnt = Game.Current as MyGame;
		Client pl = ConsoleSystem.Caller;
		Entity HitEntity = Entity.FindByIndex(InNetworkIdent);
		Pawn Ball = pl.Pawn as Pawn;
		if (Ball == null)
		{
			return;
		}
		SMBObject HitEntitySMB = HitEntity as SMBObject;
		Vector3 FinalVel = InVelocity;
		Vector3 RelativeVel = InVelocity;
		Vector3 VelAtPos = new Vector3(0,0,0);
		if (HitEntitySMB != null)
		{
			VelAtPos = HitEntitySMB.GetVelocityAtPoint(HitPosition, Global.TickInterval);
			RelativeVel = InVelocity - VelAtPos;
		}
		Ball.LastGroundNormal = HitNormal;
		Ball.LastGroundVel = RelativeVel - (HitNormal * Vector3.Dot(RelativeVel, HitNormal));
		Ball.LastHit = Time.Now;
		float RelativeComponent = Vector3.Dot(RelativeVel, HitNormal);
		if (RelativeComponent > 0)
		{
			return;
		}
		if (RelativeComponent < -240)
		{	
			GameEnt.FromEntityToAllBut(pl, "ball_impact_hard", Ball);
		}else
		if (RelativeComponent < -150)
		{	
			GameEnt.FromEntityToAllBut(pl, "ball_impact_medium", Ball);
		}else
		if (RelativeComponent < -100)
		{	
			GameEnt.FromEntityToAllBut(pl, "ball_impact_soft", Ball);
		}

		RelativeComponent = Math.Abs(RelativeComponent);
		float ResultSpeed = RelativeComponent * 1f;
		float AddSpeed = RelativeComponent * 0.5f;
		AddSpeed = Math.Max(0, AddSpeed - 10);
		FinalVel += HitNormal * (ResultSpeed + AddSpeed);
		Ball.LastVelAtPos = VelAtPos;
		if (RelativeComponent > 150)
		{
			foreach (Client player in Client.All)
			{
				if (player != pl)
				{
					Pawn.CreateClientCollisionParticles(To.Single(player), RelativeComponent, HitPosition, HitNormal, FinalVel);
				}
			}
		}
		Ball.LatestNow = InNow;
		Ball.Velocity = FinalVel;
	}

	[ConCmd.Server]
	public static void SendServerBallUpdate(float InNow, Vector3 InVelocity)
	{
		MyGame GameEnt = Game.Current as MyGame;
		Client pl = ConsoleSystem.Caller;
		Pawn Ball = pl.Pawn as Pawn;
		if (Ball == null)
		{
			return;
		}
		Ball.LatestNow = InNow;
		Ball.Velocity = InVelocity;
	}

	public Vector3 ApplyCollisionResponse(Vector3 InVelocity, Vector3 HitNormal, Entity HitEntity, Vector3 HitPosition, float RealDelta)
	{
		if (HitEntity is PartyBall)
		{
			PartyBall DaBall = HitEntity as PartyBall;
			Vector3 HitAxis = Vector3.Cross((HitPosition - DaBall.Position).Normal, HitNormal).Normal;
			DaBall.AngVel = Rotation.FromAxis(HitAxis, InVelocity.Length * -0.05f);
			HitNormal = (HitNormal + (InVelocity.Normal * 0.5f)).Normal;
		}
		SMBObject HitEntitySMB = HitEntity as SMBObject;
		Vector3 FinalVel = InVelocity;
		Vector3 RelativeVel = InVelocity;
		Vector3 VelAtPos = new Vector3(0,0,0);
		if (HitEntitySMB != null)
		{
			VelAtPos = HitEntitySMB.GetVelocityAtPoint(HitPosition, Global.TickInterval);
			RelativeVel = InVelocity - VelAtPos;
		}
		LastGroundNormal = HitNormal;
		LastGroundVel = RelativeVel - (HitNormal * Vector3.Dot(RelativeVel, HitNormal));
		LastHit = Time.Now;
		float RelativeComponent = Vector3.Dot(RelativeVel, HitNormal);
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

		RelativeComponent = Math.Abs(RelativeComponent);
		float ResultSpeed = RelativeComponent * 1f;
		float AddSpeed = RelativeComponent * 0.5f;
		AddSpeed = Math.Max(0, AddSpeed - 10);
		FinalVel += HitNormal * (ResultSpeed + AddSpeed);
		LastVelAtPos = VelAtPos;
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
		if (Time.Now > LastServerMessage + 0.033 | RelativeComponent > 150)
		{
			SendServerCollision(InVelocity, HitNormal, HitEntity.NetworkIdent, HitPosition, Time.Now);
			LastServerMessage = Time.Now;
		}
		//point 0 = position
		//point 1 = velocity
		//point 2 = emit rate
		//Vector3 SparkPowerAndDir = (LastGroundVel + (HitNormal * LastGroundVel.Length * 0.25f)) * 0.75f;
		//float SparksPerSec = (LastGroundVel.Length / 4) - 50;
		//if (SparksPerSec < 25)
		//{
		//	SparksPerSec = 0;
		//}
		//Log.Info(HitPosition);
		//Log.Info(SparkPowerAndDir);
		//Log.Info(SparksPerSec);

		return FinalVel;
	}

	[ConCmd.Server]
	public static void Spectate()
	{
		Client pl = ConsoleSystem.Caller;
		//Log.Info(pl);
		//Log.Info(pl.Pawn);
		pl.Pawn.Delete();
		SpectatorPawn NewPawn = new SpectatorPawn();
		pl.Pawn = NewPawn;
	}
	public bool TryBallCollisionDiscrete(float RealDelta, bool DoCollisionResponse)
	{
		bool CollisionHappened = false;
		for (int i = 0; i < 4; i++)
		{
			//Log.Info(i);
			//Vector3 RealStartPosition = Position
			//if (i == 0)
			//{
			//	RealStartPosition = SMBObject.TransformPosition(CheckEnt.Transform, SMBObject.InverseTransformPosition(CheckEnt.OldTransform, Position));
			//}
			TraceResult MoveTrace = Trace.Sphere(10, ClientPosition, ClientPosition).WithTag("solid").IncludeClientside(true).Run();
			if (MoveTrace.Hit)
			{	
				CollisionHappened = true;
				if (ControlEnabled == false)
				{
					FirstHit = Time.Now;
					if (GameEnt.HasFirstHit == false)
					{
						GameEnt.FirstHitTime = Time.Now;
						GameEnt.HasFirstHit = true;
					}
					GameEnt.PlayGlobalSound("an_go");
				}
				ControlEnabled = true;
				TraceResult DepenTrace;
				Vector3 UseNormal = MoveTrace.Normal;
				if (MoveTrace.StartedSolid)
				{
					DepenTrace = Trace.Ray(ClientPosition, MoveTrace.HitPosition).WithTag("solid").IncludeClientside(true).Run();
					if (DepenTrace.Hit)
					{
						ClientPosition = DepenTrace.HitPosition + (MoveTrace.Normal * 10.001f);
						UseNormal = DepenTrace.Normal;
						//DebugOverlay.Sphere(DepenTrace.HitPosition, 3, new Color(0,255,0), Time.Delta, false);
					}
				}
				TraceResult FinalTrace = Trace.Sphere(9, ClientPosition, ClientPosition - (MoveTrace.Normal * 2)).WithTag("solid").IncludeClientside(true).Run();

				//DebugOverlay.Sphere(MoveTrace.HitPosition, 3, new Color(255,0,0), Time.Delta, false);
				//DebugOverlay.Sphere(FinalTrace.HitPosition, 4, new Color(0,0,255), Time.Delta, false);

				//DebugOverlay.Line(MoveTrace.HitPosition, MoveTrace.HitPosition + (FinalTrace.Normal * 32), new Color(255,255,255), 0.25f, false);
				if (DoCollisionResponse)
				{
					ClientVelocity = ApplyCollisionResponse(ClientVelocity, FinalTrace.Normal, MoveTrace.Entity, MoveTrace.HitPosition, RealDelta);
				}
			}else
			{
				break;
			}
		}
		return CollisionHappened;
	}

	public bool TryBallCollisionContinuous(float RealDelta)
	{
		string[] ColTags = new string[] {"solid", "testingcollision"};
		float MinFrac = 1;
		Vector3 UsedCollisionNormal = new Vector3(0, 0, 1);
		Vector3 UsedCollisionPosition = new Vector3(0,0,0);
		Vector3 OriginalPosition = ClientPosition;
		Vector3 OriginalVelocity = ClientVelocity;
		Vector3 ModifiedPosition = ClientPosition;
		Vector3 ModifiedVelocity = ClientVelocity;
		foreach (Entity element in Entity.All)
		{

			SMBObject CheckEnt = element as SMBObject;
			if (CheckEnt == null | !CheckEnt.IsValid())
			{
				continue;
			}
			//Log.Info(element);
			//Log.Info("Testing against object");
			//Log.Info(CheckEnt);
			Vector3 TemporaryPosition = OriginalPosition;
			Vector3 TemporaryVelocity = OriginalVelocity;
			Vector3 RealStartPosition = SMBObject.TransformPosition(CheckEnt.UninterpolatedTransform, SMBObject.InverseTransformPosition(CheckEnt.OldTransform, TemporaryPosition));
			TraceResult MoveTrace = Trace.Ray(RealStartPosition, TemporaryPosition + (TemporaryVelocity * RealDelta)).WithTag(CheckEnt.CollisionTag).IncludeClientside(true).Run();
			if (MoveTrace.Hit)
			{
				//DebugOverlay.Sphere( RealStartPosition, 1, new Color(255,0,0), 30, false );
				//DebugOverlay.Sphere( MoveTrace.HitPosition, 1, new Color(0,0,255), 30, false );
				//DebugOverlay.Sphere( TemporaryPosition + (TemporaryVelocity * RealDelta), 1, new Color(255,255,255), 30, false );
				//DebugOverlay.Line( RealStartPosition, MoveTrace.HitPosition, new Color(0,255,0), 30, false );
				//DebugOverlay.Line( MoveTrace.HitPosition, TemporaryPosition + (TemporaryVelocity * RealDelta), new Color(0,255,255), 30, false );

				if (ControlEnabled == false)
				{
					FirstHit = Time.Now;
					if (GameEnt.HasFirstHit == false)
					{
						GameEnt.FirstHitTime = Time.Now;
						GameEnt.HasFirstHit = true;
					}
				}
				ControlEnabled = true;
				TemporaryPosition = MoveTrace.HitPosition + (MoveTrace.Normal * 10);
				TemporaryVelocity = ApplyCollisionResponse(TemporaryVelocity, MoveTrace.Normal, MoveTrace.Entity, MoveTrace.HitPosition, RealDelta);
				//DebugOverlay.Line( MoveTrace.HitPosition, MoveTrace.HitPosition + (MoveTrace.Normal * 100), new Color(255,255,0), 30, false );
				if (MoveTrace.Fraction < MinFrac)
				{
					MinFrac = MoveTrace.Fraction;
					ModifiedPosition = TemporaryPosition;
					ModifiedVelocity = TemporaryVelocity;
				}
			}
		}
		if (MinFrac == 0)
		{
			return false;
		}
		else
		if (MinFrac < 1)
		{
			//Log.Info(MinFrac);
			ClientVelocity = ModifiedVelocity;
			ClientPosition = ModifiedPosition;
			return true;
		}
		else
		{
			return false;
		}
	}

	public override void Spawn()
	{
		base.Spawn();
		SetupPhysicsFromSphere( PhysicsMotionType.Keyframed, new Vector3(0, 0, 0), 10 );
		EnableDrawing = false;
		EnableTouch = true;
		PhysicsBody.EnableTouch = true;
		Tags.Add("PlayerBall");
		ChangeBallState(0);
		SetModel( "models/dev/new_model/new_model.vmdl" );
		GameEnt = Game.Current as MyGame;
		ServerRenderBall = new ModelEntity();
		ServerRenderBall.SetModel( "models/ballbase.vmdl" );
		ServerRenderBall.EnableDrawing = true;
		RenderBallAng = Rotation.Identity;

	}

	public override void ClientSpawn()
	{
		base.Spawn();

		RenderBallAng = Rotation.Identity;
		SetModel( "models/dev/new_model/new_model.vmdl" );
		RenderBall = new ModelEntity();
		RenderBall.SetModel( "models/ballbase.vmdl" );
		EnableDrawing = false;
		RenderBall.EnableDrawing = true;
		ControlEnabled = false;
		RespawnBall(true);
		SpawnTime = Time.Now;
		GameEnt = Game.Current as MyGame;

	}

	protected override void OnDestroy()
	{
		if (RenderBall != null)
		{
			RollingWoop.Stop();
			RollingGrind.Stop();
			RenderBall.Delete();
		}
		if (Local.Client == null)
		{
			ServerRenderBall.Delete();
		}
	}

	public override void Touch( Entity OtherEnt )
	{
		Log.Info("Hi");
	}

	public void RespawnBall(bool FirstRespawn)
	{
		RollingWoop.Stop();
		RollingGrind.Stop();
		ClientVelocity = new Vector3(0, 0, 0);
		ControlEnabled = false;
		MyGame GameEnt = Game.Current as MyGame;
		ClientPosition = GameEnt.CurrentSpawnPos + new Vector3(0, 0, 60);
		BallCamAng = GameEnt.CurrentSpawnRot;
		OldPosition = ClientPosition;
		//Log.Info(GameEnt);
		ChangeBallState(0);
		SpawnTime = Time.Now - 3.5f;
		if (FirstRespawn)
		{
			SpawnTime = Time.Now;
		}
		AboutToGainControl = false;
		LastGroundVel = Vector3.Zero;
		RenderBall.Rotation = Rotation.Random;
		EnableDrawing = false;
		RenderBall.EnableDrawing = false;
		RollingWoop = Sound.FromEntity("ball_woop", this);
		RollingGrind = Sound.FromEntity("ball_grind", this);
		RollingWoop.SetVolume(0);
		RollingGrind.SetVolume(0);
		ConsoleSystem.SetValue("snd_occlusion", 0);
		ConsoleSystem.SetValue("snd_doppler", 0);
		ConsoleSystem.SetValue("steamaudio_enable", 0);
		Map.Camera.ZFar = 100000;
	}

	public void ChangeBallState(int InState)
	{
		if (BallState != InState)
		{
			if (InState == 2 )
			{
				Sound.FromEntity("goaltape_break", this);
				GameEnt.PlayGlobalSoundDelayed("an_goal", 0.5f);
				Particles ImpactParticle = Particles.Create("particles/goaltwinkle.vpcf", this, true);
				ImpactParticle.Simulating = true;
				ImpactParticle.EnableDrawing = true;
			}
			if (InState == 1 )
			{
				Sound.FromEntity("ball_fallout", this);
				GameEnt.PlayGlobalSoundDelayed("an_fallout", 0.5f);
			}
		}
		BallState = InState;
		LastStateChange = Time.Now;
		BlastingUp = false;
	}

	public virtual Vector3 TickBallMovement(Vector3 AnalogInput)
	{
		float RealDelta = Math.Min(Global.TickInterval, Time.Delta);
		AnalogInput = new Vector3(MathX.Clamp(AnalogInput.x, -1, 1), MathX.Clamp(AnalogInput.y, -1, 1), 0);
		if (BallState == 0)
		{
			if (Time.Now > SpawnTime + 5)
			{
				EnableDrawing = true;
				RenderBall.EnableDrawing = true;
				Rotation FixedEyeRot = Rotation.From(0f, BallCamAng.yaw, 0f);
				//physics
		
				Rotation GravDir = new Rotation(0f, 0f, -1f, 0f);
				if (ControlEnabled == true)
				{
					YawTilt = MathX.Lerp(YawTilt, AnalogInput.y * 15, RealDelta * 10);
					PitchTilt = MathX.Lerp(PitchTilt, AnalogInput.x * 15, RealDelta * 10);
					float InX = (float)Math.Pow(Math.Abs(AnalogInput.x), 2) * Math.Sign(AnalogInput.x);
					float InY = (float)Math.Pow(Math.Abs(AnalogInput.y), 2) * Math.Sign(AnalogInput.y);
					//Log.Info(AnalogInput);
					GravDir = GravDir.RotateAroundAxis(FixedEyeRot.Right, InX * -23);
					GravDir = GravDir.RotateAroundAxis(FixedEyeRot.Forward, InY * -23);
					GravDir = GravDir.Normal;
				}
				Vector3 AddVelocity = GravDir.Down * 588 * RealDelta;
				ClientVelocity += AddVelocity;
				OldVelocity = ClientVelocity;
				Vector3 OldPos = ClientPosition;
				ClientPosition = ClientPosition + (ClientVelocity * RealDelta);
				bool DidContinuous = TryBallCollisionContinuous(RealDelta);
				if (!DidContinuous)
				{
					TryBallCollisionDiscrete(RealDelta, true);
				}else
				{
					TryBallCollisionDiscrete(RealDelta, false);
				}
				if (LastHit + 0.1f < Time.Now)
				{
					AirTime = AirTime + RealDelta;
					GroundTime = 0;
					if (Time.Now > LastServerMessage + 0.033)
					{
						SendServerBallUpdate(Time.Now, ClientVelocity);
						LastServerMessage = Time.Now;
					}
				}else
				{
					GroundTime = GroundTime + RealDelta;
					AirTime = 0;
				}
				if (GroundTime > 0)
				{
					Vector3 RelativeVel = ClientVelocity - LastVelAtPos;
					RelativeVel += -RelativeVel * 0.54f * RealDelta;
					ClientVelocity = RelativeVel + LastVelAtPos;
					//Log.Info(RelativeVel.Length);
					RollingWoop.SetVolume(MathX.Clamp(MathX.Remap(RelativeVel.Length, 50, 400, 0, 0.5f), 0, 1));
					RollingWoop.SetPitch(MathX.Clamp(MathX.Remap(RelativeVel.Length, 50, 600, 0.3f, 2.25f), 0.25f, 2));
					RollingGrind.SetVolume(MathX.Clamp(MathX.Remap(RelativeVel.Length, 300, 600, 0, 1f), 0, 1f));
					RollingGrind.SetPitch(MathX.Clamp(MathX.Remap(RelativeVel.Length, 300, 700, 1f, 2f), 1f, 2f));
				}else
				{
					RollingWoop.SetVolume(0);
					RollingGrind.SetVolume(0);
				}
				foreach (Entity element in Entity.All)
				{
					if (element != null && element.Tags.Has("goaltrigger"))
					{
						SMBObject TriggerOwner = element.Owner as SMBObject;
						Vector3 TriggerTestStart = SMBObject.TransformPosition(TriggerOwner.UninterpolatedTransform, SMBObject.InverseTransformPosition(TriggerOwner.OldTransform, OldPosition));
						Trace GoalTrace = Trace.Ray(TriggerTestStart, ClientPosition);
						GoalTrace.WithTag("goaltrigger");
						TraceResult GoalTraceResult = GoalTrace.Run();
						if (GoalTraceResult.Hit && BallState != 2)
						{
							GoalPost GoalEnt = GoalTraceResult.Entity.Owner as GoalPost;
							ChangeBallState(2);
							float TimeRemaining = GameEnt.StageMaxTime - (Time.Now - GameEnt.FirstHitTime);
							GameEnt.Score += (int)(TimeRemaining * 100);
							TapeStick ClosestStick = GoalEnt.GoalTape.Sticks[0];
							float ClosestDist = 1000;
							foreach (TapeStick Stick in GoalEnt.GoalTape.Sticks)
							{
								Vector3 StickCentre = (Stick.PointA.Position + Stick.PointB.Position) / 2;
								float Dist = (StickCentre - ClientPosition).Length;
								if (Dist < ClosestDist)
								{
									ClosestDist = Dist;
									ClosestStick = Stick;
								}
							}
							GoalEnt.GoalTape.Sticks.Remove(ClosestStick);
							GoalEnt.GoalTape.UpdateRopeMesh(true);
						}
						continue;
					}
				}
			}
		}else
		if (BallState == 1)
		{
			Vector3 AddVelocity = new Vector3(0, 0, -1) * 588 * RealDelta;
			ClientVelocity += AddVelocity;
			ClientPosition = ClientPosition + (ClientVelocity * RealDelta);
			TryBallCollisionDiscrete(RealDelta, true);
			TryBallCollisionContinuous(RealDelta);
			if (Time.Now > LastStateChange + 2)
			{
				RespawnBall(false);
			}
		}else
		if (BallState == 2)
		{
			if (Time.Now > LastStateChange + 4)
			{
				Spectate();
			}
			if (Time.Now > LastStateChange + 2)
			{
				ClientVelocity += new Vector3(0, 0, 800 * RealDelta);
			}else
			{
				ClientVelocity += -ClientVelocity * RealDelta * 3;
			}
			ClientPosition = ClientPosition + (ClientVelocity * RealDelta);
			TryBallCollisionDiscrete(RealDelta, true);
			TryBallCollisionContinuous(RealDelta);
			RollingWoop.SetVolume(0);
			RollingGrind.SetVolume(0);
		}

		BBox StageBounds = new BBox(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
		foreach (Entity element in Entity.All)
		{
			if (element is SMBObject)
			{
				StageBounds = StageBounds.AddPoint(element.WorldSpaceBounds.Mins);
				StageBounds = StageBounds.AddPoint(element.WorldSpaceBounds.Maxs);
			}
		}

		if ( ClientPosition.z < StageBounds.Mins.z - 50 && BallState == 0)
		{
			ChangeBallState(1);
		}
		if ( ControlEnabled && Input.Pressed( InputButton.Reload ) )
		{
			RespawnBall(false);
		}
		if ( Input.Pressed( InputButton.Duck ))
		{
			Log.Info(ClientPosition);
		}

		if (Input.Down( InputButton.Jump ))
		{
			ClientVelocity = new Vector3(ClientVelocity.x, ClientVelocity.y, ClientVelocity.z + (2000 * RealDelta));
			//ClientPosition += new Vector3(0, 0, 800 * RealDelta);
		}

		if (Input.Down( InputButton.Duck ))
		{
			ClientVelocity = new Vector3(ClientVelocity.x, ClientVelocity.y, ClientVelocity.z + (-2000 * RealDelta));
			//ClientPosition += new Vector3(0, 0, 800 * RealDelta);
		}


		//Vector3 SpinAxis = Vector3.Cross(LastGroundNormal, LastGroundVel).Normal;
		//Rotation = Rotation.RotateAroundAxis(SpinAxis, Velocity.Length * -0.05f);

		// If we're running serverside and Attack1 was just pressed, spawn a ragdoll
		//if ( IsServer && Input.Pressed( InputButton.PrimaryAttack ) )
		//{
		//	var ragdoll = new ModelEntity();
		//	ragdoll.SetModel( "models/citizen/citizen.vmdl" );
		//	ragdoll.Position = EyePosition + EyeRotation.Forward * 40;
		//	ragdoll.Rotation = Rotation.LookAt( Vector3.Random.Normal );
		//	ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
		//	ragdoll.PhysicsGroup.Velocity = EyeRotation.Forward * 1000;
		//}

		OldPosition = ClientPosition;
		return ClientPosition;
	}


	public override void BuildInput( InputBuilder InputBuilderStruct)
	{
		Vector3 NewPosition = TickBallMovement(InputBuilderStruct.AnalogMove);
		InputBuilderStruct.Position = NewPosition;
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		Position = Input.Position;
		//DebugOverlay.Sphere(Position, 10, new Color(0,255,0), Time.Delta, false);
		if ( Local.Client == null )
		{
			ServerRenderBall.Position = Position;
			Vector3 SpinAxis = Vector3.Cross(LastGroundNormal, LastGroundVel).Normal;
			Rotation HelperRot = Rotation.FromAxis(SpinAxis, LastGroundVel.Length * Time.Delta * 3);
			RenderBallAng = HelperRot * RenderBallAng;
			ServerRenderBall.Rotation = RenderBallAng.Normal;
		}else
		{
			ServerRenderBall.EnableDrawing = false;
		}
		if ( Local.Client == null && Input.Pressed( InputButton.Run ))
		{
			GameEnt.NextGameState = Time.Now;
		}

		//MoveHelper helper = new MoveHelper( Position, Velocity );
		//helper.Trace = helper.Trace.Radius( 10 );
		//helper.Trace.WithTag("solid");
		//IReadOnlyList<SMBObject> Ents = SMBObject.All;

		//Vector3 VelAtPoint = MoveTrace.Body.GetVelocityAtPoint(MoveTrace.HitPosition);
		//Vector3 RelativeVel = Velocity - VelAtPoint;
		//Log.Info(VelAtPoint);
		//float NormalVel = Math.Max(Math.Abs(Vector3.Dot(MoveTrace.Normal, RelativeVel)) - 20, 0) * 0.5f;
		//Vector3 ReflecVec = Vector3.Reflect(RelativeVel.Normal, MoveTrace.Normal);
		//ReflecVec = RelativeVel.Length * ReflecVec;
		//ReflecVec = ReflecVec.SubtractDirection(MoveTrace.Normal, 1f);
		//ReflecVec = ReflecVec + (MoveTrace.Normal * NormalVel);
		//Velocity = ReflecVec;
		//LastGroundVel = Velocity;
		//LastGroundNormal = MoveTrace.Normal;
		//LastHit = Time.Now;

		//float DistTravelled = helper.TryMove( Time.Delta );
//
		//if ( DistTravelled > 0 )
		//{
		//	Position = helper.Position;
		//	Velocity = helper.Velocity;
		//}
		//if ( MoveTrace.Hit )
		//{
		//}
	}

	/// <summary>
	/// Called every frame on the client
	/// </summary>
	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );
		//if (!IsLocalPawn)
		//{
		//	RenderBall.EnableDrawing = true;
		//}
		//visual ball stuff
		RenderBall.Position = ClientPosition;
		Vector3 SpinAxis = Vector3.Cross(LastGroundNormal, LastGroundVel).Normal;
		Rotation HelperRot = Rotation.FromAxis(SpinAxis, LastGroundVel.Length * Time.Delta * 3);
		RenderBallAng = HelperRot * RenderBallAng;
		RenderBall.Rotation = RenderBallAng.Normal;
//
		//camera stuff
		if (BallState == 0)
		{
			if (Time.Now > SpawnTime + 3.5 && !AboutToGainControl)
			{
				GameEnt.PlayGlobalSound("an_ready");
				AboutToGainControl = true;
			}
			if (Time.Now > SpawnTime + 5)
			{
				float RealDelta = Time.Delta;
				Vector3 ReducedVel = ClientVelocity;
				ReducedVel.z -= 2 * Math.Sign(ReducedVel.z);
				if (Math.Abs(ReducedVel.z) <= 2)
				{
					ReducedVel.z = 0;
				}
				Angles VelAngles = ReducedVel.EulerAngles;
				Angles VelAnglesForVert = (ReducedVel + new Vector3(15, 0, 0)).EulerAngles;
				float RealVelPitch = VelAnglesForVert.pitch;
				if (RealVelPitch > 90)
				{
					RealVelPitch -= 360;
				}
				RealVelPitch = Math.Min(RealVelPitch, 72);
				Vector3 VelNoZ = ClientVelocity;
				VelNoZ.z = 0;
				float VelNoZMag = VelNoZ.Length;
				float CamMoveFracVelVert = (Math.Abs(ClientVelocity.z) + 100);
				float CamMoveFracVelHoriz;
				float SlowStart = 0.1f;
				float FastStart = 1;
				float MaxSpeed = 50;
				if (VelNoZMag > FastStart)
				{
					float Ratio = Math.Min((VelNoZMag - FastStart) / MaxSpeed, 1);
					CamMoveFracVelHoriz = (Ratio * 0.75f) + 0.25f;
				}else
				if (VelNoZMag > SlowStart)
				{
					float Ratio = Math.Min((VelNoZMag - SlowStart) / (FastStart - SlowStart), 1);
					CamMoveFracVelHoriz = Ratio * 0.25f;
				}else
				{
					CamMoveFracVelHoriz = 0;
				}
				//Log.Info(CamMoveFracVelHoriz);
				float PitchDiffFrac = Math.Abs(MathX.Clamp(AngleDifference(BallCamAng.pitch, RealVelPitch), -30, 30) / 30);
				float YawDiffFrac = Math.Abs(MathX.Clamp(AngleDifference(BallCamAng.yaw, VelAngles.yaw), -30, 30) / 30);
				float CamMoveFracPitch = CamMoveFracVelVert * PitchDiffFrac * 1f;
				float CamMoveFracYaw = CamMoveFracVelHoriz * YawDiffFrac * 220f;
				float NewPitch = ApproachAngle(BallCamAng.pitch, RealVelPitch, CamMoveFracPitch * RealDelta);
				float NewYaw = ApproachAngle(BallCamAng.yaw, VelAngles.yaw, CamMoveFracYaw * RealDelta);
				Vector3 CameraOrigin = ClientPosition;
				if (ControlEnabled == false)
				{
					NewPitch = 0;
					TraceResult StageStartTrace = Trace.Ray(ClientPosition, ClientPosition + new Vector3(0, 0, -10000)).WithTag("solid").IncludeClientside(true).Run();
					CameraOrigin = StageStartTrace.HitPosition + (StageStartTrace.Normal * 10);
				}
				if (Time.Now < FirstHit + 0.75f)
				{
					NewPitch = 0;
				}
				BallCamAng = new Angles(NewPitch, NewYaw, 0f);
				//YawTilt = MathX.Lerp(YawTilt, AnalogInput.y * 12, RealDelta * 10);
				//PitchTilt = MathX.Lerp(PitchTilt, AnalogInput.x * 12, RealDelta * 10);
				Rotation NewCamRotation = new Angles(NewPitch + 15f, NewYaw, 0f).ToRotation();
				Rotation FixedEyeRot = Rotation.From(0f, NewYaw, 0f);
				Rotation HelperRotPitch = Rotation.FromAxis(FixedEyeRot.Right, PitchTilt);
				Rotation HelperRotYaw = Rotation.FromAxis(FixedEyeRot.Forward, YawTilt);
				MyGame GameEnt = Game.Current as MyGame;
				GameEnt.StageTilt = (HelperRotPitch * HelperRotYaw);
				EyeRotation = GameEnt.StageTilt * NewCamRotation;
				//EyeRotation = NewCamRotation;
				//EyeRotation = EyeRotation.Normal;
				float FakeRotOffsetUpDown = (NewPitch * -0.15f) + 15;
				float FakeRotOffsetDist = (NewPitch * 0.15f) + 45;
				//EyePosition = CameraOrigin - (EyeRotation.Forward * FakeRotOffsetDist) + (EyeRotation.Up * FakeRotOffsetUpDown);
				EyePosition = CameraOrigin - (EyeRotation.Forward * FakeRotOffsetDist) + (EyeRotation.Up * FakeRotOffsetUpDown);
				CameraVelocity = (EyePosition - CameraPosition) / RealDelta;
				CameraPosition = EyePosition;
				CameraRotation = EyeRotation;
				CameraGoalDesiredPosition = EyePosition;
			}else
			{
				Vector3 CameraOrigin = ClientPosition;
				TraceResult StageStartTrace = Trace.Ray(ClientPosition, ClientPosition + new Vector3(0, 0, -10000)).WithTag("solid").IncludeClientside(true).Run();
				CameraOrigin = StageStartTrace.HitPosition + (StageStartTrace.Normal * 10);
				YawTilt = 0;
				PitchTilt = 0;
				Rotation NewCamRotation = BallCamAng.ToRotation() * Rotation.FromPitch(15);
				Vector3 DesiredEyePosition = CameraOrigin - (NewCamRotation.Forward * 45) + (NewCamRotation.Up * 15);
				Rotation DesiredEyeRotation = NewCamRotation.Normal;
				BBox StageBounds = new BBox(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
				foreach (Entity element in Entity.All)
				{
					if (element is SMBObject)
					{
						StageBounds = StageBounds.AddPoint(element.WorldSpaceBounds.Mins);
						StageBounds = StageBounds.AddPoint(element.WorldSpaceBounds.Maxs);
					}
				}
				float Ratio = (Time.Now - SpawnTime) * 0.2f;
				float InSineRatio = inSine(Ratio, 0, 1, 1);
				float SineRatio = InOutSine(Ratio, 0, 1, 1);
				float QuadRatio = InOutQuad(Ratio, 0, 1, 1);
				float PowRatio = (float)Math.Pow(SineRatio, 0.65f);
				float PowRatio2 = (float)Math.Pow(SineRatio, 2.25f);
				float PowRatio3 = (float)Math.Pow(SineRatio, 0.8f);
				Rotation FinalRot = new Angles(MathX.Lerp(45, DesiredEyeRotation.Pitch(), SineRatio), DesiredEyeRotation.Yaw() + (360 * (1 - PowRatio)), 0).ToRotation();
				Vector3 BBoxCenter = StageBounds.Center;
				float LeadInDist = Math.Max(Math.Max(StageBounds.Size.x, StageBounds.Size.y), StageBounds.Size.z);
				Vector3 RootPos = Vector3.Lerp(BBoxCenter, DesiredEyePosition, QuadRatio);
				Vector3 FinalPos = RootPos + (FinalRot.Forward * -LeadInDist * (1 - PowRatio3));
				EyeRotation = FinalRot;
				EyePosition = FinalPos;
			}
		}else
		if (BallState == 1)
		{
			CameraVelocity += -CameraVelocity * Time.Delta * 2;
			CameraPosition = CameraPosition + (CameraVelocity * Time.Delta);
			CameraRotation = Rotation.Slerp(CameraRotation, Rotation.LookAt(-(CameraPosition - ClientPosition), new Vector3(0, 0, 1)), Time.Delta * 5, true);
			EyeRotation = CameraRotation;
			EyePosition = CameraPosition;
		}else
		if (BallState == 2)
		{
			LastGroundVel += -LastGroundVel * Time.Delta * 2;
			Vector3 UsePosition = ClientPosition + new Vector3(0, 0, 15);
			if (Time.Now > LastStateChange + 2)
			{
				if (!BlastingUp)
				{
					Sound Blastoff = Sound.FromEntity("ball_blastoff", this);
					BlastUpZLock = ClientPosition;
					BlastingUp = true;
				}
				UsePosition = BlastUpZLock;
			}
			CameraGoalDesiredPosition = UsePosition + (Rotation.FromYaw(90 * Time.Delta) * (Rotation.LookAt(CameraGoalDesiredPosition - UsePosition)).Normal.Forward * 60);
			CameraVelocity += (CameraGoalDesiredPosition - CameraPosition) * 16f * Time.Delta;
			CameraVelocity += -CameraVelocity * Time.Delta * 4;
			CameraPosition = CameraPosition + (CameraVelocity * Time.Delta);
			CameraRotation = Rotation.Slerp(CameraRotation, Rotation.LookAt(-(CameraPosition - ClientPosition), new Vector3(0, 0, 1)), Time.Delta * 30, true);
			EyeRotation = CameraRotation;
			EyePosition = CameraPosition;
		}
		if (BallState != 0 | !ControlEnabled)
		{
			MyGame GameEnt = Game.Current as MyGame;
			Rotation FixedEyeRot = Rotation.From(0f, BallCamAng.yaw, 0f);
			Rotation HelperRotPitch = Rotation.FromAxis(FixedEyeRot.Right, 0);
			Rotation HelperRotYaw = Rotation.FromAxis(FixedEyeRot.Forward, 0);
			GameEnt.StageTilt = (HelperRotPitch * HelperRotYaw);
		}
	}
}
