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
		SMBStage NewStage = new SMBStage("Waiting for Players...", 60, new Vector3(0, 0, 30), new Angles(0, 180, 0), "sky_jun", "waitingforplayers_2", 1);
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
		SMBStage NewStage = new SMBStage("Simple", 90, new Vector3(-1640, 0, 10), new Angles(0, 0, 0), "sky_jun", "w1_jungle", 2);
		NewStage.AddStageObject("models/stages/test_world/st001.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddStageObject("models/stages/test_world/st001_moveplat1.vmdl", new Vector3(0,0,0), Rotation.Identity, Plat1Keyframes);
		NewStage.AddStageObject("models/stages/test_world/st001_moveplat1.vmdl", new Vector3(-240,480,0), Rotation.Identity, Plat2Keyframes);
		NewStage.AddStageObject("models/bg/jungle1/bg_jun1_pot_large.vmdl", GameEnt.BlenderPos(26, 42, 0), Rotation.FromYaw(45));
		NewStage.AddStageObject("models/bg/jungle1/bg_jun1_pot_large.vmdl", GameEnt.BlenderPos(26, 30, 0), Rotation.FromYaw(90));
		NewStage.AddStageObject("models/bg/jungle1/bg_jun1_pot_large.vmdl", GameEnt.BlenderPos(-26, 2, 0), Rotation.FromYaw(135));
		NewStage.AddStageObject("models/bg/jungle1/bg_jun1_pot_large.vmdl", GameEnt.BlenderPos(-26, -10, 0), Rotation.FromYaw(180));
		SMBObject SpinnerPlat = NewStage.AddStageObject("models/stages/test_world/st001_spinner.vmdl", new Vector3(2080,0,-50), Rotation.Identity);
		NewStage.AddGoal(new Vector3(2080, 0, -90), Rotation.FromYaw(0));
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
		SMBStage NewStage = new SMBStage("Hollow", 60, GameEnt.BlenderPos(0, 70, 0.5f), new Angles(0, 0, 0), "sky_jun", "w1_jungle", 1);
		NewStage.AddStageObject("models/stages/test_world/st002.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(0, -86, 0), Rotation.Identity);
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
		SMBStage NewStage = new SMBStage("Repulse", 60, new Vector3(0, 0, 10), new Angles(0, 0, 0), "sky_jun", "w1_jungle", 1);
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
		SMBStage NewStage = new SMBStage("Downhill", 60, new Vector3(0, 0, 1210), new Angles(0, 0, 0), "sky_jun", "w1_jungle", 2);
		NewStage.AddStageObject("models/stages/test_world/st004.vmdl", new Vector3(0,0,1200), Rotation.Identity);
		NewStage.AddGoal(new Vector3(1400,-1600,-1200), Rotation.FromYaw(0));
	}
}

public class st005
{
	public static void CreateStage()
	{
		SMBStage NewStage = new SMBStage("Jump", 60, new Vector3(0, 0, 10), new Angles(0, 90, 0), "sky_jun", "w1_jungle", 1);
		NewStage.AddStageObject("models/stages/test_world/st005.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(new Vector3(-270,1290,-350), Rotation.FromYaw(-90));
	}
}

public class st006
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Concentric", 60, new Vector3(0, 0, 10), new Angles(0, -90, 0), "sky_jun", "w1_jungle", 1);
		NewStage.AddStageObject("models/stages/test_world/st006.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(72, 0, -3.12992f), Rotation.FromYaw(-90));
	}
}