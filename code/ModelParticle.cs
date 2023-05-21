using Sandbox;
using Editor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public partial class SMBModelParticle : ModelEntity
{	
	public float ParticleLife {get;set;}

	public float ParticleSize {get;set;}

	public float ParticleSpawnTime {get;set;}

	public PointLightEntity ParticleLight {get;set;}

	public bool Instantiated {get;set;}

	public Rotation AngleVelocity {get;set;}

	public override void Spawn()
	{
		base.Spawn();
	}

	public virtual void Instantiate(Vector3 InPosition, Vector3 InVelocity, float InSize)
	{
		SetModel("particles/collision_star.vmdl");
		EnableAllCollisions = false;
		ParticleSpawnTime = Time.Now;
		ParticleLife = Game.Random.Float( 0.5f, 1.5f);
		ParticleSize = InSize;
		Scale = InSize;
		Position = InPosition;
		Velocity = InVelocity;
		ParticleLight = new PointLightEntity();
		ParticleLight.Brightness = 0f;
		ParticleLight.Color = new Color(1, 0.8f, 0.6f);
		ParticleLight.Range = 10;
		ParticleLight.LinearAttenuation = 1;
		ParticleLight.QuadraticAttenuation = 1;
		ParticleLight.Falloff = 1000;
		ParticleLight.DynamicShadows = false;
		Rotation = Rotation.Random;
		AngleVelocity = Rotation.Random;
		Instantiated = true;
	}

	public virtual void UpdateParticle()
	{
		Velocity += new Vector3(0, 0, -400) * Time.Delta;
		TraceResult ParticleTrace = Trace.Sphere(1, SceneObject.Position, SceneObject.Position + (Velocity * Time.Delta)).WithTag("solid").IncludeClientside(true).Run();
		if (ParticleTrace.Hit && Vector3.Dot(ParticleTrace.Normal, Velocity) < 0)
		{
			SceneObject.Position = ParticleTrace.HitPosition + (ParticleTrace.Normal * 2.01f);
			Velocity += ParticleTrace.Normal * Vector3.Dot(Velocity, ParticleTrace.Normal) * -1.5f;
			Velocity += -Velocity * 0.25f;
		}else
		{
			SceneObject.Position += Velocity * Time.Delta;
		}
		SceneObject.Rotation *= Rotation.Lerp(Rotation.Identity, AngleVelocity, Time.Delta * 4);
		ParticleLight.Position = SceneObject.Position;
		if (Time.Now > ParticleSpawnTime + 0.05f)
		{
			float LifeRemainingRatio = 1 - ((Time.Now - ParticleSpawnTime) / ParticleLife);
			ParticleLight.Brightness = 0.1f * LifeRemainingRatio;
			SceneObject.Transform = SceneObject.Transform.WithScale(ParticleSize * LifeRemainingRatio);
		}
		if (Velocity.LengthSquared < 100)
		{
			ParticleLife -= Time.Delta;
		}
		if (Time.Now > ParticleSpawnTime + ParticleLife)
		{
			Delete();
		}
	}

	[GameEvent.Client.Frame]
	public virtual void ManageParticle()
	{
		if (!Instantiated)
		{
			return;
		}
		UpdateParticle();
		//DebugOverlay.Text(Time.Delta.ToString(), Position, Time.Delta, 150);
		//DebugOverlay.Sphere(Position, 1, new Color(0,255,0), Time.Delta, false);

	}

	protected override void OnDestroy()
	{
		ParticleLight.Delete();
	}
}
