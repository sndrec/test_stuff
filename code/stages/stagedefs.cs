using Sandbox;
using SandboxEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

// This is a stupid way to do this. I don't know how else to do it.
// Until beginning to learn C# a week ago, I have only ever written anything in Lua.
// If you know how to refactor this to be more dynamic and less dumb, please send in a pull request. Or give me pointers.

public class stwfp
{
	public static void RotatePlatform(SMBObject InObject)
	{
		InObject.Rotation = Rotation.FromYaw(Time.Now * 10);
	}

	public static void CreateStage()
	{
		//SMBObject.SimulateSMBObjectDelegate PlatformAnimation = new SMBObject.SimulateSMBObjectDelegate(RotatePlatform);
		SMBStage NewStage = new SMBStage("Waiting for Players...", 60, new Vector3(0, 0, 30), new Angles(0, 180, 0), "sky_jun", "waitingforplayers_1", 1);
		SMBObject MainPlat = NewStage.AddStageObject("models/stages/waitingforplayers/waitingforplayers_1.vmdl", new Vector3(0,0,0), Rotation.Identity);
		MainPlat.SimulateSMBObjectCustom = RotatePlatform;
	}
}

public class st001
{
	public static void RotatePlatform(SMBObject InObject)
	{
		InObject.Rotation = Rotation.FromYaw(Time.Now * 50);
	}
	public static void CreateStage()
	{
		List<AnimKeyframe> Plat1Keyframes = new List<AnimKeyframe>
		{
			new AnimKeyframe(0, 3, new Vector3(0, 0, 0), Rotation.Identity),
			new AnimKeyframe(4, 3, new Vector3(-240, 0, 0), Rotation.Identity),
			new AnimKeyframe(5, 3, new Vector3(-240, 0, 0), Rotation.Identity),
			new AnimKeyframe(9, 3, new Vector3(0, 0, 0), Rotation.Identity),
			new AnimKeyframe(10, 3, new Vector3(0, 0, 0), Rotation.Identity)
		};

		List<AnimKeyframe> Plat2Keyframes = new List<AnimKeyframe>
		{
			new AnimKeyframe(0, 3, new Vector3(-240, 480, 0), Rotation.Identity),
			new AnimKeyframe(4, 3, new Vector3(0, 480, 0), Rotation.Identity),
			new AnimKeyframe(5, 3, new Vector3(0, 480, 0), Rotation.Identity),
			new AnimKeyframe(9, 3, new Vector3(-240, 480, 0), Rotation.Identity),
			new AnimKeyframe(10, 3, new Vector3(-240, 480, 0), Rotation.Identity)
		};
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage(
			"Simple",
			90,
			new Vector3(-1640, 0, 10),
			new Angles(0, 0, 0),
			"sky_jun",
			"mus_jungle",
			2);
		NewStage.AddStageObject("models/stages/test_world/st001.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddStageObject("models/stages/test_world/st001_moveplat1.vmdl", new Vector3(0,0,0), Rotation.Identity, Plat1Keyframes);
		NewStage.AddStageObject("models/stages/test_world/st001_moveplat1.vmdl", new Vector3(-240,480,0), Rotation.Identity, Plat2Keyframes);
		NewStage.AddStageObject("models/bg/jungle1/bg_jun1_pot_large.vmdl", GameEnt.BlenderPos(26, 42, 0), Rotation.FromYaw(45));
		NewStage.AddStageObject("models/bg/jungle1/bg_jun1_pot_large.vmdl", GameEnt.BlenderPos(26, 30, 0), Rotation.FromYaw(90));
		NewStage.AddStageObject("models/bg/jungle1/bg_jun1_pot_large.vmdl", GameEnt.BlenderPos(-26, 2, 0), Rotation.FromYaw(135));
		NewStage.AddStageObject("models/bg/jungle1/bg_jun1_pot_large.vmdl", GameEnt.BlenderPos(-26, -10, 0), Rotation.FromYaw(180));
		SMBObject SpinnerPlat = NewStage.AddStageObject("models/stages/test_world/st001_spinner.vmdl", new Vector3(2080,0,-50), Rotation.Identity);
		NewStage.AddGoal(new Vector3(2080, 0, -90), Rotation.FromYaw(180));
		SpinnerPlat.SimulateSMBObjectCustom = RotatePlatform;
	}
}

//Example Animation Keyframes:
//
//List<AnimKeyframe> PlatKeyframes = new List<AnimKeyframe>
//{
//	new AnimKeyframe(0, 3, new Vector3(0, 0, 0), Rotation.Identity),
//	new AnimKeyframe(3, 3, new Vector3(1000, 0, 0), Rotation.FromYaw(15)),
//	new AnimKeyframe(6, 3, new Vector3(0, 0, 0), Rotation.Identity)
//};

public class st002
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Hollow", 60, GameEnt.BlenderPos(0, 70, 0.5f), new Angles(0, 0, 0), "sky_jun", "mus_jungle", 1);
		NewStage.AddStageObject("models/stages/test_world/st002.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddBGObject("models/bg/jungle1/bg_jun1_beanstalk.vmdl", GameEnt.BlenderPos(0, 44, 0f), Rotation.Identity);
		NewStage.AddBGObject("models/bg/jungle1/bg_jun1_beanstalk.vmdl", GameEnt.BlenderPos(0, 0, 0f), Rotation.Identity);
		NewStage.AddBGObject("models/bg/jungle1/bg_jun1_beanstalk.vmdl", GameEnt.BlenderPos(0, -52, 0f), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(0, -86, 0), Rotation.FromYaw(180));
	}
}

