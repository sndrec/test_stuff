using Sandbox;
using SandboxEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public partial class PlayerStateManager : Entity
{

	[Net]
	public int Score {get;set;}

	public override void Spawn()
	{
		base.Spawn();
	}

	public void AddScore(int InScore)
	{
		Score += InScore;
		Log.Info("New Score = " + Score);
	}

	[ConCmd.Server]
	public static void AddScoreFromClient(int InNetworkId, int InScore)
	{
		PlayerStateManager Manager = Entity.FindByIndex(InNetworkId) as PlayerStateManager;
		Log.Info(Manager);
		Log.Info("Adding score!");
		Manager.Score += InScore;
		Log.Info("New Score = " + Manager.Score);
	}

}
