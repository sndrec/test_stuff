using Sandbox;
using SandboxEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public partial class Banana : SMBTrigger
{

	[Net]
	Pawn Collector {get;set;}

	[Net]
	float CollectTime {get;set;}

	[Net]
	bool ServerKnowsCollected {get;set;}

	[Net]
	Vector3 PosWhenCollected {get;set;}

	bool IsCollected {get;set;}

	Vector3 ClientsideBananaPos {get;set;}

	Rotation ClientsideBananaRot {get;set;}

	public override void Spawn()
	{
		base.Spawn();
		Tags.Add("bananasingle");
		SetModel("models/banana_single.vmdl");
		SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		EnableDrawing = true;
		Predictable = true;
		ClientsideBananaPos = Position;
		ClientsideBananaRot = Rotation;
	}
	public override void ClientSpawn()
	{
		base.ClientSpawn();
		Predictable = true;
		ClientsideBananaPos = Position;
		ClientsideBananaRot = Rotation;
	}

	public override void OnEnterTrigger(Pawn InBall)
	{
		base.OnEnterTrigger(InBall);
		if (!IsCollected)
		{
			IsCollected = true;
			CollectedBy(this.NetworkIdent, InBall.NetworkIdent);
		}
	}
	public override void OnExitTrigger(Pawn InBall)
	{
		base.OnExitTrigger(InBall);
		if (!IsCollected)
		{
			IsCollected = true;
			CollectedBy(this.NetworkIdent, InBall.NetworkIdent);
		}
	}

	[ConCmd.Server]
	public static void CollectedBy(int InNetworkIdent, int InNetworkIdent2)
	{
		Banana OurBanana = Entity.FindByIndex(InNetworkIdent) as Banana;
		if (OurBanana == null)
		{
			return;
		}
		if (OurBanana.ServerKnowsCollected)
		{
			return;
		}
		Pawn Ball = Entity.FindByIndex(InNetworkIdent2) as Pawn;
		Ball.OurManager.AddScore(100);
		Sound.FromEntity("fx_banana", OurBanana);
		MyGame GameEnt = Game.Current as MyGame;
		GameEnt.CurrentStage.OnBananaCollectedManager(Ball, 1);
		//Event.Run( "mygame.bananacollected", Ball, 1 );
		OurBanana.Owner = Ball;
		OurBanana.ServerKnowsCollected = true;
		OurBanana.CollectTime = Time.Now;
		OurBanana.PosWhenCollected = OurBanana.Position;
	}

	[Event.Tick.Server]
	public virtual void DestroyAfterCollected()
	{
		if (ServerKnowsCollected)
		{
			Pawn Ball = Owner as Pawn;
			float Ratio1 = Math.Min((Time.Now - CollectTime) * 2, 1);
			float Ratio2 = Math.Min(((Time.Now - CollectTime) - 0.5f) * 2, 1);
			Vector3 TargetPos1 = Ball.Position + (Vector3.Up * 32);
			Vector3 TargetPos2 = Ball.Position + (Vector3.Up * 64);
			if (Ratio2 > 0)
			{
				Position = Vector3.Lerp(TargetPos1, TargetPos2, Ratio2, true);
				Scale = MathX.Clamp(1 - Ratio2, 0.001f, 1);
			}else
			{
				Position = Vector3.Lerp(PosWhenCollected, TargetPos1, Ratio1, true);
			}
		}
		if (ServerKnowsCollected && Time.Now > CollectTime + 1)
		{
			Delete();
		}
		Predictable = true;
	}

	[Event.Frame]
	public virtual void BananaFrame()
	{
		if (!ServerKnowsCollected)
		{
			ClientsideBananaPos = Position;
			ClientsideBananaRot = Rotation.FromYaw(Time.Now * 360);
			SceneObject.Position = ClientsideBananaPos;
			SceneObject.Rotation = ClientsideBananaRot;
		}else
		{
			ClientsideBananaRot *= Rotation.FromYaw(Time.Delta * 720);
			if (Owner == Local.Client.Pawn)
			{
				Pawn Ball = Owner as Pawn;
				float Ratio1 = Math.Min((Time.Now - CollectTime) * 2, 1);
				float Ratio2 = Math.Min(((Time.Now - CollectTime) - 0.5f) * 2, 1);
				Vector3 TargetPos1 = Ball.ClientPosition + (CurrentView.Rotation.Up * 32) + (CurrentView.Rotation.Forward * 12);
				Vector3 TargetPos2 = Ball.ClientPosition + (CurrentView.Rotation.Up * 42) + (CurrentView.Rotation.Right * -56);
				if (Ratio2 > 0)
				{
					ClientsideBananaPos = Vector3.Lerp(TargetPos1, TargetPos2, Ratio2, true);
					Scale = MathX.Clamp(1 - Ratio2, 0.001f, 1);
				}else
				{
					ClientsideBananaPos = Vector3.Lerp(PosWhenCollected, TargetPos1, Ratio1, true);
				}
				Position = ClientsideBananaPos;
				Rotation = ClientsideBananaRot;
				SceneObject.Position = ClientsideBananaPos;
				SceneObject.Rotation = ClientsideBananaRot;
			}
		}
	}



}