public class st003
{
	public static void CreateStage()
	{
		List<AnimKeyframe> Plat1Keyframes = new List<AnimKeyframe>
		{
			new AnimKeyframe(0, 3, new Vector3(0, 0, 0), Rotation.Identity),
			new AnimKeyframe(6, 3, new Vector3(0, -640, 0), Rotation.Identity),
			new AnimKeyframe(7, 3, new Vector3(0, -640, 0), Rotation.Identity),
			new AnimKeyframe(14, 3, new Vector3(0, 0, 0), Rotation.Identity),
			new AnimKeyframe(15, 3, new Vector3(0, 0, 0), Rotation.Identity)
		};

		List<AnimKeyframe> Plat2Keyframes = new List<AnimKeyframe>
		{
			new AnimKeyframe(0, 3, new Vector3(0, 0, 0), Rotation.Identity),
			new AnimKeyframe(6, 3, new Vector3(0, 640, 0), Rotation.Identity),
			new AnimKeyframe(7, 3, new Vector3(0, 640, 0), Rotation.Identity),
			new AnimKeyframe(14, 3, new Vector3(0, 0, 0), Rotation.Identity),
			new AnimKeyframe(15, 3, new Vector3(0, 0, 0), Rotation.Identity)
		};
		SMBStage NewStage = new SMBStage("Repulse", 60, new Vector3(0, 0, 10), new Angles(0, 0, 0), "sky_jun", "mus_jungle", 1);
		NewStage.AddStageObject("models/stages/test_world/st003_mainplat.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddStageObject("models/stages/test_world/st003_movingplat1.vmdl", new Vector3(0,0,0), Rotation.Identity, Plat1Keyframes);
		NewStage.AddStageObject("models/stages/test_world/st003_movingplat2.vmdl", new Vector3(0,0,0), Rotation.Identity, Plat2Keyframes);
		NewStage.AddGoal(new Vector3(1650, 0, 0), Rotation.FromYaw(180));
	}
}

public class st004
{
	public static void CreateStage()
	{
		SMBStage NewStage = new SMBStage("Downhill", 60, new Vector3(0, 0, 1210), new Angles(0, 0, 0), "sky_jun", "mus_jungle", 2);
		NewStage.AddStageObject("models/stages/test_world/st004.vmdl", new Vector3(0,0,1200), Rotation.Identity);
		NewStage.AddGoal(new Vector3(1400,-1600,-1200), Rotation.FromYaw(180));
	}
}

public class st005
{
	public static void CreateStage()
	{
		SMBStage NewStage = new SMBStage("Jump", 60, new Vector3(0, 0, 10), new Angles(0, 90, 0), "sky_sky", "mus_skyhigh", 1);
		NewStage.AddStageObject("models/stages/test_world/st005.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(new Vector3(-270,1290,-350), Rotation.FromYaw(-90));
	}
}

public class st006
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Concentric", 60, new Vector3(0, 0, 10), new Angles(0, -90, 0), "sky_sky", "mus_skyhigh", 1);
		NewStage.AddStageObject("models/stages/test_world/st006.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(72, 0, -3.12992f), Rotation.FromYaw(-90));
	}
}

public class st007
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Cut Corners", 60, GameEnt.BlenderPos(40, -4, 224.5f), new Angles(0, 180, 0), "sky_sky", "mus_skyhigh", 2);
		NewStage.AddStageObject("models/stages/test_world/st007.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(70.8796f, -36.0482f, -15.2052f), Rotation.From(new Angles(-29.5f, -70, 0)));
	}
}

