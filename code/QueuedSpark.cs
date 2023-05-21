using Sandbox;
using Editor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public struct QueuedSpark
{	
	public Vector3 Position {get;set;}
	public Vector3 Velocity {get;set;}
	public float Size {get;set;}
	public string Texture {get;set;}

	public QueuedSpark(Vector3 InPosition, Vector3 InVelocity, float InSize, string InTexture)
	{
		Position = InPosition;
		Velocity = InVelocity;
		Size = InSize;
		Texture = InTexture;
	}
}

public struct QueuedCollisionStar
{	
	public Vector3 Position {get;set;}
	public Vector3 Velocity {get;set;}
	public float Size {get;set;}

	public QueuedCollisionStar(Vector3 InPosition, Vector3 InVelocity, float InSize)
	{
		Position = InPosition;
		Velocity = InVelocity;
		Size = InSize;
	}
}

public struct OldRelativePositionData
{
	public Vector3 Position {get;set;}
	public float Time {get;set;}

	public OldRelativePositionData(Vector3 InPosition, float InTime)
	{
		Position = InPosition;
		Time = InTime;
	}
}