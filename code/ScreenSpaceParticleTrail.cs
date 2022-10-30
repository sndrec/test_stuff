using Sandbox;
using SandboxEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public partial class ScreenSpaceParticleTrail : Entity
{	
	public List<OldRelativePositionData> OldRelativePositions {get;set;}

	public float ParticleWidth {get;set;}

	public float ParticleLife {get;set;}

	public float ParticleSpawnTime {get;set;}

	public Mesh ParticleMesh {get;set;}

	public Model ParticleModel {get;set;}

	public SceneObject ParticleSceneObject {get;set;}

	public PointLightEntity ParticleLight {get;set;}

	public bool Instantiated {get;set;}

	private List<SimpleVertex> vertices {get;set;}

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

	public override void Spawn()
	{
		base.Spawn();
	}

	public void Instantiate(Vector3 InPosition, Vector3 InVelocity, float InSize, string InTexture)
	{
		ParticleMesh = new Mesh(Material.Load(InTexture));
		ParticleMesh.CreateVertexBuffer<SimpleVertex>( SimpleVertex.Layout );
		ParticleModel = Model.Builder.AddMesh(ParticleMesh).Create();
		ParticleSceneObject = new SceneObject(Map.Scene, ParticleModel, new Transform(Vector3.Zero));
		vertices = new List<SimpleVertex>();

		ColorHsv SparkColorHsv = new ColorHsv(Rand.Float(0f, 48f), 1, Rand.Float(1f, 1f), 1);
		Color SparkColor = SparkColorHsv.ToColor();

		ParticleSceneObject.Attributes.Set("SparkTint", new Vector4(SparkColor.r, SparkColor.g, SparkColor.b, 1));
		ParticleSpawnTime = Time.Now;
		ParticleLife = Rand.Float(0.5f, 1.5f);
		ParticleWidth = InSize;
		Position = InPosition;
		Velocity = InVelocity;
		ParticleLight = new PointLightEntity();
		ParticleLight.Brightness = 0f;
		ParticleLight.Color = new Color(1, 0.8f, 0.6f);
		ParticleLight.Range = 3;
		ParticleLight.LinearAttenuation = -1;
		ParticleLight.QuadraticAttenuation = 1;
		ParticleLight.Falloff = 1000;
		ParticleLight.DynamicShadows = false;
		OldRelativePositions = new List<OldRelativePositionData>();
		Instantiated = true;
	}

	public virtual void UpdateParticleMesh()
	{
		Vector3 LocalRight = CurrentView.Rotation.Right;
		Vector3 LocalForward = CurrentView.Rotation.Forward;
		float PositionCount = OldRelativePositions.Count;
		if (PositionCount == 0)
		{
			return;
		}

		//Only generate a spark trail from the beginning to the end.
		//Seems like it doesn't really offer any perf gain over the more elaborate trail
		//Probably most of the perf loss is from having to write a new vertex buffer at all
		//If this is going to be noticeably more performant we need to lock the buffer once we create the spark

		Vector3 OldPosition = TransformPosition(new Transform(CurrentView.Position, CurrentView.Rotation, 1), OldRelativePositions[0].Position);
		Vector3 NewPosition = Position;
		Vector2 OldScreenPos = new Vector2(OldPosition.ToScreen());
		Vector2 ScreenPos = new Vector2(NewPosition.ToScreen());
		//Log.Info(Vector2.Distance(ScreenPos, OldScreenPos) * 0.02);
		//Log.Info(ParticleWidth);
		//Log.Info("-------");
		float LifeRemainingRatio = 1 - ((Time.Now - ParticleSpawnTime) / ParticleLife);
		if (Vector2.Distance(ScreenPos, OldScreenPos) * 100 < ParticleWidth * LifeRemainingRatio * 2)
		{
			Vector2 ScreenDir = (OldScreenPos - ScreenPos).Normal;
			OldScreenPos = ScreenPos + (ScreenDir * ParticleWidth * LifeRemainingRatio * 0.02);
		}
		Vector2 ParticleScreenDir = (ScreenPos - OldScreenPos).Normal;
		Vector2 ParticleScreenDirRight = new Vector2(ParticleScreenDir.y, -ParticleScreenDir.x);
		
		
		Vector3 DirToBottomLeft = Screen.GetDirection((OldScreenPos + (ParticleScreenDirRight * -0.01 * ParticleWidth * LifeRemainingRatio)) * Screen.Size);
		Vector3 DirToTopLeft = Screen.GetDirection((OldScreenPos + (ParticleScreenDirRight * 0.01 * ParticleWidth * LifeRemainingRatio)) * Screen.Size);
		Vector3 DirToTopRight = Screen.GetDirection((ScreenPos + (ParticleScreenDirRight * 0.01 * ParticleWidth * LifeRemainingRatio)) * Screen.Size);
		Vector3 DirToBottomRight = Screen.GetDirection((ScreenPos + (ParticleScreenDirRight * -0.01 * ParticleWidth * LifeRemainingRatio)) * Screen.Size);
		
		float DistToStart = (OldPosition - CurrentView.Position).Length;
		float DistToEnd = (NewPosition - CurrentView.Position).Length;
		vertices.Clear();
		{
			//bottom left
			vertices.Add( new SimpleVertex( CurrentView.Position + (DistToStart * DirToBottomLeft), LocalForward, LocalRight, new Vector2(0, 0) ));
			//top left
			vertices.Add( new SimpleVertex( CurrentView.Position + (DistToStart * DirToTopLeft), LocalForward, LocalRight, new Vector2(0, 1) ));
		
			//top right
			vertices.Add( new SimpleVertex( CurrentView.Position + (DistToEnd * DirToTopRight), LocalForward, LocalRight, new Vector2(1, 1) ));
		
			//top right
			vertices.Add( new SimpleVertex( CurrentView.Position + (DistToEnd * DirToTopRight), LocalForward, LocalRight, new Vector2(1, 1) ));
			//bottom right
			vertices.Add( new SimpleVertex( CurrentView.Position + (DistToEnd * DirToBottomRight), LocalForward, LocalRight, new Vector2(1, 0) ));
			//bottom left
			vertices.Add( new SimpleVertex( CurrentView.Position + (DistToStart * DirToBottomLeft), LocalForward, LocalRight, new Vector2(0, 0) ));
		}

		//Generate a more detailed spark trail

		//for (int i = 0; i < OldRelativePositions.Count; i++)
		//{
		//	//Log.Info(OldRelativePositions);
		//	//Log.Info(OldRelativePositions[i].Position);
		//	Vector3 OldPosition = TransformPosition(new Transform(CurrentView.Position, CurrentView.Rotation, 1), OldRelativePositions[i].Position);
		//	Vector3 NewPosition = Position;
		//	Vector2 OldScreenPos = new Vector2(OldPosition.ToScreen());
		//	Vector2 ScreenPos = new Vector2(NewPosition.ToScreen());
		//	if (Vector2.Distance(ScreenPos, OldScreenPos) < ParticleWidth)
		//	{
		//		Vector2 ScreenDir = (OldScreenPos - ScreenPos).Normal;
		//		OldScreenPos = ScreenPos + (ScreenDir * ParticleWidth * 0.01);
		//	}
		//	if (i + 1 < OldRelativePositions.Count)
		//	{
		//		NewPosition = TransformPosition(new Transform(CurrentView.Position, CurrentView.Rotation, 1), OldRelativePositions[i + 1].Position);
		//		ScreenPos = new Vector2(NewPosition.ToScreen());
		//	}
		//	Vector2 ParticleScreenDir = (ScreenPos - OldScreenPos).Normal;
		//	Vector2 ParticleScreenDirRight = new Vector2(ParticleScreenDir.y, -ParticleScreenDir.x);
		//
		//	float LifeRemainingRatio = 1 - ((Time.Now - ParticleSpawnTime) / ParticleLife);
		//
		//	Vector3 DirToBottomLeft = Screen.GetDirection((OldScreenPos + (ParticleScreenDirRight * -0.01 * ParticleWidth * LifeRemainingRatio)) * Screen.Size);
		//	Vector3 DirToTopLeft = Screen.GetDirection((OldScreenPos + (ParticleScreenDirRight * 0.01 * ParticleWidth * LifeRemainingRatio)) * Screen.Size);
		//	Vector3 DirToTopRight = Screen.GetDirection((ScreenPos + (ParticleScreenDirRight * 0.01 * ParticleWidth * LifeRemainingRatio)) * Screen.Size);
		//	Vector3 DirToBottomRight = Screen.GetDirection((ScreenPos + (ParticleScreenDirRight * -0.01 * ParticleWidth * LifeRemainingRatio)) * Screen.Size);
		//
		//	float DistToStart = (OldPosition - CurrentView.Position).Length;
		//	float DistToEnd = (NewPosition - CurrentView.Position).Length;
		//	
		//	float UVXStart = i / OldRelativePositions.Count;
		//	float UVXEnd = (i + 1) / OldRelativePositions.Count;
		//	{
		//		//bottom left
		//		vertices.Add( new SimpleVertex( CurrentView.Position + (DistToStart * DirToBottomLeft), LocalForward, LocalRight, new Vector2(UVXStart, 0) ));
		//		//top left
		//		vertices.Add( new SimpleVertex( CurrentView.Position + (DistToStart * DirToTopLeft), LocalForward, LocalRight, new Vector2(UVXStart, 1) ));
		//
		//		//top right
		//		vertices.Add( new SimpleVertex( CurrentView.Position + (DistToEnd * DirToTopRight), LocalForward, LocalRight, new Vector2(UVXEnd, 1) ));
		//
		//		//top right
		//		vertices.Add( new SimpleVertex( CurrentView.Position + (DistToEnd * DirToTopRight), LocalForward, LocalRight, new Vector2(UVXEnd, 1) ));
		//		//bottom right
		//		vertices.Add( new SimpleVertex( CurrentView.Position + (DistToEnd * DirToBottomRight), LocalForward, LocalRight, new Vector2(UVXEnd, 0) ));
		//		//bottom left
		//		vertices.Add( new SimpleVertex( CurrentView.Position + (DistToStart * DirToBottomLeft), LocalForward, LocalRight, new Vector2(UVXStart, 0) ));
		//	}
		//}
		ParticleMesh.SetVertexBufferSize(vertices.Count);
		ParticleMesh.SetVertexRange(0, vertices.Count);
		ParticleMesh.SetVertexBufferData(vertices);
	}

	public virtual void UpdateParticle()
	{
		Velocity += new Vector3(0, 0, -400) * Time.Delta;
		TraceResult ParticleTrace = Trace.Ray(Position, Position + (Velocity * Time.Delta)).WithTag("solid").WithoutTags("smbtrigger").IncludeClientside(true).Run();
		if (ParticleTrace.Hit && Vector3.Dot(ParticleTrace.Normal, Velocity) < 0)
		{
			Position = ParticleTrace.HitPosition;
			Velocity += ParticleTrace.Normal * Vector3.Dot(Velocity, ParticleTrace.Normal) * -1.75f;
		}else
		{
			Position += Velocity * Time.Delta;
		}
		ParticleLight.Position = Position;
		if (Time.Now > ParticleSpawnTime + 0.05f)
		{
			float LifeRemainingRatio = 1 - ((Time.Now - ParticleSpawnTime) / ParticleLife);
			ParticleLight.Brightness = 0.005f * LifeRemainingRatio;
		}
		if (Time.Now > ParticleSpawnTime + ParticleLife)
		{
			Delete();
		}
		if (Vector3.Dot(Position - CurrentView.Position, CurrentView.Rotation.Forward) < 0)
		{
			Delete();
		}
	}

	[Event.Frame]
	public virtual void ManageParticle()
	{
		if (!Instantiated)
		{
			return;
		}
		UpdateParticle();
		UpdateParticleMesh();

		OldRelativePositionData OldRelativePositionCurrent = new OldRelativePositionData(InverseTransformPosition(new Transform(CurrentView.Position, CurrentView.Rotation, 1), Position), Time.Now);
		OldRelativePositions.Add(OldRelativePositionCurrent);

		for (int i = 0; i < OldRelativePositions.Count; i++)
		{
			if (Time.Now > OldRelativePositions[0].Time + 0.02)
			{
				OldRelativePositions.RemoveAt(0);
			}else
			{
				break;
			}
		}
	}

	protected override void OnDestroy()
	{
		ParticleSceneObject.Delete();
		ParticleLight.Delete();
	}
}