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
		MyGame GameEnt = GameManager.Current as MyGame;
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

	[ClientInput] public Angles ViewAngles { get; set; }
	[ClientInput] public Vector3 InputPosition { get; set; }
	public override void BuildInput()
	{
		float RealDelta = Math.Min(Game.TickInterval, Time.Delta);
		if (JustSpawned)
		{
			BBox StageBounds = GetStageBounds();
			SpecPosition = StageBounds.Center + (Rotation.From(new Angles(-45, 45, 0)).Forward * Math.Max(Math.Max(StageBounds.Size.x, StageBounds.Size.y), StageBounds.Size.z) * 0.75f);
			ViewAngles = new Angles(45, -135, 0);
			JustSpawned = false;
		}
		float speedMult = 2;

		if (Input.Down( "Run" ))
		{
			speedMult = 5;
		}
		if (SpectatingPlayer)
		{
			speedMult = 0;
		}
		Vector3 FDir = ViewAngles.ToRotation().Forward;
		float FStr = Input.AnalogMove.x * 5000 * speedMult * RealDelta;
		Vector3 SDir = ViewAngles.ToRotation().Right;
		float SStr = Input.AnalogMove.y * -5000 * speedMult * RealDelta;
		SpecVelocity += (FDir * FStr) + (SDir * SStr);
		SpecVelocity += -SpecVelocity * RealDelta * 10;
		SpecPosition += SpecVelocity * RealDelta;
		//Rotation MouseLookDiff = (OldLookRotation.Inverse * Input.Rotation).Normal;

		if (SpecPlayerIndex >= Game.Clients.Count)
		{
			SpectatingPlayer = false;
			SpecPlayerIndex = 0;
			SpecPosition = SpecPosition + (ViewAngles.ToRotation().Forward * -60);
		}
		if (SpectatingPlayer)
		{
			Pawn SpecBall = Game.Clients.ElementAt(SpecPlayerIndex).Pawn as Pawn;
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
					if (DesiredSpecPlayer < Game.Clients.Count)
					{
						if ( Game.Clients.ElementAt( DesiredSpecPlayer) != null && Game.Clients.ElementAt(DesiredSpecPlayer).Pawn != null && Game.Clients.ElementAt(DesiredSpecPlayer).Pawn is Pawn)
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
					SpecPosition = SpecPosition + (ViewAngles.ToRotation().Forward * -60);
				}
			}
		}

		if (Input.Pressed( "Jump" ))
		{
			if (!SpectatingPlayer)
			{
				if ( Game.Clients.ElementAt( SpecPlayerIndex) != null && Game.Clients.ElementAt(SpecPlayerIndex).Pawn != null && Game.Clients.ElementAt(SpecPlayerIndex).Pawn is Pawn)
				{
					SpectatingPlayer = true;
				}else
				{
					bool CanSpecPlayer = false;
					int DesiredSpecPlayer = 0;
					Log.Info(DesiredSpecPlayer);
					while (true)
					{
						if (DesiredSpecPlayer < Game.Clients.Count)
						{
							if ( Game.Clients.ElementAt( DesiredSpecPlayer) != null && Game.Clients.ElementAt(DesiredSpecPlayer).Pawn != null && Game.Clients.ElementAt(DesiredSpecPlayer).Pawn is Pawn)
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
				SpecPosition = SpecPosition + (ViewAngles.ToRotation().Forward * -60);
			}
		}

		if (Input.Pressed( "attack1" ))
		{
			if (Game.Clients.Count == 1)
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
					if (DesiredSpecPlayer < Game.Clients.Count)
					{
						if ( Game.Clients.ElementAt( DesiredSpecPlayer) != null && Game.Clients.ElementAt(DesiredSpecPlayer).Pawn != null && Game.Clients.ElementAt(DesiredSpecPlayer).Pawn is Pawn)
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
					if (DesiredSpecPlayer < Game.Clients.Count)
					{
						if ( Game.Clients.ElementAt( DesiredSpecPlayer) != null && Game.Clients.ElementAt(DesiredSpecPlayer).Pawn != null && Game.Clients.ElementAt(DesiredSpecPlayer).Pawn is Pawn)
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
					SpecPosition = SpecPosition + (ViewAngles.ToRotation().Forward * -60);
				}
			}
		}
		if (Input.Pressed( "attack2" ))
		{
			if (Game.Clients.Count == 1)
			{
				return;
			}
			if (!SpectatingPlayer)
			{
				bool CanSpecPlayer = false;
				int DesiredSpecPlayer = Game.Clients.Count - 1;
				Log.Info(DesiredSpecPlayer);
				while (true)
				{
					if (DesiredSpecPlayer < Game.Clients.Count && DesiredSpecPlayer >= 0)
					{
						if ( Game.Clients.ElementAt( DesiredSpecPlayer) != null && Game.Clients.ElementAt(DesiredSpecPlayer).Pawn != null && Game.Clients.ElementAt(DesiredSpecPlayer).Pawn is Pawn)
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
					if (DesiredSpecPlayer < Game.Clients.Count && DesiredSpecPlayer >= 0)
					{
						if (Game.Clients.ElementAt(DesiredSpecPlayer) != null && Game.Clients.ElementAt(DesiredSpecPlayer).Pawn != null && Game.Clients.ElementAt(DesiredSpecPlayer).Pawn is Pawn)
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
					SpecPosition = SpecPosition + (ViewAngles.ToRotation().Forward * -60);
				}
			}
		}
		InputPosition = SpecPosition;
		ViewAngles += Input.AnalogLook;
		if (SpectatingPlayer)
		{
			Pawn SpecBall = Game.Clients.ElementAt( SpecPlayerIndex).Pawn as Pawn;
			if (SpecBall != null)
			{
				SpecPosition = SpecBall.SRBPos;
				InputPosition = SpecPosition + (ViewAngles.ToRotation().Forward * -60);
			}
		}
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );
		Camera.Position = InputPosition;
		Camera.Rotation = ViewAngles.ToRotation();
	}

	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );
		Camera.Position = InputPosition;
		Camera.Rotation = ViewAngles.ToRotation();
	}
}
