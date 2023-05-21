using Sandbox;
using Editor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public partial class ConfettiParticle : SMBModelParticle
{	

	public override void Spawn()
	{
		base.Spawn();
	}

	public override void Instantiate(Vector3 InPosition, Vector3 InVelocity, float InSize)
	{
		SetModel("models/plane.vmdl");
		EnableAllCollisions = false;
		ParticleSpawnTime = Time.Now;
		ParticleLife = 4f;
		ParticleSize = InSize;
		Scale = InSize;
		Position = InPosition;
		Velocity = InVelocity * Game.Random.Float( 0.25f, 1.25f);
		Rotation = Rotation.Random;
		AngleVelocity = Rotation.Random;
		Instantiated = true;
		ColorHsv ConfettiColorHsv = new ColorHsv( Game.Random.Float( 0f, 360f), 1, Game.Random.Float(1f, 1f), 1);
		Color ConfettiColor = ConfettiColorHsv.ToColor();
		SceneObject.Attributes.Set("ConfettiTint", new Vector4(ConfettiColor.r, ConfettiColor.g, ConfettiColor.b, 1));
	}

	public override void UpdateParticle()
	{
		TraceResult ParticleTrace = Trace.Sphere(2, SceneObject.Position, SceneObject.Position + (Velocity * Time.Delta)).WithTag("solid").WithTag("regularfloor").WithoutTags("smbtrigger").WithoutTags("goalpost").IncludeClientside(true).Run();
		if (ParticleTrace.Hit && Vector3.Dot(ParticleTrace.Normal, Velocity) < 0)
		{
			Velocity += -Velocity * Time.Delta * 30;
			AngleVelocity = Rotation.Lerp(AngleVelocity, Rotation.Identity, Time.Delta * 4);
			//Rotation DesiredRotation = new Angles(90, SceneObject.Rotation.Yaw(), SceneObject.Rotation.Roll()).ToRotation();
			Rotation DesiredRotation = Rotation.LookAt(ParticleTrace.Normal, SceneObject.Rotation.Up);
			SceneObject.Rotation = Rotation.Lerp(SceneObject.Rotation, DesiredRotation, Time.Delta * 8);
		}else
		{

			Velocity += new Vector3(0, 0, -450) * Time.Delta;
			Velocity += SceneObject.Rotation.Forward * Vector3.Dot(SceneObject.Rotation.Forward, Velocity) * Time.Delta * -48;
			Velocity += -Velocity * Time.Delta * 6;
		}
		SceneObject.Position += Velocity * Time.Delta;
		SceneObject.Rotation *= Rotation.Lerp(Rotation.Identity, AngleVelocity, Time.Delta * 4);
		if (Time.Now > ParticleSpawnTime + ParticleLife)
		{
			Delete();
		}
	}

	protected override void OnDestroy()
	{
	}
}
