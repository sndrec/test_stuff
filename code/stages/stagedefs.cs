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
		InObject.Rotation *= Rotation.FromYaw(Time.Delta * 10).Normal;
	}

	public static void CreateStage()
	{
		//SMBObject.SimulateSMBObjectDelegate PlatformAnimation = new SMBObject.SimulateSMBObjectDelegate(RotatePlatform);
		SMBStage NewStage = new SMBStage("Waiting for Players...", 60, new Vector3(0, 0, 30), new Angles(0, 180, 0), "sky_default", "waitingforplayers", 1);
		SMBObject MainPlat = NewStage.AddStageObject("models/stages/waitingforplayers/waitingforplayers_1.vmdl", new Vector3(0,0,0), Rotation.Identity);
		MainPlat.SimulateSMBObjectCustom = RotatePlatform;

		foreach (Entity element in Entity.All)
		{
			if (element is PlayerStateManager)
			{
				PlayerStateManager PSM = element as PlayerStateManager;
				PSM.SetScore(0);
				PSM.SetTime( 0 );
			}
		}
	}
}

public class st001
{
	public static void RotatePlatform(SMBObject InObject)
	{
		InObject.Rotation *= Rotation.FromYaw(Time.Delta * 50).Normal;
	}
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		List<PosAnimKeyFrame> Plat1KeyFrames = new List<PosAnimKeyFrame>
		{
			new PosAnimKeyFrame(0, 3, new Vector3(0, 0, 0)),
			new PosAnimKeyFrame(4, 3, new Vector3(-240, 0, 0)),
			new PosAnimKeyFrame(5, 3, new Vector3(-240, 0, 0)),
			new PosAnimKeyFrame(9, 3, new Vector3(0, 0, 0)),
			new PosAnimKeyFrame(10, 3, new Vector3(0, 0, 0))
		};

		List<PosAnimKeyFrame> Plat2KeyFrames = new List<PosAnimKeyFrame>
		{
			new PosAnimKeyFrame(0, 3, new Vector3(-240, 480, 0)),
			new PosAnimKeyFrame(4, 3, new Vector3(0, 480, 0)),
			new PosAnimKeyFrame(5, 3, new Vector3(0, 480, 0)),
			new PosAnimKeyFrame(9, 3, new Vector3(-240, 480, 0)),
			new PosAnimKeyFrame(10, 3, new Vector3(-240, 480, 0))
		};
		List<Vector3> BananaTable = new List<Vector3>
		{
			GameEnt.ConfigPos(600.0f, 5000.0f, 50.0f),
			GameEnt.ConfigPos(-700.0f, 6300.0f, 50.0f),
			GameEnt.ConfigPos(-1250.0f, 5927.639389038086f, 19.721364974975586f),
			GameEnt.ConfigPos(-1250.0f, 5272.360610961914f, 19.721364974975586f),
			GameEnt.ConfigPos(-2727.639389038086f, 5250.0f, 19.721364974975586f),
			GameEnt.ConfigPos(-2727.639389038086f, 5950.0f, 19.721364974975586f),
			GameEnt.ConfigPos(-4600.0f, 6200.0f, 50.0f),
			GameEnt.ConfigPos(-3300.0099182128906f, 4115.435028076172f, 161.10302209854126f),
			GameEnt.ConfigPos(-4699.997711181641f, 4115.439605712891f, 160.99555492401123f),
			GameEnt.ConfigPos(-3250.0f, 4900.0f, 50.0f),
			GameEnt.ConfigPos(-3601.0543823242188f, 2000.6319046020508f, 124.90811347961426f),
			GameEnt.ConfigPos(-4000.180435180664f, 1601.215934753418f, 124.90811347961426f),
			GameEnt.ConfigPos(-3601.2279510498047f, 1200.0605583190918f, 124.90811347961426f),
			GameEnt.ConfigPos(-4401.216125488281f, 1200.180435180664f, 124.90811347961426f),
			GameEnt.ConfigPos(-4401.216125488281f, 2000.180435180664f, 124.90811347961426f),
			GameEnt.ConfigPos(4398.842620849609f, 2000.4142761230469f, 124.9082088470459f),
			GameEnt.ConfigPos(3998.8887786865234f, 1600.5254745483398f, 124.90811347961426f),
			GameEnt.ConfigPos(4399.701309204102f, 1201.1924743652344f, 124.9082088470459f),
			GameEnt.ConfigPos(3599.367904663086f, 1201.0543823242188f, 124.90811347961426f),
			GameEnt.ConfigPos(3598.7838745117188f, 2000.180435180664f, 124.90811347961426f),
			GameEnt.ConfigPos(269.9732780456543f, 1598.5248565673828f, -34.580451250076294f),
			GameEnt.ConfigPos(-1.484321616590023f, 1869.729232788086f, -31.875663995742798f),
			GameEnt.ConfigPos(-270.0251579284668f, 1598.527431488037f, -35.15053391456604f),
			GameEnt.ConfigPos(-1.462133601307869f, 1329.709529876709f, -38.040536642074585f),
			GameEnt.ConfigPos(191.81712865829468f, 1789.718246459961f, -32.51048028469086f),
			GameEnt.ConfigPos(-191.8408989906311f, 1789.7441864013672f, -32.90172815322876f),
			GameEnt.ConfigPos(-190.03983736038208f, 1407.8911781311035f, -37.27567493915558f),
			GameEnt.ConfigPos(190.01156091690063f, 1407.9168319702148f, -36.86164319515228f),
			GameEnt.ConfigPos(3300.003433227539f, 116.79737567901611f, 159.7435712814331f),
			GameEnt.ConfigPos(4699.999237060547f, 116.79596900939941f, 159.7076416015625f),
			GameEnt.ConfigPos(4399.939727783203f, -1998.7722396850586f, 124.90811347961426f),
			GameEnt.ConfigPos(3998.9456176757812f, -2399.368095397949f, 124.90811347961426f),
			GameEnt.ConfigPos(4398.772048950195f, -2799.93953704834f, 124.90811347961426f),
			GameEnt.ConfigPos(3598.8075256347656f, -2799.701499938965f, 124.90811347961426f),
			GameEnt.ConfigPos(3599.585723876953f, -1998.8426208496094f, 124.90811347961426f),
			GameEnt.ConfigPos(2002.486801147461f, -2400.0f, -100.06186962127686f),
			GameEnt.ConfigPos(700.0f, -3100.0f, 50.0f),
			GameEnt.ConfigPos(-600.0f, -1800.0f, 50.0f),
			GameEnt.ConfigPos(-0.0f, -7600.0f, -400.00009536743164f),
			GameEnt.ConfigPos(-0.0f, -6200.0f, -400.00009536743164f),
			GameEnt.ConfigPos(-0.0f, -9000.0f, -400.0f),
			GameEnt.ConfigPos(-2100.0f, -6762.693786621094f, -399.99990463256836f),
			GameEnt.ConfigPos(-3637.3069763183594f, -8300.0f, -399.99990463256836f),
			GameEnt.ConfigPos(-4199.999618530273f, -10400.000762939453f, -400.0f),
			GameEnt.ConfigPos(-3637.3069763183594f, -12500.0f, -399.9998092651367f),
			GameEnt.ConfigPos(-2099.9990463256836f, -14037.307739257812f, -400.0f),
			GameEnt.ConfigPos(-0.0f, -14600.0f, -400.0f),
			GameEnt.ConfigPos(2100.0017166137695f, -14037.306213378906f, -400.00009536743164f),
			GameEnt.ConfigPos(3637.3069763183594f, -12500.0f, -399.99990463256836f),
			GameEnt.ConfigPos(4200.000762939453f, -10399.99771118164f, -400.0f),
			GameEnt.ConfigPos(3637.3069763183594f, -8300.0f, -400.00009536743164f),
			GameEnt.ConfigPos(2099.9982833862305f, -6762.691497802734f, -400.0f),
			GameEnt.ConfigPos(-1979.899024963379f, -8420.101165771484f, -400.0f),
			GameEnt.ConfigPos(-2800.0f, -10400.0f, -400.00009536743164f),
			GameEnt.ConfigPos(-1979.8988342285156f, -12379.898834228516f, -400.0f),
			GameEnt.ConfigPos(0.00019073486328125f, -13200.0f, -400.0f),
			GameEnt.ConfigPos(1979.8988342285156f, -12379.898834228516f, -400.0f),
			GameEnt.ConfigPos(2800.0f, -10400.0f, -400.0f),
			GameEnt.ConfigPos(1979.8988342285156f, -8420.101165771484f, -399.99990463256836f),
			GameEnt.ConfigPos(-1331.4791679382324f, -9967.375946044922f, -400.0f),
			GameEnt.ConfigPos(-822.8996276855469f, -11532.624053955078f, -400.00009536743164f),
			GameEnt.ConfigPos(822.8996276855469f, -11532.624053955078f, -400.00009536743164f),
			GameEnt.ConfigPos(1331.4791679382324f, -9967.375946044922f, -399.99990463256836f),
		};
		SMBStage NewStage = new SMBStage(
			"Simple",
			90,
			new Vector3(-1640, 0, 10),
			new Angles(0, 0, 0),
			"sky_field",
			"mus_field",
			2);
		NewStage.AddBananas(BananaTable);
		NewStage.AddBananaBunch(GameEnt.ConfigPos(1112.4391555786133f, -15124.917602539062f, 1.452857255935669f));
		NewStage.AddBananaBunch(GameEnt.ConfigPos(-1117.4531936645508f, -15124.917602539062f, 1.452857255935669f));
		NewStage.AddStageObject("models/stages/test_world/st001.vmdl", new Vector3(0,0,0), Rotation.Identity);
		SMBObject Plat1 = NewStage.AddStageObject("models/stages/test_world/st001_moveplat1.vmdl", new Vector3(0,0,0), Rotation.Identity);
		Plat1.AddPosKeyFrames(Plat1KeyFrames);
		SMBObject Plat2 = NewStage.AddStageObject("models/stages/test_world/st001_moveplat1.vmdl", new Vector3(-240,480,0), Rotation.Identity);
		Plat2.AddPosKeyFrames(Plat2KeyFrames);
		Plat1.EnableKeyFrameAnimation(true, false);
		Plat2.EnableKeyFrameAnimation(true, false);
		SMBObject SpinnerPlat = NewStage.AddStageObject("models/stages/test_world/st001_spinner.vmdl", new Vector3(2080,0,-50), Rotation.Identity);
		NewStage.AddGoal(new Vector3(2080, 0, -90), Rotation.FromYaw(180));
		SpinnerPlat.SimulateSMBObjectCustom = RotatePlatform;
	}
}

//Example Animation KeyFrames:
//
//List<AnimKeyFrame> PlatKeyFrames = new List<AnimKeyFrame>
//{
//	new AnimKeyFrame(0, 3, new Vector3(0, 0, 0), Rotation.Identity),
//	new AnimKeyFrame(3, 3, new Vector3(1000, 0, 0), Rotation.FromYaw(15)),
//	new AnimKeyFrame(6, 3, new Vector3(0, 0, 0), Rotation.Identity)
//};