public class st008
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Loops for Days", 60, GameEnt.BlenderPos(-152, -24, 48.5f), new Angles(0, 90, 0), "sky_sky", "mus_skyhigh", 2);
		NewStage.AddStageObject("models/stages/test_world/st008.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(104f, 86f, -39.0237f), Rotation.From(new Angles(0, 0, 0)));
	}
}

public class st009
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Assessment I", 60, GameEnt.BlenderPos(27.0f, -33.0f, 16.125f), new Angles(0, -90, 0), "sky_sky", "mus_skyhigh", 2);
		NewStage.AddStageObject("models/stages/test_world/st009.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(-32.5f, -1.5f, 1.75f), Rotation.From(new Angles(0, 90, 0)));
	}
}

public class staward
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Award Ceremony", 30, GameEnt.BlenderPos(0f, 13.5f, 0.5f), new Angles(0, 0, 0), "sky_bonus", "waitingforplayers_2", 2);
		NewStage.AddStageObject("models/stages/award_main.vmdl", new Vector3(0,0,0), Rotation.Identity);
		int NumPlayers = Client.All.Count;
		AwardCopter Copter1 = new AwardCopter();
		AwardCopter Copter2 = new AwardCopter();
		AwardCopter Copter3 = new AwardCopter();

		if (NumPlayers < 3)
		{
			Copter3.Delete();
		}
		if (NumPlayers < 2)
		{
			Copter2.Delete();
		}
		if (Copter1 != null)
		{
			NewStage.StageObjects.Add(Copter1);
		}
		if (Copter2 != null)
		{
			NewStage.StageObjects.Add(Copter2);
		}
		if (Copter3 != null)
		{
			NewStage.StageObjects.Add(Copter3);
		}
		PlayerStateManager FirstPlace = null;
		PlayerStateManager SecondPlace = null;
		PlayerStateManager ThirdPlace = null;
		//bool FoundAtLeastOneManager = false;
		//foreach (Entity element in Entity.All)
		//{
		//	if (element is PlayerStateManager)
		//	{
		//		FirstPlace = element as PlayerStateManager;
		//		SecondPlace = element as PlayerStateManager;
		//		ThirdPlace = element as PlayerStateManager;
		//		OurManager = element as PlayerStateManager;
		//		FoundAtLeastOneManager = true;
		//		break;
		//	}
		//}
		//if (!FoundAtLeastOneManager)
		//{
		//	Log.Info("No managers!");
		//	return;
		//}
		foreach (Client pl in Client.All)
		{
			PlayerStateManager OurManager = null;
			foreach (Entity element in Entity.All)
			{
				if (element is PlayerStateManager && element.Owner == pl)
				{
					OurManager = element as PlayerStateManager;
					break;
				}
			}
			if (OurManager == null)
			{
				Log.Info("Missing manager?!");
				return;
			}
			if (FirstPlace == null)
			{
				FirstPlace = OurManager;
				continue;
			}
			if (NumPlayers > 0)
			{
				if (OurManager.Score > FirstPlace.Score)
				{
					if (NumPlayers > 1 && FirstPlace != null)
					{
						if (NumPlayers > 2 && SecondPlace != null)
						{
							ThirdPlace = SecondPlace;
						}
					SecondPlace = FirstPlace;
					}
					FirstPlace = OurManager;
					continue;
				}
			}
			if (NumPlayers > 1)
			{
				if (SecondPlace == null)
				{
					SecondPlace = OurManager;
					continue;
				}
				if (OurManager.Score > SecondPlace.Score)
				{
					if (NumPlayers > 2 && SecondPlace != null)
					{
						ThirdPlace = SecondPlace;
					}
					SecondPlace = OurManager;
					continue;
				}
	
			}
			if (NumPlayers > 2)
			{
				if (ThirdPlace == null)
				{
					ThirdPlace = OurManager;
					continue;
				}
				if (OurManager.Score > ThirdPlace.Score)
				{
					ThirdPlace = OurManager;
					continue;
				}
			}
		}
		if (Copter1 != null)
		{
			Copter1.TargetPawnManager = FirstPlace;
			Copter1.Position = new Vector3(0, 0, -150);
			Copter1.Placement = 1;
			Copter1.TargetPosition = new Vector3(0, 0, 60);
		}
		if (Copter2 != null)
		{
			Copter2.TargetPawnManager = SecondPlace;
			Copter2.Position = new Vector3(0, 0, -100);
			Copter2.Placement = 2;
			Copter2.TargetPosition = new Vector3(0, 0, 90);
		}
		if (Copter3 != null)
		{
			Copter3.TargetPawnManager = ThirdPlace;
			Copter3.Position = new Vector3(0, 0, -50);
			Copter3.Placement = 3;
			Copter3.TargetPosition = new Vector3(0, 0, 120);
		}
	}
}

