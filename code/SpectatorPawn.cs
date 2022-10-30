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

	public bool SpectatingPlayer {get;set;}

	public Vector3 SpecPosition {get;set;}

	public Vector3 SpecVelocity {get;set;}

	public int SpecPlayerIndex {get;set;}

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
		SpecPlayerIndex = 0;

	}

	public BBox GetStageBounds()
	{
		MyGame GameEnt = Game.Current as MyGame;
		if (GameEnt.StageBounds.Volume == 0)
		{
			BBox StageBounds = new BBox(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
			foreach (Entity element in Entity.All)
			{
				if (element is SMBObject && !element.Tags.Has("BGObject"))
				{
					StageBounds = StageBounds.AddPoint(element.WorldSpaceBounds.Mins);
					StageBounds = StageBounds.AddPoint(element.WorldSpaceBounds.Maxs);
				}
			}
			return StageBounds;
		}else
		{
			return GameEnt.StageBounds;
		}
	}

	public override void BuildInput( InputBuilder InputBuilderStruct)
	{
		float RealDelta = Math.Min(Global.TickInterval, Time.Delta);
		if (JustSpawned)
		{
			BBox StageBounds = GetStageBounds();
			SpecPosition = StageBounds.Center + (Rotation.From(new Angles(-45, 45, 0)).Forward * Math.Max(Math.Max(StageBounds.Size.x, StageBounds.Size.y), StageBounds.Size.z) * 0.75f);
			InputBuilderStruct.ViewAngles = new Angles(45, -135, 0);
			JustSpawned = false;
		}
		float speedMult = 2;

		if (Input.Down( InputButton.Run ))
		{
			speedMult = 5;
		}
		if (SpectatingPlayer)
		{
			speedMult = 0;
		}
		Vector3 FDir = InputBuilderStruct.ViewAngles.ToRotation().Forward;
		float FStr = InputBuilderStruct.AnalogMove.x * 5000 * speedMult * RealDelta;
		Vector3 SDir = InputBuilderStruct.ViewAngles.ToRotation().Right;
		float SStr = InputBuilderStruct.AnalogMove.y * -5000 * speedMult * RealDelta;
		SpecVelocity += (FDir * FStr) + (SDir * SStr);
		SpecVelocity += -SpecVelocity * RealDelta * 10;
		SpecPosition += SpecVelocity * RealDelta;
		//Rotation MouseLookDiff = (OldLookRotation.Inverse * Input.Rotation).Normal;

		if (SpecPlayerIndex >= Client.All.Count)
		{
			SpectatingPlayer = false;
			SpecPlayerIndex = 0;
			SpecPosition = SpecPosition + (InputBuilderStruct.ViewAngles.ToRotation().Forward * -60);
		}
		if (SpectatingPlayer)
		{
			Pawn SpecBall = Client.All[SpecPlayerIndex].Pawn as Pawn;
			if (SpecBall != null)
			{
				SpecPosition = SpecBall.SRBPos;
			}else
			{
				bool CanSpecPlayer = false;
				int DesiredSpecPlayer = 0;
				Log.Info(DesiredSpecPlayer);
				while (true)
				{
					if (DesiredSpecPlayer < Client.All.Count)
					{
						if (Client.All[DesiredSpecPlayer] != null && Client.All[DesiredSpecPlayer].Pawn != null && Client.All[DesiredSpecPlayer].Pawn is Pawn)
						{
							CanSpecPlayer = true;
							break;
						}else
						{
							DesiredSpecPlayer++;
						}
					}else
					{
						break;
					}
				}
				if (CanSpecPlayer)
				{
					SpectatingPlayer = true;
					SpecPlayerIndex = DesiredSpecPlayer;
				}else
				{
					SpectatingPlayer = false;
					SpecPlayerIndex = 0;
					SpecPosition = SpecPosition + (InputBuilderStruct.ViewAngles.ToRotation().Forward * -60);
				}
			}
		}

		if (Input.Pressed(InputButton.Jump))
		{
			if (!SpectatingPlayer)
			{
				if (Client.All[SpecPlayerIndex] != null && Client.All[SpecPlayerIndex].Pawn != null && Client.All[SpecPlayerIndex].Pawn is Pawn)
				{
					SpectatingPlayer = true;
				}else
				{
					bool CanSpecPlayer = false;
					int DesiredSpecPlayer = 0;
					Log.Info(DesiredSpecPlayer);
					while (true)
					{
						if (DesiredSpecPlayer < Client.All.Count)
						{
							if (Client.All[DesiredSpecPlayer] != null && Client.All[DesiredSpecPlayer].Pawn != null && Client.All[DesiredSpecPlayer].Pawn is Pawn)
							{
								CanSpecPlayer = true;
								break;
							}else
							{
								DesiredSpecPlayer++;
							}
						}else
						{
							break;
						}
					}
					if (CanSpecPlayer)
					{
						SpectatingPlayer = true;
						SpecPlayerIndex = DesiredSpecPlayer;
					}
				}
			}else
			{
				SpectatingPlayer = false;
				SpecPosition = SpecPosition + (InputBuilderStruct.ViewAngles.ToRotation().Forward * -60);
			}
		}

		if (Input.Pressed(InputButton.PrimaryAttack))
		{
			if (Client.All.Count == 1)
			{
				return;
			}
			if (!SpectatingPlayer)
			{
				bool CanSpecPlayer = false;
				int DesiredSpecPlayer = 0;
				Log.Info(DesiredSpecPlayer);
				while (true)
				{
					if (DesiredSpecPlayer < Client.All.Count)
					{
						if (Client.All[DesiredSpecPlayer] != null && Client.All[DesiredSpecPlayer].Pawn != null && Client.All[DesiredSpecPlayer].Pawn is Pawn)
						{
							CanSpecPlayer = true;
							break;
						}else
						{
							DesiredSpecPlayer++;
						}
					}else
					{
						break;
					}
				}
				if (CanSpecPlayer)
				{
					SpectatingPlayer = true;
					SpecPlayerIndex = DesiredSpecPlayer;
				}
			}else
			{
				bool CanSpecPlayer = false;
				int DesiredSpecPlayer = SpecPlayerIndex + 1;
				Log.Info(DesiredSpecPlayer);
				while (true)
				{
					if (DesiredSpecPlayer < Client.All.Count)
					{
						if (Client.All[DesiredSpecPlayer] != null && Client.All[DesiredSpecPlayer].Pawn != null && Client.All[DesiredSpecPlayer].Pawn is Pawn)
						{
							CanSpecPlayer = true;
							break;
						}else
						{
							DesiredSpecPlayer++;
						}
					}else
					{
						break;
					}
				}
				if (CanSpecPlayer)
				{
					SpectatingPlayer = true;
					SpecPlayerIndex = DesiredSpecPlayer;
				}else
				{
					SpectatingPlayer = false;
					SpecPlayerIndex = 0;
					SpecPosition = SpecPosition + (InputBuilderStruct.ViewAngles.ToRotation().Forward * -60);
				}
			}
		}
		if (Input.Pressed(InputButton.SecondaryAttack))
		{
			if (Client.All.Count == 1)
			{
				return;
			}
			if (!SpectatingPlayer)
			{
				bool CanSpecPlayer = false;
				int DesiredSpecPlayer = Client.All.Count - 1;
				Log.Info(DesiredSpecPlayer);
				while (true)
				{
					if (DesiredSpecPlayer < Client.All.Count && DesiredSpecPlayer >= 0)
					{
						if (Client.All[DesiredSpecPlayer] != null && Client.All[DesiredSpecPlayer].Pawn != null && Client.All[DesiredSpecPlayer].Pawn is Pawn)
						{
							CanSpecPlayer = true;
							break;
						}else
						{
							DesiredSpecPlayer--;
						}
					}else
					{
						break;
					}
				}
				if (CanSpecPlayer)
				{
					SpectatingPlayer = true;
					SpecPlayerIndex = DesiredSpecPlayer;
				}
			}else
			{
				bool CanSpecPlayer = false;
				int DesiredSpecPlayer = SpecPlayerIndex - 1;
				Log.Info(DesiredSpecPlayer);
				while (true)
				{
					if (DesiredSpecPlayer < Client.All.Count && DesiredSpecPlayer >= 0)
					{
						if (Client.All[DesiredSpecPlayer] != null && Client.All[DesiredSpecPlayer].Pawn != null && Client.All[DesiredSpecPlayer].Pawn is Pawn)
						{
							CanSpecPlayer = true;
							break;
						}else
						{
							DesiredSpecPlayer--;
						}
					}else
					{
						break;
					}
				}
				if (CanSpecPlayer)
				{
					SpectatingPlayer = true;
					SpecPlayerIndex = DesiredSpecPlayer;
				}else
				{
					SpectatingPlayer = false;
					SpecPlayerIndex = 0;
					SpecPosition = SpecPosition + (InputBuilderStruct.ViewAngles.ToRotation().Forward * -60);
				}
			}
		}
		InputBuilderStruct.Position = SpecPosition;
		InputBuilderStruct.ViewAngles += InputBuilderStruct.AnalogLook;
		if (SpectatingPlayer)
		{
			Pawn SpecBall = Client.All[SpecPlayerIndex].Pawn as Pawn;
			if (SpecBall != null)
			{
				SpecPosition = SpecBall.SRBPos;
				InputBuilderStruct.Position = SpecPosition + (InputBuilderStruct.ViewAngles.ToRotation().Forward * -60);
			}
		}
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		Position = Input.Position;
		EyeRotation = Input.Rotation;
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );
		Position = Input.Position;
		EyeRotation = Input.Rotation;
	}
}
