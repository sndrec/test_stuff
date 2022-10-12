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

	bool IsCollected {get;set;}

	public override void Spawn()
	{
		base.Spawn();
		Tags.Add("bananasingle");
		SetModel("models/banana_single.vmdl");
		SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		EnableDrawing = true;
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
		Entity OurBanana = Entity.FindByIndex(InNetworkIdent);
		if (OurBanana == null)
		{
			return;
		}
		Pawn Ball = Entity.FindByIndex(InNetworkIdent2) as Pawn;
		Ball.OurManager.AddScore(100);
		Sound.FromEntity("fx_banana", OurBanana);
		MyGame GameEnt = Game.Current as MyGame;
		GameEnt.CurrentStage.OnBananaCollectedManager(Ball, 1);
		Event.Run( "mygame.bananacollected", Ball, 1 );
		Log.Info("Let's banana.");
		OurBanana.Delete();
	}

	[Event.Frame]
	public virtual void BananaFrame()
	{
		//Log.Info("hi");
		//DebugOverlay.Sphere(Position, 10, new Color(0,255,0), Time.Delta, false);
		SceneObject.Rotation = Rotation.FromYaw(Time.Now * 360);
	}


}
