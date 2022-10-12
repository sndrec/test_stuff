using Sandbox;
using System;
using System.Linq;

namespace Sandbox;

public partial class Pawn : ModelEntity
{
	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	[Net, Predicted]
	public ModelEntity ServerRenderBall {get;set;}
	
	[Net]
	public float LatestNow {get;set;}

	[Net, Predicted]
	public AnimatedEntity BallCitizen {get;set;}

	[Net]
	public PlayerStateManager OurManager {get;set;}

	[Net]
	public float BallRestitution {get;set;}

	[Net]
	public float BallFriction {get;set;}

	[Net]
	public float BallGravity {get;set;}

	[Net]
	public float BallMaxTilt {get;set;}

	[Net]
	public float BallMaxVisualTilt {get;set;}

	public Rotation CitizenRotation {get;set;}

	public ClothingContainer CitizenClothing {get;set;}

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
	public void ServerSetPosition(Vector3 InVector)
	{
		ClientPosition = InVector;
	}

	[ClientRpc]
	public void ServerSetVelocity(Vector3 InVector)
	{
		ClientVelocity = InVector;
	}

	[ClientRpc]
	public void ServerSetRotation(Rotation InRotation)
	{
		BallCamAng = InRotation.Angles();
	}

	[ClientRpc]
	public void ServerAddPosition(Vector3 InVector)
	{
		ClientPosition += InVector;
	}

	[ClientRpc]
	public void ServerAddVelocity(Vector3 InVector)
	{
		ClientVelocity += InVector;
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
		float AddSpeed = RelativeComponent * BallRestitution;
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

		return FinalVel;
	}

	[ConCmd.Server]
	public static void Spectate()
	{
		Client pl = ConsoleSystem.Caller;
		pl.Pawn.Delete();
		SpectatorPawn NewPawn = new SpectatorPawn();
		pl.Pawn = NewPawn;
	}

	public bool TryDiscreteSteps(float RealDelta, bool DoCollisionResponse, int numSteps)
	{
		bool DidCollisionOccur = false;
		for (int i = 0; i < numSteps; i++)
		{
			ClientPosition += (ClientVelocity * RealDelta) / numSteps;
			bool CollidedThisIteration = TryBallCollisionDiscrete(RealDelta, DoCollisionResponse);
			if (CollidedThisIteration)
			{
				DidCollisionOccur = true;
			}
		}
		return DidCollisionOccur;
	}

