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
	public float StartTime {get;set;}
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
	public BBox StageBounds;

	public SMBStage(string InName, float InMaxTime, Vector3 InSpawnPos, Angles InSpawnRot, string InSkyName, string InGameMusic, float InBGScale)
	{
		Name = InName;
		MaxTime = InMaxTime;
		StartTime = Time.Now;
		StageObjects = new List<SMBObject>();
		BGObjects = new List<ModelEntity>();
		SpawnPosition = InSpawnPos;
		SpawnRotation = InSpawnRot;
		StageSky = InSkyName;
		StageBounds = new BBox(Vector3.Zero, Vector3.Zero);
		MyGame GameEnt = Game.Current as MyGame;
		GameEnt.CurrentSpawnPos = InSpawnPos;
		GameEnt.CurrentSpawnRot = InSpawnRot;
		GameEnt.StageMaxTime = InMaxTime;
		GameEnt.CurrentStage = this;
		GameEnt.CurrentSky = InSkyName;
		GameEnt.DesiredSong = InGameMusic;
		GameEnt.BGScale = InBGScale;
		GameEnt.StageName = InName;
		GameEnt.StageBounds = StageBounds;
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
		NewObject.UninterpolatedTransform = new Transform(InPosition, InRotation, 1);
		NewObject.OldTransform = new Transform(InPosition, InRotation, 1);
		StageObjects.Add(NewObject);
		return NewObject;
	}

	public SMBBumper AddBumper(Vector3 InPosition, Rotation InRotation)
	{
		SMBBumper NewObject = new SMBBumper();
		NewObject.SetModel("models/bumper.vmdl");
		NewObject.SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		NewObject.Position = InPosition;
		NewObject.Rotation = InRotation;
		NewObject.EnableAllCollisions = true;
		NewObject.EnableDrawing = true;
		NewObject.UninterpolatedTransform = new Transform(InPosition, InRotation, 1);
		NewObject.OldTransform = new Transform(InPosition, InRotation, 1);
		StageObjects.Add(NewObject);
		return NewObject;
	}

	public SMBFalloutVolume AddFalloutVolume(string InModel, Vector3 InPosition, Rotation InRotation)
	{
		SMBFalloutVolume NewObject = new SMBFalloutVolume();
		NewObject.SetModel(InModel);
		NewObject.SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		NewObject.Position = InPosition;
		NewObject.Rotation = InRotation;
		NewObject.EnableAllCollisions = true;
		NewObject.EnableDrawing = true;
		NewObject.UninterpolatedTransform = new Transform(InPosition, InRotation, 1);
		NewObject.OldTransform = new Transform(InPosition, InRotation, 1);
		StageObjects.Add(NewObject);
		return NewObject;
	}

	public Banana AddBanana(Vector3 InPosition)
	{
		Banana NewBanana = new Banana();
		NewBanana.Position = InPosition;
		NewBanana.EnableAllCollisions = true;
		NewBanana.EnableDrawing = true;
		NewBanana.UninterpolatedTransform = new Transform(InPosition, Rotation.Identity, 1);
		NewBanana.OldTransform = new Transform(InPosition, Rotation.Identity, 1);
		StageObjects.Add(NewBanana);
		return NewBanana;
	}

	public BananaBunch AddBananaBunch(Vector3 InPosition)
	{
		BananaBunch NewBanana = new BananaBunch();
		NewBanana.Position = InPosition;
		NewBanana.EnableAllCollisions = true;
		NewBanana.EnableDrawing = true;
		NewBanana.UninterpolatedTransform = new Transform(InPosition, Rotation.Identity, 1);
		NewBanana.OldTransform = new Transform(InPosition, Rotation.Identity, 1);
		StageObjects.Add(NewBanana);
		return NewBanana;
	}

	public void AddBananas(List<Vector3> Poses)
	{
		foreach (Vector3 Pos in Poses)
		{
			AddBanana(Pos);
		}
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
		NewObject.UninterpolatedTransform = new Transform(InPosition, InRotation, 1);
		NewObject.OldTransform = new Transform(InPosition, InRotation, 1);
		BGObjects.Add(NewObject);
		NewObject.Tags.Add("BGObject");
		return NewObject;
	}

	public GoalPost AddGoal(Vector3 InPosition, Rotation InRotation, string PostModel = "models/goalpost.vmdl", string TriggerModel = "models/goaltrigger.vmdl")
	{
		GoalPost NewGoal = new GoalPost();
		NewGoal.Position = InPosition;
		NewGoal.Rotation = InRotation;
		NewGoal.SetPostAndTriggerModel(PostModel, TriggerModel);
		NewGoal.UninterpolatedTransform = new Transform(InPosition, InRotation, 1);
		NewGoal.OldTransform = new Transform(InPosition, InRotation, 1);
		StageObjects.Add(NewGoal);
		return NewGoal;
	}

	public void SetStageBounds(BBox InBounds)
	{
		StageBounds = InBounds;
		MyGame GameEnt = Game.Current as MyGame;
		GameEnt.StageBounds = StageBounds;
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