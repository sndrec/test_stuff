using Sandbox;
using SandboxEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public partial class SMBBumper : SMBObject
{

	public float LastHit {get;set;} = 0;
	public PointLightEntity BumperLight {get;set;}

	public override void ClientSpawn()
	{
		base.ClientSpawn();
		BumperLight = new PointLightEntity();
		BumperLight.Brightness = 0f;
		BumperLight.Color = new Color(1, 0.8f, 0.6f);
		BumperLight.Range = 3;
		BumperLight.LinearAttenuation = -1;
		BumperLight.QuadraticAttenuation = 1;
		BumperLight.Falloff = 1000;
		BumperLight.DynamicShadows = false;
	}

	public override bool OnCollideMaster(Pawn Ball, Vector3 VelAtPos, Vector3 InVelocity, Vector3 HitNormal, Vector3 HitPosition, float RealDelta)
	{
		Vector3 DirToBall = (Ball.ClientPosition - (Position + CollisionBounds.Center - (Rotation.Up * 7))).Normal;
		Ball.ClientVelocity = ((InVelocity - VelAtPos).Length + 35) * DirToBall;
		LastHit = Time.Now;
		Sound BumperSound = Sound.FromEntity("fx_bumper", this);
		return false;
	}

	[Event.PreRender]
	public void BumperRender()
	{
		float Ratio = 1 - MathX.Clamp((Time.Now - LastHit) * 2, 0, 1);
		SceneObject.Transform = new Transform(Position, (Rotation.FromYaw(Time.Delta * 45 * ((0.5f + Ratio) * 2)) * SceneObject.Rotation).Normal, 1 + (Ratio * 0.5f));
		BumperLight.Brightness = Ratio;
		BumperLight.Range = Ratio * 25;
		BumperLight.Position = Position + CollisionBounds.Center;
	}

}