public class st002
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Hollow", 60, GameEnt.BlenderPos(0, 70, 0.5f), new Angles(0, 0, 0), "sky_field", "mus_field", 1);
		NewStage.AddStageObject("models/stages/test_world/st002.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(0, -86, 0), Rotation.FromYaw(180));
		List<Vector3> BananaTable = new List<Vector3>
		{
			GameEnt.ConfigPos(700.0f, 6100.0f, 75.0f),
			GameEnt.ConfigPos(-700.0f, 6100.0f, 75.0f),
			GameEnt.ConfigPos(300.0f, 5693.798446655273f, 37.11389899253845f),
			GameEnt.ConfigPos(-0.0f, 5693.798446655273f, 37.11389899253845f),
			GameEnt.ConfigPos(-300.0f, 5693.798446655273f, 37.11389899253845f),
			GameEnt.ConfigPos(300.0f, 3106.20174407959f, 37.11389899253845f),
			GameEnt.ConfigPos(-0.0f, 3106.20174407959f, 37.11389899253845f),
			GameEnt.ConfigPos(-300.0f, 3106.20174407959f, 37.11389899253845f),
			GameEnt.ConfigPos(300.0f, 1686.2640380859375f, 1.647612452507019f),
			GameEnt.ConfigPos(-0.0f, 1686.2640380859375f, 1.647612452507019f),
			GameEnt.ConfigPos(-300.0f, 1686.2640380859375f, 1.647612452507019f),
			GameEnt.ConfigPos(300.0f, -1686.2640380859375f, 1.647612452507019f),
			GameEnt.ConfigPos(-0.0f, -1686.2640380859375f, 1.647612452507019f),
			GameEnt.ConfigPos(-300.0f, -1686.2640380859375f, 1.647612452507019f),
			GameEnt.ConfigPos(300.0f, -3122.360610961914f, -30.278682708740234f),
			GameEnt.ConfigPos(-0.0f, -3122.360610961914f, -30.278682708740234f),
			GameEnt.ConfigPos(-300.0f, -3122.360610961914f, -30.278682708740234f),
			GameEnt.ConfigPos(700.0f, -2700.0f, 74.99995231628418f),
			GameEnt.ConfigPos(700.0f, -2150.0f, 75.0f),
			GameEnt.ConfigPos(-700.0f, -2700.0f, 75.0f),
			GameEnt.ConfigPos(-700.0f, -2150.0f, 74.99995231628418f),
			GameEnt.ConfigPos(700.0f, 2100.0f, 74.99995231628418f),
			GameEnt.ConfigPos(700.0f, 2650.0f, 75.0f),
			GameEnt.ConfigPos(-700.0f, 2100.0f, 75.0f),
			GameEnt.ConfigPos(-700.0f, 2650.0f, 75.0f),
			GameEnt.ConfigPos(-1195.588779449463f, 4400.0f, 33.528727293014526f),
			GameEnt.ConfigPos(1195.588779449463f, 4400.0f, 33.52877497673035f),
			GameEnt.ConfigPos(1545.588779449463f, 4400.0f, 64.52786326408386f),
			GameEnt.ConfigPos(-1545.588779449463f, 4400.0f, 64.5279049873352f),
			GameEnt.ConfigPos(-1390.0790214538574f, 0f, -16.715973615646362f),
			GameEnt.ConfigPos(1390.0790214538574f, 0.0f, -16.71597957611084f),
			GameEnt.ConfigPos(1790.079116821289f, 0.0f, 64.26165103912354f),
			GameEnt.ConfigPos(-1790.079116821289f, 0f, 64.26160335540771f),
			GameEnt.ConfigPos(-1633.3030700683594f, -5200.0f, -62.57957220077515f),
			GameEnt.ConfigPos(-1983.3030700683594f, -5200.0f, 61.416929960250854f),
			GameEnt.ConfigPos(1983.3030700683594f, -5200.0f, 61.41711473464966f),
			GameEnt.ConfigPos(1633.3030700683594f, -5200.0f, -62.57971525192261f)
		};
		NewStage.AddBananas(BananaTable);
		NewStage.AddBananaBunch(GameEnt.ConfigPos(250.0f, -7277.639007568359f, -30.278682708740234f));
		NewStage.AddBananaBunch(GameEnt.ConfigPos(-250.0f, -7277.639007568359f, -30.278682708740234f));
	}
}

public class st003
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		List<PosAnimKeyFrame> Plat1KeyFrames = new List<PosAnimKeyFrame>
		{
			new PosAnimKeyFrame(0, 3, new Vector3(0, 0, 0)),
			new PosAnimKeyFrame(6, 3, new Vector3(0, -640, 0)),
			new PosAnimKeyFrame(7, 3, new Vector3(0, -640, 0)),
			new PosAnimKeyFrame(14, 3, new Vector3(0, 0, 0)),
			new PosAnimKeyFrame(15, 3, new Vector3(0, 0, 0))
		};

		List<PosAnimKeyFrame> Plat2KeyFrames = new List<PosAnimKeyFrame>
		{
			new PosAnimKeyFrame(0, 3, new Vector3(0, 0, 0)),
			new PosAnimKeyFrame(6, 3, new Vector3(0, 640, 0)),
			new PosAnimKeyFrame(7, 3, new Vector3(0, 640, 0)),
			new PosAnimKeyFrame(14, 3, new Vector3(0, 0, 0)),
			new PosAnimKeyFrame(15, 3, new Vector3(0, 0, 0))
		};

		List<Vector3> StaticBananas = new List<Vector3>
		{
			GameEnt.ConfigPos(2200.0f, -4000.0f, 75.0f),
			GameEnt.ConfigPos(-2200.0f, -4000.0f, 75.0f),
			GameEnt.ConfigPos(-0.0f, -4000.0f, 75.0f),
			GameEnt.ConfigPos(1600.0f, -6400.0f, 75.0f),
			GameEnt.ConfigPos(-1600.0f, -6400.0f, 75.0f),
			GameEnt.ConfigPos(-1600.0f, -1600.0f, 75.0f),
			GameEnt.ConfigPos(1600.0f, -1600.0f, 75.0f)
		};
		List<Vector3> Plat1Bananas = new List<Vector3>
		{
			GameEnt.ConfigPos(2000.0f, -2800.0f, 75f),
			GameEnt.ConfigPos(2000.0f, -3600.0f, 75f),
			GameEnt.ConfigPos(1200.0f, -3600.0f, 75f),
			GameEnt.ConfigPos(1200.0f, -2800.0f, 75f)
		};
		List<Vector3> Plat2Bananas = new List<Vector3>
		{
			GameEnt.ConfigPos(-1200.0f, -4500.0f, 75f),
			GameEnt.ConfigPos(-1200.0f, -5300.0f, 75f),
			GameEnt.ConfigPos(-2000.0f, -5300.0f, 75f),
			GameEnt.ConfigPos(-2000.0f, -4500.0f, 75f)
		};
		SMBStage NewStage = new SMBStage("Repulse", 60, new Vector3(0, 0, 10), new Angles(0, 0, 0), "sky_field", "mus_field", 1);

		NewStage.AddBananas(StaticBananas);
		NewStage.AddStageObject("models/stages/test_world/st003_mainplat.vmdl", new Vector3(0,0,0), Rotation.Identity);
		SMBObject Plat1 = NewStage.AddStageObject("models/stages/test_world/st003_movingplat1.vmdl", new Vector3(0,0,0), Rotation.Identity);
		Plat1.AddPosKeyFrames(Plat1KeyFrames);
		SMBObject Plat2 = NewStage.AddStageObject("models/stages/test_world/st003_movingplat2.vmdl", new Vector3(0,0,0), Rotation.Identity);
		Plat2.AddPosKeyFrames(Plat2KeyFrames);
		Plat1.EnableKeyFrameAnimation(true, false);
		Plat2.EnableKeyFrameAnimation(true, false);
		NewStage.AddGoal(new Vector3(1650, 0, 0), Rotation.FromYaw(180));
		foreach (Vector3 Pos in Plat1Bananas)
		{
			Banana OurBanana = NewStage.AddBanana(Pos);
			OurBanana.SetParent(Plat2);
		}
		foreach (Vector3 Pos in Plat2Bananas)
		{
			Banana OurBanana = NewStage.AddBanana(Pos);
			OurBanana.SetParent(Plat1);
		}
	}
}

public class st004
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Downhill", 60, GameEnt.BlenderPos(-54.25f, 50.75f, 80.5f), new Angles(0, 90, 0), "sky_field", "mus_field", 2);
		NewStage.AddStageObject("models/stages/test_world/st004.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(60.0f, -49.25f, -57.5f), Rotation.FromYaw(-90));
		List<Vector3> BananaTable = new List<Vector3>
		{
			GameEnt.ConfigPos(-600.0f, 5550.0f, 6800.0f),
			GameEnt.ConfigPos(-1094.974708557129f, 5344.974899291992f, 6800.0f),
			GameEnt.ConfigPos(-1300.0000953674316f, 4850.0f, 6800.0f),
			GameEnt.ConfigPos(-1300.0f, 2800.0f, 6800.0f),
			GameEnt.ConfigPos(-1094.9748992919922f, 2305.0254821777344f, 6800.0f),
			GameEnt.ConfigPos(-599.9999523162842f, 2100.0f, 6800.0f),
			GameEnt.ConfigPos(-300.0f, 3100.0f, 6800.0f),
			GameEnt.ConfigPos(-300.0f, 4550.0f, 6800.0f),
			GameEnt.ConfigPos(5200.000381469727f, -400.0f, 5550.0f),
			GameEnt.ConfigPos(5694.974899291992f, -194.9748992919922f, 5550.000381469727f),
			GameEnt.ConfigPos(5900.000381469727f, 300.0001907348633f, 5550.0f),
			GameEnt.ConfigPos(5900.0f, 2350.0f, 5550.000381469727f),
			GameEnt.ConfigPos(5694.974517822266f, 2844.9745178222656f, 5550.000381469727f),
			GameEnt.ConfigPos(5199.999618530273f, 3050.0f, 5550.0f),
			GameEnt.ConfigPos(4900.0f, 2050.0f, 5550.0f),
			GameEnt.ConfigPos(4900.0f, 600.0f, 5550.000381469727f),
			GameEnt.ConfigPos(-600.0f, 550.0f, 4300.0f),
			GameEnt.ConfigPos(-1094.974708557129f, 344.9748992919922f, 4300.0f),
			GameEnt.ConfigPos(-1300.0000953674316f, -150.0f, 4299.999618530273f),
			GameEnt.ConfigPos(-1300.0f, -2200.0f, 4299.999618530273f),
			GameEnt.ConfigPos(-1094.9748992919922f, -2694.9745178222656f, 4300.000381469727f),
			GameEnt.ConfigPos(-599.9999523162842f, -2900.0f, 4299.999618530273f),
			GameEnt.ConfigPos(-300.0f, -1900.0f, 4299.999618530273f),
			GameEnt.ConfigPos(-300.0f, -450.0f, 4300.0f),
			GameEnt.ConfigPos(5200.000381469727f, -5400.0f, 3050.0f),
			GameEnt.ConfigPos(5694.974899291992f, -5194.974899291992f, 3050.0f),
			GameEnt.ConfigPos(5900.000381469727f, -4700.0f, 3050.0f),
			GameEnt.ConfigPos(5900.0f, -2650.0f, 3050.0f),
			GameEnt.ConfigPos(5694.974517822266f, -2155.0254821777344f, 3050.000762939453f),
			GameEnt.ConfigPos(5199.999618530273f, -1950.0f, 3050.000762939453f),
			GameEnt.ConfigPos(4900.0f, -2950.0f, 3050.0f),
			GameEnt.ConfigPos(4900.0f, -4400.0f, 3050.000762939453f),
			GameEnt.ConfigPos(1212.1268272399902f, -2000.0f, 3948.507308959961f),
			GameEnt.ConfigPos(3412.126922607422f, -2000.0f, 3398.507308959961f),
			GameEnt.ConfigPos(1212.1268272399902f, -2850.0f, 3948.507308959961f),
			GameEnt.ConfigPos(3412.126922607422f, -2850.0f, 3398.507308959961f),
			GameEnt.ConfigPos(1187.8731727600098f, 500.0f, 4648.507308959961f),
			GameEnt.ConfigPos(3387.873077392578f, 500.0f, 5198.507308959961f),
			GameEnt.ConfigPos(1187.8731727600098f, -350.0f, 4648.507308959961f),
			GameEnt.ConfigPos(3387.873077392578f, -350.0f, 5198.507308959961f),
			GameEnt.ConfigPos(1212.1268272399902f, 3000.0f, 6448.506927490234f),
			GameEnt.ConfigPos(3412.126922607422f, 3000.0f, 5898.506927490234f),
			GameEnt.ConfigPos(1212.1268272399902f, 2150.0f, 6448.506927490234f),
			GameEnt.ConfigPos(3412.126922607422f, 2150.0f, 5898.507308959961f),
			GameEnt.ConfigPos(1187.8731727600098f, 5500.0f, 7148.506927490234f),
			GameEnt.ConfigPos(3387.873077392578f, 5500.0f, 7698.506927490234f),
			GameEnt.ConfigPos(1187.8731727600098f, 4650.0f, 7148.506927490234f),
			GameEnt.ConfigPos(3387.873077392578f, 4650.0f, 7698.506927490234f)
		};
		NewStage.AddBananas(BananaTable);
	}
}

