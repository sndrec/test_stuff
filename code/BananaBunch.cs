using Sandbox;
using Editor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public partial class BananaBunch : Banana
{

	public override void Spawn()
	{
		base.Spawn();
		Tags.Add("bananabunch");
		SetModel("models/banana_bunch.vmdl");
		SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		EnableDrawing = true;
		Predictable = true;
		ClientsideBananaPos = Position;
	}

	public override void OnEnterTrigger(Pawn InBall)
	{
		if (!IsCollected)
		{
			Pawn Ball = Game.LocalClient.Pawn as Pawn;
			if (Ball != null)
			{
				OffsetToBall = ClientsideBananaPos - Ball.ClientPosition;
			}
			IsCollected = true;
			BunchCollectedBy(this.NetworkIdent, InBall.NetworkIdent);
		}
	}
	public override void OnExitTrigger(Pawn InBall)
	{
		if (!IsCollected)
		{
			Pawn Ball = Game.LocalClient.Pawn as Pawn;
			if (Ball != null)
			{
				OffsetToBall = ClientsideBananaPos - Ball.ClientPosition;
			}
			IsCollected = true;
			BunchCollectedBy(this.NetworkIdent, InBall.NetworkIdent);
		}
	}

	[ConCmd.Server]
	public static void BunchCollectedBy(int InNetworkIdent, int InNetworkIdent2)
	{
		BananaBunch OurBanana = Entity.FindByIndex(InNetworkIdent) as BananaBunch;
		if (OurBanana == null)
		{
			return;
		}
		if (OurBanana.ServerKnowsCollected)
		{
			return;
		}
		OurBanana.SetParent(null);
		Pawn Ball = Entity.FindByIndex(InNetworkIdent2) as Pawn;
		Ball.OurManager.AddScore(1000);
		Sound.FromEntity("fx_bananabunch", OurBanana);
		MyGame GameEnt = GameManager.Current as MyGame;
		GameEnt.CurrentStage.OnBananaCollectedManager(Ball, 1);
		//Event.Run( "mygame.bananacollected", Ball, 1 );
		OurBanana.Owner = Ball;
		OurBanana.ServerKnowsCollected = true;
		OurBanana.CollectTime = Time.Now;
		OurBanana.PosWhenCollected = OurBanana.Position;
	}

	[GameEvent.Tick.Server]
	public override void DestroyAfterCollected()
	{
		
		if (ServerKnowsCollected)
		{
			//Rotation *= Rotation.FromYaw(720 * Time.Delta);
			Pawn Ball = Owner as Pawn;
			if (Ball == null)
			{
				return;
			}
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

	public override void BananaFrame()
	{
		Pawn Ball = Game.LocalClient.Pawn as Pawn;
		if (!ServerKnowsCollected)
		{
			ClientsideBananaPos = Position;
			SceneObject.Position = ClientsideBananaPos;
			ClientsideBananaRot *= Rotation.FromYaw(270 * Time.Delta);
			SceneObject.Rotation = ClientsideBananaRot;
			SceneObject.Transform = SceneObject.Transform.WithScale(1);
		}else
		{
			ClientsideBananaRot *= Rotation.FromYaw(540 * Time.Delta);
			SceneObject.Rotation = ClientsideBananaRot;
			if (Ball != null && Owner == Ball)
			{
				float Ratio1 = Math.Min((Time.Now - CollectTime) * 2, 1);
				float Ratio2 = Math.Min(((Time.Now - CollectTime) - 0.5f) * 2, 1);
				Vector3 TargetPos1 = Ball.ClientPosition + (Camera.Rotation.Up * 32) + (Camera.Rotation.Forward * 12);
				Vector3 TargetPos2 = Ball.ClientPosition + (Camera.Rotation.Up * 42) + (Camera.Rotation.Right * -56);
				if (Ratio2 > 0)
				{
					ClientsideBananaPos = Vector3.Lerp(TargetPos1, TargetPos2, Ratio2, true);
					SceneObject.Transform = SceneObject.Transform.WithScale(MathX.Clamp(1 - Ratio2, 0.001f, 1));
				}else
				{
					ClientsideBananaPos = Vector3.Lerp(Ball.ClientPosition + OffsetToBall, TargetPos1, Ratio1, true);
				}
				SceneObject.Position = ClientsideBananaPos;
			}
		}
		
	}



}
