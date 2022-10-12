using Sandbox;
using SandboxEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;
	
[Library("smb_object"), HammerEntity]
[Title("SMB Object"), Category("Stage"), Icon("place")]
[Model]
public partial class SMBObject : ModelEntity
{
	/// <summary>
	/// Called when the entity is first created 
	/// </summary>

	[Net]
	public Transform OldTransform { get; set; }

	[Net]
	public Transform UninterpolatedTransform {get;set;}

	public float LoopStart {get;set;}

	public float LoopEnd {get;set;}

	public float AnimTime {get;set;}

	public float AnimPlaybackRate {get;set;}

	public int CurrentKeyFrameIndex {get;set;}

	public int NextKeyFrameIndex {get;set;}

	public List<AnimKeyframe> Keyframes {get;set;}

	public delegate void SimulateSMBObjectDelegate(SMBObject InObject);
	public SimulateSMBObjectDelegate SimulateSMBObjectCustom;
	public delegate void OnCollide(SMBObject InObject);
	public OnCollide OnCollideMember;
	public delegate Vector3 CustomVelocityAtPoint(SMBObject InObject, Vector3 InPoint, float DeltaTime);
	public CustomVelocityAtPoint CustomVelocityAtPointMember;

	[Net]
	public string CollisionTag {get;set;}

	public static float InterpLinear(float t, float b, float c, float d)
	{
		return c * t / d + b;
	}

	public static float InterpInOutSine(float t, float b, float c, float d)
	{
		return -c / 2 * ((float)Math.Cos(3.141592f * t / d) - 1) + b;
	}

	public static float InterpInSine(float t, float b, float c, float d)
	{
		return -c * (float)Math.Cos(t / d * (3.141592 / 2)) + c + b;
	}

	public static float InterpOutSine(float t, float b, float c, float d)
	{
		return c * (float)Math.Sin(t / d * (3.141592 / 2)) + b;
	}

	public virtual void AddKeyframe(AnimKeyframe InFrame)
	{
		Keyframes.Add(InFrame);
		Keyframes.Sort((x, y) => x.Time.CompareTo(y.Time));
	}

	public virtual void AddKeyframes(List<AnimKeyframe> InKeyframes)
	{
		foreach (AnimKeyframe KeyFrame in InKeyframes)
		{
			Keyframes.Add(KeyFrame);
		}
		Keyframes.Sort((x, y) => x.Time.CompareTo(y.Time));
	}

	public override void Spawn()
	{
		base.Spawn();
		UninterpolatedTransform = Transform;
		CollisionTag = "SMBObject" + Guid.NewGuid();
		Tags.Add(CollisionTag);
		Tags.Add("solid");
	}

	public static Vector3 RotateVector(Vector3 InPoint, Rotation rotation)
	{
		Vector3 Q = new Vector3(rotation.x, rotation.y, rotation.z);
		Vector3 T = 2 * Vector3.Cross(Q, InPoint);
		Vector3 Result = InPoint + (rotation.w * T) + Vector3.Cross(Q, T);
		return Result;
	}

	public static Vector3 UnrotateVector(Vector3 InPoint, Rotation rotation)
	{
		Vector3 Q = new Vector3(-rotation.x, -rotation.y, -rotation.z);
		Vector3 T = 2 * Vector3.Cross(Q, InPoint);
		Vector3 Result = InPoint + (rotation.w * T) + Vector3.Cross(Q, T);
		return Result;
	}

	public static Vector3 TransformPosition(Transform InTransform, Vector3 InPoint)
	{
		return RotateVector(InTransform.Scale * InPoint, InTransform.Rotation) + InTransform.Position;
	}
	public static Vector3 InverseTransformPosition(Transform InTransform, Vector3 InPoint)
	{
		return UnrotateVector(InPoint - InTransform.Position, InTransform.Rotation) * (new Vector3(1, 1, 1) / InTransform.Scale);
	}
	public virtual Vector3 GetVelocityAtPoint(Vector3 InPoint, float DeltaTime)
	{
		if (CustomVelocityAtPointMember != null)
		{
			return CustomVelocityAtPointMember(this, InPoint, DeltaTime);
		}
		Vector3 InvPoint = InverseTransformPosition(UninterpolatedTransform, InPoint);
		Vector3 OldPoint = TransformPosition(OldTransform, InvPoint);
		Vector3 VelAtPoint = (InPoint - OldPoint) / DeltaTime;
		//Log.Info(VelAtPoint);
		return VelAtPoint;
	}
	public virtual void SimulateSMBObject()
	{
		//Rotation = Rotation.FromYaw(Time.Now * 10);
		//Rotation = Rotation.Normal;
		//Scale = 1f;
		//Position += new Vector3(10 * Time.Delta, 0, 0);
		if (Keyframes != null && Keyframes.Count > 1)
		{
			AnimKeyframe CurrentKeyFrame = Keyframes[CurrentKeyFrameIndex];
			AnimKeyframe NextKeyFrame = Keyframes[NextKeyFrameIndex];
	
			float Ratio = 0;
			Vector3 StartPosition = CurrentKeyFrame.Position;
			Vector3 EndPosition = NextKeyFrame.Position;
			Rotation StartRotation = CurrentKeyFrame.Rotation;
			Rotation EndRotation = NextKeyFrame.Rotation;
			float t = (AnimTime - CurrentKeyFrame.Time) / (NextKeyFrame.Time - CurrentKeyFrame.Time);
			if (CurrentKeyFrame.InterpType == 0)
			{
				Ratio = InterpLinear(t, 0, 1, 1);
			}else
			if (CurrentKeyFrame.InterpType == 1)
			{
				Ratio = InterpInSine(t, 0, 1, 1);
			}else
			if (CurrentKeyFrame.InterpType == 2)
			{
				Ratio = InterpOutSine(t, 0, 1, 1);
			}else
			if (CurrentKeyFrame.InterpType == 3)
			{
				Ratio = InterpInOutSine(t, 0, 1, 1);
			}
			Position = Vector3.Lerp(StartPosition, EndPosition, Ratio, true);
			Rotation = Rotation.Slerp(StartRotation, EndRotation, Ratio);
	
			AnimTime = AnimTime + (Time.Delta * AnimPlaybackRate);
			if (AnimTime > Keyframes[NextKeyFrameIndex].Time)
			{
				CurrentKeyFrameIndex++;
				NextKeyFrameIndex = CurrentKeyFrameIndex + 1;
			}
			if (CurrentKeyFrameIndex == (Keyframes.Count - 1))
			{
				CurrentKeyFrameIndex = 0;
				NextKeyFrameIndex = 1;
				AnimTime = 0;
			}
		}
		if (SimulateSMBObjectCustom != null)
		{
			SimulateSMBObjectCustom(this);
		}

		//Log.Info(AnimTime);
		//Log.Info(CurrentKeyFrameIndex);
		//Log.Info(NextKeyFrameIndex);
	}
	[Event.Tick.Server]
	public void SMBTickMaster()
	{
		OldTransform = UninterpolatedTransform;
		SimulateSMBObject();
		UninterpolatedTransform = Transform;
	}
}
