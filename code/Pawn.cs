using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public partial class Pawn : ModelEntity
{
	/// <summary>
	/// Called when the entity is first created
	/// </summary>

	[Net]
	public float LatestNow {get;set;}

	[Net]
	public PlayerStateManager OurManager {get;set;}

	[Net]
	public float BallRestitution {get;set;} = 0.5f;

	[Net]
	public float BallFriction {get;set;} = 0.54f;

	[Net]
	public float BallGravity {get;set;} = 588f;

	[Net]
	public float BallMaxTilt {get;set;} = 23f;

	[Net]
	public Vector3 BallServerOldPos{get;set;}

	[Net]
	public Vector3 BallServerUninterpolatedPos{get;set;}

	[Net]
	public Vector3 BallServerUninterpolatedVel{get;set;}

	[Net]
	public string ClothingString {get;set;}

	[Net]
	public bool HasClothesString {get;set;} = false;

	[Net]
	public bool Ready {get;set;} = false;

	public AnimatedEntity BallCitizen {get;set;}

	public Vector3 StoredAnalogInput {get;set;}

	public List<QueuedSpark> QueuedSparks {get;set;}

	public List<QueuedCollisionStar> QueuedStars {get;set;}

	public ModelEntity ClientsideModelGeneric {get;set;}

	public AnimatedEntity ReadyGo {get;set;}

	[Net]
	public Rotation ServerCitizenRotation {get;set;}

	public Rotation CitizenRotation {get;set;}

	public ClothingContainer CitizenClothing {get;set;}

	[Net]
	public Vector3 LastGroundNormalServer {get;set;}

	[Net]
	public Vector3 LastGroundVelServer {get;set;}

	public Vector3 LastGroundNormal {get;set;}

	public Vector3 LastGroundVel {get;set;}

	public Vector3 LastVelAtPos {get;set;}

	public Vector3 OldVelocity {get;set;}

	public Vector3 OldPosition {get;set;}

	public Rotation TrueGravityOrientation {get;set;} = Rotation.Identity;

	public Rotation InterpolatedGravityOrientation {get;set;} = Rotation.Identity;

	public float AirTime {get;set;}

	public float GroundTime {get;set;}

	public float LastHit {get;set;}

	public float YawTilt {get;set;}

	public float PitchTilt {get;set;}

	public float LastStateChange {get;set;}

	public float SpawnTime {get;set;}

	public ModelEntity RenderBall {get;set;}

	public Rotation RenderBallAng {get;set;}

	[Net]
	public Rotation RenderBallAngServer {get;set;}

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

	public float GoalTime {get;set;} = 0;

	public bool AboutToGainControl {get;set;}

	public float NextSpark {get;set;}

	public bool NoCollide {get;set;}

	public Angles LookOffset {get;set;}

	public Angles AnalogLookReal {get;set;}

	public float LastCameraMoveTime {get;set;}

	public bool FirstPlay {get;set;} = true;

	public bool BlastoffSoundPlayed {get;set;} = false;

	public Vector3 SRBPos {get;set;}

	public Vector3 SRBVel {get;set;}

	public bool Clothed {get;set;}

	public BBox StageBounds {get;set;}

	public float TrueSpawnTime {get;set;}


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
	public void CreateClientCollisionParticles(float RelativeComponent, Vector3 HitPosition, Vector3 HitNormal, Vector3 FinalVel)
	{
		if (RelativeComponent > 150)
		{
			int NumStars = (int)(RelativeComponent / 64);
			for (int i = 0; i < NumStars; i++)
			{
				Vector3 StarVel = (HitNormal * RelativeComponent * 0.125f) + (FinalVel * 0.35f);
				StarVel += Vector3.Random * RelativeComponent * 0.4f;
				CreateStar(HitPosition + (HitNormal * 2f), StarVel, 0.1f);
			}
			int NumSparks = (int)(RelativeComponent / 24);
			for (int i = 0; i < NumSparks; i++)
			{
				CreateSpark(HitPosition + HitNormal, (Vector3.Random * RelativeComponent * 0.5f) + (FinalVel * 0.5f), 0.125f, "materials/spark.vmat");
			}
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
		Pawn BallWeHit = HitEntity as Pawn;
		if (BallWeHit != null)
		{
			VelAtPos = BallWeHit.Velocity;
			RelativeVel = InVelocity - VelAtPos;
		}
		Ball.LastGroundNormalServer = HitNormal;
		Ball.LastGroundVelServer = RelativeVel - (HitNormal * Vector3.Dot(RelativeVel, HitNormal));
		Ball.LastHit = Time.Now;
		float RelativeComponent = Vector3.Dot(RelativeVel, HitNormal);
		if (RelativeComponent > 0)
		{
			return;
		}
		if (BallWeHit != null)
		{
			BallWeHit.ApplyCollisionResponseFromServer(InVelocity, -HitNormal, Ball, HitPosition, Global.TickInterval, false);
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
					Ball.CreateClientCollisionParticles(To.Single(player), RelativeComponent, HitPosition, HitNormal, FinalVel);
				}
			}
		}
		if (Ball.LastGroundVelServer.LengthSquared > 160000 && Time.Now > Ball.NextSpark)
		{
			float RelVelLen = Ball.LastGroundVelServer.Length;
			float SparkSpeedRatio = (MathX.Clamp(RelVelLen, 400, 800) - 400) / 400;
			float VelInherit = MathX.Lerp(0.85f, 0.7f, SparkSpeedRatio);
			float RandomAdd = MathX.Lerp(150, 350, SparkSpeedRatio);
			for (int i = 0; i < (int)((SparkSpeedRatio * 3) + 1); i++)
			{
				Ball.CreateSparkFromServer(HitPosition + HitNormal, (Ball.LastGroundVelServer * VelInherit) + (HitNormal * 110) + (Vector3.Random * RandomAdd), 0.125f, "materials/spark.vmat");
			}
			Ball.NextSpark = Time.Now + 0.015f;
		}
		Ball.LatestNow = InNow;
		Ball.Velocity = FinalVel;
		Ball.BallServerUninterpolatedVel = FinalVel;
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
		Ball.BallServerUninterpolatedVel = InVelocity;
	}

	public void CreateSpark(Vector3 InPosition, Vector3 InVelocity, float InSize, string InTexture)
	{
		if (QueuedSparks == null)
		{
			QueuedSparks = new List<QueuedSpark>();
		}
		QueuedSpark TempQueuedSpark = new QueuedSpark(InPosition, InVelocity, InSize, InTexture);
		QueuedSparks.Add(TempQueuedSpark);
	}

	public void CreateStar(Vector3 InPosition, Vector3 InVelocity, float InSize)
	{
		if (QueuedStars == null)
		{
			QueuedStars = new List<QueuedCollisionStar>();
		}
		QueuedCollisionStar TempQueuedStar = new QueuedCollisionStar(InPosition, InVelocity, InSize);
		QueuedStars.Add(TempQueuedStar);
	}

	[ClientRpc]
	public void CreateSparkFromServer(Vector3 InPosition, Vector3 InVelocity, float InSize, string InTexture)
	{
		if (QueuedSparks == null)
		{
			QueuedSparks = new List<QueuedSpark>();
		}
		QueuedSpark TempQueuedSpark = new QueuedSpark(InPosition, InVelocity, InSize, InTexture);
		QueuedSparks.Add(TempQueuedSpark);
	}

	[ClientRpc]
	public void CreateStarFromServer(Vector3 InPosition, Vector3 InVelocity, float InSize)
	{
		if (QueuedStars == null)
		{
			QueuedStars = new List<QueuedCollisionStar>();
		}
		QueuedCollisionStar TempQueuedStar = new QueuedCollisionStar(InPosition, InVelocity, InSize);
		QueuedStars.Add(TempQueuedStar);
	}

	[ClientRpc]
	public void ApplyCollisionResponseFromServer(Vector3 InVelocity, Vector3 HitNormal, Entity HitEntity, Vector3 HitPosition, float RealDelta, bool SendToServer)
	{
		ClientVelocity = ApplyCollisionResponse(InVelocity, HitNormal, HitEntity, HitPosition, RealDelta, SendToServer);
	}

	public Vector3 ApplyCollisionResponse(Vector3 InVelocity, Vector3 HitNormal, Entity HitEntity, Vector3 HitPosition, float RealDelta, bool SendToServer = true)
	{
		float RestitutionMult = 1f;
		if (HitEntity is PartyBall)
		{
			GoalPost Post = HitEntity.Owner as GoalPost;
			HitNormal = (HitNormal + (InVelocity.Normal * 0.5f)).Normal;
			float RelativeComponentPB = Vector3.Dot( ClientVelocity, HitNormal );
			Post.PartyBallSimVelocity += HitNormal * RelativeComponentPB * -0.12f;
			RestitutionMult = 0f;
		}
		Pawn Ball = HitEntity as Pawn;
		Vector3 UseVelocity = InVelocity;
		if (Ball != null)
		{
			UseVelocity = ClientVelocity;
		}
		SMBObject HitEntitySMB = HitEntity as SMBObject;
		Vector3 FinalVel = UseVelocity;
		Vector3 RelativeVel = UseVelocity;
		Vector3 VelAtPos = new Vector3(0,0,0);
		if (HitEntitySMB != null)
		{
			VelAtPos = HitEntitySMB.GetVelocityAtPoint(HitPosition, Global.TickInterval);
			RelativeVel = UseVelocity - VelAtPos;
			if (HitEntitySMB.OnCollideMaster(this, VelAtPos, InVelocity, HitNormal, HitPosition, RealDelta) == false)
			{
				return ClientVelocity;
			}
		}
		if (Ball != null)
		{
			if (NoCollide)
			{
				return UseVelocity;
			}
			RestitutionMult = 0f;
			if (!SendToServer)
			{
				VelAtPos = InVelocity;
				RelativeVel = UseVelocity - VelAtPos;
			}else
			{
				VelAtPos = Ball.Velocity;
				RelativeVel = UseVelocity - VelAtPos;
			}
		}
		if (HitEntity == null)
		{
			return UseVelocity;
		}
		LastGroundNormal = HitNormal;
		LastGroundVel = RelativeVel - (HitNormal * Vector3.Dot(RelativeVel, HitNormal));
		LastHit = Time.Now;
		float RelativeComponent = Vector3.Dot(RelativeVel, HitNormal);
		if (RelativeComponent > 0)
		{
			return UseVelocity;
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
		float AddSpeed = RelativeComponent * (BallRestitution * RestitutionMult);
		AddSpeed = Math.Max(0, AddSpeed - 10);
		FinalVel += HitNormal * (ResultSpeed + AddSpeed);
		LastVelAtPos = VelAtPos;
		if (RelativeComponent > 150)
		{
			//todo: limit sparks and stars to a sane amount
			//in order to prevent stutter on hard collisions
			int NumStars = (int)Math.Min((RelativeComponent / 64), 24);
			for (int i = 0; i < NumStars; i++)
			{
				Vector3 StarVel = (HitNormal * RelativeComponent * 0.125f) + (FinalVel * 0.35f);
				StarVel += Vector3.Random * RelativeComponent * 0.4f;
				CreateStar(HitPosition + (HitNormal * 2f), StarVel, 0.1f);
			}

			int NumSparks = (int)Math.Min((RelativeComponent / 24), 50);
			for (int i = 0; i < NumSparks; i++)
			{
				CreateSpark(HitPosition + HitNormal, (Vector3.Random * RelativeComponent * 0.5f) + (FinalVel * 0.5f), 0.125f, "materials/spark.vmat");
			}

		}
		if (LastGroundVel.LengthSquared > 160000 && Time.Now > NextSpark)
		{
			float RelVelLen = LastGroundVel.Length;
			float SparkSpeedRatio = (MathX.Clamp(RelVelLen, 400, 800) - 400) / 400;
			float VelInherit = MathX.Lerp(0.85f, 0.7f, SparkSpeedRatio);
			float RandomAdd = MathX.Lerp(150, 350, SparkSpeedRatio);
			for (int i = 0; i < (int)((SparkSpeedRatio * 3) + 1); i++)
			{
				CreateSpark(HitPosition + HitNormal, (LastGroundVel * VelInherit) + (HitNormal * 110) + (Vector3.Random * RandomAdd), 0.125f, "materials/spark.vmat");
			}
			NextSpark = Time.Now + 0.015f;
		}
		if (HitEntity != null && SendToServer)
		{
			if ((Time.Now > LastServerMessage + 0.033 && HitEntity is Pawn) | Time.Now > LastServerMessage + 0.1 | RelativeComponent > 150)
			{
				SendServerCollision(UseVelocity, HitNormal, HitEntity.NetworkIdent, HitPosition, Time.Now);
				LastServerMessage = Time.Now;
			}
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

	[ConCmd.Server]
	public static void SetPlayerClothes(string InJson)
	{
		Pawn Ball = ConsoleSystem.Caller.Pawn as Pawn;
		if (Ball != null)
		{
			Ball.ClothingString = InJson;
			Ball.HasClothesString = true;
		}
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
		NoCollide = true;
		RenderBallAngServer = Rotation.Identity;
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

	[ClientRpc]
	public void UpdateCitizenClothing(Client cl)
	{
		if (Clothed)
		{
			return;
		}
		if (BallCitizen == null)
		{
			return;
		}
		if (this == null)
		{
			return;
		}
		CitizenClothing = new ClothingContainer();
		CitizenClothing.Deserialize(ClothingString);
		CitizenClothing.DressEntity(BallCitizen, false, false);
		Clothed = true;
	}

	public override void ClientSpawn()
	{
		base.Spawn();
		Clothed = false;
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
		QueuedSparks = new List<QueuedSpark>();
		QueuedStars = new List<QueuedCollisionStar>();
		NoCollide = true;
		LookOffset = Angles.Zero;
		FirstPlay = true;
		TrueSpawnTime = Time.Now;
		SRBPos = Vector3.Zero;
		TrueGravityOrientation = Rotation.Identity;
		BallCitizen = new AnimatedEntity();
		BallCitizen.SetModel("models/citizen/citizen.vmdl");
		BallCitizen.EnableTouch = false;
		BallCitizen.Predictable = true;
		BallCitizen.Owner = this;
		BallCitizen.Scale = 0.2f;
		BallCitizen.AnimateOnServer = false;
		BallCitizen.AnimGraph = AnimationGraph.Load("models/citizen/citizen_ro.vanmgrph");
		if (GameEnt.CurrentGameState == 0)
		{
			ClientsideModelGeneric = new ModelEntity();
			ClientsideModelGeneric.SetModel("models/status_notready.vmdl");
			ClientsideModelGeneric.EnableShadowCasting = false;
		}
	}

	protected override void OnDestroy()
	{
		if (RenderBall != null)
		{
			RollingWoop.Stop();
			RollingGrind.Stop();
			RenderBall.Delete();
			BallCitizen.Delete();
		}
		if (ClientsideModelGeneric != null)
		{
			ClientsideModelGeneric.Delete();
		}
		if (ReadyGo != null)
		{
			ReadyGo.Delete();
		}

	}

	public override void Touch( Entity OtherEnt )
	{
		Log.Info("Hi");
	}

	public void RespawnBall(bool FirstRespawn)
	{
		if (Owner as Client != Local.Client)
		{
			return;
		}
		RollingWoop.Stop();
		RollingGrind.Stop();
		ClientVelocity = new Vector3(0, 0, 0);
		ControlEnabled = false;
		GameEnt = Game.Current as MyGame;
		ClientPosition = GameEnt.CurrentSpawnPos + new Vector3(0, 0, 60);
		BallCamAng = GameEnt.CurrentSpawnRot;
		CitizenRotation = GameEnt.CurrentSpawnRot.ToRotation();
		OldPosition = ClientPosition;
		ChangeBallState(0);
		SpawnTime = Time.Now - 3.5f;
		FirstPlay = false;
		if (FirstRespawn)
		{
			SpawnTime = Time.Now;
			FirstPlay = true;
			GameEnt.HasFirstHit = false;
			GameEnt.FirstHitTime = 0;
			FirstHit = 0;
		}
		TrueSpawnTime = Time.Now;
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
		ConsoleSystem.SetValue("r_farz", 5000000);
		ConsoleSystem.SetValue("r_nearz", 16);
		ConsoleSystem.SetValue("csm_cascade0_override_dist", 256);
		ConsoleSystem.SetValue("csm_cascade1_override_dist", 1024);
		ConsoleSystem.SetValue("csm_cascade2_override_dist", 4096);
		ConsoleSystem.SetValue("csm_cascade3_override_dist", 16384);
		TrueGravityOrientation = Rotation.Identity;
		NoCollide = true;
		LookOffset = Angles.Zero;
		SetPlayerClothes(ConsoleSystem.GetValue( "avatar" ));
		GetStageBounds();
		if (GameEnt.CurrentGameState != 0)
		{
			if (ClientsideModelGeneric != null)
			{
				ClientsideModelGeneric.Delete();
			}
			ClientsideModelGeneric = new ModelEntity();
			ClientsideModelGeneric.SetModel("models/plane.vmdl");
			ClientsideModelGeneric.SetMaterialOverride(Material.Load("materials/screenoverlay.vmat"));
			ClientsideModelGeneric.EnableShadowCasting = false;
		}
		BallRestitution = ConsoleSystem.GetValue( "smb_ball_restitution" ).ToFloat();
		BallFriction = ConsoleSystem.GetValue( "smb_ball_friction" ).ToFloat();
		BallGravity = ConsoleSystem.GetValue( "smb_ball_gravity" ).ToFloat();
		BallMaxTilt = ConsoleSystem.GetValue( "smb_ball_gravity_tilt" ).ToFloat();
	}

	public void GetStageBounds()
	{
		if (GameEnt.StageBounds.Volume == 0)
		{
			StageBounds = new BBox(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
			foreach (Entity element in Entity.All)
			{
				if (element is SMBObject && !element.Tags.Has("BGObject"))
				{
					StageBounds = StageBounds.AddPoint(element.WorldSpaceBounds.Mins);
					StageBounds = StageBounds.AddPoint(element.WorldSpaceBounds.Maxs);
				}
			}
		}else
		{
			StageBounds = GameEnt.StageBounds;
		}
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

	[ClientRpc]
	public void ServerChangeBallState(int InState)
	{
		ChangeBallState(InState);
	}

	public virtual void RotateGravityToTargetDirection(Vector3 InDir)
	{
		Vector3 CurDir = TrueGravityOrientation.Up;
		Vector3 Axis = Vector3.Cross(CurDir, InDir).Normal;
		TrueGravityOrientation = (Rotation.FromAxis(Axis, Vector3.GetAngle(InDir, CurDir)) * TrueGravityOrientation).Normal;
	}

	public virtual Vector3 TickBallMovement(Vector3 AnalogInput)
	{
		float RealDelta = Math.Min(Global.TickInterval, Time.Delta);
		AnalogInput = new Vector3(AnalogInput.x * (float)Math.Abs(AnalogInput.x), AnalogInput.y * (float)Math.Abs(AnalogInput.y), 0);
		AnalogInput *= LookOffset.WithPitch(0).ToRotation();
		if (Input.Down(InputButton.Forward) | Input.Down(InputButton.Back) | Input.Down(InputButton.Left) | Input.Down(InputButton.Right))
		{
			AnalogInput *= 1.5f;
		}
		AnalogInput = new Vector3(MathX.Clamp(AnalogInput.x, -1, 1), MathX.Clamp(AnalogInput.y, -1, 1), 0);

		if (BallState == 0)
		{
			if (Time.Now > SpawnTime + 5)
			{
				EnableDrawing = true;
				RenderBall.EnableDrawing = true;
				Rotation FixedEyeRot = TrueGravityOrientation * Rotation.From(0f, BallCamAng.yaw, 0f);
				//physics

				Rotation GravDir = TrueGravityOrientation * new Rotation(0f, 0f, -1f, 0f);
				if (ControlEnabled == true)
				{
					YawTilt = MathX.Lerp(YawTilt, AnalogInput.y, RealDelta * 21);
					PitchTilt = MathX.Lerp(PitchTilt, AnalogInput.x, RealDelta * 21);
					float InX = (float)Math.Pow(Math.Abs(AnalogInput.x), 2) * Math.Sign(AnalogInput.x);
					float InY = (float)Math.Pow(Math.Abs(AnalogInput.y), 2) * Math.Sign(AnalogInput.y);

					//Vector3 YawAxis = Vector3.Lerp(FixedEyeRot.Forward, FixedEyeRot.Up, (float)Math.Max(BallCamAng.pitch / 88, 0));
					Rotation PitchTiltRotation = Rotation.FromAxis(FixedEyeRot.Right, PitchTilt * BallMaxTilt);
					Rotation YawTiltRotation = Rotation.FromAxis(FixedEyeRot.Forward, YawTilt * BallMaxTilt);
					//Rotation PitchTiltRotationReal = Rotation.FromAxis(FixedEyeRot.Right, PitchTilt * Time.Delta * 90);
					//Rotation YawTiltRotationReal = Rotation.FromAxis(FixedEyeRot.Forward, YawTilt * Time.Delta * 90);
					//GravDir = GravDir.RotateAroundAxis(FixedEyeRot.Right, PitchTilt * -BallMaxTilt);
					//GravDir = GravDir.RotateAroundAxis(FixedEyeRot.Forward, YawTilt * -BallMaxTilt);
					//RotateGravityToTargetDirection(TrueGravityOrientation.Up * PitchTiltRotationReal * YawTiltRotationReal);
					GravDir = PitchTiltRotation * YawTiltRotation * GravDir;
					GravDir = GravDir.Normal;
					//DebugOverlay.Line(ClientPosition, ClientPosition - (TrueGravityOrientation.Up * 20), Time.Delta, true);
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

				foreach (Entity element in Entity.All)
				{
					if (element is Pawn && element != this && !NoCollide)
					{
						Pawn Ball = element as Pawn;
						if (Vector3.DistanceBetweenSquared(ClientPosition, Ball.SRBPos) < 400)
						{
							//Log.Info(Vector3.DistanceBetweenSquared(ClientPosition, Ball.Position));
							Vector3 HitNormal = (ClientPosition - Ball.SRBPos).Normal;
							if (HitNormal.LengthSquared == 0)
							{
								HitNormal = Vector3.Up;
							}
							ClientPosition = Ball.SRBPos + (HitNormal * 20.001f);
							Vector3 HitPos = Ball.SRBPos + (HitNormal * 10);
							DebugOverlay.Sphere(HitPos, 1, new Color(255,0,0), Time.Delta, false);
							DebugOverlay.Line(HitPos, HitPos + (HitNormal * 16), new Color(255,255,255), Time.Delta, false);
							ClientVelocity = ApplyCollisionResponse(ClientVelocity, HitNormal, Ball, HitPos, RealDelta);
							break;
						}
					}
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
					RollingWoop.SetVolume(MathX.Clamp(MathX.Remap(RelativeVel.Length, 50, 400, 0, 0.25f), 0, 0.25f));
					RollingWoop.SetPitch(MathX.Clamp(MathX.Remap(RelativeVel.Length, 50, 600, 0.3f, 2.25f), 0.25f, 2));
					RollingGrind.SetVolume(MathX.Clamp(MathX.Remap(RelativeVel.Length, 300, 600, 0, 0.5f), 0, 0.5f));
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
						TriggerTrace.WithTag(TriggerEnt.CollisionTag);
						TraceResult[] TriggerTraceResults = TriggerTrace.RunAll();
						if (TriggerTraceResults != null)
						{
							foreach (TraceResult TriggerHit in TriggerTraceResults)
							{
								if (TriggerHit.Entity.Tags.Has("goaltrigger") && BallState != 2)
								{
									GoalPost GoalEnt = TriggerHit.Entity.Owner as GoalPost;
									GoalEnt.PartyBallSimVelocity = ClientVelocity * 0.33f;
									ChangeBallState(2);
									//Particles ImpactParticle = Particles.Create("particles/goalconfetti.vpcf");
									//ImpactParticle.SetPosition(0, GoalEnt.PartyBall.Position - (GoalEnt.PartyBall.Rotation.Up * 15));
									//ImpactParticle.SetPosition(1, ClientVelocity * 0.75f);
									//ImpactParticle.Simulating = true;
									//ImpactParticle.EnableDrawing = true;
									float TimeRemaining = GameEnt.StageMaxTime - (Time.Now - GameEnt.FirstHitTime);
									GoalTime = TimeRemaining;
									string TimeInSeconds = ((float)Math.Floor(TimeRemaining)).ToString();
									string Milliseconds = ((float)(Math.Floor((TimeRemaining % 1) * 1000) / 1000)).ToString();
									if (Milliseconds.Length > 1)
									{
										Milliseconds = Milliseconds.Substring(1, Milliseconds.Length - 1);
									}
									if (Milliseconds.Length > 4)
									{
										Milliseconds = Milliseconds.Substring(0, 4);
									}
									if (Milliseconds.Length < 4)
									{
										Milliseconds = Milliseconds + "0";
									}

									MyGame.SendKillfeedEntry( Local.Client.PlayerId.ToString(), Local.Client.Name, TimeInSeconds + Milliseconds, "" );

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
			}else
			{
				ClientPosition = GameEnt.CurrentSpawnPos + new Vector3(0, 0, 60);
				BallCamAng = GameEnt.CurrentSpawnRot;
				CitizenRotation = GameEnt.CurrentSpawnRot.ToRotation();
				OldPosition = ClientPosition;
				NoCollide = true;
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

		float FalloutHeight = StageBounds.Mins.z - 50;
		if (GameEnt.FalloutHeight != 0)
		{
			FalloutHeight = GameEnt.FalloutHeight;
		}
		if ( ClientPosition.z < FalloutHeight && BallState == 0)
		{
			ChangeBallState(1);
		}

		if ( ControlEnabled && BallState != 2 && Input.Pressed( InputButton.Reload ) )
		{
			if (Client.All.Count == 1)
			{
			}
			else
			{
				RespawnBall( false );
			}
			Sound.FromEntity("fx_select", this);
		}

		if (Local.Client.PlayerId == 76561197997644728L)
		{
			if (ControlEnabled && Input.Down( InputButton.Jump ))
			{
				ClientVelocity += new Vector3(0, 0, 1000 * RealDelta) * TrueGravityOrientation;
			}
			if (Input.Pressed( InputButton.Duck ))
			{
				Log.Info(ClientPosition);
			}
		}

		//foreach (QueuedSpark Spark in QueuedSparks)
		//{
		//	ScreenSpaceParticleTrail TestParticle = new ScreenSpaceParticleTrail();
		//	TestParticle.Instantiate(Spark.Position, Spark.Velocity, Spark.Size, Spark.Texture);
		//}
		//foreach (QueuedCollisionStar Star in QueuedStars)
		//{
		//	SMBModelParticle NewStar = new SMBModelParticle();
		//	NewStar.Instantiate(Star.Position, Star.Velocity, Star.Size);
		//}
		//QueuedSparks.Clear();
		//QueuedStars.Clear();

		OldPosition = ClientPosition;
		return ClientPosition;
	}

	[Event.Frame]
	public void DoBallFX()
	{
		if (Owner == Local.Client)
		{
			return;
		}

		if (QueuedSparks == null)
		{
			QueuedSparks = new List<QueuedSpark>();
		}
		if (QueuedStars == null)
		{
			QueuedStars = new List<QueuedCollisionStar>();
		}
		foreach (QueuedSpark Spark in QueuedSparks)
		{
			ScreenSpaceParticleTrail TestParticle = new ScreenSpaceParticleTrail();
			TestParticle.Instantiate(Spark.Position, Spark.Velocity, Spark.Size, Spark.Texture);
		}
		foreach (QueuedCollisionStar Star in QueuedStars)
		{
			SMBModelParticle NewStar = new SMBModelParticle();
			NewStar.Instantiate(Star.Position, Star.Velocity, Star.Size);
		}
		QueuedSparks.Clear();
		QueuedStars.Clear();
	}

	public override void BuildInput( InputBuilder InputBuilderStruct)
	{
		StoredAnalogInput = InputBuilderStruct.AnalogMove;
		InputBuilderStruct.Position = ClientPosition;
		InputBuilderStruct.ViewAngles = BallCamAng;
		AnalogLookReal = InputBuilderStruct.AnalogLook;
		if (AnalogLookReal != new Angles(0,0,0))
		{
			LastCameraMoveTime = Time.Now;
		}
		//InputBuilderStruct.InputDirection = new Vector3(InputBuilderStruct.AnalogLook.pitch * 0.01f, InputBuilderStruct.AnalogLook.yaw * 0.01f, InputBuilderStruct.AnalogLook.roll * 0.25f);
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		BallServerOldPos = Position;
		Position = Input.Position;
		BallServerUninterpolatedPos = Input.Position;
		BallCamAng = Input.Rotation.Angles();

		if (Velocity.LengthSquared > 1)
		{
			bool WantToEnableCollision = true;
			if (Vector3.DistanceBetweenSquared(Position, GameEnt.CurrentSpawnPos) < 62500)
			{
				WantToEnableCollision = false;
			}
			if (WantToEnableCollision && NoCollide)
			{
				foreach (Entity element in Entity.All)
				{
					if (element is Pawn && element != this)
					{
						Pawn Ball = element as Pawn;
						if (Vector3.DistanceBetweenSquared(Position, Ball.Position) < 484)
						{
							WantToEnableCollision = false;
							break;
						}
					}
				}
			}
			if (WantToEnableCollision && NoCollide && ConsoleSystem.GetValue( "smb_playercollision" ).ToBool())
			{
				NoCollide = false;
			}
		}

		if ( Local.Client == null )
		{
			Vector3 SpinAxis = Vector3.Cross(LastGroundNormalServer, LastGroundVelServer).Normal;
			Rotation HelperRot = Rotation.FromAxis(SpinAxis, LastGroundVelServer.Length * Time.Delta * 3);
			RenderBallAngServer = (HelperRot * RenderBallAngServer).Normal;
			Vector3 CitizenRotVel = (Velocity + (BallCamAng.ToRotation().Forward * 5) + new Vector3(0.001f, 0.001f, 0)) * new Vector3(1, 1, 0);
			Rotation DesiredCitizenRotation = Rotation.LookAt(CitizenRotVel, TrueGravityOrientation.Up);
			float AngleBetween = (ServerCitizenRotation.Inverse * DesiredCitizenRotation).Angle();
			float RotateRate = 2f;
			float Frac = (100 / AngleBetween) * Time.Delta * RotateRate;
			ServerCitizenRotation = Rotation.Slerp(ServerCitizenRotation, DesiredCitizenRotation, Frac).Normal;
			float AmountToSpin = MathX.Lerp(0.25f, 3, (LastGroundVelServer.Length - 400) / 300, true);
			Rotation HelperRot2 = Rotation.FromAxis(SpinAxis, LastGroundVelServer.Length * Time.Delta * AmountToSpin);
			ServerCitizenRotation = (HelperRot2 * ServerCitizenRotation).Normal;
		}
		using ( Prediction.Off() )
		{
			if ( cl.IsListenServerHost )
			{
				if ( Input.Pressed( InputButton.Slot0 ) )
				{
					GameEnt.NextGameState = Time.Now;
				}
				if ( Input.Pressed( InputButton.Flashlight ) )
				{
					GameEnt.NextGameState += 60;
					GameEnt.StageMaxTime += 60;
				}
				if ( Local.Client == null )
				{
					if ( cl.PlayerId == 76561197997644728L ) // cheats for Twilight!
					{
						if ( Input.Pressed( InputButton.Duck ) )
						{
							GameEnt.CurrentStage.AddBumper( Position + (new Angles( 0, BallCamAng.yaw, 0 ).ToRotation().Forward * 100) - new Vector3( 0, 0, 10 ), Rotation.Identity );
						}
					}
					if ( Client.All.Count == 1 )
					{
						if ( Input.Down( InputButton.Jump ) && Time.Now < GameEnt.LastGameStateChange + 3.5f )
						{
							GameEnt.SetTimescale( 2.5f );
						}
						if ( Input.Released( InputButton.Jump ) && Time.Now < GameEnt.LastGameStateChange + 3.5f )
						{
							GameEnt.SetTimescale( 1f );
						}
						if ( Input.Pressed( InputButton.Reload ) ) // singleplayer full retry
						{
							GameEnt.PlaySpecificStageInCourse( GameEnt.CurrentCourse, GameEnt.StageInCourse );
							GameEnt.SetTimescale( 4 );
							GameEnt.SpawnAllBalls();
							Sound.FromEntity( "fx_select", this );
						}
					}
					if ( Time.Now > GameEnt.LastGameStateChange + 3.5f )
					{
						GameEnt.SetTimescale( 1 );
					}
				}
			}
		}

		if ( Input.Pressed( InputButton.Jump ))
		{
			Ready = !Ready;
		}

	}

	[Event.Frame]
	public void SmoothServerBallPos()
	{
		if (!Clothed && HasClothesString)
		{
			UpdateCitizenClothing(Owner as Client);
		}

		if (Owner == Local.Client as Entity)
		{
			return;
		}
		float Ping = (float)((Owner as Client).Ping + Local.Client.Ping) * 0.001f;
		RenderBall.EnableDrawing = true;
		//SRBVel = Vector3.Lerp(SRBVel, BallServerUninterpolatedVel, Time.Delta * 30, true);
		SRBPos = Vector3.Lerp(SRBPos, BallServerUninterpolatedPos, Time.Delta * 30, true);
		RenderBall.Rotation = RenderBallAngServer;
		RenderBall.Position = SRBPos;
		BallCitizen.Position = SRBPos + (ServerCitizenRotation.Up * -9);
		BallCitizen.Rotation = ServerCitizenRotation;

		var dir = LastGroundVelServer;
		var forward = Vector3.Dot(BallCitizen.Rotation.Forward, dir);
		var sideward = Vector3.Dot(BallCitizen.Rotation.Right, dir );

		var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

		BallCitizen.SetAnimParameter( "move_direction", angle );
		BallCitizen.SetAnimParameter( "move_speed", LastGroundVelServer.Length );
		BallCitizen.SetAnimParameter( "move_groundspeed", LastGroundVelServer.WithZ( 0 ).Length );
		BallCitizen.SetAnimParameter( "move_y", sideward );
		BallCitizen.SetAnimParameter( "move_x", forward );
		BallCitizen.SetAnimParameter( "move_z", LastGroundVelServer.z );
		BallCitizen.SetAnimParameter( "b_grounded", LastGroundVelServer.Length < 500 && Trace.Ray(SRBPos, SRBPos + new Vector3(0, 0, -1000)).WithTag("solid").IncludeClientside(true).Run().Hit);


		if (GameEnt.CurrentGameState == 0 && ClientsideModelGeneric != null)
		{
			if (Ready)
			{
				ClientsideModelGeneric.SetModel("models/status_ready.vmdl");
			}else
			{
				ClientsideModelGeneric.SetModel("models/status_notready.vmdl");
			}
			ClientsideModelGeneric.Position = SRBPos + new Vector3(0,0,15);
			ClientsideModelGeneric.Rotation = CurrentView.Rotation * Rotation.FromYaw(180);
		}

	}

	[Event.PreRender]
	public void PreRenderBall()
	{
		if (GameEnt.CurrentGameState != 0 && ClientsideModelGeneric != null)
		{
			ClientsideModelGeneric.SceneObject.Transform = new Transform(CurrentView.Position + (CurrentView.Rotation.Forward * 20), CurrentView.Rotation, 3);
			float SpawnTimeRatio = MathX.Clamp(0.6f - (Time.Now - TrueSpawnTime), 0, 0.6f) * 1.666f;
			SpawnTimeRatio *= SpawnTimeRatio * SpawnTimeRatio;
			ClientsideModelGeneric.RenderColor = new Color(1, 1, 1, SpawnTimeRatio);
		}
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );
		bool ManualCameraEnabled = ConsoleSystem.GetValue( "smb_manualcamera" ).ToBool();
		TickBallMovement(StoredAnalogInput);
		float RealDelta = Math.Min(Global.TickInterval, Time.Delta);

		//if (true)
		//{
		//	float NewYaw = BallCamAng.yaw;
		//	Rotation FixedEyeRot = TrueGravityOrientation * Rotation.From(0f, NewYaw, 0f);
		//	Rotation HelperRotPitch = Rotation.FromAxis(FixedEyeRot.Right, -AnalogLookReal.pitch);
		//	Rotation HelperRotYaw = Rotation.FromAxis(FixedEyeRot.Forward, AnalogLookReal.yaw);
		//	RotateGravityToTargetDirection(TrueGravityOrientation.Up * HelperRotPitch * HelperRotYaw);
		//	AnalogLookReal = new Angles(0, 0, 0);
		//}

		//Log.Info(TrueGravityOrientation);

		if (QueuedSparks == null)
		{
			QueuedSparks = new List<QueuedSpark>();
		}
		if (QueuedStars == null)
		{
			QueuedStars = new List<QueuedCollisionStar>();
		}
		foreach (QueuedSpark Spark in QueuedSparks)
		{
			ScreenSpaceParticleTrail TestParticle = new ScreenSpaceParticleTrail();
			TestParticle.Instantiate(Spark.Position, Spark.Velocity, Spark.Size, Spark.Texture);
		}
		foreach (QueuedCollisionStar Star in QueuedStars)
		{
			SMBModelParticle NewStar = new SMBModelParticle();
			NewStar.Instantiate(Star.Position, Star.Velocity, Star.Size);
		}
		QueuedSparks.Clear();
		QueuedStars.Clear();

		RenderBall.Position = ClientPosition;
		Vector3 SpinAxis = Vector3.Cross(LastGroundNormal, LastGroundVel).Normal;
		Rotation HelperRot = Rotation.FromAxis(SpinAxis, LastGroundVel.Length * RealDelta * 3);
		RenderBallAng = HelperRot * RenderBallAng;
		RenderBall.Rotation = RenderBallAng.Normal;

		Vector3 CitizenRotVel = ((((ClientVelocity * TrueGravityOrientation.Inverse) * new Vector3(1, 1, 0)) * TrueGravityOrientation) +
			((TrueGravityOrientation * BallCamAng.ToRotation()).Forward * 5) +
			new Vector3(0.001f, 0.001f, 0.001f));

		Rotation DesiredCitizenRotation = Rotation.LookAt(CitizenRotVel, TrueGravityOrientation.Up);
		float AngleBetween = (CitizenRotation.Inverse * DesiredCitizenRotation).Angle();
		float RotateRate = 2f;
		float Frac = (100 / AngleBetween) * RealDelta * RotateRate;
		CitizenRotation = Rotation.Slerp(CitizenRotation, DesiredCitizenRotation, Frac).Normal;
		float AmountToSpin = MathX.Lerp(0.25f, 3, (LastGroundVel.Length - 400) / 300, true);
		Rotation HelperRot2 = Rotation.FromAxis(SpinAxis, LastGroundVel.Length * RealDelta * AmountToSpin);
		CitizenRotation = (HelperRot2 * CitizenRotation).Normal;
		BallCitizen.Position = ClientPosition + (CitizenRotation.Up * -9);
		BallCitizen.Rotation = CitizenRotation;
		//BallCitizen.ResetInterpolation();
		var dir = LastGroundVel;
		var forward = Vector3.Dot(BallCitizen.Rotation.Forward, dir);
		var sideward = Vector3.Dot(BallCitizen.Rotation.Right, dir );

		//Log.Info(dir);
		//Log.Info(forward);
		//Log.Info(sideward);
		//Log.Info("-----");

		var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

		BallCitizen.SetAnimParameter( "move_direction", angle );
		BallCitizen.SetAnimParameter( "move_speed", dir.Length );
		BallCitizen.SetAnimParameter( "move_groundspeed", dir.WithZ( 0 ).Length );
		BallCitizen.SetAnimParameter( "move_y", sideward );
		BallCitizen.SetAnimParameter( "move_x", forward );
		BallCitizen.SetAnimParameter( "move_z", dir.z );
		BallCitizen.SetAnimParameter( "b_grounded", dir.Length < 500 && Trace.Ray(ClientPosition, ClientPosition + new Vector3(0, 0, -10000)).WithTag("solid").IncludeClientside(true).Run().Hit);


		float AutoCameraFactor = MathX.Clamp((Time.Now - LastCameraMoveTime) - 1, 0, 1);
		if (ControlEnabled)
		{
			LookOffset += new Angles(AnalogLookReal.pitch, AnalogLookReal.yaw, 0);
			if (ManualCameraEnabled)
			{
				//float OldLookPitch = LookOffset.pitch;
				//if (AnalogLookReal.pitch > 0 && )
				//todo: when our pitch is outside the limit, check if we're trying to move back inside,
				//and immediately clamp pitch back within the limit
			}else
			{
				//LookOffset += new Angles(AnalogLookReal.pitch, AnalogLookReal.yaw, 0);
				LookOffset = new Angles(MathX.Lerp(LookOffset.pitch, 0, Time.Delta * 8 * AutoCameraFactor, true), MathX.Lerp(LookOffset.yaw, 0, Time.Delta * 8 * AutoCameraFactor, true), 0);
			}
		}
		float NewLookOffsetYaw = LookOffset.yaw;
		if (NewLookOffsetYaw > 180)
		{
			NewLookOffsetYaw -= 360;
		}else
		if (NewLookOffsetYaw < -180)
		{
			NewLookOffsetYaw += 360;
		}

		LookOffset = new Angles(MathX.Clamp(LookOffset.pitch, -150, 150), NewLookOffsetYaw, 0);
//
		//camera stuff
		if (BallState == 0)
		{
			if (Time.Now > SpawnTime + 3.5 && !AboutToGainControl)
			{
				GameEnt.PlayGlobalSound("an_ready");
				if (ReadyGo != null)
				{
					ReadyGo.Delete();
				}
				ReadyGo = new AnimatedEntity();
				ReadyGo.SetModel("models/readygo.vmdl");
				ReadyGo.EnableShadowCasting = false;
				AboutToGainControl = true;
			}
			if (Time.Now > SpawnTime + 5)
			{
				Vector3 ReducedVel = ClientVelocity * TrueGravityOrientation.Inverse;
				ReducedVel.z = MathX.Approach(ReducedVel.z, 0, 2);
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
				Vector3 VelNoZ = ClientVelocity * TrueGravityOrientation.Inverse;
				VelNoZ.z = 0;
				float VelNoZMag = VelNoZ.Length;
				float CamMoveFracVelVert = Math.Min(Math.Abs(ReducedVel.z * 0.75f) + 100, 500);
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
				float OldPitch = BallCamAng.pitch;
				float OldYaw = BallCamAng.yaw;
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
					if (ManualCameraEnabled)
					{
						LookOffset -= new Angles(0, AngleDifference(NewYaw, OldYaw), 0);
					}else
					{
						LookOffset -= new Angles(0, AngleDifference(NewYaw, OldYaw), 0) * (1 - AutoCameraFactor);
					}
				}else
				if (ManualCameraEnabled)
				{
					LookOffset -= new Angles(AngleDifference(NewPitch, OldPitch), AngleDifference(NewYaw, OldYaw), 0);
				}else
				{
					LookOffset -= new Angles(AngleDifference(NewPitch, OldPitch), AngleDifference(NewYaw, OldYaw), 0) * (1 - AutoCameraFactor);
				}
				BallCamAng = new Angles(NewPitch, NewYaw, 0f);
				//YawTilt = MathX.Lerp(YawTilt, AnalogInput.y * 12, RealDelta * 10);
				//PitchTilt = MathX.Lerp(PitchTilt, AnalogInput.x * 12, RealDelta * 10);
				Rotation NewCamRotation = new Angles(MathX.Clamp(NewPitch + 20f + LookOffset.pitch, -48, 88), NewYaw + LookOffset.yaw, 0f).ToRotation();

				Rotation FixedEyeRot = TrueGravityOrientation * Rotation.From(0f, NewYaw, 0f);
				Rotation HelperRotPitch = Rotation.FromAxis(FixedEyeRot.Right, PitchTilt * ConsoleSystem.GetValue( "smb_stagetilteffect_maxangle" ).ToFloat());
				Rotation HelperRotYaw = Rotation.FromAxis(FixedEyeRot.Forward, YawTilt * ConsoleSystem.GetValue( "smb_stagetilteffect_maxangle" ).ToFloat());
				GameEnt.StageTilt = Rotation.Slerp(GameEnt.StageTilt, HelperRotPitch * HelperRotYaw, RealDelta * 15f);

				EyeRotation = GameEnt.StageTilt * TrueGravityOrientation * NewCamRotation;

				float FakeRotOffsetUpDown = ((NewCamRotation.Pitch() - 20f) * -0.15f) + 10;
				float FakeRotOffsetDist = ((NewCamRotation.Pitch() - 20f) * 0.15f) + 55;

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
				float Ratio = (((Time.Now - SpawnTime) / 5f) - 0.7f) * 3.3333333f;
				if (FirstPlay)
				{
					Ratio = (Time.Now - SpawnTime) / 5f;
				}
				float InSineRatio = inSine(Ratio, 0, 1, 1);
				float SineRatio = InOutSine(Ratio, 0, 1, 1);
				float QuadRatio = InOutQuad(Ratio, 0, 1, 1);
				float PowRatio = (float)Math.Pow(SineRatio, 0.65f);
				float PowRatio2 = (float)Math.Pow(SineRatio, 2.25f);
				float PowRatio3 = (float)Math.Pow(SineRatio, 0.8f);
				Rotation FinalRot = new Angles(MathX.Lerp(45, DesiredEyeRotation.Pitch(), SineRatio), DesiredEyeRotation.Yaw() + (360 * (1 - PowRatio)), 0).ToRotation();
				GetStageBounds();
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
			CameraVelocity += -CameraVelocity * RealDelta * 2;
			CameraPosition = CameraPosition + (CameraVelocity * RealDelta);
			CameraRotation = Rotation.Slerp(CameraRotation, Rotation.LookAt(-(CameraPosition - ClientPosition), new Vector3(0, 0, 1)), RealDelta * 5, true);
			EyeRotation = CameraRotation;
			EyePosition = CameraPosition;
		}else
		if (BallState == 2)
		{
			if (Time.Now > LastStateChange + 1.5f && !BlastoffSoundPlayed)
			{
				Sound Blastoff = Sound.FromEntity("ball_blastoff", this);
				BlastoffSoundPlayed = true;
			}
			if (Time.Now > LastStateChange + 2)
			{
				if (!BlastingUp)
				{
					//Sound Blastoff = Sound.FromEntity("ball_blastoff", this);
					BlastUpZLock = ClientPosition;
					BlastingUp = true;
				}
			}
			Vector3 DeltaPosition = (ClientPosition - CameraPosition);
			float OurDot = Vector3.Dot(DeltaPosition, CameraVelocity);
			DeltaPosition = DeltaPosition.Normal;
			float AdjustedDelta = RealDelta * 60;
			float AdjustedDot = OurDot * -0.01f;
			if (!BlastingUp)
			{
				//float OldZ = CameraVelocity.z + (-CameraVelocity.z * RealDelta);
				//CameraVelocity += DeltaPosition * AdjustedDot * AdjustedDelta;
			}
			if (AdjustedDot > 0)
			{
				CameraVelocity += DeltaPosition * AdjustedDelta * 2.5f;
			}
			CameraVelocity += -CameraVelocity * RealDelta * 1.25f;
			CameraVelocity += new Vector3(CameraVelocity.Length * 0.01f * DeltaPosition.y * AdjustedDelta, CameraVelocity.Length * 0.01f * DeltaPosition.x * AdjustedDelta, 0);
			//CameraVelocity += -CameraVelocity * RealDelta;
			CameraPosition = CameraPosition + (CameraVelocity * RealDelta);
			Vector3 CameraPivot = (ClientPosition + new Vector3(0, 0, 15));
			Vector3 DeltaPivot = (CameraPosition - CameraPivot);
			float PivotDist = DeltaPivot.Length;
			if (PivotDist > 0.00001f && !BlastingUp)
			{
				Vector3 Dir = DeltaPivot.Normal;
				CameraPosition = CameraPivot + (DeltaPivot.Normal * MathX.Lerp(PivotDist, 40, RealDelta * 2));
			}
			if (!BlastingUp)
			{
				CameraPosition += new Vector3(0, 0, CameraPivot.z - CameraPosition.z) * 0.1f;
			}
			CameraRotation = Rotation.Slerp(CameraRotation, Rotation.LookAt(-(CameraPosition - ClientPosition), new Vector3(0, 0, 1)), RealDelta * 15, true);
			EyeRotation = CameraRotation;
			EyePosition = CameraPosition;
		}
		if (BallState != 0 | !ControlEnabled)
		{
			MyGame GameEnt = Game.Current as MyGame;
			GameEnt.StageTilt = Rotation.Slerp(GameEnt.StageTilt, Rotation.Identity, Time.Delta * 15f);
		}
		if (GameEnt.CurrentGameState == 0 && ClientsideModelGeneric != null)
		{
			if (Ready)
			{
				ClientsideModelGeneric.SetModel("models/status_ready.vmdl");
			}else
			{
				ClientsideModelGeneric.SetModel("models/status_notready.vmdl");
			}
			ClientsideModelGeneric.Position = ClientPosition + new Vector3(0,0,15);
			ClientsideModelGeneric.Rotation = CurrentView.Rotation * Rotation.FromYaw(180);
		}

		if (ReadyGo.IsValid())
		{
			ReadyGo.Position = EyePosition + (EyeRotation.Forward * 20);
			ReadyGo.Rotation = EyeRotation * Rotation.FromPitch(-90);
			float TimeSinceSpawn = Time.Now - SpawnTime;
			Color OurColor = new Color(1, 1, 1, 1);
			if (TimeSinceSpawn < 5)
			{
				OurColor = new Color(0.12f, 0.09f, 0.04f, 0.91f);
			}else
			{
				float Flash = 0;
				if (TimeSinceSpawn > 5.5)
				{
					Flash = ((float)Math.Sin(Time.Now * 80) + 1) * 0.5f;
				}
				Flash *= Flash;
				Color NoFlashColor = new Color(0.07f, 0.1f, 0.18f, 0.85f);
				Color FlashColor = new Color(0.3f, 0.5f, 1f, 0.5f);
				OurColor = Color.Lerp(NoFlashColor, FlashColor, Flash);
			}
			ReadyGo.RenderColor = OurColor;
			if (TimeSinceSpawn > 7)
			{
				ReadyGo.Delete();
			}
		}
	}
}