public class st005
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Jump", 60, GameEnt.BlenderPos(-25, 25, 0.5f), new Angles(0, 45, 0), "sky_christmas", "mus_desertruins_intro", 1.5f);
		NewStage.AddStageObject("models/stages/test_world/st005.vmdl", new Vector3(0,0,0), Rotation.Identity);
		List<SMBObject> TiltPlats1 = new List<SMBObject>();
		List<SMBObject> TiltPlats2 = new List<SMBObject>();
		List<SMBObject> TiltPlats3 = new List<SMBObject>();
		TiltPlats1.Add( NewStage.AddStageObject( "models/stages/test_world/st005_tiltingplatform.vmdl", GameEnt.BlenderPos( -16, 16, 0 ), Rotation.Identity ));
		TiltPlats1.Add( NewStage.AddStageObject( "models/stages/test_world/st005_tiltingplatform.vmdl", GameEnt.BlenderPos( 0, 16, 0 ), Rotation.Identity ));
		TiltPlats1.Add( NewStage.AddStageObject( "models/stages/test_world/st005_tiltingplatform.vmdl", GameEnt.BlenderPos( -16, 0, 0 ), Rotation.Identity ));
		TiltPlats1.Add( NewStage.AddStageObject( "models/stages/test_world/st005_tiltingplatform.vmdl", GameEnt.BlenderPos( 0, 0, 0 ), Rotation.Identity ));
		TiltPlats1.Add( NewStage.AddStageObject( "models/stages/test_world/st005_tiltingplatform.vmdl", GameEnt.BlenderPos( 0, -16, 0 ), Rotation.Identity ));
		TiltPlats1.Add( NewStage.AddStageObject( "models/stages/test_world/st005_tiltingplatform.vmdl", GameEnt.BlenderPos( 16, 0, 0 ), Rotation.Identity ));
		TiltPlats1.Add( NewStage.AddStageObject( "models/stages/test_world/st005_tiltingplatform.vmdl", GameEnt.BlenderPos( 16, -16, 0 ), Rotation.Identity ));
		TiltPlats2.Add( NewStage.AddStageObject( "models/stages/test_world/st005_tiltingplatform.vmdl", GameEnt.BlenderPos( -8, 16, 0 ), Rotation.Identity ));
		TiltPlats2.Add( NewStage.AddStageObject( "models/stages/test_world/st005_tiltingplatform.vmdl", GameEnt.BlenderPos( -8, 0, 0 ), Rotation.Identity ));
		TiltPlats2.Add( NewStage.AddStageObject( "models/stages/test_world/st005_tiltingplatform.vmdl", GameEnt.BlenderPos( 8, 0, 0 ), Rotation.Identity ));
		TiltPlats2.Add( NewStage.AddStageObject( "models/stages/test_world/st005_tiltingplatform.vmdl", GameEnt.BlenderPos( 8, -16, 0 ), Rotation.Identity ));
		TiltPlats3.Add( NewStage.AddStageObject( "models/stages/test_world/st005_tiltingplatform.vmdl", GameEnt.BlenderPos( -16, 8, 0 ), Rotation.Identity ));
		TiltPlats3.Add( NewStage.AddStageObject( "models/stages/test_world/st005_tiltingplatform.vmdl", GameEnt.BlenderPos( 0, 8, 0 ), Rotation.Identity ));
		TiltPlats3.Add( NewStage.AddStageObject( "models/stages/test_world/st005_tiltingplatform.vmdl", GameEnt.BlenderPos( 0, -8, 0 ), Rotation.Identity ));
		TiltPlats3.Add( NewStage.AddStageObject( "models/stages/test_world/st005_tiltingplatform.vmdl", GameEnt.BlenderPos( 16, -8, 0 ), Rotation.Identity ));

		List<RotAnimKeyFrame> TiltKeyFrames1 = new List<RotAnimKeyFrame>
		{
			new RotAnimKeyFrame(0, 1, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(2, 2, Rotation.From(new Angles(35, 0, 0))),
			new RotAnimKeyFrame(4, 1, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(6, 2, Rotation.From(new Angles(0, 0, 35))),
			new RotAnimKeyFrame(8, 1, Rotation.From(new Angles(0, 0, 0)))
		};
		List<RotAnimKeyFrame> TiltKeyFrames2 = new List<RotAnimKeyFrame>
		{
			new RotAnimKeyFrame(0, 1, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(2, 2, Rotation.From(new Angles(-35, 0, 0))),
			new RotAnimKeyFrame(4, 1, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(6, 2, Rotation.From(new Angles(0, 0, 35))),
			new RotAnimKeyFrame(8f, 1, Rotation.From(new Angles(0, 0, 0)))
		};
		List<RotAnimKeyFrame> TiltKeyFrames3 = new List<RotAnimKeyFrame>
		{
			new RotAnimKeyFrame(0, 1, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(2, 2, Rotation.From(new Angles(-35, 0, 0))),
			new RotAnimKeyFrame(4, 1, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(6, 2, Rotation.From(new Angles(0, 0, -35))),
			new RotAnimKeyFrame(8, 1, Rotation.From(new Angles(0, 0, 0)))
		};

		foreach (SMBObject Obj in TiltPlats1)
		{
			Obj.AddRotKeyFrames( TiltKeyFrames2 );
			Obj.EnableKeyFrameAnimation( false, true );
			Obj.RotAnimPlaybackRate = 2f;
		}
		foreach ( SMBObject Obj in TiltPlats2 )
		{
			Obj.AddRotKeyFrames( TiltKeyFrames3 );
			Obj.EnableKeyFrameAnimation( false, true );
			Obj.RotAnimPlaybackRate = 2f;
		}
		foreach ( SMBObject Obj in TiltPlats3 )
		{
			Obj.AddRotKeyFrames( TiltKeyFrames1 );
			Obj.EnableKeyFrameAnimation( false, true );
			Obj.RotAnimPlaybackRate = 2f;
		}
		NewStage.AddGoal(GameEnt.BlenderPos(25f, -25f, 0f), Rotation.FromYaw(-135));
	}
}

public class st006
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Concentric", 60, GameEnt.BlenderPos(-20f, 0, 0.5f), new Angles(0, 90, 0), "sky_field", "mus_field", 1);
		NewStage.AddStageObject("models/stages/test_world/st006.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(72, 0, -3.12992f), Rotation.FromYaw(-90));
		List<Vector3> BananaTable = new List<Vector3>
		{
			GameEnt.ConfigPos(942.0760154724121f, 1631.4605712890625f, -206.66460990905762f),
			GameEnt.ConfigPos(-2200.0f, 471.209192276001f, -133.38404893875122f),
			GameEnt.ConfigPos(-2599.9244689941406f, 473.2903480529785f, -131.28997087478638f),
			GameEnt.ConfigPos(-3000.068473815918f, 471.26665115356445f, -132.81612396240234f),
			GameEnt.ConfigPos(-2200.0f, -371.2096691131592f, -133.3840847015381f),
			GameEnt.ConfigPos(-2599.923896789551f, -373.2929229736328f, -131.32154941558838f),
			GameEnt.ConfigPos(-3000.069046020508f, -371.26431465148926f, -132.79180526733398f),
			GameEnt.ConfigPos(423.39367866516113f, 4449.7589111328125f, -131.02662563323975f),
			GameEnt.ConfigPos(419.39477920532227f, 4850.201797485352f, -133.2432746887207f),
			GameEnt.ConfigPos(421.2594985961914f, 5250.066375732422f, -132.8891634941101f),
			GameEnt.ConfigPos(-423.32677841186523f, 4449.871444702148f, -131.6715955734253f),
			GameEnt.ConfigPos(-419.3953514099121f, 4850.201797485352f, -133.24272632598877f),
			GameEnt.ConfigPos(-421.2601661682129f, 5250.066375732422f, -132.88830518722534f),
			GameEnt.ConfigPos(421.2597370147705f, -5250.066375732422f, -132.88872241973877f),
			GameEnt.ConfigPos(419.39496994018555f, -4850.201797485352f, -133.2434892654419f),
			GameEnt.ConfigPos(423.3262062072754f, -4449.871444702148f, -131.67312145233154f),
			GameEnt.ConfigPos(-421.26011848449707f, -5250.066375732422f, -132.88874626159668f),
			GameEnt.ConfigPos(-419.3951606750488f, -4850.201797485352f, -133.2432746887207f),
			GameEnt.ConfigPos(-423.3940601348877f, -4449.7589111328125f, -131.02586269378662f),
			GameEnt.ConfigPos(-2400.289726257324f, 44.424208998680115f, -260.446834564209f),
			GameEnt.ConfigPos(-2800.162887573242f, 46.50798738002777f, -261.68813705444336f),
			GameEnt.ConfigPos(1.2270626612007618f, 4650.0f, -263.0067825317383f),
			GameEnt.ConfigPos(1.2268719263374805f, 5050.0f, -263.0067825317383f),
			GameEnt.ConfigPos(-1.2274443171918392f, -5050.0f, -263.0067825317383f),
			GameEnt.ConfigPos(-1.2274443171918392f, -4650.0f, -263.0067825317383f),
			GameEnt.ConfigPos(657.9240322113037f, 1139.8208618164062f, -206.95490837097168f),
			GameEnt.ConfigPos(-942.0762062072754f, 1631.4605712890625f, -206.6645860671997f),
			GameEnt.ConfigPos(-657.9241275787354f, 1139.8208618164062f, -206.95490837097168f),
			GameEnt.ConfigPos(-942.0757293701172f, -1631.460952758789f, -206.66382312774658f),
			GameEnt.ConfigPos(-657.9238891601562f, -1139.820957183838f, -206.95528984069824f),
			GameEnt.ConfigPos(942.0766830444336f, -1631.4605712890625f, -206.66382312774658f),
			GameEnt.ConfigPos(657.924222946167f, -1139.8207664489746f, -206.95490837097168f),
			GameEnt.ConfigPos(-0.08009104058146477f, -1596.729850769043f, -263.09845447540283f),
			GameEnt.ConfigPos(0.07984994444996119f, 1596.729850769043f, -263.0988359451294f),
			GameEnt.ConfigPos(0.08046840084716678f, -3796.7300415039062f, -263.09921741485596f),
			GameEnt.ConfigPos(-3944.801712036133f, -1056.8707466125488f, -206.4643383026123f),
			GameEnt.ConfigPos(-3396.2345123291016f, -910.1544380187988f, -207.15479850769043f),
			GameEnt.ConfigPos(-2887.4942779541016f, -2888.052558898926f, -206.83701038360596f),
			GameEnt.ConfigPos(-2486.516761779785f, -2485.9588623046875f, -206.83703422546387f),
			GameEnt.ConfigPos(-2887.494659423828f, 2888.052558898926f, -206.8366527557373f),
			GameEnt.ConfigPos(-2486.516761779785f, 2485.959053039551f, -206.83741569519043f),
			GameEnt.ConfigPos(-3944.801712036133f, 1056.8702697753906f, -206.4643383026123f),
			GameEnt.ConfigPos(-3396.2345123291016f, 910.1546287536621f, -207.155179977417f),
			GameEnt.ConfigPos(-1056.870174407959f, 3944.801712036133f, -206.46471977233887f),
			GameEnt.ConfigPos(-910.1541519165039f, 3396.2345123291016f, -207.15441703796387f),
			GameEnt.ConfigPos(-1056.8699836730957f, -3944.8020935058594f, -206.4643144607544f),
			GameEnt.ConfigPos(-910.1542472839355f, -3396.234130859375f, -207.15479850769043f),
			GameEnt.ConfigPos(-1898.3875274658203f, -3288.0504608154297f, -263.03207874298096f),
			GameEnt.ConfigPos(-3288.0504608154297f, -1898.3877182006836f, -263.03207874298096f),
			GameEnt.ConfigPos(-3796.7300415039062f, -0.08029642631299794f, -263.0988359451294f),
			GameEnt.ConfigPos(-3288.0504608154297f, 1898.3880996704102f, -263.03207874298096f),
			GameEnt.ConfigPos(-1898.3875274658203f, 3288.0504608154297f, -263.0328416824341f),
			GameEnt.ConfigPos(-0.08008693112060428f, 3796.7300415039062f, -263.09845447540283f),
			GameEnt.ConfigPos(-6069.838333129883f, -1626.2725830078125f, -206.264066696167f),
			GameEnt.ConfigPos(-5521.271133422852f, -1479.5563697814941f, -207.35507011413574f),
			GameEnt.ConfigPos(-6000.0f, 0.0f, -262.99171447753906f),
			GameEnt.ConfigPos(-6069.838333129883f, 1626.2718200683594f, -206.26444816589355f),
			GameEnt.ConfigPos(-5521.271133422852f, 1479.5563697814941f, -207.3554515838623f),
			GameEnt.ConfigPos(-5193.306350708008f, 2998.3877182006836f, -262.9939317703247f),
			GameEnt.ConfigPos(-4443.129348754883f, 4443.687057495117f, -206.8366289138794f),
			GameEnt.ConfigPos(-4042.151641845703f, 4041.5939331054688f, -206.83703422546387f),
			GameEnt.ConfigPos(-2998.3877182006836f, 5193.306350708008f, -262.99355030059814f),
			GameEnt.ConfigPos(-1626.272201538086f, 6069.83757019043f, -206.26482963562012f),
			GameEnt.ConfigPos(-1479.5563697814941f, 5521.271133422852f, -207.35430717468262f),
			GameEnt.ConfigPos(0.08027709554880857f, 5996.72966003418f, -263.09921741485596f),
			GameEnt.ConfigPos(-4443.128967285156f, -4443.68782043457f, -206.83703422546387f),
			GameEnt.ConfigPos(-4042.151641845703f, -4041.593551635742f, -206.83624744415283f),
			GameEnt.ConfigPos(-5193.306350708008f, -2998.3877182006836f, -262.9931688308716f),
			GameEnt.ConfigPos(-1626.2714385986328f, -6069.838333129883f, -206.26368522644043f),
			GameEnt.ConfigPos(-1479.5563697814941f, -5521.271133422852f, -207.35468864440918f),
			GameEnt.ConfigPos(-2998.3877182006836f, -5193.306350708008f, -262.99431324005127f),
			GameEnt.ConfigPos(-0.08027709554880857f, -5996.72966003418f, -263.09921741485596f)
		};
		NewStage.AddBananas(BananaTable);
	}
}

public class st007
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Cut Corners", 90, GameEnt.BlenderPos(40, -4, 224.5f), new Angles(0, 180, 0), "sky_field", "mus_field", 2);
		NewStage.AddStageObject("models/stages/test_world/st007.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(70.8796f, -36.0482f, -15.2052f), Rotation.From(new Angles(-29.5f, -70, 0)));
		List<Vector3> BananaTable = new List<Vector3>
		{
			GameEnt.ConfigPos(-3454.7924041748047f, 6312.969970703125f, 16845.81756591797f),
			GameEnt.ConfigPos(-2631.329345703125f, 6817.673492431641f, 16467.95196533203f),
			GameEnt.ConfigPos(-1754.739761352539f, 5941.084289550781f, 15761.766052246094f),
			GameEnt.ConfigPos(-1515.6699180603027f, 6897.36328125f, 15826.142883300781f),
			GameEnt.ConfigPos(-825.0235557556152f, 6392.6605224609375f, 15309.724426269531f),
			GameEnt.ConfigPos(503.14245223999023f, 6552.040100097656f, 14559.526062011719f),
			GameEnt.ConfigPos(25.002974271774292f, 5781.703948974609f, 14676.329040527344f),
			GameEnt.ConfigPos(609.3958854675293f, 4719.171142578125f, 14102.748107910156f),
			GameEnt.ConfigPos(1193.7888145446777f, 5409.8175048828125f, 13906.059265136719f),
			GameEnt.ConfigPos(2150.0686645507812f, 5834.830474853516f, 13432.612609863281f),
			GameEnt.ConfigPos(1964.1254425048828f, 4666.044235229492f, 13291.178894042969f),
			GameEnt.ConfigPos(2787.5885009765625f, 5064.493942260742f, 12890.473937988281f),
			GameEnt.ConfigPos(3053.221893310547f, 3975.3978729248047f, 12499.455261230469f),
			GameEnt.ConfigPos(3982.9376220703125f, 4400.410842895508f, 12041.694641113281f),
			GameEnt.ConfigPos(3611.0519409179688f, 5144.183731079102f, 12421.238708496094f),
			GameEnt.ConfigPos(4939.2181396484375f, 4666.043853759766f, 11533.985900878906f),
			GameEnt.ConfigPos(4487.641143798828f, 3630.0743103027344f, 11578.007507324219f),
			GameEnt.ConfigPos(5354.60205078125f, 3114.3495559692383f, 10195.558166503906f),
			GameEnt.ConfigPos(5839.9566650390625f, 2279.3352127075195f, 9845.594787597656f),
			GameEnt.ConfigPos(4943.153381347656f, 1423.436164855957f, 9217.115783691406f),
			GameEnt.ConfigPos(5893.595123291016f, 1162.120246887207f, 9262.092590332031f),
			GameEnt.ConfigPos(5372.916412353516f, 483.43677520751953f, 8800.590515136719f),
			GameEnt.ConfigPos(5501.264953613281f, -848.0859756469727f, 8117.619323730469f),
			GameEnt.ConfigPos(4742.2943115234375f, -352.10397243499756f, 8234.428405761719f),
			GameEnt.ConfigPos(3666.4161682128906f, -911.5480422973633f, 7728.889465332031f),
			GameEnt.ConfigPos(4343.2403564453125f, -1511.8951797485352f, 7540.715026855469f),
			GameEnt.ConfigPos(4745.826721191406f, -2477.8303146362305f, 7105.140686035156f),
			GameEnt.ConfigPos(3581.696319580078f, -2264.668846130371f, 6993.217468261719f),
			GameEnt.ConfigPos(3960.8261108398438f, -3097.20401763916f, 6624.024963378906f),
			GameEnt.ConfigPos(2865.828514099121f, -3337.355422973633f, 6284.510803222656f),
			GameEnt.ConfigPos(3269.0353393554688f, -4276.73454284668f, 5863.172912597656f),
			GameEnt.ConfigPos(4021.2814331054688f, -3922.3018646240234f, 6197.120666503906f),
			GameEnt.ConfigPos(3512.2848510742188f, -5238.951110839844f, 5398.750305175781f),
			GameEnt.ConfigPos(2487.1328353881836f, -4763.327407836914f, 5453.236389160156f),
			GameEnt.ConfigPos(2319.4231033325195f, -6330.210113525391f, 4064.9063110351562f),
			GameEnt.ConfigPos(1446.3552474975586f, -6743.210601806641f, 3680.7174682617188f),
			GameEnt.ConfigPos(669.3621158599854f, -5777.238845825195f, 3080.799674987793f),
			GameEnt.ConfigPos(328.6062479019165f, -6702.176666259766f, 3078.6176681518555f),
			GameEnt.ConfigPos(-303.6142110824585f, -6125.968933105469f, 2631.068229675293f),
			GameEnt.ConfigPos(-1641.2214279174805f, -6141.254425048828f, 1922.987174987793f),
			GameEnt.ConfigPos(-1082.8315734863281f, -5426.946258544922f, 2081.6083908081055f),
			GameEnt.ConfigPos(-2204.721260070801f, -4931.241226196289f, 1389.338493347168f),
			GameEnt.ConfigPos(-3201.241683959961f, -5250.697708129883f, 921.4277267456055f),
			GameEnt.ConfigPos(-3752.010726928711f, -4416.130447387695f, 467.19188690185547f),
			GameEnt.ConfigPos(-4419.137191772461f, -4833.602142333984f, 193.32715272903442f),
			GameEnt.ConfigPos(-6052.760696411133f, -3948.2025146484375f, -846.3579177856445f),
			GameEnt.ConfigPos(-5056.408309936523f, -4286.250686645508f, -251.32617950439453f),
			GameEnt.ConfigPos(-7254.088592529297f, -3549.053192138672f, -1570.7993507385254f),
			GameEnt.ConfigPos(-7407.9742431640625f, -3493.2815551757812f, -1646.7432022094727f),
			GameEnt.ConfigPos(-7546.471405029297f, -3441.7999267578125f, -1715.0928497314453f),
			GameEnt.ConfigPos(-7669.580078125f, -3394.608688354492f, -1775.8480072021484f),
			GameEnt.ConfigPos(-7792.688751220703f, -3355.9974670410156f, -1836.6031646728516f),
			GameEnt.ConfigPos(-7908.103179931641f, -3321.6766357421875f, -1893.5611724853516f),
			GameEnt.ConfigPos(-8023.517608642578f, -3283.065414428711f, -1950.5193710327148f)
		};
		NewStage.AddBananas(BananaTable);
	}
}

public class st008
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Loops for Days", 60, GameEnt.BlenderPos(-152, -24, 48.5f), new Angles(0, 90, 0), "sky_field", "mus_field", 2);
		NewStage.AddStageObject("models/stages/test_world/st008.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(104f, 86f, -39.0237f), Rotation.From(new Angles(0, 0, 0)));
		List<Vector3> BananaTable = new List<Vector3>
		{
			GameEnt.ConfigPos(12775.088500976562f, -1800.0f, 4414.573669433594f),
			GameEnt.ConfigPos(12775.088500976562f, -3000.0f, 4414.573669433594f),
			GameEnt.ConfigPos(11764.644622802734f, -2400.0f, 3560.8428955078125f),
			GameEnt.ConfigPos(10764.644622802734f, -1800.0f, 2560.8421325683594f),
			GameEnt.ConfigPos(10764.644622802734f, -3000.0f, 2560.8421325683594f),
			GameEnt.ConfigPos(9764.644622802734f, -2400.0f, 1560.8421325683594f),
			GameEnt.ConfigPos(8764.644622802734f, -1800.0f, 560.8428478240967f),
			GameEnt.ConfigPos(8764.644622802734f, -3000.0f, 560.8428478240967f),
			GameEnt.ConfigPos(7764.644622802734f, -2400.0f, -439.1571521759033f),
			GameEnt.ConfigPos(6764.644622802734f, -1800.0f, -1439.1578674316406f),
			GameEnt.ConfigPos(6764.644622802734f, -3000.0f, -1439.1578674316406f),
			GameEnt.ConfigPos(5764.644622802734f, -2400.0f, -2439.1578674316406f),
			GameEnt.ConfigPos(4766.650009155273f, -1800.0f, -3429.3453216552734f),
			GameEnt.ConfigPos(4766.650009155273f, -3000.0f, -3429.3453216552734f),
			GameEnt.ConfigPos(3782.291030883789f, -2400.0f, -4023.2452392578125f),
			GameEnt.ConfigPos(-4839.215087890625f, -5250.160980224609f, -4044.391632080078f),
			GameEnt.ConfigPos(-5973.723220825195f, -4171.342468261719f, -4276.1810302734375f),
			GameEnt.ConfigPos(-7073.8739013671875f, -4549.897384643555f, -4100.579071044922f),
			GameEnt.ConfigPos(-7943.931579589844f, -3804.7115325927734f, -4162.054443359375f),
			GameEnt.ConfigPos(-9023.368835449219f, -3730.8284759521484f, -4004.0393829345703f),
			GameEnt.ConfigPos(-8934.090423583984f, -2242.5832748413086f, -4273.611068725586f),
			GameEnt.ConfigPos(-9672.484588623047f, -1860.2294921875f, -4165.783309936523f),
			GameEnt.ConfigPos(-9455.3955078125f, -650.1232624053955f, -4336.36474609375f),
			GameEnt.ConfigPos(-10822.781372070312f, -18.167676031589508f, -4035.6014251708984f),
			GameEnt.ConfigPos(-10099.406433105469f, 900.9553909301758f, -4228.084182739258f)
		};
		NewStage.AddBananas(BananaTable);
		NewStage.AddBananaBunch(GameEnt.ConfigPos(2675.0f, -2950.0f, -2525.0f));
		NewStage.AddBananaBunch(GameEnt.ConfigPos(2675.0f, -3350.0f, -2525.0f));
		NewStage.AddBananaBunch(GameEnt.ConfigPos(2675.0f, -3750.0f, -2525.0f));
		NewStage.AddBananaBunch(GameEnt.ConfigPos(2675.0f, -4150.0f, -2525.0f));
	}
}

public class st009
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Assessment I", 120, GameEnt.BlenderPos(27.0f, -33.0f, 16.125f), new Angles(0, -90, 0), "sky_sky", "mus_desertruins_intro", 2);
		NewStage.AddStageObject("models/stages/test_world/st009.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(-64.0f, -0.5f, 1.75f), Rotation.From(new Angles(0, 90, 0)));
	}
}

