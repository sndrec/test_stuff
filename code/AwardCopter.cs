using Sandbox;
using SandboxEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public partial class AwardCopter : SMBObject
{

	public float SpawnTime {get;set;}
	public PlayerStateManager TargetPawnManager {get;set;}
	public Vector3 TargetPosition {get;set;}
	public SMBObject Blade {get;set;}
	public SMBObject BallA {get;set;}
	public SMBObject BallB {get;set;}
	public float LastBallStateChange {get;set;}
	public bool BallOpen {get;set;}
	public int Placement {get;set;}
	public float MovementFactor {get;set;}
	public float MovementDrag {get;set;}
	public Rotation DesiredRotation {get;set;}
	public Sound CopterSound {get;set;}

	public static void BallASimulate(SMBObject InObject)
	{
		AwardCopter Copter = InObject.Owner as AwardCopter;
		InObject.Position = Copter.Position;
		Rotation OpenRot = Copter.Rotation * Rotation.FromPitch(45);
		Rotation CloseRot = Copter.Rotation;
		if (Copter.BallOpen)
		{
			InObject.Rotation = Rotation.Slerp(InObject.Rotation, OpenRot, Time.Delta * 12);
		}else
		{
			InObject.Rotation = Rotation.Slerp(InObject.Rotation, CloseRot, Time.Delta * 12);
		}
	}

	public static void BallBSimulate(SMBObject InObject)
	{
		AwardCopter Copter = InObject.Owner as AwardCopter;
		InObject.Position = Copter.Position;
		Rotation OpenRot = Copter.Rotation * Rotation.FromPitch(-45);
		Rotation CloseRot = Copter.Rotation;
		if (Copter.BallOpen)
		{
			InObject.Rotation = Rotation.Slerp(InObject.Rotation, OpenRot, Time.Delta * 12);
		}else
		{
			InObject.Rotation = Rotation.Slerp(InObject.Rotation, CloseRot, Time.Delta * 12);
		}
	}

	public override void Spawn()
	{
		base.Spawn();
		EnableAllCollisions = true;
		EnableDrawing = true;

		Blade = new SMBObject();
		BallA = new SMBObject();
		BallB = new SMBObject();

		Blade.SetModel("models/stages/award_copter_blade.vmdl");
		BallA.SetModel("models/stages/award_copter_ball_a.vmdl");
		BallB.SetModel("models/stages/award_copter_ball_b.vmdl");

		BallA.SimulateSMBObjectCustom = BallASimulate;
		BallB.SimulateSMBObjectCustom = BallBSimulate;

		BallA.SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		BallB.SetupPhysicsFromModel(PhysicsMotionType.Keyframed);

		Blade.Owner = this;
		BallA.Owner = this;
		BallB.Owner = this;

		MovementFactor = 4f;
		MovementDrag = 2f;

		BallOpen = false;
		LastBallStateChange = Time.Now;
		TargetPosition = new Vector3(0, 0, 100);
		SpawnTime = Time.Now + 5;

		CopterSound = Sound.FromEntity("copter_loop", this);
	}

	public override void SimulateSMBObject()
	{
		float TimeElapsed = Time.Now - SpawnTime;
		Blade.Position = Position;
		Blade.Rotation = Rotation * Rotation.FromYaw(Time.Now * 720);
		if (TimeElapsed < 0)
		{
			return;
		}
		Velocity += (TargetPosition - Position) * Time.Delta * MovementFactor;
		Velocity += -Velocity * Time.Delta * MovementDrag;
		Position += Velocity * Time.Delta;
		MyGame GameEnt = Game.Current as MyGame;

		float VelocityLengthNoZ = (Velocity * new Vector3(1, 1, 0)).Length;
		Rotation TiltFromMotion = Rotation.FromAxis(Vector3.Cross(Velocity, Vector3.Up).Normal, -VelocityLengthNoZ * 0.15f);
		Rotation GoalRotation = Rotation.FromYaw(90) * Rotation.From(Vector3.VectorAngle((Velocity + new Vector3(1, 0, 0)) * new Vector3(1, 1, 0)));
		DesiredRotation = Rotation.Slerp(DesiredRotation, GoalRotation, Time.Delta * Velocity.Length * 0.02f, true).Normal;
		Rotation = TiltFromMotion * DesiredRotation;
		//Rotation = Rotation.FromAxis(Vector3.Cross(Velocity, Vector3.Up).Normal, -VelocityLengthNoZ * 0.15f);
		Client pl = TargetPawnManager.Owner as Client;
		if (pl == null)
		{
			return;
		}
		Pawn TargetPawn = pl.Pawn as Pawn;
		if (TargetPawn == null)
		{
			return;
		}
		if (TimeElapsed > 20f)
		{
			//and back into the hole.
			MovementFactor = 1f;
			MovementDrag = 0.5f;
			TargetPosition = GameEnt.BlenderPos(0, 0f, -4f);
		}else
		if (TimeElapsed > 16f)
		{
			//back up...
			MovementFactor = 8f;
			MovementDrag = 2f;
			if (Placement == 1)
			{
				TargetPosition = GameEnt.BlenderPos(0, 0f, 6f);
			}
			if (Placement == 2)
			{
				TargetPosition = GameEnt.BlenderPos(0f, 0f, 4f);
			}
			if (Placement == 3)
			{
				TargetPosition = GameEnt.BlenderPos(0f, 0f, 2f);
			}
			BallOpen = false;
		}else
		if (TimeElapsed > 15.5f)
		{
			//release!
			MovementFactor = 3f;
			MovementDrag = 0.5f;
			BallOpen = true;
		}else
		if (TimeElapsed > 15f)
		{
			//let's make sure we're on top.
			MovementFactor = 64f;
			MovementDrag = 10f;
		}else
		if (TimeElapsed > 13f)
		{
			//lower down...
			MovementFactor = 4f;
			MovementDrag = 3f;
			if (Placement == 1)
			{
				TargetPosition = GameEnt.BlenderPos(0, -13.0f, 5f);
			}
			if (Placement == 2)
			{
				TargetPosition = GameEnt.BlenderPos(5.5f, -13.0f, 4f);
			}
			if (Placement == 3)
			{
				TargetPosition = GameEnt.BlenderPos(-5.5f, -13.0f, 3f);
			}
		}else
		if (TimeElapsed > 10f)
		{
			//start traveling to pedestal
			MovementFactor = 0.5f;
			MovementDrag = 0.5f;
			if (Placement == 1)
			{
				TargetPosition = GameEnt.BlenderPos(0, -20.0f, 6f);
			}
			if (Placement == 2)
			{
				TargetPosition = GameEnt.BlenderPos(5.5f, -20.0f, 5f);
			}
			if (Placement == 3)
			{
				TargetPosition = GameEnt.BlenderPos(-5.5f, -20.0f, 4f);
			}
		}else
		if (TimeElapsed > 8f)
		{
			//head to the middle...
			MovementFactor = 0.5f;
			MovementDrag = 0f;
			if (Placement == 1)
			{
				TargetPosition = GameEnt.BlenderPos(0, 0f, 12f);
			}
			if (Placement == 2)
			{
				TargetPosition = GameEnt.BlenderPos(8f, 0f, 8f);
			}
			if (Placement == 3)
			{
				TargetPosition = GameEnt.BlenderPos(-8f, 0f, 4f);
			}
		}else
		if (TimeElapsed > 7f)
		{
			//snag!
			MovementFactor = 512f;
			MovementDrag = 16f;
			TargetPosition = TargetPawn.Position + new Vector3(0, 0, 11);
			TargetPawn.ServerSetVelocity(TargetPawn.Velocity + (-TargetPawn.Velocity * Time.Delta * 12));
			BallOpen = false;
		}else
		if (TimeElapsed > 6f)
		{
			//start moving down
			MovementFactor = 2048f;
			MovementDrag = 48f;
			TargetPosition = (TargetPawn.Position + (TargetPawn.Velocity * Time.Delta * 12)) + new Vector3(0, 0, 16);
			TargetPawn.ServerSetVelocity(TargetPawn.Velocity + (-TargetPawn.Velocity * Time.Delta * 4));
			BallOpen = true;
		}else
		if (TimeElapsed > 4.5f)
		{
			//chase faster!
			MovementFactor = 96f;
			MovementDrag = 12f;
			TargetPosition = (TargetPawn.Position + (TargetPawn.Velocity * Time.Delta * 14)) + new Vector3(0, 0, 40);
			BallOpen = true;
		}else
		if (TimeElapsed > 3f)
		{
			//start chasing
			MovementFactor = 16f;
			MovementDrag = 6f;
			TargetPosition = (TargetPawn.Position + (TargetPawn.Velocity * Time.Delta * 32)) + new Vector3(0, 0, 40);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Blade != null)
		{
			Blade.Delete();
		}
		if (BallA != null)
		{
			BallA.Delete();
		}
		if (BallB != null)
		{
			BallB.Delete();
		}
		CopterSound.Stop();
	}

}
