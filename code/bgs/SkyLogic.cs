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
		Log.Info("Want to generate " + inName);
		foreach (Entity element in Entity.All)
		{
			if (element.Tags.Has("BGObject") && element.IsClientOnly)
			{
				Log.Info("Deleting old BG Object...");
				element.Delete();
			}
		}
		switch (inName)
		{
			case "sky_jun":
				sky_jun.GenerateBG();
				break;
		}
		MyGame GameEnt = Game.Current as MyGame;
		foreach (Entity element in Entity.All)
		{
			if (element.Tags.Has("BGObject") && element.IsClientOnly)
			{
				Log.Info(GameEnt.BGScale);
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

public static partial class sky_jun
{
	[ClientRpc]
	public static void GenerateBG()
	{	
		Transform JunModelTransform = Transform.Zero;
		ModelEntity JunModel = new ModelEntity("models/bg/jungle1/bg_jun1.vmdl");
		JunModel.Tags.Add("BGObject");
	}
}