public class st010
{
	public static void RotatePlatform(SMBObject InObject)
	{
		InObject.Rotation *= Rotation.FromYaw(Time.Delta * 90).Normal;
	}
	public static void CreateStage()
	{
		List<RotAnimKeyFrame> Flipper1KeyFrames = new List<RotAnimKeyFrame>
		{
			new RotAnimKeyFrame(0, 3, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(1, 3, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(1.5f, 3, Rotation.From(new Angles(90, 0, 0))),
			new RotAnimKeyFrame(3, 3, Rotation.From(new Angles(90, 0, 0))),
			new RotAnimKeyFrame(3.5f, 3, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(5, 3, Rotation.From(new Angles(0, 0, 0)))
		};
		List<RotAnimKeyFrame> Flipper2KeyFrames = new List<RotAnimKeyFrame>
		{
			new RotAnimKeyFrame(0, 3, Rotation.From(new Angles(0, 180, 0))),
			new RotAnimKeyFrame(1, 3, Rotation.From(new Angles(0, 180, 0))),
			new RotAnimKeyFrame(1.5f, 3, Rotation.From(new Angles(90, 180, 0))),
			new RotAnimKeyFrame(3, 3, Rotation.From(new Angles(90, 180, 0))),
			new RotAnimKeyFrame(3.5f, 3, Rotation.From(new Angles(0, 180, 0))),
			new RotAnimKeyFrame(5, 3, Rotation.From(new Angles(0, 180, 0)))
		};
		List<RotAnimKeyFrame> Flipper3KeyFrames = new List<RotAnimKeyFrame>
		{
			new RotAnimKeyFrame(0, 3, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(1, 3, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(1.5f, 3, Rotation.From(new Angles(90, 0, 0))),
			new RotAnimKeyFrame(3, 3, Rotation.From(new Angles(90, 0, 0))),
			new RotAnimKeyFrame(3.5f, 3, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(5, 3, Rotation.From(new Angles(0, 0, 0)))
		};
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Snakeway", 60, GameEnt.BlenderPos(-16.0f, 12f, 0.5f), new Angles(0, -45, 0), "sky_sky", "mus_desertruins_intro", 2);
		NewStage.AddStageObject("models/stages/test_world/st010_main.vmdl", GameEnt.BlenderPos(0f, 0f, 0f), Rotation.Identity);
		SMBObject Flipper1 = NewStage.AddStageObject("models/stages/test_world/st010_flipper1.vmdl", GameEnt.BlenderPos(0f, -10.0f, -0.18f), Rotation.From(new Angles(0, 0, 0)));
		Flipper1.AddRotKeyFrames(Flipper1KeyFrames);
		Flipper1.EnableKeyFrameAnimation(false, true);
		Flipper1.RotAnimTime = 3.3333f;
		SMBObject Flipper2 = NewStage.AddStageObject("models/stages/test_world/st010_flipper1.vmdl", GameEnt.BlenderPos(-8f, -14f, -0.18f), Rotation.From(new Angles(0, 180, 0)));
		Flipper2.AddRotKeyFrames(Flipper2KeyFrames);
		Flipper2.EnableKeyFrameAnimation(false, true);
		Flipper2.RotAnimTime = 1.6666f;
		SMBObject Flipper3 = NewStage.AddStageObject("models/stages/test_world/st010_flipper1.vmdl", GameEnt.BlenderPos(-16.0f, -10f, -0.18f), Rotation.From(new Angles(0, 0, 0)));
		Flipper3.AddRotKeyFrames(Flipper3KeyFrames);
		Flipper3.EnableKeyFrameAnimation(false, true);
		SMBObject Spinner = NewStage.AddStageObject("models/stages/test_world/st010_spinner.vmdl", GameEnt.BlenderPos(12.0f, 12f, -0.5f), Rotation.Identity);
		Spinner.SimulateSMBObjectCustom = RotatePlatform;
		NewStage.AddGoal(GameEnt.BlenderPos(16.0f, -12f, 0f), Rotation.From(new Angles(0, 135, 0)));
	}
}

public class st011
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("The Tower", 60, GameEnt.BlenderPos(0, 6, 0.5f), new Angles(0, 0, 0), "sky_field", "mus_field", 1);
		NewStage.AddStageObject("models/stages/test_world/st011.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(-0, -38f, -36f), Rotation.From(new Angles(0f, 90f, 0)));

		List<Vector3> BananaTable = new List<Vector3>
		{
			GameEnt.ConfigPos(700.0f, -600.0f, 50.0f),
			GameEnt.ConfigPos(-700.0f, -600.0f, 50.0f),
			GameEnt.ConfigPos(-1300.0f, 0.0f, 50.0f),
			GameEnt.ConfigPos(1300.0f, 0.0f, 50.0f),
			GameEnt.ConfigPos(700.0f, 600.0f, 50.0f),
			GameEnt.ConfigPos(-700.0f, 600.0f, 50.0f),
			GameEnt.ConfigPos(1000.0000953674316f, -2300.0f, -550.0f),
			GameEnt.ConfigPos(1000.0f, -899.9999046325684f, -550.0f),
			GameEnt.ConfigPos(1600.0f, -299.99990463256836f, -550.0f),
			GameEnt.ConfigPos(1600.0001907348633f, -2900.0f, -550.0f),
			GameEnt.ConfigPos(2200.0f, -2299.9998092651367f, -550.0f),
			GameEnt.ConfigPos(2200.0f, -899.9998092651367f, -550.0f),
			GameEnt.ConfigPos(-1000.0002861022949f, -900.0001907348633f, -550.0f),
			GameEnt.ConfigPos(-1000.0f, -2300.0001907348633f, -550.0f),
			GameEnt.ConfigPos(-1599.9998092651367f, -2900.0003814697266f, -550.0f),
			GameEnt.ConfigPos(-1600.0005722045898f, -300.00038146972656f, -550.0f),
			GameEnt.ConfigPos(-2200.0f, -900.0005722045898f, -550.0f),
			GameEnt.ConfigPos(-2200.0f, -2300.00057220459f, -550.0f),
			GameEnt.ConfigPos(-699.9998092651367f, -2600.0f, -1150.0f),
			GameEnt.ConfigPos(700.0001907348633f, -2600.0f, -1150.0f),
			GameEnt.ConfigPos(1300.0003814697266f, -3199.9998092651367f, -1150.0f),
			GameEnt.ConfigPos(-1299.9996185302734f, -3200.0f, -1150.0f),
			GameEnt.ConfigPos(-699.9996185302734f, -3800.0f, -1150.0f),
			GameEnt.ConfigPos(700.0003814697266f, -3800.0f, -1150.0f),
			GameEnt.ConfigPos(1000.0000953674316f, -2300.0f, -1750.0f),
			GameEnt.ConfigPos(1000.0f, -899.9999046325684f, -1750.0f),
			GameEnt.ConfigPos(1600.0f, -299.99990463256836f, -1750.0f),
			GameEnt.ConfigPos(1600.0001907348633f, -2900.0f, -1750.0f),
			GameEnt.ConfigPos(2200.0f, -2299.9998092651367f, -1750.0f),
			GameEnt.ConfigPos(2200.0f, -899.9998092651367f, -1750.0f),
			GameEnt.ConfigPos(-1000.0002861022949f, -900.0001907348633f, -1750.0f),
			GameEnt.ConfigPos(-1000.0f, -2300.0001907348633f, -1750.0f),
			GameEnt.ConfigPos(-1599.9998092651367f, -2900.0003814697266f, -1750.0f),
			GameEnt.ConfigPos(-1600.0005722045898f, -300.00038146972656f, -1750.0f),
			GameEnt.ConfigPos(-2200.0f, -900.0005722045898f, -1750.0f),
			GameEnt.ConfigPos(-2200.0f, -2300.00057220459f, -1750.0f),
			GameEnt.ConfigPos(700.0f, -600.0f, -2350.0f),
			GameEnt.ConfigPos(-700.0f, -600.0f, -2350.0f),
			GameEnt.ConfigPos(-1300.0f, 0.0f, -2350.0f),
			GameEnt.ConfigPos(1300.0f, 0.0f, -2350.0f),
			GameEnt.ConfigPos(700.0f, 600.0f, -2350.0f),
			GameEnt.ConfigPos(-700.0f, 600.0f, -2350.0f),
			GameEnt.ConfigPos(1000.0000953674316f, -2300.0f, -2950.0f),
			GameEnt.ConfigPos(1000.0f, -899.9999046325684f, -2950.0f),
			GameEnt.ConfigPos(1600.0f, -299.99990463256836f, -2950.0f),
			GameEnt.ConfigPos(1600.0001907348633f, -2900.0f, -2950.0f),
			GameEnt.ConfigPos(2200.0f, -2299.9998092651367f, -2950.0f),
			GameEnt.ConfigPos(2200.0f, -899.9998092651367f, -2950.0f),
			GameEnt.ConfigPos(-1000.0002861022949f, -900.0001907348633f, -2950.0f),
			GameEnt.ConfigPos(-1000.0f, -2300.0001907348633f, -2950.0f),
			GameEnt.ConfigPos(-1599.9998092651367f, -2900.0003814697266f, -2950.0f),
			GameEnt.ConfigPos(-1600.0005722045898f, -300.00038146972656f, -2950.0f),
			GameEnt.ConfigPos(-2200.0f, -900.0005722045898f, -2950.0f),
			GameEnt.ConfigPos(-2200.0f, -2300.00057220459f, -2950.0f),
			GameEnt.ConfigPos(-699.9998092651367f, -2600.0f, -3550.0f),
			GameEnt.ConfigPos(700.0001907348633f, -2600.0f, -3550.0f),
			GameEnt.ConfigPos(1300.0003814697266f, -3199.9998092651367f, -3550.0f),
			GameEnt.ConfigPos(-1299.9996185302734f, -3200.0f, -3550.0f),
			GameEnt.ConfigPos(-699.9996185302734f, -3800.0f, -3550.0f),
			GameEnt.ConfigPos(700.0003814697266f, -3800.0f, -3550.0f)
		};
		NewStage.AddBananas(BananaTable);
	}
}

public class st012
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Slots", 120, GameEnt.BlenderPos(0f, 24.0f, 0.5f), new Angles(0, 0, 0), "sky_field", "mus_field", 1);
		NewStage.AddStageObject("models/stages/test_world/st012_main.vmdl", new Vector3(0,0,0), Rotation.Identity);
		List<PosAnimKeyFrame> PistonKeyFrames = new List<PosAnimKeyFrame>
		{
			new PosAnimKeyFrame(0, 3, GameEnt.BlenderPos(0, 0, 0)),
			new PosAnimKeyFrame(3, 3, GameEnt.BlenderPos(0, 0, 0)),
			new PosAnimKeyFrame(3.5f, 3, GameEnt.BlenderPos(0, 0, 18)),
			new PosAnimKeyFrame(5, 3, GameEnt.BlenderPos(0, 0, 18)),
			new PosAnimKeyFrame(6f, 3, GameEnt.BlenderPos(0, 0, 0)),
			new PosAnimKeyFrame(12f, 3, GameEnt.BlenderPos(0, 0, 0))
		};

		List<SMBObject> Pistons = new List<SMBObject>
		{
			NewStage.AddStageObject("models/stages/test_world/st012_piston1.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston2.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston3.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston4.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston5.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston6.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston7.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston8.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston9.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston10.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston11.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston12.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston13.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston14.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston15.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston16.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston17.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston18.vmdl", new Vector3(0,0,0), Rotation.Identity),
			NewStage.AddStageObject("models/stages/test_world/st012_piston19.vmdl", new Vector3(0,0,0), Rotation.Identity)
		};

		for(int i = 0; i < Pistons.Count; i++)
		{
			Pistons[i].AddPosKeyFrames(PistonKeyFrames);
			Pistons[i].EnableKeyFrameAnimation(true, false);
			Pistons[i].PosAnimTime = (float)((float)(Pistons.Count - i) / (float)Pistons.Count) * 12;
		}


		List<Vector3> BananaTable = new List<Vector3>
		{
			GameEnt.ConfigPos(400.0f, 0.0f, 50.0f),
			GameEnt.ConfigPos(-400.0f, 0.0f, 50.0f),
			GameEnt.ConfigPos(-0.0f, -400.0f, 50.0f),
			GameEnt.ConfigPos(800.0f, -400.0f, 50.0f),
			GameEnt.ConfigPos(1200.0f, 0.0f, 50.0f),
			GameEnt.ConfigPos(-800.0f, -400.0f, 50.0f),
			GameEnt.ConfigPos(-1200.0f, 0.0f, 50.0f),
			GameEnt.ConfigPos(400.0f, -800.0f, 50.0f),
			GameEnt.ConfigPos(-400.0f, -800.0f, 50.0f),
			GameEnt.ConfigPos(-0.0f, -1200.0f, 50.0f),
			GameEnt.ConfigPos(800.0f, -1200.0f, 50.0f),
			GameEnt.ConfigPos(1200.0f, -800.0f, 50.0f),
			GameEnt.ConfigPos(-800.0f, -1200.0f, 50.0f),
			GameEnt.ConfigPos(-1200.0f, -800.0f, 50.0f),
			GameEnt.ConfigPos(400.0f, 800.0f, 50.0f),
			GameEnt.ConfigPos(-400.0f, 800.0f, 50.0f),
			GameEnt.ConfigPos(-0.0f, 400.0f, 50.0f),
			GameEnt.ConfigPos(800.0f, 400.0f, 50.0f),
			GameEnt.ConfigPos(1200.0f, 800.0f, 50.0f),
			GameEnt.ConfigPos(-800.0f, 400.0f, 50.0f),
			GameEnt.ConfigPos(-1200.0f, 800.0f, 50.0f),
			GameEnt.ConfigPos(400.0f, 1600.0f, 50.0f),
			GameEnt.ConfigPos(-400.0f, 1600.0f, 50.0f),
			GameEnt.ConfigPos(-0.0f, 1200.0f, 50.0f),
			GameEnt.ConfigPos(800.0f, 1200.0f, 50.0f),
			GameEnt.ConfigPos(-800.0f, 1200.0f, 50.0f),
			GameEnt.ConfigPos(400.0f, -1600.0f, 50.0f),
			GameEnt.ConfigPos(-400.0f, -1600.0f, 50.0f),
			GameEnt.ConfigPos(-0.0f, 0.0f, -300.0f),
			GameEnt.ConfigPos(-800.0f, 0.0f, -300.0f),
			GameEnt.ConfigPos(-400.0f, -400.0f, -300.0f),
			GameEnt.ConfigPos(400.0f, -400.0f, -300.0f),
			GameEnt.ConfigPos(800.0f, 0.0f, -300.0f),
			GameEnt.ConfigPos(-0.0f, -800.0f, -300.0f),
			GameEnt.ConfigPos(-800.0f, -800.0f, -300.0f),
			GameEnt.ConfigPos(-400.0f, -1200.0f, -300.0f),
			GameEnt.ConfigPos(400.0f, -1200.0f, -300.0f),
			GameEnt.ConfigPos(800.0f, -800.0f, -300.0f),
			GameEnt.ConfigPos(-0.0f, 800.0f, -300.0f),
			GameEnt.ConfigPos(-800.0f, 800.0f, -300.0f),
			GameEnt.ConfigPos(-400.0f, 400.0f, -300.0f),
			GameEnt.ConfigPos(400.0f, 400.0f, -300.0f),
			GameEnt.ConfigPos(800.0f, 800.0f, -300.0f),
			GameEnt.ConfigPos(-0.0f, 1600.0f, -300.0f),
			GameEnt.ConfigPos(-400.0f, 1200.0f, -300.0f),
			GameEnt.ConfigPos(400.0f, 1200.0f, -300.0f),
			GameEnt.ConfigPos(-0.0f, -1600.0f, -300.0f)
		};
		NewStage.AddBananas(BananaTable);

		NewStage.AddGoal(GameEnt.BlenderPos(0, -24, 0f), Rotation.From(new Angles(0, 180, 0)));
	}
}

public class st013
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Tilted Tiles", 60, GameEnt.BlenderPos(-69.0f, -13.0f, 32.5f), new Angles(0, 135, 0), "sky_sky", "mus_desertruins_intro", 2);
		NewStage.AddStageObject("models/stages/test_world/st013.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(68.0f, 24.0f, -28.0f), Rotation.From(new Angles(0f, -45f, 0)));
	}
}

public class st014
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Manygolf", 60, GameEnt.BlenderPos(0f, 72f, 40.5f), new Angles(0, 0, 0), "sky_sky", "mus_desertruins_intro", 2);
		NewStage.AddStageObject("models/stages/test_world/st014.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(0f, -87.0f, -43.752f), Rotation.From(new Angles(0f, 0f, 0)));
	}
}

public class st015
{
	public static void RotatePlatform(SMBObject InObject)
	{
		InObject.Rotation *= Rotation.FromPitch(Time.Delta * -105);
	}
	public static void CreateStage()
	{
		List<RotAnimKeyFrame> Spinner1KeyFrames = new List<RotAnimKeyFrame>
		{
			new RotAnimKeyFrame(0, 3, Rotation.From(new Angles(0, -36, 0))),
			new RotAnimKeyFrame(5, 3, Rotation.From(new Angles(0, 36, 0))),
			new RotAnimKeyFrame(10f, 3, Rotation.From(new Angles(0, -36, 0)))
		};
		List<RotAnimKeyFrame> Spinner2KeyFrames = new List<RotAnimKeyFrame>
		{
			new RotAnimKeyFrame(0, 3, Rotation.From(new Angles(0, 36, 0))),
			new RotAnimKeyFrame(5, 3, Rotation.From(new Angles(0, -36, 0))),
			new RotAnimKeyFrame(10f, 3, Rotation.From(new Angles(0, 36, 0)))
		};
		List<RotAnimKeyFrame> HammerKeyFrames = new List<RotAnimKeyFrame>
		{
			new RotAnimKeyFrame(0, 3, Rotation.From(new Angles(75, 0, 0))),
			new RotAnimKeyFrame(2, 3, Rotation.From(new Angles(-75, 0, 0))),
			new RotAnimKeyFrame(4f, 3, Rotation.From(new Angles(75, 0, 0)))
		};
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Crash Course", 120, GameEnt.BlenderPos(-2f, 0f, 0.5f), new Angles(0, 90, 0), "sky_sky", "mus_desertruins_intro", 2);
		NewStage.AddStageObject("models/stages/test_world/st015_main.vmdl", new Vector3(0,0,0), Rotation.Identity);
		SMBObject Spinner = NewStage.AddStageObject("models/stages/test_world/st015_cylinder.vmdl", GameEnt.BlenderPos(8f, 0f, -4f), Rotation.FromPitch(100));
		Spinner.SimulateSMBObjectCustom = RotatePlatform;
		SMBObject Spinwire1 = NewStage.AddStageObject("models/stages/test_world/st015_spinwire.vmdl", GameEnt.BlenderPos(16.0f, 8f, -1.001f), Rotation.Identity);
		SMBObject Spinwire2 = NewStage.AddStageObject("models/stages/test_world/st015_spinwire.vmdl", GameEnt.BlenderPos(16.0f, 8f, -1.002f), Rotation.Identity);
		SMBObject Hammer1 = NewStage.AddStageObject("models/stages/test_world/st015_hammer.vmdl", GameEnt.BlenderPos(-4f, -18.0f, 10.25f), Rotation.Identity);
		SMBObject Hammer2 = NewStage.AddStageObject("models/stages/test_world/st015_hammer.vmdl", GameEnt.BlenderPos(4f, -18.0f, 10.25f), Rotation.Identity);
		SMBObject Hammer3 = NewStage.AddStageObject("models/stages/test_world/st015_hammer.vmdl", GameEnt.BlenderPos(12f, -18.0f, 10.25f), Rotation.Identity);
		SMBObject Hammer4 = NewStage.AddStageObject("models/stages/test_world/st015_goalholder.vmdl", GameEnt.BlenderPos(20f, -18.0f, 12.5f), Rotation.Identity);
		Spinwire1.AddRotKeyFrames(Spinner1KeyFrames);
		Spinwire1.EnableKeyFrameAnimation(false, true);
		Spinwire1.RotAnimTime = 1.5f;
		Spinwire2.AddRotKeyFrames(Spinner2KeyFrames);
		Spinwire2.EnableKeyFrameAnimation(false, true);
		Spinwire2.RotAnimTime = 1.5f;
		Hammer1.AddRotKeyFrames(HammerKeyFrames);
		Hammer1.EnableKeyFrameAnimation(false, true);
		Hammer1.RotAnimTime = 0f;
		Hammer2.AddRotKeyFrames(HammerKeyFrames);
		Hammer2.EnableKeyFrameAnimation(false, true);
		Hammer2.RotAnimTime = 1f;
		Hammer3.AddRotKeyFrames(HammerKeyFrames);
		Hammer3.EnableKeyFrameAnimation(false, true);
		Hammer3.RotAnimTime = 2f;
		Hammer4.AddRotKeyFrames(HammerKeyFrames);
		Hammer4.EnableKeyFrameAnimation(false, true);
		Hammer4.RotAnimTime = 3f;
		GoalPost Goal = NewStage.AddGoal(GameEnt.BlenderPos(20f, -18f, -1f), Rotation.From(new Angles(0f, 0f, 0)));
		Goal.SetParent(Hammer4);
	}
}


public class st016
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;

