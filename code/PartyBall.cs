﻿using Sandbox;
using SandboxEditor;
using System;
using System.Linq;

namespace Sandbox;

public partial class PartyBall : AnimatedEntity
{	

	public Rotation AngVel {get;set;}

	public override void Spawn()
	{
		base.Spawn();
	}
}
