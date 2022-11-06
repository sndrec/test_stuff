using Sandbox;
using SandboxEditor;
using Sandbox.Component;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

[Library( "goal_post" ), HammerEntity]
[Title( "Monkey Ball Goalpost" ), Category( "Stage" ), Icon( "place" )]
[EditorModel( "models/goalpost.vmdl" )]
public partial class GoalPost : SMBObject
{
	/// <summary>
	/// Called when the entity is first created 
	/// </summary>

	public ModelEntity GoalTrigger {get;set;}

	public PartyBall PartyBall {get;set;}

	public Vector3 PartyBallSimPosition { get; set; }

	public Vector3 PartyBallSimVelocity { get; set; }

	public GoalTapeEntity GoalTape {get;set;}

	public bool GoalCrossed {get;set;}

	[Net]
	public int WarpDist {get;set;}

	public override void Spawn()
	{
		base.Spawn();
		SetModel("models/goalpost.vmdl");
		SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		//SetupPhysicsFromSphere( PhysicsMotionType.Keyframed, new Vector3(0, 0, 0), 10 );
		EnableDrawing = true;
		EnableTouch = true;
		PhysicsBody.EnableTouch = true;
		Tags.Add("goalpost");
		GoalTrigger = new SMBTrigger();
		GoalTrigger.SetModel("models/goaltrigger.vmdl");
		GoalTrigger.SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		GoalTrigger.Tags.Add("goaltrigger");
		GoalTrigger.EnableTraceAndQueries = true;
		GoalTrigger.EnableSolidCollisions = false;
		GoalTrigger.Owner = this;
		EnableAllCollisions = true;
		Predictable = true;
        GoalCrossed = false;
	}

	public void SetPostAndTriggerModel(string InPostModel, string InTriggerModel)
	{
		SetModel(InPostModel);
		SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		GoalTrigger.SetModel(InTriggerModel);
		GoalTrigger.SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
	}

	public override void ClientSpawn()
	{
		PartyBall = new PartyBall();
		PartyBall.SetModel("models/partyball.vmdl");
		PartyBall.SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		PartyBall.Tags.Add("solid");
		PartyBall.Tags.Add("goalpost");
		PartyBall.Owner = this;
		PartyBall.UseAnimGraph = false;
		PartyBall.AnimateOnServer = false;
		PartyBall.PlaybackRate = 0;
		PartyBallSimPosition = Position;
		PartyBallSimVelocity = Vector3.Zero;
		PartyBall.EnableTraceAndQueries = true;
		PartyBall.EnableSolidCollisions = true;
		PartyBall.EnableAllCollisions = true;
		GoalCrossed = false;

		GoalTape = new GoalTapeEntity();
		GoalTape.Owner = this;
		List<TapePoint> GoalTapePoints = new List<TapePoint>{};
		Vector3 TapeStart = Position + (Rotation.Up * 14) + (Rotation.Right * -18);
		Vector3 TapeEnd = Position + (Rotation.Up * 14) + (Rotation.Right * 18);
		float TapeSegments = 10;
		for (int i = 0; i < TapeSegments; i++)
		{
			Vector3 PointPos = Vector3.Lerp(TapeStart, TapeEnd, i / (TapeSegments - 1));
			TapePoint TempPoint = new TapePoint(PointPos, (i == 0 | i == TapeSegments - 1));
			GoalTapePoints.Add(TempPoint);
		}
		GoalTape.CreateRope(GoalTapePoints);
	}

	public override void SimulateSMBObject()
	{
		base.SimulateSMBObject();
		//Position = new Vector3(Position.x, Position.y, Position.z + ((float)Math.Sin(Time.Now * 6) * 2));
		//Rotation = Rotation.FromYaw((float)Math.Sin(Time.Now * 4) * 80);
		//Rotation = Rotation.Normal;
		//Position += new Vector3((float)Math.Sin(Time.Now * 8) * Time.Delta * 800, (float)Math.Cos(Time.Now * 8) * Time.Delta * 800, 0);
		//Position += new Vector3(Time.Delta * 1500, 0, 0);
		GoalTrigger.Position = Position;
		GoalTrigger.Rotation = Rotation;

		//if (Time.Tick % 120 == 0)
		//{
		//	PartyBall.AngVel = Rotation.Random;
		//}

		//PartyBallRotation = Rotation.FromPitch(90);
	}

	[Event.Frame]
	public void AnchorTapeEnds()
	{
		Vector3 TapeStart = Position + (Rotation.Up * 14) + (Rotation.Right * -18);
		Vector3 TapeEnd = Position + (Rotation.Up * 14) + (Rotation.Right * 18);
		GoalTape.Points[0].Position = TapeStart;
		GoalTape.Points[GoalTape.Points.Count - 1].Position = TapeEnd;
	}


	//todo: make this based off of gravity and not the visual stage tilt
	[Event.Frame]
	public void SimulatePartyBall()
	{
		MyGame GameEnt = Game.Current as MyGame;
		PartyBall.Position = Position + (Rotation.Up * 60);
		PartyBallSimVelocity += (new Vector3( 0, 0, -588 ) * Time.Delta) * GameEnt.StageTilt;
		PartyBallSimPosition += PartyBallSimVelocity * Time.Delta;

		Vector3 DirFromPartyBallMount = (PartyBallSimPosition - PartyBall.Position).Normal;
		PartyBallSimPosition = PartyBall.Position + (DirFromPartyBallMount * 20);
		PartyBallSimVelocity -= Vector3.Dot( PartyBallSimVelocity, DirFromPartyBallMount ) * DirFromPartyBallMount;
		PartyBallSimVelocity += -PartyBallSimVelocity * Time.Delta;
		DirFromPartyBallMount = (PartyBallSimPosition - PartyBall.Position).Normal;

		PartyBall.Rotation = Rotation.LookAt( DirFromPartyBallMount, Rotation.Forward ) * Rotation.FromPitch( -90 );

		//DebugOverlay.Sphere( PartyBallSimPosition, 2, new Color( 0, 255, 0 ), Time.Delta, false );
		//DebugOverlay.Line( PartyBallSimPosition, PartyBallSimPosition + (PartyBallSimVelocity), new Color( 255, 255, 255 ), Time.Delta, false );


		if (Local.Client.Pawn is Pawn)
        {
        	Pawn Ball = Local.Client.Pawn as Pawn;
      		if (Ball.BallState == 2 && GoalCrossed == false)
      		{
				PartyBall.PlaybackRate = 1;
				for (int i = 0; i < 150; i++)
				{
					ConfettiParticle Confetti = new ConfettiParticle();
					Confetti.Instantiate(PartyBall.Position + (Vector3.Random * 10) + (Vector3.Up * -10), (Vector3.Random * (250 + (Ball.ClientVelocity.Length * 0.1f))) + (Ball.ClientVelocity * 2), 0.2f);
					//CreateStar(ClientPosition + (CurrentView.Rotation.Forward * 50) + (Vector3.Random * 20) + (Vector3.Up * 20), new Vector3(0, 0, 50), 0.1f);
				}
				GoalCrossed = true;
      		}
        }
	}

	protected override void OnDestroy()
	{
		if (GoalTrigger != null)
		{
			GoalTrigger.Delete();
		}
		if (PartyBall != null)
		{
			PartyBall.Delete();
		}
		if (GoalTape != null)
		{
			GoalTape.Delete();
		}
	}
}