		List<PosAnimKeyFrame> Block1KeyFrames = new List<PosAnimKeyFrame>
		{
			new PosAnimKeyFrame(0, 3, GameEnt.BlenderPos(55.5f, 0f, -48.0f)),
			new PosAnimKeyFrame(1, 3, GameEnt.BlenderPos(55.5f, 0f, -48.0f)),
			new PosAnimKeyFrame(3f, 3, GameEnt.BlenderPos(-55.5f, 0f, -48.0f)),
			new PosAnimKeyFrame(4, 3, GameEnt.BlenderPos(-55.5f, 0f, -48.0f)),
			new PosAnimKeyFrame(6f, 3, GameEnt.BlenderPos(55.5f, 0f, -48.0f))
		};
		List<PosAnimKeyFrame> Block2KeyFrames = new List<PosAnimKeyFrame>
		{
			new PosAnimKeyFrame(0, 3, GameEnt.BlenderPos(55.5f, 0f, -104.0f)),
			new PosAnimKeyFrame(1, 3, GameEnt.BlenderPos(55.5f, 0f, -104.0f)),
			new PosAnimKeyFrame(3f, 3, GameEnt.BlenderPos(-55.5f, 0f, -104.0f)),
			new PosAnimKeyFrame(4, 3, GameEnt.BlenderPos(-55.5f, 0f, -104.0f)),
			new PosAnimKeyFrame(6f, 3, GameEnt.BlenderPos(55.5f, 0f, -104.0f))
		};
		List<PosAnimKeyFrame> Block3KeyFrames = new List<PosAnimKeyFrame>
		{
			new PosAnimKeyFrame(0, 3, GameEnt.BlenderPos(55.5f, 152.0f - 96, -104.0f)),
			new PosAnimKeyFrame(1, 3, GameEnt.BlenderPos(55.5f, 152.0f - 96, -104.0f)),
			new PosAnimKeyFrame(3f, 3, GameEnt.BlenderPos(-55.5f, 152.0f - 96, -104.0f)),
			new PosAnimKeyFrame(4, 3, GameEnt.BlenderPos(-55.5f, 152.0f - 96, -104.0f)),
			new PosAnimKeyFrame(6f, 3, GameEnt.BlenderPos(55.5f, 152.0f - 96, -104.0f))
		};
		List<PosAnimKeyFrame> Block4KeyFrames = new List<PosAnimKeyFrame>
		{
			new PosAnimKeyFrame(0, 3, GameEnt.BlenderPos(55.5f, 152.0f - 96, -48.0f)),
			new PosAnimKeyFrame(1, 3, GameEnt.BlenderPos(55.5f, 152.0f - 96, -48.0f)),
			new PosAnimKeyFrame(3f, 3, GameEnt.BlenderPos(-55.5f, 152.0f - 96, -48.0f)),
			new PosAnimKeyFrame(4, 3, GameEnt.BlenderPos(-55.5f, 152.0f - 96, -48.0f)),
			new PosAnimKeyFrame(6f, 3, GameEnt.BlenderPos(55.5f, 152.0f - 96, -48.0f))
		};
		List<PosAnimKeyFrame> Block5KeyFrames = new List<PosAnimKeyFrame>
		{
			new PosAnimKeyFrame(0, 3, GameEnt.BlenderPos(55.5f, 124f - 96, -76f)),
			new PosAnimKeyFrame(1, 3, GameEnt.BlenderPos(55.5f, 124f - 96, -76f)),
			new PosAnimKeyFrame(3f, 3, GameEnt.BlenderPos(-55.5f, 124f - 96, -76f)),
			new PosAnimKeyFrame(4, 3, GameEnt.BlenderPos(-55.5f, 124f - 96, -76f)),
			new PosAnimKeyFrame(6f, 3, GameEnt.BlenderPos(55.5f, 124f - 96, -76f))
		};

