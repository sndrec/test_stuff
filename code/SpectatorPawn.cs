using Sandbox;
using System;
using System.Linq;

namespace Sandbox;

partial class SpectatorPawn : ModelEntity
{
	/// <summary>
	/// Called when the entity is first created 
	/// </summary>

	public bool JustSpawned {get;set;}

	public Vector3 SpecPosition {get;set;}

	public Vector3 SpecVelocity {get;set;}

	public override void Spawn()
	{
		base.Spawn();
		EnableDrawing = false;
		EnableTouch = false;
		SetModel( "models/dev/new_model/new_model.vmdl" );
		JustSpawned = true;
	}

	public override void ClientSpawn()
	{
		base.Spawn();

		SetModel( "models/dev/new_model/new_model.vmdl" );
		EnableDrawing = false;
		JustSpawned = true;

	}

	public override void BuildInput( InputBuilder InputBuilderStruct)
	{
		float RealDelta = Math.Min(Global.TickInterval, Time.Delta);
		if (JustSpawned)
		{
			BBox StageBounds = new BBox(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
			foreach (Entity element in Entity.All)
			{
				if (element is SMBObject)
				{
					StageBounds = StageBounds.AddPoint(element.WorldSpaceBounds.Mins);
					StageBounds = StageBounds.AddPoint(element.WorldSpaceBounds.Maxs);
				}
			}

			SpecPosition = StageBounds.Center + (Rotation.From(new Angles(-45, 45, 0)).Forward * Math.Max(Math.Max(StageBounds.Size.x, StageBounds.Size.y), StageBounds.Size.z) * 0.75f);
			InputBuilderStruct.ViewAngles = new Angles(45, -135, 0);
			JustSpawned = false;
		}
		float speedMult = 1;

		if (Input.Down( InputButton.Run ))
		{
			speedMult = 3;
		}
		Vector3 FDir = InputBuilderStruct.ViewAngles.ToRotation().Forward;
		float FStr = InputBuilderStruct.AnalogMove.x * 5000 * speedMult * RealDelta;
		Vector3 SDir = InputBuilderStruct.ViewAngles.ToRotation().Right;
		float SStr = InputBuilderStruct.AnalogMove.y * -5000 * speedMult * RealDelta;
		SpecVelocity += (FDir * FStr) + (SDir * SStr);
		SpecVelocity += -SpecVelocity * RealDelta * 10;
		SpecPosition += SpecVelocity * RealDelta;
		//Rotation MouseLookDiff = (OldLookRotation.Inverse * Input.Rotation).Normal;
		InputBuilderStruct.Position = SpecPosition;
		InputBuilderStruct.ViewAngles += InputBuilderStruct.AnalogLook;
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		Position = Input.Position;
		EyeRotation = Input.Rotation;

		if ( Input.Pressed( InputButton.Reload ) )
		{
			var pawn = new Pawn();
			Client.Pawn = pawn;
			if (Local.Client == null)
			{
				this.Delete();
			}
		}
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );
		Position = Input.Position;
		EyeRotation = Input.Rotation;
	}
}
