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

	public float PosLoopStart {get;set;}

	public float PosLoopEnd {get;set;}

	public float RotLoopStart {get;set;}

	public float RotLoopEnd {get;set;}

	public float PosAnimTime {get;set;}

	public float RotAnimTime {get;set;}

	public float PosAnimPlaybackRate {get;set;}

	public float RotAnimPlaybackRate {get;set;}

	public float SpawnTime {get;set;}

	public int CurrentPosKeyFrameIndex {get;set;}

	public int NextPosKeyFrameIndex {get;set;}

	public int CurrentRotKeyFrameIndex {get;set;}

	public int NextRotKeyFrameIndex {get;set;}

	public List<PosAnimKeyFrame> PosKeyFrames {get;set;}
	public List<RotAnimKeyFrame> RotKeyFrames {get;set;}

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

	public virtual void AddPosKeyFrame(PosAnimKeyFrame InFrame)
	{
		PosKeyFrames.Add(InFrame);
		PosKeyFrames.Sort((x, y) => x.Time.CompareTo(y.Time));
	}

	public virtual void AddRotKeyFrame(RotAnimKeyFrame InFrame)
	{
		RotKeyFrames.Add(InFrame);
		RotKeyFrames.Sort((x, y) => x.Time.CompareTo(y.Time));
	}

	public virtual void AddPosKeyFrames(List<PosAnimKeyFrame> InKeyFrames)
	{
		foreach (PosAnimKeyFrame KeyFrame in InKeyFrames)
		{
			PosKeyFrames.Add(KeyFrame);
		}
		PosKeyFrames.Sort((x, y) => x.Time.CompareTo(y.Time));
	}

	public virtual void AddRotKeyFrames(List<RotAnimKeyFrame> InKeyFrames)
	{
		foreach (RotAnimKeyFrame KeyFrame in InKeyFrames)
		{
			RotKeyFrames.Add(KeyFrame);
		}
		RotKeyFrames.Sort((x, y) => x.Time.CompareTo(y.Time));
	}
	public virtual void EnableKeyFrameAnimation(bool EnablePos, bool EnableRot, float PosPlaybackRate = 1, float RotPlaybackRate = 1)
	{
		if (EnablePos)
		{
			CurrentPosKeyFrameIndex = 0;
			NextPosKeyFrameIndex = 1;
			PosAnimPlaybackRate = PosPlaybackRate;
			PosAnimTime = 0;
		}

		if (EnableRot)
		{
			CurrentRotKeyFrameIndex = 0;
			NextRotKeyFrameIndex = 1;
			RotAnimPlaybackRate = RotPlaybackRate;
			RotAnimTime = 0;
		}
	}

	public override void Spawn()
	{
		base.Spawn();
		UninterpolatedTransform = Transform;
		CollisionTag = "SMBObject" + Guid.NewGuid();
		Tags.Add(CollisionTag);
		Tags.Add("solid");
		Tags.Add("regularfloor");
		PosKeyFrames = new List<PosAnimKeyFrame>();
		RotKeyFrames = new List<RotAnimKeyFrame>();
		SpawnTime = Time.Now;
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
		if (PosKeyFrames != null && PosKeyFrames.Count > 1)
		{
			PosAnimKeyFrame CurrentPosKeyFrame = PosKeyFrames[CurrentPosKeyFrameIndex];
			PosAnimKeyFrame NextPosKeyFrame = PosKeyFrames[NextPosKeyFrameIndex];
	
			float Ratio = 0;
			Vector3 StartPosition = CurrentPosKeyFrame.Position;
			Vector3 EndPosition = NextPosKeyFrame.Position;
			float t = (PosAnimTime - CurrentPosKeyFrame.Time) / (NextPosKeyFrame.Time - CurrentPosKeyFrame.Time);
			if (CurrentPosKeyFrame.InterpType == 0)
			{
				Ratio = InterpLinear(t, 0, 1, 1);
			}else
			if (CurrentPosKeyFrame.InterpType == 1)
			{
				Ratio = InterpInSine(t, 0, 1, 1);
			}else
			if (CurrentPosKeyFrame.InterpType == 2)
			{
				Ratio = InterpOutSine(t, 0, 1, 1);
			}else
			if (CurrentPosKeyFrame.InterpType == 3)
			{
				Ratio = InterpInOutSine(t, 0, 1, 1);
			}
			Position = Vector3.Lerp(StartPosition, EndPosition, Ratio, true);
	
			PosAnimTime = PosAnimTime + (Time.Delta * PosAnimPlaybackRate);
			if (PosAnimTime > PosKeyFrames[NextPosKeyFrameIndex].Time)
			{
				CurrentPosKeyFrameIndex++;
				NextPosKeyFrameIndex = CurrentPosKeyFrameIndex + 1;
			}
			if (CurrentPosKeyFrameIndex == (PosKeyFrames.Count - 1))
			{
				CurrentPosKeyFrameIndex = 0;
				NextPosKeyFrameIndex = 1;
				PosAnimTime = 0;
			}
		}
		if (RotKeyFrames != null && RotKeyFrames.Count > 1)
		{
			RotAnimKeyFrame CurrentRotKeyFrame = RotKeyFrames[CurrentRotKeyFrameIndex];
			RotAnimKeyFrame NextRotKeyFrame = RotKeyFrames[NextRotKeyFrameIndex];
	
			float Ratio = 0;
			Rotation StartRotation = CurrentRotKeyFrame.Rotation;
			Rotation EndRotation = NextRotKeyFrame.Rotation;
			float t = (RotAnimTime - CurrentRotKeyFrame.Time) / (NextRotKeyFrame.Time - CurrentRotKeyFrame.Time);
			if (CurrentRotKeyFrame.InterpType == 0)
			{
				Ratio = InterpLinear(t, 0, 1, 1);
			}else
			if (CurrentRotKeyFrame.InterpType == 1)
			{
				Ratio = InterpInSine(t, 0, 1, 1);
			}else
			if (CurrentRotKeyFrame.InterpType == 2)
			{
				Ratio = InterpOutSine(t, 0, 1, 1);
			}else
			if (CurrentRotKeyFrame.InterpType == 3)
			{
				Ratio = InterpInOutSine(t, 0, 1, 1);
			}
			Rotation = Rotation.Slerp(StartRotation, EndRotation, Ratio);
	
			RotAnimTime = RotAnimTime + (Time.Delta * RotAnimPlaybackRate);
			if (RotAnimTime > RotKeyFrames[NextRotKeyFrameIndex].Time)
			{
				CurrentRotKeyFrameIndex++;
				NextRotKeyFrameIndex = CurrentRotKeyFrameIndex + 1;
			}
			if (CurrentRotKeyFrameIndex == (RotKeyFrames.Count - 1))
			{
				CurrentRotKeyFrameIndex = 0;
				NextRotKeyFrameIndex = 1;
				RotAnimTime = 0;
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