		SMBStage NewStage = new SMBStage("Block Party", 60, GameEnt.BlenderPos(0f, -12f - 96, 0.5f), new Angles(0, 180, 0), "sky_sky", "mus_desertruins_intro", 2);
		NewStage.AddStageObject("models/stages/test_world/st016.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddStageObject("models/stages/test_world/desertruins_col.vmdl", new Vector3(0,0,0), Rotation.Identity);
		SMBObject Block1 = NewStage.AddStageObject("models/stages/test_world/st016_block.vmdl", new Vector3(0,0,0), Rotation.Identity);
		SMBObject Block2 = NewStage.AddStageObject("models/stages/test_world/st016_block.vmdl", new Vector3(0,0,0), Rotation.Identity);
		SMBObject Block3 = NewStage.AddStageObject("models/stages/test_world/st016_block.vmdl", new Vector3(0,0,0), Rotation.Identity);
		SMBObject Block4 = NewStage.AddStageObject("models/stages/test_world/st016_block.vmdl", new Vector3(0,0,0), Rotation.Identity);
		SMBObject Block5 = NewStage.AddStageObject("models/stages/test_world/st016_block.vmdl", new Vector3(0,0,0), Rotation.Identity);

		Block1.AddPosKeyFrames(Block1KeyFrames);
		Block1.EnableKeyFrameAnimation(true, false);
		Block1.PosAnimTime = 0f;
		Block1.PosAnimPlaybackRate = 1f;
		Block2.AddPosKeyFrames(Block2KeyFrames);
		Block2.EnableKeyFrameAnimation(true, false);
		Block2.PosAnimTime = 2f;
		Block2.PosAnimPlaybackRate = 1.2f;
		Block3.AddPosKeyFrames(Block3KeyFrames);
		Block3.EnableKeyFrameAnimation(true, false);
		Block3.PosAnimTime = 4f;
		Block3.PosAnimPlaybackRate = 1.4f;
		Block4.AddPosKeyFrames(Block4KeyFrames);
		Block4.EnableKeyFrameAnimation(true, false);
		Block4.PosAnimTime = 6f;
		Block4.PosAnimPlaybackRate = 1.6f;
		Block5.AddPosKeyFrames(Block5KeyFrames);
		Block5.EnableKeyFrameAnimation(true, false);
		Block5.PosAnimTime = 8f;
		Block5.PosAnimPlaybackRate = 1.8f;

		NewStage.AddFalloutVolume("models/stages/test_world/st016_fallouttrigger.vmdl", Vector3.Zero, Rotation.Identity);

		NewStage.SetStageBounds(new BBox(GameEnt.BlenderPos(-64, -112, -152), GameEnt.BlenderPos(64, 80, 32)));

		NewStage.AddGoal(GameEnt.BlenderPos(0f, 160f - 96, -148.0f), Rotation.From(new Angles(0f, 0f, 0)));
	}
}


