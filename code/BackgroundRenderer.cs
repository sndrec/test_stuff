using Sandbox;
using SandboxEditor;
using System;
using System.Linq;

namespace Sandbox;

[SceneCamera.AutomaticRenderHook]
internal class MyRenderHook : RenderHook 
{

	public override void OnStage( SceneCamera target, Stage renderStage )
	{
		if ( renderStage == Stage.AfterOpaque )
		{
			Map.Camera.ZFar = 100000;
			foreach (Entity element in Entity.All)
			{
				if (element.Tags.Has("BGObject") && element is ModelEntity)
				{
					ModelEntity MEnt = element as ModelEntity;
					Graphics.Render(MEnt.SceneObject);
				}
			}
		}
	}
}