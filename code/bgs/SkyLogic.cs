using Sandbox;
using SandboxEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

//As stated elsewhere in my code...
//This is dumb. I just don't know another way to do it.
//"If it works, it works"

public static partial class SkyGenerator
{	
	[ClientRpc]
	public static void CreateBackground(string inName)
	{
		foreach (Entity element in Entity.All)
		{
			if (element.Tags.Has("BGObject") && element.IsClientOnly)
			{
				element.Delete();
			}
		}
		switch (inName)
		{
			case "sky_jun":
				sky_jun.GenerateBG();
				break;
			case "sky_sky":
				sky_sky.GenerateBG();
				break;
			case "sky_bonus":
				sky_bonus.GenerateBG();
				break;
		}
		MyGame GameEnt = Game.Current as MyGame;
		foreach (Entity element in Entity.All)
		{
			if (element.Tags.Has("BGObject") && element.IsClientOnly)
			{
				element.Scale = GameEnt.BGScale;
				element.Position *= GameEnt.BGScale;
				if (element is ModelEntity)
				{
					ModelEntity MEnt = element as ModelEntity;
				}
			}
		}
	}
}

public static partial class sky_jun
{
	[ClientRpc]
	public static void GenerateBG()
	{	
		ModelEntity JunModel = new ModelEntity("models/bg/field/field_main.vmdl");
		JunModel.Tags.Add("BGObject");
		MyGame GameEnt = Game.Current as MyGame;
		foreach (Entity element in Entity.All)
		{
			if (element is EnvironmentLightEntity)
			{
				EnvironmentLightEntity Light = element as EnvironmentLightEntity;
				Light.Brightness = 1;
				Light.Color = new Color(1, 0.9f, 1);
				Light.SkyColor = new Color(0.7f,0.85f,1);
				Light.SkyIntensity = 0.5f;
				Light.Rotation = Rotation.From(new Angles(65, 10, 0));
				GameEnt.LightAngles = Light.Rotation;
			}
		}
	}
}

public static partial class sky_sky
{
	[ClientRpc]
	public static void GenerateBG()
	{	
		ModelEntity JunModel = new ModelEntity("models/bg/sky/sky_sky.vmdl");
		JunModel.Tags.Add("BGObject");
		MyGame GameEnt = Game.Current as MyGame;
		foreach (Entity element in Entity.All)
		{
			if (element is EnvironmentLightEntity)
			{
				EnvironmentLightEntity Light = element as EnvironmentLightEntity;
				Light.Brightness = 1;
				Light.Color = new Color(1, 0.9f, 1);
				Light.SkyColor = new Color(0.7f,0.85f,1);
				Light.SkyIntensity = 0.5f;
				Light.Rotation = Rotation.From(new Angles(45, 0, 0));
				GameEnt.LightAngles = Light.Rotation;
				ConsoleSystem.SetValue("fog_override", 1);
				ConsoleSystem.SetValue("fog_override_color", "255 255 255");
				ConsoleSystem.SetValue("fog_override_enable", 1);
				ConsoleSystem.SetValue("fog_override_max_density", 1);
				ConsoleSystem.SetValue("fog_override_start", 1000);
				ConsoleSystem.SetValue("fog_override_end", 4000);
				break;
			}
		}
	}
}

public static partial class sky_bonus
{
	[ClientRpc]
	public static void GenerateBG()
	{	
		ModelEntity BonusModel = new ModelEntity("models/bg/bonus/bonus_main.vmdl");
		BonusModel.Tags.Add("BGObject");
		MyGame GameEnt = Game.Current as MyGame;
		foreach (Entity element in Entity.All)
		{
			if (element is EnvironmentLightEntity)
			{
				EnvironmentLightEntity Light = element as EnvironmentLightEntity;
				Light.Brightness = 1;
				Light.Color = new Color(1, 0.9f, 1);
				Light.SkyColor = new Color(0.7f,0.85f,1);
				Light.SkyIntensity = 0.5f;
				Light.Rotation = Rotation.From(new Angles(45, 0, 0));
				GameEnt.LightAngles = Light.Rotation;
			}
		}
	}
}