public class st017
{
	public static void SimulatePlatter(SMBObject InObject)
	{
		float CurTime = Time.Now - InObject.SpawnTime;
		InObject.Rotation = Rotation.From((float)Math.Sin(CurTime * 2) * 3, 0, (float)Math.Cos(CurTime * 2) * 3);
		InObject.Position = new Vector3(0, 0, 50);
	}
	public static void RotatePlatform(SMBObject InObject)
	{
		//InObject.Rotation *= Rotation.FromYaw(Time.Delta * -25).Normal;
		float CurTime = Time.Now - InObject.SpawnTime;
		InObject.Rotation = Rotation.From((float)Math.Sin(CurTime * 2) * 3, 0, (float)Math.Cos(CurTime * 2) * 3);
		InObject.Rotation = (InObject.Rotation * Rotation.FromYaw(CurTime * 25)).Normal;
		InObject.Position = new Vector3(0, 0, 50);
	}
	public static void RotatePlatform2(SMBObject InObject)
	{
		//InObject.Rotation *= Rotation.FromYaw(Time.Delta * 35).Normal;
		float CurTime = Time.Now - InObject.SpawnTime;
		InObject.Rotation = Rotation.From((float)Math.Sin(CurTime * 2) * 3, 0, (float)Math.Cos(CurTime * 2) * 3);
		InObject.Rotation = (InObject.Rotation * Rotation.FromYaw(CurTime * -35)).Normal;
		InObject.Position = new Vector3(0, 0, 50);
	}
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Sea Sickness", 60, GameEnt.BlenderPos(0f, 50f, 0f), new Angles(0, 0, 0), "sky_sky", "mus_desertruins_intro", 2);
		NewStage.AddStageObject("models/stages/test_world/st017_outer.vmdl", new Vector3(0,0,0), Rotation.Identity);
		SMBObject Platter = NewStage.AddStageObject("models/stages/test_world/st017_intermediary.vmdl", new Vector3(0,0,0), Rotation.Identity);
		SMBObject PlatterSpinner1 = NewStage.AddStageObject("models/stages/test_world/st017_outerspinner.vmdl", new Vector3(0,0,0), Rotation.Identity);
		SMBObject PlatterSpinner2 = NewStage.AddStageObject("models/stages/test_world/st017_innerspinner.vmdl", new Vector3(0,0,0), Rotation.Identity);
		SMBObject Center = NewStage.AddStageObject("models/stages/test_world/st017_center.vmdl", new Vector3(0,0,0), Rotation.Identity);
		GoalPost Goal = NewStage.AddGoal(GameEnt.BlenderPos(0f, 0f, -8.0f), Rotation.From(new Angles(0f, 180f, 0)));
		Platter.SimulateSMBObjectCustom = SimulatePlatter;
		PlatterSpinner1.SimulateSMBObjectCustom = RotatePlatform;
		PlatterSpinner2.SimulateSMBObjectCustom = RotatePlatform2;
		Center.SimulateSMBObjectCustom = SimulatePlatter;
		Goal.SetParent(Center);
	}
}

public class st018
{
	public static void RotatePlatform(SMBObject InObject)
	{
		InObject.Rotation *= Rotation.FromYaw(Time.Delta * -40).Normal;
	}
	public static void RotatePlatform2(SMBObject InObject)
	{
		InObject.Rotation *= Rotation.FromYaw(Time.Delta * 40).Normal;
	}

	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Gecko", 120, GameEnt.BlenderPos(-1.95044f, 64.0012f, -5f), new Angles(0, 90, 0), "sky_sky", "mus_desertruins_intro", 2);
		NewStage.AddStageObject("models/stages/test_world/st018.vmdl", new Vector3(0,0,0), Rotation.Identity);
		SMBObject Spinner1 = NewStage.AddStageObject("models/stages/test_world/st018_spinner.vmdl", GameEnt.BlenderPos(0, 8, 0), Rotation.Identity);
		SMBObject Spinner2 = NewStage.AddStageObject("models/stages/test_world/st018_spinner.vmdl", GameEnt.BlenderPos(0, 24, 0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(0f, -80.0f, -10f), Rotation.From(new Angles(0f, 180f, 0)));
		Spinner1.SimulateSMBObjectCustom = RotatePlatform2;
		Spinner2.SimulateSMBObjectCustom = RotatePlatform;
	}
}


public class st019
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Folded Paper", 60, GameEnt.BlenderPos(0f, 32f, 0.5f), new Angles(0, 0, 0), "sky_sky", "mus_desertruins_intro", 2);
		NewStage.AddStageObject("models/stages/test_world/st019.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.AddGoal(GameEnt.BlenderPos(0f, -32f, -16f), Rotation.From(new Angles(0f, 180f, 0)));
	}
}