	public bool TryBallCollisionDiscrete(float RealDelta, bool DoCollisionResponse)
	{
		string[] IgnoreTags = {"smbtrigger", "nocol"};
		bool CollisionHappened = false;
		for (int i = 0; i < 4; i++)
		{
			TraceResult MoveTrace = Trace.Sphere(10, ClientPosition, ClientPosition).WithTag("solid").WithoutTags(IgnoreTags).IncludeClientside(true).Run();
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
					DepenTrace = Trace.Ray(ClientPosition, MoveTrace.HitPosition).WithTag("solid").WithoutTags(IgnoreTags).IncludeClientside(true).Run();
					if (DepenTrace.Hit)
					{
						//bool BehindFace = Vector3.Dot(ClientPosition - DepenTrace.HitPosition, DepenTrace.Normal) > 0;
						//Log.Info(Vector3.Dot(ClientPosition - DepenTrace.HitPosition, DepenTrace.Normal));
						ClientPosition = DepenTrace.HitPosition + (MoveTrace.Normal * 10.001f);
						UseNormal = DepenTrace.Normal;
						//DebugOverlay.Sphere(DepenTrace.HitPosition, 3, new Color(0,255,0), Time.Delta, false);
					}else
					{
						continue;
					}
				}
				TraceResult FinalTrace = Trace.Sphere(9, ClientPosition, ClientPosition - (MoveTrace.Normal * 2)).WithTag("solid").WithoutTags(IgnoreTags).IncludeClientside(true).Run();

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
		string[] IgnoreTags = {"smbtrigger", "nocol"};
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
			Vector3 TemporaryPosition = OriginalPosition;
			Vector3 TemporaryVelocity = OriginalVelocity;
			Vector3 RealStartPosition = SMBObject.TransformPosition(CheckEnt.UninterpolatedTransform, SMBObject.InverseTransformPosition(CheckEnt.OldTransform, TemporaryPosition));
			TraceResult MoveTrace = Trace.Ray(RealStartPosition, TemporaryPosition + (TemporaryVelocity * RealDelta)).WithTag(CheckEnt.CollisionTag).WithoutTags(IgnoreTags).IncludeClientside(true).Run();
			if (MoveTrace.Hit)
			{
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
		ServerRenderBall.Predictable = true;
		ServerRenderBall.Owner = this;
		RenderBallAng = Rotation.Identity;
		BallCitizen = new AnimatedEntity();
		BallCitizen.SetModel("models/citizen/citizen.vmdl");
		BallCitizen.EnableTouch = false;
		BallCitizen.Predictable = true;
		BallCitizen.Owner = this;
		BallCitizen.Scale = 0.2f;
		BallRestitution = 0.5f;
		BallFriction = 0.54f;
		BallGravity = 588f;
		BallMaxTilt = 23f;
		BallMaxVisualTilt = 13.2f;
	}

	public void GetPlayerStateManager()
	{
		foreach (Entity element in Entity.All)
		{
			if (element is PlayerStateManager)
			{
				if (element.Owner == Owner)
				{
					OurManager = element as PlayerStateManager;
					break;
				}
			}
		}
	}

	public void UpdateCitizenClothing(Client cl)
	{
		CitizenClothing = new ClothingContainer();
		CitizenClothing.LoadFromClient(cl);
		CitizenClothing.DressEntity(BallCitizen);
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
		CitizenRotation = Rotation.Identity;

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
		ConsoleSystem.SetValue("r_farz", 100000);
		ConsoleSystem.SetValue("r_nearz", 16);
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
		AnalogInput = new Vector3(MathX.Clamp(AnalogInput.x * (float)Math.Abs(AnalogInput.x), -1, 1), MathX.Clamp(AnalogInput.y * (float)Math.Abs(AnalogInput.y), -1, 1), 0);
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
					YawTilt = MathX.Lerp(YawTilt, AnalogInput.y, RealDelta * 21);
					PitchTilt = MathX.Lerp(PitchTilt, AnalogInput.x, RealDelta * 21);
					float InX = (float)Math.Pow(Math.Abs(AnalogInput.x), 2) * Math.Sign(AnalogInput.x);
					float InY = (float)Math.Pow(Math.Abs(AnalogInput.y), 2) * Math.Sign(AnalogInput.y);
					GravDir = GravDir.RotateAroundAxis(FixedEyeRot.Right, PitchTilt * -BallMaxTilt);
					GravDir = GravDir.RotateAroundAxis(FixedEyeRot.Forward, YawTilt * -BallMaxTilt);
					GravDir = GravDir.Normal;
				}
				Vector3 AddVelocity = GravDir.Down * BallGravity * RealDelta;
				ClientVelocity += AddVelocity;
				OldVelocity = ClientVelocity;
				Vector3 OldPos = ClientPosition;
				//TryBallCollisionContinuousSphere(RealDelta);
				//ClientPosition = ClientPosition + (ClientVelocity * RealDelta);
				bool DidContinuous = TryBallCollisionContinuous(RealDelta);
				if (!DidContinuous)
				{
					//ClientPosition = ClientPosition + (ClientVelocity * RealDelta);
					TryDiscreteSteps(RealDelta, true, 4);
				}else
				{
					//ClientPosition = ClientPosition + (ClientVelocity * RealDelta);
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
					RelativeVel += -RelativeVel * BallFriction * RealDelta;
					ClientVelocity = RelativeVel + LastVelAtPos;
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
					if (element != null && element.Tags.Has("smbtrigger"))
					{
						SMBObject TriggerEnt = element as SMBObject;
						Vector3 TriggerTestStart = SMBObject.TransformPosition(TriggerEnt.UninterpolatedTransform, SMBObject.InverseTransformPosition(TriggerEnt.OldTransform, OldPosition));
						Trace TriggerTrace = Trace.Ray(TriggerTestStart, ClientPosition);
						TriggerTrace.WithTag("smbtrigger");
						TraceResult[] TriggerTraceResults = TriggerTrace.RunAll();
						if (TriggerTraceResults != null)
						{
							foreach (TraceResult TriggerHit in TriggerTraceResults)
							{
								if (TriggerHit.Entity.Tags.Has("goaltrigger") && BallState != 2)
								{
									GoalPost GoalEnt = TriggerHit.Entity.Owner as GoalPost;
									ChangeBallState(2);
									Particles ImpactParticle = Particles.Create("particles/goalconfetti.vpcf");
									ImpactParticle.SetPosition(0, GoalEnt.PartyBall.Position - (GoalEnt.PartyBall.Rotation.Up * 15));
									ImpactParticle.SetPosition(1, ClientVelocity * 0.75f);
									ImpactParticle.Simulating = true;
									ImpactParticle.EnableDrawing = true;
									float TimeRemaining = GameEnt.StageMaxTime - (Time.Now - GameEnt.FirstHitTime);
									Log.Info("Time remaining = " + TimeRemaining);
									PlayerStateManager.AddScoreFromClient(OurManager.NetworkIdent, (int)(TimeRemaining * 100));
									int ClosestStickIndex = 0;
									float ClosestDist = 1000;
									for (int i = 0; i < GoalEnt.GoalTape.Sticks.Count; i++)
									{	
										TapeStick Stick = GoalEnt.GoalTape.Sticks[i];
										Vector3 StickCentre = (Stick.PointA.Position + Stick.PointB.Position) / 2;
										float Dist = (StickCentre - ClientPosition).Length;
										if (Dist < ClosestDist)
										{
											ClosestDist = Dist;
											ClosestStickIndex = i;
										}
									}
									GoalEnt.GoalTape.Sticks[ClosestStickIndex].PointA.Velocity = ClientVelocity;
									GoalEnt.GoalTape.Sticks[ClosestStickIndex].PointB.Velocity = ClientVelocity;
									GoalEnt.GoalTape.Sticks.RemoveAt(ClosestStickIndex);
									GoalEnt.GoalTape.UpdateRopeMesh(true);
									continue;
								}else
								{
									SMBTrigger CheckEnt = TriggerHit.Entity as SMBTrigger;
									if (Vector3.Dot(TriggerHit.Direction, TriggerHit.Normal) < 0)
									{
										CheckEnt.OnEnterTrigger(this);
									}else
									{
										CheckEnt.OnExitTrigger(this);
									}
								}
							}
						}
					}
				}
			}
		}else
		if (BallState == 1)
		{
			Vector3 AddVelocity = new Vector3(0, 0, -1) * BallGravity * RealDelta;
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
			LastGroundVel += -LastGroundVel * Time.Delta * 2;
		}

		BBox StageBounds = new BBox(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
		foreach (Entity element in Entity.All)
		{
			if (element is SMBObject && !element.Tags.Has("BGObject"))
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
		}

		if (Input.Down( InputButton.Duck ))
		{
			ClientVelocity = new Vector3(ClientVelocity.x, ClientVelocity.y, ClientVelocity.z + (-2000 * RealDelta));
		}

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
			Vector3 SpinAxis = Vector3.Cross(LastGroundNormal, LastGroundVel).Normal;
			Rotation HelperRot = Rotation.FromAxis(SpinAxis, LastGroundVel.Length * Time.Delta * 3);
			RenderBallAng = HelperRot * RenderBallAng;
			ServerRenderBall.Rotation = RenderBallAng.Normal;
			ServerRenderBall.Position = Position;
			ServerRenderBall.ResetInterpolation();
			BallCitizen.Position = Position + (CitizenRotation.Up * -9);
			BallCitizen.ResetInterpolation();
		}else
		{
			ServerRenderBall.EnableDrawing = false;
		}
		if ( Local.Client == null && Input.Pressed( InputButton.Run ))
		{
			GameEnt.NextGameState = Time.Now;
		}


	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );
		RenderBall.Position = ClientPosition;
		Vector3 SpinAxis = Vector3.Cross(LastGroundNormal, LastGroundVel).Normal;
		Rotation HelperRot = Rotation.FromAxis(SpinAxis, LastGroundVel.Length * Time.Delta * 3);
		RenderBallAng = HelperRot * RenderBallAng;
		RenderBall.Rotation = RenderBallAng.Normal;

		Rotation DesiredCitizenRotation = Rotation.LookAt((ClientVelocity * new Vector3(1, 1, 0)) + BallCamAng.ToRotation().Forward);
		float AngleBetween = (CitizenRotation.Inverse * DesiredCitizenRotation).Angle();
		float RotateRate = 2f;
		float Frac = (100 / AngleBetween) * Time.Delta * RotateRate;
		CitizenRotation = Rotation.Slerp(CitizenRotation, DesiredCitizenRotation, Frac).Normal;
		float AmountToSpin = MathX.Lerp(0.25f, 3, (LastGroundVel.Length - 400) / 300, true);
		Rotation HelperRot2 = Rotation.FromAxis(SpinAxis, LastGroundVel.Length * Time.Delta * AmountToSpin);
		CitizenRotation = (HelperRot2 * CitizenRotation).Normal;
		ServerRenderBall.Position = ClientPosition;
		ServerRenderBall.ResetInterpolation();
		BallCitizen.Position = ClientPosition + (CitizenRotation.Up * -9);
		BallCitizen.Rotation = CitizenRotation;
		BallCitizen.ResetInterpolation();
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
				RealVelPitch = MathX.Clamp(RealVelPitch, -68, 68);
				Vector3 VelNoZ = ClientVelocity;
				VelNoZ.z = 0;
				float VelNoZMag = VelNoZ.Length;
				float CamMoveFracVelVert = Math.Min(Math.Abs(ClientVelocity.z * 0.75f) + 100, 500);
				float CamMoveFracVelHoriz;
				float SlowStart = 0.1f;
				float FastStart = 1;
				float MaxSpeed = 100;
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
					if (StageStartTrace.Hit)
					{
						CameraOrigin = StageStartTrace.HitPosition + (StageStartTrace.Normal * 10);
					}
				}
				if (Time.Now < FirstHit + 0.75f)
				{
					NewPitch = 0;
				}
				BallCamAng = new Angles(NewPitch, NewYaw, 0f);
				//YawTilt = MathX.Lerp(YawTilt, AnalogInput.y * 12, RealDelta * 10);
				//PitchTilt = MathX.Lerp(PitchTilt, AnalogInput.x * 12, RealDelta * 10);
				Rotation NewCamRotation = new Angles(NewPitch + 20f, NewYaw, 0f).ToRotation();
				Rotation FixedEyeRot = Rotation.From(0f, NewYaw, 0f);
				Rotation HelperRotPitch = Rotation.FromAxis(FixedEyeRot.Right, PitchTilt * BallMaxVisualTilt);
				Rotation HelperRotYaw = Rotation.FromAxis(FixedEyeRot.Forward, YawTilt * BallMaxVisualTilt);
				MyGame GameEnt = Game.Current as MyGame;
				GameEnt.StageTilt = Rotation.Slerp(GameEnt.StageTilt, HelperRotPitch * HelperRotYaw, Time.Delta * 15f);
				EyeRotation = GameEnt.StageTilt * NewCamRotation;
				//EyeRotation = NewCamRotation;
				//EyeRotation = EyeRotation.Normal;
				float FakeRotOffsetUpDown = (NewPitch * -0.15f) + 10;
				float FakeRotOffsetDist = (NewPitch * 0.15f) + 55;
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
				if (StageStartTrace.Hit)
				{
					CameraOrigin = StageStartTrace.HitPosition + (StageStartTrace.Normal * 10);
				}
				YawTilt = 0;
				PitchTilt = 0;
				Rotation NewCamRotation = BallCamAng.ToRotation() * Rotation.FromPitch(20);
				Vector3 DesiredEyePosition = CameraOrigin - (NewCamRotation.Forward * 55) + (NewCamRotation.Up * 10);
				Rotation DesiredEyeRotation = NewCamRotation.Normal;
				BBox StageBounds = new BBox(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
				foreach (Entity element in Entity.All)
				{
					if (element is SMBObject && !element.Tags.Has("BGObject"))
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
			if (Time.Now > LastStateChange + 2)
			{
				if (!BlastingUp)
				{
					Sound Blastoff = Sound.FromEntity("ball_blastoff", this);
					BlastUpZLock = ClientPosition;
					BlastingUp = true;
				}
			}
			Vector3 DeltaPosition = (ClientPosition - CameraPosition);
			float OurDot = Vector3.Dot(DeltaPosition, CameraVelocity);
			DeltaPosition = DeltaPosition.Normal;
			float AdjustedDelta = Time.Delta * 60;
			float AdjustedDot = OurDot * -0.01f;
			if (!BlastingUp)
			{
				//float OldZ = CameraVelocity.z + (-CameraVelocity.z * Time.Delta);
				//CameraVelocity += DeltaPosition * AdjustedDot * AdjustedDelta;
			}
			if (AdjustedDot > 0)
			{
				CameraVelocity += DeltaPosition * AdjustedDelta * 2.5f;
			}
			CameraVelocity += -CameraVelocity * Time.Delta * 1.25f;
			CameraVelocity += new Vector3(CameraVelocity.Length * 0.01f * DeltaPosition.y * AdjustedDelta, CameraVelocity.Length * 0.01f * DeltaPosition.x * AdjustedDelta, 0);
			//CameraVelocity += -CameraVelocity * Time.Delta;
			CameraPosition = CameraPosition + (CameraVelocity * Time.Delta);
			Vector3 CameraPivot = (ClientPosition + new Vector3(0, 0, 15));
			Vector3 DeltaPivot = (CameraPosition - CameraPivot);
			float PivotDist = DeltaPivot.Length;
			if (PivotDist > 0.00001f && !BlastingUp)
			{
				Vector3 Dir = DeltaPivot.Normal;
				CameraPosition = CameraPivot + (DeltaPivot.Normal * MathX.Lerp(PivotDist, 40, Time.Delta * 2));
			}
			if (!BlastingUp)
			{
				CameraPosition += new Vector3(0, 0, CameraPivot.z - CameraPosition.z) * 0.1f;
			}
			CameraRotation = Rotation.Slerp(CameraRotation, Rotation.LookAt(-(CameraPosition - ClientPosition), new Vector3(0, 0, 1)), Time.Delta * 15, true);
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
