using Sandbox;
using SandboxEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using Sandbox.Internal;

namespace Sandbox;

public class SMBLeaderboard
{
	public static void SubmitAllTimesToBoard()
	{
		foreach ( Entity element in Entity.All )
		{
			if ( element is PlayerStateManager )
			{
				PlayerStateManager PSM = element as PlayerStateManager;
				SMBLeaderboard.TrySubmitTimeForPlayer( PSM );
			}
		}
	}

	public async static void TrySubmitTimeForPlayer(PlayerStateManager PSM)
	{
		if (!PSM.CanSubmitScore)
		{
			Log.Info("Score submission not permitted for " + PSM.Owner);
			return;
		}
		Log.Info("Submitting time and score for " + PSM.Owner);
		Leaderboard? board = await Leaderboard.FindOrCreate( "Advanced_Time", true );
		if ( board.HasValue )
		{
			LeaderboardUpdate? result = await LeaderboardExtensions.Submit( board.Value, PSM.Owner as Client, (int)(PSM.TotalTime * 1000), false );
			if ( !result.HasValue )
			{
				Log.Info( "Failed to submit leaderboard for " + PSM.Owner );
			}
		}

		Leaderboard? board2 = await Leaderboard.FindOrCreate( "Advanced_Score", false );
		if ( board2.HasValue )
		{
			LeaderboardUpdate? result2 = await LeaderboardExtensions.Submit( board2.Value, PSM.Owner as Client, PSM.Score, false );
			if ( !result2.HasValue )
			{
				Log.Info( "Failed to submit leaderboard for " + PSM.Owner );
			}
		}
	}

}