public class st020
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage("Crankthrough", 60, GameEnt.BlenderPos(0f, 45f, 12.5f), new Angles(0, 0, 0), "sky_christmas", "mus_desertruins_intro", 2);
		NewStage.AddStageObject("models/stages/test_world/st020.vmdl", new Vector3(0,0,0), Rotation.Identity);
		SMBObject Pipe = NewStage.AddStageObject( "models/stages/test_world/st020_pipe.vmdl", GameEnt.BlenderPos(0, 24f, 14f ), Rotation.Identity );
		NewStage.AddGoal(GameEnt.BlenderPos(0f, -46.0f, -2.67958f ), Rotation.From(new Angles(0f, -90, 0)));
		List<RotAnimKeyFrame> PipeKeyFrames = new List<RotAnimKeyFrame>
		{
			new RotAnimKeyFrame(0, 3, Rotation.From(new Angles(0, 0, 89))),
			new RotAnimKeyFrame(6, 3, Rotation.From(new Angles(0, 0, -89))),
			new RotAnimKeyFrame(12, 3, Rotation.From(new Angles(0, 0, 89)))
		};

		Pipe.AddRotKeyFrames( PipeKeyFrames );
		Pipe.EnableKeyFrameAnimation( false, true );
		NewStage.SetFalloutPlane( -600 );

	}
}
public class st021
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage( "On Your Way", 60, GameEnt.BlenderPos( 5.55f, 26f, 0.5f ), new Angles( 0, 90, 0 ), "sky_christmas", "mus_desertruins_intro", 2 );
		NewStage.AddStageObject( "models/stages/test_world/st021.vmdl", new Vector3( 0, 0, 0 ), Rotation.Identity );
		NewStage.AddGoal( GameEnt.BlenderPos( -5.55f, 26, 0 ), Rotation.From( new Angles( 0f, -90f, 0 ) ) );
	}
}
public class st022
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage( "Around the Bend", 60, GameEnt.BlenderPos( 0f, -12.7271f, 2.54059f + 0.5f ), new Angles( 0, 180, 0 ), "sky_christmas", "mus_desertruins_intro", 2 );
		NewStage.AddStageObject( "models/stages/test_world/st022.vmdl", new Vector3( 0, 0, 0 ), Rotation.Identity );
		NewStage.AddGoal( GameEnt.BlenderPos( 0f, 12.7271f, 2.54059f ), Rotation.From( new Angles( 0f, 0f, 0 ) ) );
	}
}
public class st023
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage( "Down to Business", 60, GameEnt.BlenderPos( 0, -40f, 40.5f ), new Angles( 0, 180, 0 ), "sky_christmas", "mus_desertruins_intro", 2 );
		NewStage.AddStageObject( "models/stages/test_world/st023.vmdl", new Vector3( 0, 0, 0 ), Rotation.Identity );
		NewStage.AddGoal( GameEnt.BlenderPos( 0f, 13.497f, -26.0f ), Rotation.From( new Angles( 0f, 90f, 39 ) ) );
		NewStage.SetStageBounds( new BBox( GameEnt.BlenderPos( -20.0f, -20.0f, -35.0f ), GameEnt.BlenderPos( 20, 20, 56 ) ) );
	}
}
public class st024
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage( "Unpacked", 60, GameEnt.BlenderPos( 12f, -12f, 0.5f ), new Angles( 0, 180, 0 ), "sky_christmas", "mus_desertruins_intro", 2 );
		NewStage.AddStageObject( "models/stages/test_world/st024.vmdl", new Vector3( 0, 0, 0 ), Rotation.Identity );
		NewStage.AddGoal( GameEnt.BlenderPos( 12f, 13.5f, 0f ), Rotation.From( new Angles( 0f, 0, 0 ) ) );
	}
}
public class st025
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage( "Swerve", 60, GameEnt.BlenderPos( 4f, 20f, 0.5f ), new Angles( 0, -45, 0 ), "sky_christmas", "mus_desertruins_intro", 2 );
		NewStage.AddStageObject( "models/stages/test_world/st025.vmdl", new Vector3( 0, 0, 0 ), Rotation.Identity );
		NewStage.AddGoal( GameEnt.BlenderPos( -4f, -20.0f, -0.375f ), Rotation.From( new Angles( 0f, 135f, 0 ) ) );
	}
}
public class st026
{
	public static void CreateStage()
	{
		List<RotAnimKeyFrame> Hammer1KeyFrames = new List<RotAnimKeyFrame>
		{
			new RotAnimKeyFrame(0, 1, Rotation.From(new Angles(0, 0, -75))),
			new RotAnimKeyFrame(2, 2, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(3, 1, Rotation.From(new Angles(0, 0, -15))),
			new RotAnimKeyFrame(4, 2, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(6, 1, Rotation.From(new Angles(0, 0, -75)))
		};

		List<RotAnimKeyFrame> Hammer2KeyFrames = new List<RotAnimKeyFrame>
		{
			new RotAnimKeyFrame(0, 1, Rotation.From(new Angles(0, 0, 75))),
			new RotAnimKeyFrame(2, 2, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(3, 1, Rotation.From(new Angles(0, 0, 15))),
			new RotAnimKeyFrame(4, 2, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(6, 1, Rotation.From(new Angles(0, 0, 75)))
		};

		List<RotAnimKeyFrame> Plat1KeyFrames = new List<RotAnimKeyFrame>
		{
			new RotAnimKeyFrame(0, 0, Rotation.From(new Angles(0, 180, 0))),
			new RotAnimKeyFrame(2, 2, Rotation.From(new Angles(0, 180, 0))),
			new RotAnimKeyFrame(3, 1, Rotation.From(new Angles(0, 180, -30))),
			new RotAnimKeyFrame(4, 0, Rotation.From(new Angles(0, 180, 0))),
			new RotAnimKeyFrame(6, 0, Rotation.From(new Angles(0, 180, 0)))
		};

		List<RotAnimKeyFrame> Plat2KeyFrames = new List<RotAnimKeyFrame>
		{
			new RotAnimKeyFrame(0, 0, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(2, 2, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(3, 1, Rotation.From(new Angles(0, 0, -30))),
			new RotAnimKeyFrame(4, 0, Rotation.From(new Angles(0, 0, 0))),
			new RotAnimKeyFrame(6, 0, Rotation.From(new Angles(0, 0, 0)))
		};

		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage( "Incoming Strike", 90, GameEnt.BlenderPos( 0f, 24f, 0.5f ), new Angles( 0, -45, 0 ), "sky_christmas", "mus_desertruins_intro", 2 );
		NewStage.AddStageObject( "models/stages/test_world/st026_main.vmdl", new Vector3( 0, 0, 0 ), Rotation.Identity );
		SMBObject Hammer1 = NewStage.AddStageObject( "models/stages/test_world/st026_hammer.vmdl", GameEnt.BlenderPos( 12, -12, 18.375f ), Rotation.From( new Angles( 0f, 180f, 0 )) );
		SMBObject Hammer2 = NewStage.AddStageObject( "models/stages/test_world/st026_hammer.vmdl", GameEnt.BlenderPos( -12, 12, 18.375f ), Rotation.Identity );
		SMBObject RoundPlat1 = NewStage.AddStageObject( "models/stages/test_world/st026_roundplatform.vmdl", GameEnt.BlenderPos( 12, -12, 18.375f ), Rotation.From( new Angles( 0f, 180f, 0 )) );
		SMBObject RoundPlat2 = NewStage.AddStageObject( "models/stages/test_world/st026_roundplatform.vmdl", GameEnt.BlenderPos( -12, 12, 18.375f ), Rotation.Identity );
		NewStage.AddGoal( GameEnt.BlenderPos( 0f, -24f, 0f ), Rotation.From( new Angles( 0f, 135f, 0 ) ) );
		Hammer1.AddRotKeyFrames( Hammer1KeyFrames );
		Hammer1.EnableKeyFrameAnimation( false, true );
		Hammer2.AddRotKeyFrames( Hammer2KeyFrames );
		Hammer2.EnableKeyFrameAnimation( false, true );
		RoundPlat1.AddRotKeyFrames( Plat1KeyFrames );
		RoundPlat1.EnableKeyFrameAnimation( false, true );
		RoundPlat2.AddRotKeyFrames( Plat2KeyFrames );
		RoundPlat2.EnableKeyFrameAnimation( false, true );

		Hammer1.RotAnimTime = 3;
		RoundPlat1.RotAnimTime = 3;
	}
}
public class st027
{
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage( "Walk the Dog", 90, GameEnt.BlenderPos( -32.0f, 64.0f, 40.5f ), new Angles( 0, 45, 0 ), "sky_christmas", "mus_desertruins_intro", 2 );
		NewStage.AddStageObject( "models/stages/test_world/st027.vmdl", new Vector3( 0, 0, 0 ), Rotation.Identity );
		NewStage.AddGoal( GameEnt.BlenderPos( 35.5f, -57.0f, -39.0f ), Rotation.From( new Angles( 0f, 0f, 0 ) ) );
	}
}
public class st028
{
	public static void RotateSpinner( SMBObject InObject )
	{
		InObject.Rotation *= Rotation.FromYaw( Time.Delta * -75 ).Normal;
	}
	public static void RotateGoalSpinner( SMBObject InObject )
	{
		InObject.Rotation *= Rotation.FromYaw( Time.Delta * -360 ).Normal;
	}
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage( "Fidget", 90, GameEnt.BlenderPos( 7.06569f, 8.03458f, 0.5f ), new Angles( 0, 205, 0 ), "sky_christmas", "mus_desertruins_intro", 2 );
		NewStage.AddStageObject( "models/stages/test_world/st028_main.vmdl", new Vector3( 0, 0, 0 ), Rotation.Identity );
		SMBObject Spinner1 = NewStage.AddStageObject( "models/stages/test_world/st028_badspinner.vmdl", GameEnt.BlenderPos( 0f, 22f, 0f ), Rotation.Identity );
		SMBObject Spinner2 = NewStage.AddStageObject( "models/stages/test_world/st028_badspinner.vmdl", GameEnt.BlenderPos( -20f, -10f, 0f ), Rotation.Identity );
		SMBObject GoalSpinner = NewStage.AddStageObject( "models/stages/test_world/st028_goalspinner.vmdl", GameEnt.BlenderPos( 20f, -10f, 0.5f ), Rotation.Identity );
		GoalPost StageGoal = NewStage.AddGoal( GameEnt.BlenderPos( 11.0f, -10.0f, 0f ), Rotation.From( new Angles( 0f, 0f, 0 ) ) );
		StageGoal.SetParent( GoalSpinner );
		Spinner1.SimulateSMBObjectCustom = RotateSpinner;
		Spinner2.SimulateSMBObjectCustom = RotateSpinner;
		GoalSpinner.SimulateSMBObjectCustom = RotateGoalSpinner;
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
		foreach ( Entity element in Entity.All )
		{
			if ( element is PlayerStateManager )
			{
				PlayerStateManager PSM = element as PlayerStateManager;
				Log.Info( "Stats for " + PSM.Owner );
				Log.Info( "Total time taken: " + PSM.TotalTime);
				Log.Info( "Total score: " + PSM.Score );
			}
		}
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
		if (GameEnt.CurrentStage.UserInt1 <= 0)
		{
			GameEnt.NextGameState = Time.Now + 4;
			foreach (Client pl in Client.All)
			{
				Pawn Ball = pl.Pawn as Pawn;
				Ball.ServerChangeBallState(2);
			}
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
			NewStage.AddBanana(new Vector3(CircleX, CircleY, 15));
		}
		for (int i = 0; i < 30; i++)
		{
			float CircleX = (float)Math.Sin(((float)i / 30) * 3.14159 * 2) * 160;
			float CircleY = (float)Math.Cos(((float)i / 30) * 3.14159 * 2) * 160;
			NewStage.AddBanana(new Vector3(CircleX, CircleY, 15));
		};
		NewStage.AddStageObject("models/stages/test_world/st093.vmdl", new Vector3(0,0,0), Rotation.Identity);
		ST093Floor WaveFloor = new ST093Floor();
		NewStage.StageObjects.Add(WaveFloor);
		NewStage.OnBananaCollectedMember = BonusBananaCollected;
	}
}

public class stw1bonus2
{
	public static void BonusBananaCollected(Pawn InBall, int InBananaValue)
	{
		MyGame GameEnt = Game.Current as MyGame;
		GameEnt.CurrentStage.UserInt1 -= InBananaValue;
		if (GameEnt.CurrentStage.UserInt1 <= 0)
		{
			GameEnt.NextGameState = Time.Now + 4;
			foreach (Client pl in Client.All)
			{
				Pawn Ball = pl.Pawn as Pawn;
				Ball.ServerChangeBallState(2);
			}
		}
	}
	public static void CreateStage()
	{
		MyGame GameEnt = Game.Current as MyGame;
		SMBStage NewStage = new SMBStage(
			"Bonus Clover",
			30,
			new Vector3(0, 0, 10),
			new Angles(0, 45, 0),
			"sky_bonus",
			"mus_bonus",
			2);
		NewStage.UserInt1 = 60;
		for (int i = 0; i < 12; i++)
		{
			float CircleX = (float)Math.Sin(((float)i / 12) * 3.14159 * 2) * 80;
			float CircleY = (float)Math.Cos(((float)i / 12) * 3.14159 * 2) * 80;
			NewStage.AddBanana(new Vector3(CircleX, CircleY, 15) + GameEnt.BlenderPos(8, 8, 0));
		}
		for (int i = 0; i < 12; i++)
		{
			float CircleX = (float)Math.Sin(((float)i / 12) * 3.14159 * 2) * 80;
			float CircleY = (float)Math.Cos(((float)i / 12) * 3.14159 * 2) * 80;
			NewStage.AddBanana(new Vector3(CircleX, CircleY, 15) + GameEnt.BlenderPos(8, -8, 0));
		}
		for (int i = 0; i < 12; i++)
		{
			float CircleX = (float)Math.Sin(((float)i / 12) * 3.14159 * 2) * 80;
			float CircleY = (float)Math.Cos(((float)i / 12) * 3.14159 * 2) * 80;
			NewStage.AddBanana(new Vector3(CircleX, CircleY, 15) + GameEnt.BlenderPos(-8, -8, 0));
		}
		for (int i = 0; i < 12; i++)
		{
			float CircleX = (float)Math.Sin(((float)i / 12) * 3.14159 * 2) * 80;
			float CircleY = (float)Math.Cos(((float)i / 12) * 3.14159 * 2) * 80;
			NewStage.AddBanana(new Vector3(CircleX, CircleY, 15) + GameEnt.BlenderPos(-8, 8, 0));
		}
		for (int i = 0; i < 12; i++)
		{
			float CircleX = (float)Math.Sin(((float)i / 12) * 3.14159 * 2) * 110;
			float CircleY = (float)Math.Cos(((float)i / 12) * 3.14159 * 2) * 110;
			NewStage.AddBanana(new Vector3(CircleX, CircleY, 15));
		}
		NewStage.AddStageObject("models/stages/test_world/st095.vmdl", new Vector3(0,0,0), Rotation.Identity);
		NewStage.OnBananaCollectedMember = BonusBananaCollected;
	}
}
