using Sandbox;
using SandboxEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public partial class SMBStage
{

	//In case it wasn't obvious, I have no idea what the fuck I'm doing.

	public string Name {get;set;}
	public float MaxTime {get;set;}
	public Vector3 SpawnPosition {get;set;}
	public Angles SpawnRotation {get;set;}
	public string StageSky {get;set;}
	public List<SMBObject> StageObjects {get;set;}
	public List<ModelEntity> BGObjects {get;set;}
	public delegate void OnFallout(Pawn InBall);
	public OnFallout OnFalloutMember;
	public delegate void OnBananaCollected(Pawn InBall, int BananaValue);
	public OnBananaCollected OnBananaCollectedMember;
	public delegate void OnGoalCrossed(Pawn InBall);
	public OnGoalCrossed OnGoalCrossedMember;
	public delegate void OnTimeOver();
	public OnTimeOver OnTimeOverMember;
	public delegate void OnStageBegin();
	public OnStageBegin OnStageBeginMember;
	public delegate void OnStageDestroyed();
	public OnStageDestroyed OnStageDestroyedMember;
	public int UserInt1;
	public int UserInt2;
	public int UserInt3;
	public int UserInt4;
	public float UserFloat1;
	public float UserFloat2;
	public float UserFloat3;
	public float UserFloat4;

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
		GameEnt.StageName = InName;
	}

	public void OnFalloutManager(Pawn InBall)
	{
		if (OnFalloutMember != null)
		{
			OnFalloutMember(InBall);
		}
	}
	public void OnBananaCollectedManager(Pawn InBall, int BananaValue)
	{
		if (OnBananaCollectedMember != null)
		{
			OnBananaCollectedMember(InBall, BananaValue);
		}
	}
	public void OnGoalCrossedManager(Pawn InBall)
	{
		if (OnGoalCrossedMember != null)
		{
			OnGoalCrossedMember(InBall);
		}
	}
	public void OnTimeOverManager()
	{
		if (OnTimeOverMember != null)
		{
			OnTimeOverMember();
		}
	}
	public void OnStageBeginManager()
	{
		if (OnStageBeginMember != null)
		{
			OnStageBeginMember();
		}
	}
	public void OnStageDestroyedManager()
	{
		if (OnStageDestroyedMember != null)
		{
			OnStageDestroyedMember();
		}
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

	public Banana AddBanana(Vector3 InPosition)
	{
		Banana NewBanana = new Banana();
		NewBanana.Position = InPosition;
		NewBanana.EnableAllCollisions = true;
		NewBanana.EnableDrawing = true;
		StageObjects.Add(NewBanana);
		return NewBanana;
	}

	public SMBObject AddBGObject(string InModel, Vector3 InPosition, Rotation InRotation)
	{
		SMBObject NewObject = new SMBObject();
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

	public SMBObject AddBGObject(string InModel, Vector3 InPosition, Rotation InRotation, List<AnimKeyframe> InKeyframes)
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

	public void AddGoal(Vector3 InPosition, Rotation InRotation)
	{
		GoalPost NewGoal = new GoalPost();
		NewGoal.Position = InPosition;
		NewGoal.Rotation = InRotation;
		StageObjects.Add(NewGoal);
	}

	public void DestroyStage()
	{
		OnStageDestroyedManager();
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