public class stw1bonus
{
	public static void BonusBananaCollected(Pawn InBall, int InBananaValue)
	{
		MyGame GameEnt = Game.Current as MyGame;
		GameEnt.CurrentStage.UserInt1 -= InBananaValue;
		Log.Info("Bananas remaining: " + GameEnt.CurrentStage.UserInt1);
		if (GameEnt.CurrentStage.UserInt1 <= 0)
		{
			GameEnt.NextGameState = Time.Now;
		}
	}
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage(
			"Bonus Wave",
			30,
			new Vector3(180, 180, 10),
			new Angles(0, -135, 0),
			"sky_bonus",
			"mus_bonus",
			2);
		NewStage.UserInt1 = 50;
		for (int i = 0; i < 20; i++)
		{
			float CircleX = (float)Math.Sin(((float)i / 20) * 3.14159 * 2) * 80;
			float CircleY = (float)Math.Cos(((float)i / 20) * 3.14159 * 2) * 80;
			Log.Info(CircleX + " | " + CircleY);
			NewStage.AddBanana(new Vector3(CircleX, CircleY, 15));
		}
		for (int i = 0; i < 30; i++)
		{
			float CircleX = (float)Math.Sin(((float)i / 30) * 3.14159 * 2) * 160;
			float CircleY = (float)Math.Cos(((float)i / 30) * 3.14159 * 2) * 160;
			Log.Info(CircleX + " | " + CircleY);
			NewStage.AddBanana(new Vector3(CircleX, CircleY, 15));
		};
		NewStage.AddStageObject("models/stages/test_world/st093.vmdl", new Vector3(0,0,0), Rotation.Identity);
		ST093Floor WaveFloor = new ST093Floor();
		NewStage.StageObjects.Add(WaveFloor);
		NewStage.OnBananaCollectedMember = BonusBananaCollected;
	}
}