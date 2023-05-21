using Sandbox;
using Editor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public partial class SMBTrigger : SMBObject
{

	public delegate void SMBTriggerOnEnterDelegate(Pawn InBall);

	public SMBTriggerOnEnterDelegate SMBTriggerOnEnterCustom;

	public delegate void SMBTriggerOnExitDelegate(Pawn InBall);

	public SMBTriggerOnExitDelegate SMBTriggerOnExitCustom;

	public override void Spawn()
	{
		base.Spawn();
		Tags.Add("smbtrigger");
		EnableTraceAndQueries = true;
		EnableSolidCollisions = false;
	}

	public virtual void OnEnterTrigger(Pawn InBall)
	{
		if (SMBTriggerOnEnterCustom != null)
		{
			SMBTriggerOnEnterCustom(InBall);
		}
	}

	public virtual void OnExitTrigger(Pawn InBall)
	{
		if (SMBTriggerOnExitCustom != null)
		{
			SMBTriggerOnExitCustom(InBall);
		}
	}

}
