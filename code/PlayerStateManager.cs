using Sandbox;
using Editor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public partial class PlayerStateManager : Entity
{

	[Net]
	public int Score { get; set; } = 0;

	[Net]

	public float TotalTime { get; set; } = 0;

	[Net]

	public bool CanSubmitScore {get;set;} = false;

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
	public void AddTime( float InSeconds )
	{
		Log.Info( "Adding " + InSeconds + " to the total time" );
		TotalTime += InSeconds;
		Log.Info( "Current time on course: " + TotalTime );
	}

	public void SetTime( float InSeconds )
	{
		TotalTime = InSeconds;
	}

	[ConCmd.Server]
	public static void AddScoreFromClient(int InNetworkId, int InScore)
	{
		PlayerStateManager Manager = Entity.FindByIndex(InNetworkId) as PlayerStateManager;
		Manager.AddScore(InScore);
	}

	[ConCmd.Server]
	public static void AddTimeFromClient( int InNetworkId, float InSeconds )
	{
		PlayerStateManager Manager = Entity.FindByIndex( InNetworkId ) as PlayerStateManager;
		Manager.AddTime(InSeconds);
	}

}
