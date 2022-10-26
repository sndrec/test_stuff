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

	public Rotation PartyBallRotation {get;set;}

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
		PartyBallRotation = Rotation.FromYaw(90) * Rotation.FromYaw(Rotation.Yaw());
		PartyBall.AngVel = Rotation.Identity;
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

	[Event.Frame]
	public void SimulatePartyBall()
	{
		PartyBall.Position = Position + (Rotation.Up * 60);
		Rotation BasePartyBallRot = Rotation.FromYaw(Rotation.Yaw());
		MyGame GameEnt = Game.Current as MyGame;
		BasePartyBallRot = GameEnt.StageTilt * BasePartyBallRot;
		PartyBall.AngVel = Rotation.Slerp(PartyBall.AngVel, (BasePartyBallRot * PartyBallRotation.Inverse).Normal, Time.Delta * 2);
		//PartyBall.AngVel	= Rotation.Slerp(PartyBall.AngVel, Rotation.Identity, Time.Delta);
		//Rotation GoalRotInfluence = Rotation.Slerp(Rotation.Identity, OldTransform.Rotation.Inverse * Rotation, Time.Delta * 2);
		//float GoalPosInfluencePower = (Position - OldTransform.Position).Length * (1 - Vector3.Dot((Position - OldTransform.Position).Normal, -Rotation.Up)) * 0.06725f;
		//Rotation GoalPosInfluence = Rotation.FromAxis(Vector3.Cross((Position - OldTransform.Position).Normal, -PartyBallRotation.Up), GoalPosInfluencePower);

		//PartyBall.AngVel = (GoalRotInfluence * PartyBall.AngVel).Normal;
		//PartyBall.AngVel = (GoalPosInfluence * PartyBall.AngVel).Normal;
		Rotation TrueAngVel = Rotation.Slerp(Rotation.Identity, PartyBall.AngVel, Time.Delta * 30);
		PartyBallRotation = (TrueAngVel * PartyBallRotation).Normal;
		float DegreesOffset = Vector3.GetAngle(-PartyBallRotation.Up, -Rotation.Up);
		if (DegreesOffset > 60)
		{
			Vector3 Axis = Vector3.Cross(-PartyBallRotation.Up, -Rotation.Up).Normal;
			PartyBallRotation = Rotation.FromAxis(Axis, DegreesOffset - 60) * PartyBallRotation;
			PartyBall.AngVel = Rotation.FromAxis(Axis, DegreesOffset * Time.Delta * 1.5f).Normal;

		}
		PartyBall.Rotation = PartyBallRotation;
		var glow = Components.GetOrCreate<Glow>();
        glow.Color = new Color(0.25f, 0.25f, 1f);
        glow.ObscuredColor = new Color(0.25f, 0.25f, 1f);
        glow.Width = 0.25f;
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
