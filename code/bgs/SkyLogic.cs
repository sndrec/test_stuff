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
		MyGame GameEnt = Game.Current as MyGame;
		if (!GameEnt.HasLightEnvironment)
		{
			foreach (Entity element in Entity.All)
			{
				if (element is EnvironmentLightEntity)
				{
					element.Delete();
				}
			}
			EnvironmentLightEntity LightEnvironment = new EnvironmentLightEntity();
			LightEnvironment.DynamicShadows = true;
			GameEnt.HasLightEnvironment = true;
			LightEnvironment.Brightness = 1;
			LightEnvironment.Color = new Color(1, 0.9f, 1);
			LightEnvironment.SkyColor = new Color(0.7f,0.85f,1);
			LightEnvironment.SkyIntensity = 0.5f;
			LightEnvironment.Rotation = Rotation.From(new Angles(65, 10, 0));
			GameEnt.LightAngles = LightEnvironment.Rotation;
		}
		foreach (Entity element in Entity.All)
		{
			if (element.Tags.Has("BGObject") && element.IsClientOnly)
			{
				element.Delete();
			}
		}
		switch (inName)
		{
			case "sky_field":
				sky_field.GenerateBG();
				break;
			case "sky_sky":
				sky_sky.GenerateBG();
				break;
			case "sky_default":
				sky_default.GenerateBG();
				break;
			case "sky_bonus":
				sky_bonus.GenerateBG();
				break;
			default:
				sky_default.GenerateBG();
				break;
		}
		foreach (Entity element in Entity.All)
		{
			if (element.Tags.Has("BGObject") && element.IsClientOnly)
			{
				element.Scale = GameEnt.BGScale;
				element.Position *= GameEnt.BGScale;
				element.EnableDrawing = false;
				if (element is ModelEntity)
				{
					ModelEntity MEnt = element as ModelEntity;
				}
			}
		}
	}
}

public static partial class sky_default
{
	[ClientRpc]
	public static void GenerateBG()
	{	
		ModelEntity DefaultModel = new ModelEntity("models/bg/default/debugbg.vmdl");
		DefaultModel.Tags.Add("BGObject");
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

public static partial class sky_field
{
	[ClientRpc]
	public static void GenerateBG()
	{	
		ModelEntity JunModel = new ModelEntity("models/bg/field/field_main.vmdl");
		JunModel.Tags.Add("BGObject");
		MyGame GameEnt = Game.Current as MyGame;
		Map.Scene.GradientFog.Enabled = true;
		Map.Scene.GradientFog.Color = new Color(1.33f, 1.066f, 1.6f);
		Map.Scene.GradientFog.StartDistance = 0;
		Map.Scene.GradientFog.EndDistance = 150000;
		Map.Scene.GradientFog.MaximumOpacity = 0.8f;
		Map.Scene.GradientFog.StartHeight = 0;
		Map.Scene.GradientFog.EndHeight = 150000;
		Map.Scene.GradientFog.DistanceFalloffExponent = 1;
		Map.Scene.GradientFog.VerticalFalloffExponent = 1;
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
		ModelEntity JunModel = new ModelEntity("models/bg/desertruins/desertruins.vmdl");
		JunModel.Tags.Add("BGObject");
		MyGame GameEnt = Game.Current as MyGame;
		Map.Scene.GradientFog.Enabled = true;
		Map.Scene.GradientFog.Color = new Color(0.014f, 0.025f, 0.05f);
		Map.Scene.GradientFog.StartDistance = 10000;
		Map.Scene.GradientFog.EndDistance = 1200000;
		Map.Scene.GradientFog.MaximumOpacity = 1;
		Map.Scene.GradientFog.StartHeight = 4999999;
		Map.Scene.GradientFog.EndHeight = 5000000;
		Map.Scene.GradientFog.DistanceFalloffExponent = 0.2f;
		Map.Scene.GradientFog.VerticalFalloffExponent = 0.2f;
		foreach (Entity element in Entity.All)
		{
			if (element is EnvironmentLightEntity)
			{
				EnvironmentLightEntity Light = element as EnvironmentLightEntity;
				Light.Brightness = 0.5f;
				Light.Color = new Color(0.8f, 0.9f, 1);
				Light.SkyColor = new Color(0.7f,0.85f,1);
				Light.SkyIntensity = 0.1f;
				Light.Rotation = Rotation.From(new Angles(45, 110, 0));
				GameEnt.LightAngles = Light.Rotation;
			}
		}
	}
}

public static partial class sky_bonus
{
	[ClientRpc]
	public static void GenerateBG()
	{	
		ModelEntity BonusModel = new ModelEntity("models/bg/newbonus/bonus_main.vmdl");
		BonusModel.Tags.Add("BGObject");
		MyGame GameEnt = Game.Current as MyGame;
		Map.Scene.GradientFog.Enabled = false;
		Map.Scene.GradientFog.Color = new Color(0.014f, 0.025f, 0.05f);
		Map.Scene.GradientFog.StartDistance = 0;
		Map.Scene.GradientFog.EndDistance = 100000;
		Map.Scene.GradientFog.MaximumOpacity = 1;
		Map.Scene.GradientFog.StartHeight = 4999999;
		Map.Scene.GradientFog.EndHeight = 5000000;
		Map.Scene.GradientFog.DistanceFalloffExponent = 1f;
		Map.Scene.GradientFog.VerticalFalloffExponent = 1f;
		foreach (Entity element in Entity.All)
		{
			if (element is EnvironmentLightEntity)
			{
				EnvironmentLightEntity Light = element as EnvironmentLightEntity;
				Light.Brightness = 3;
				Light.Color = new Color(1, 0.2f, 0.7f);
				Light.SkyColor = new Color(0.7f,0.85f,1);
				Light.SkyIntensity = 0.5f;
				Light.Rotation = Rotation.From(new Angles(35, -15, 0));
				GameEnt.LightAngles = Light.Rotation;
			}
		}
	}
}

