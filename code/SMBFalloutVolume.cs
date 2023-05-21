using Sandbox;
using Editor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public partial class SMBFalloutVolume : SMBTrigger
{

	public override void OnEnterTrigger(Pawn InBall)
	{
		InBall.ServerChangeBallState(1);
	}

	public override void OnExitTrigger(Pawn InBall)
	{
		InBall.ServerChangeBallState(1);
	}

}
