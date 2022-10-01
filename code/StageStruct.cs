using Sandbox;
using SandboxEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public struct SMBStage
{

	//In case it wasn't obvious, I have no idea what the fuck I'm doing.

	public string Name {get;set;}
	public float MaxTime {get;set;}
	public Vector3 SpawnPosition {get;set;}
	public Angles SpawnRotation {get;set;}
	public string StageSky {get;set;}
	public List<SMBObject> StageObjects {get;set;}
	public List<ModelEntity> BGObjects {get;set;}

	public SMBStage(string InName, float InMaxTime, Vector3 InSpawnPos, Angles InSpawnRot, string InSkyName, string InGameMusic, float InBGScale)
	{
		Name = InName;
		MaxTime = InMaxTime;
		StageObjects = new List<SMBObject>();
		BGObjects = new List<ModelEntity>();
		SpawnPosition = InSpawnPos;
		SpawnRotation = InSpawnRot;
		StageSky = InSkyName;
		MyGame GameEnt = Game.Current as MyGame;
		GameEnt.CurrentSpawnPos = InSpawnPos;
		GameEnt.CurrentSpawnRot = InSpawnRot;
		GameEnt.StageMaxTime = InMaxTime;
		GameEnt.CurrentStage = this;
		GameEnt.CurrentSky = InSkyName;
		GameEnt.DesiredSong = InGameMusic;
		GameEnt.BGScale = InBGScale;
	}

	public SMBObject AddStageObject(string InModel, Vector3 InPosition, Rotation InRotation)
	{
		SMBObject NewObject = new SMBObject();
		NewObject.SetModel(InModel);
		NewObject.SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		NewObject.Position = InPosition;
		NewObject.Rotation = InRotation;
		NewObject.EnableAllCollisions = true;
		NewObject.EnableDrawing = true;
		StageObjects.Add(NewObject);
		return NewObject;
	}

	public ModelEntity AddBGObject(string InModel, Vector3 InPosition, Rotation InRotation)
	{
		ModelEntity NewObject = new ModelEntity();
		NewObject.SetModel(InModel);
		NewObject.SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		NewObject.Position = InPosition;
		NewObject.Rotation = InRotation;
		NewObject.EnableAllCollisions = true;
		NewObject.EnableDrawing = true;
		BGObjects.Add(NewObject);
		NewObject.Tags.Add("BGObject");
		return NewObject;
	}

	public SMBObject AddStageObject(string InModel, Vector3 InPosition, Rotation InRotation, List<AnimKeyframe> InKeyframes)
	{
		SMBObject NewObject = new SMBObject();
		NewObject.SetModel(InModel);
		NewObject.SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		NewObject.Position = InPosition;
		NewObject.Rotation = InRotation;
		NewObject.EnableAllCollisions = true;
		NewObject.EnableDrawing = true;
		NewObject.Keyframes = InKeyframes;
		NewObject.Keyframes.Sort((x, y) => x.Time.CompareTo(y.Time));
		NewObject.CurrentKeyFrameIndex = 0;
		NewObject.NextKeyFrameIndex = 1;
		NewObject.AnimPlaybackRate = 1;
		NewObject.AnimTime = 0;
		StageObjects.Add(NewObject);
		return NewObject;
	}

	public void SetStageObjectDelegate(SMBObject InObject, SMBObject.SimulateSMBObjectDelegate InDelegate)
	{
		InObject.SimulateSMBObjectCustom = InDelegate;
	}

	public void AddGoal(Vector3 InPosition, Rotation InRotation)
	{
		GoalPost NewGoal = new GoalPost();
		NewGoal.Position = InPosition;
		NewGoal.Rotation = InRotation;
		StageObjects.Add(NewGoal);
		Log.Info("Created new goal.");
	}

	public void DestroyStage()
	{
		foreach (Entity element in Entity.All)
		{
			SMBObject CheckEnt = element as SMBObject;
			if (CheckEnt != null)
			{
				CheckEnt.Delete();
			}
			ModelEntity CheckEnt2 = element as ModelEntity;
			if (CheckEnt2 != null)
			{
				CheckEnt2.Delete();
			}
		}
	}

	public override string ToString() => $"({Name})";

}