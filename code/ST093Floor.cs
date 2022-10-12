using Sandbox;
using SandboxEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public partial class ST093Floor : SMBObject
{

	public Mesh FloorMesh {get;set;}

	public Model FloorModel {get;set;}

	public SceneObject FloorSceneObject {get;set;}

	public Vector3[,] FloorPoints {get;set;}

	public Vector3[,] FloorNormals {get;set;}

	public float SpawnTime {get;set;}

	public override void Spawn()
	{
		base.Spawn();
		EnableAllCollisions = true;
		EnableDrawing = true;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();
		FloorMesh = new Mesh(Material.Load("materials/stagemat/bonus/bonus_5.vmat"));
		FloorMesh.CreateVertexBuffer<SimpleVertex>( SimpleVertex.Layout );
		FloorModel = Model.Builder.AddMesh(FloorMesh).Create();
		FloorSceneObject = new SceneObject(Map.Scene, FloorModel, new Transform(Vector3.Zero));
		int GridX = 31;
		int GridY = 31;
		FloorPoints = new Vector3[GridX, GridY];
		FloorNormals = new Vector3[GridX, GridY];
		for (int itX = 0; itX < GridX; itX++)
		{
			for (int itY = 0; itY < GridY; itY++)
			{
				float XRatio = ((float)itX / (GridX - 1));
				float YRatio = ((float)itY / (GridY - 1));
				FloorPoints[itX, itY] = new Vector3((XRatio * 400) - 200, (YRatio * 400) - 200, 0);
			}
		}
		SpawnTime = Time.Now;
	}

	[Event.Frame]
	public virtual void UpdateFloorPoints()
	{
		int GridX = 31;
		int GridY = 31;
		for (int itX = 0; itX < GridX; itX++)
		{
			for (int itY = 0; itY < GridY; itY++)
			{
				Vector3 OldPos = FloorPoints[itX, itY];
				float DistanceFromCenter = (OldPos * new Vector3(1, 1, 0)).Length;
				float SinFactor = (float)Math.Max((Time.Now - (SpawnTime + (DistanceFromCenter * 0.015)) - 3.5f) * 5, 0);
				float NewZ = (float)Math.Sin(SinFactor);
				float NormalRotation = (float)Math.Cos(SinFactor);
				Vector3 NewPos = new Vector3(OldPos.x, OldPos.y, NewZ * 6);
				Vector3 SpinAxis = Vector3.Cross(OldPos, Vector3.Up).Normal;
				Vector3 RealNormal = Vector3.Up;
				if (SinFactor != 0)
				{
					RealNormal = Vector3.Up * Rotation.FromAxis(SpinAxis, NormalRotation * -30);
				}
				FloorPoints[itX, itY] = NewPos;
				FloorNormals[itX, itY] = RealNormal;
			}
		}
	}

	[Event.Frame]
	public virtual void UpdateFloorMesh()
	{
		var vertices = new List<SimpleVertex>();
		var indices = new List<int>();
		int GridX = 31;
		int GridY = 31;
		for (int itX = 0; itX < GridX; itX++)
		{
			for (int itY = 0; itY < GridY; itY++)
			{
				if (itX + 1 < FloorPoints.GetLength(0) && itY + 1 < FloorPoints.GetLength(1))
				{
					Vector3 OurPoint = FloorPoints[itX, itY];
					Vector3 OurNormal = FloorNormals[itX, itY];
					vertices.Add( new SimpleVertex( OurPoint, OurNormal, Vector3.Right, new Vector2(0, 0) ));
					indices.Add(indices.Count);

					OurPoint = FloorPoints[itX + 1, itY];
					OurNormal = FloorNormals[itX + 1, itY];
					vertices.Add( new SimpleVertex( OurPoint, OurNormal, Vector3.Right, new Vector2(0.5f, 0) ));
					indices.Add(indices.Count);

					OurPoint = FloorPoints[itX, itY + 1];
					OurNormal = FloorNormals[itX, itY + 1];
					vertices.Add( new SimpleVertex( OurPoint, OurNormal, Vector3.Right, new Vector2(0, 0.5f) ));
					indices.Add(indices.Count);
				}
				if (itX - 1 >= 0 && itY - 1 >= 0 )
				{
					Vector3 OurPoint = FloorPoints[itX - 1, itY];
					Vector3 OurNormal = FloorNormals[itX - 1, itY];
					vertices.Add( new SimpleVertex( OurPoint, OurNormal, Vector3.Right, new Vector2(0, 0.5f) ));
					indices.Add(indices.Count);

					OurPoint = FloorPoints[itX, itY - 1];
					OurNormal = FloorNormals[itX, itY - 1];
					vertices.Add( new SimpleVertex( OurPoint, OurNormal, Vector3.Right, new Vector2(0.5f, 0) ));
					indices.Add(indices.Count);

					OurPoint = FloorPoints[itX, itY];
					OurNormal = FloorNormals[itX, itY];
					vertices.Add( new SimpleVertex( OurPoint, OurNormal, Vector3.Right, new Vector2(0.5f, 0.5f) ));
					indices.Add(indices.Count);
				}
			}
		}
		if (PhysicsBody == null)
		{
			SetupPhysicsFromSphere(PhysicsMotionType.Keyframed, Vector3.Zero, 1);
			FloorMesh.SetVertexBufferSize(vertices.Count);
			FloorMesh.SetVertexRange(0, vertices.Count);
		}
		FloorMesh.SetVertexBufferData(vertices);
		PhysicsBody.ClearShapes();
		var verticesPos = new List<Vector3>();
		foreach (SimpleVertex Vertex in vertices)
		{
			verticesPos.Add(Vertex.position);
		}
		PhysicsBody.AddMeshShape(verticesPos, indices);
	}

	public override Vector3 GetVelocityAtPoint(Vector3 InPoint, float DeltaTime)
	{
		Vector3 PreviousPosition;
		Vector3 CurrentPosition;
		{
			Vector3 OldPos = InPoint;
			float DistanceFromCenter = (OldPos * new Vector3(1, 1, 0)).Length;
			float SinFactor = (float)Math.Max(((Time.Now - Time.Delta) - (SpawnTime + (DistanceFromCenter * 0.015)) - 3.5f) * 5, 0);
			float NewZ = (float)Math.Sin(SinFactor);
			PreviousPosition = new Vector3(OldPos.x, OldPos.y, NewZ * 6);
		}
		{
			Vector3 OldPos = InPoint;
			float DistanceFromCenter = (OldPos * new Vector3(1, 1, 0)).Length;
			float SinFactor = (float)Math.Max((Time.Now - (SpawnTime + (DistanceFromCenter * 0.015)) - 3.5f) * 5, 0);
			float NewZ = (float)Math.Sin(SinFactor);
			CurrentPosition = new Vector3(OldPos.x, OldPos.y, NewZ * 6);
		}
		Vector3 VelAtPoint = (CurrentPosition - PreviousPosition) / Time.Delta;
		return VelAtPoint;
	}

	protected override void OnDestroy()
	{
		if (FloorSceneObject != null)
		{
			FloorSceneObject.Delete();
		}
	}

}
