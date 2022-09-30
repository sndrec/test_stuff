using Sandbox;
using SandboxEditor;
using System;
using System.Linq;

namespace Sandbox;

public struct AnimKeyframe
{
	//interptypes
	//0 = linear
	//1 = ease in
	//2 = ease out
	//3 = ease in out
	
	public AnimKeyframe(float InTime, int InInterpType, Vector3 InPosition, Rotation InRotation)
	{
		Time = InTime;
		InterpType = InInterpType;
		Position = InPosition;
		Rotation = InRotation;
	}
	public float Time {get;set;}
	public int InterpType {get;set;}
	public Vector3 Position {get;set;}
	public Rotation Rotation {get;set;}

	public override string ToString() => $"({Time}, {InterpType}, {Position}, {Rotation})";

}