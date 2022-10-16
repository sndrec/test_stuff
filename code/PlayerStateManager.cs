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
	}

	public void SetScore(int InScore)
	{
		Score = InScore;
	}

	[ConCmd.Server]
	public static void AddScoreFromClient(int InNetworkId, int InScore)
	{
		PlayerStateManager Manager = Entity.FindByIndex(InNetworkId) as PlayerStateManager;
		Manager.Score += InScore;
	}

}
