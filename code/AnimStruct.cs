using Sandbox;
using Editor;
using System;
using System.Linq;

namespace Sandbox;

public struct PosAnimKeyFrame
{
	//interptypes
	//0 = linear
	//1 = ease in
	//2 = ease out
	//3 = ease in out
	
	public PosAnimKeyFrame(float InTime, int InInterpType, Vector3 InPosition)
	{
		Time = InTime;
		InterpType = InInterpType;
		Position = InPosition;
	}
	public float Time {get;set;}
	public int InterpType {get;set;}
	public Vector3 Position {get;set;}

	public override string ToString() => $"({Time}, {InterpType}, {Position})";

}

public struct RotAnimKeyFrame
{
	//interptypes
	//0 = linear
	//1 = ease in
	//2 = ease out
	//3 = ease in out
	
	public RotAnimKeyFrame(float InTime, int InInterpType, Rotation InRotation)
	{
		Time = InTime;
		InterpType = InInterpType;
		Rotation = InRotation;
	}
	public float Time {get;set;}
	public int InterpType {get;set;}
	public Rotation Rotation {get;set;}

	public override string ToString() => $"({Time}, {InterpType}, {Rotation})";

}
