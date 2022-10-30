using Sandbox;
using SandboxEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public class TapePoint
{
	public TapePoint(Vector3 InPosition, bool InAnchored)
	{
		Position = InPosition;
		Velocity = Vector3.Zero;
		Anchored = InAnchored;
	}
	public Vector3 Position, Velocity;
	public bool Anchored;
}

public class TapeStick
{
	public TapeStick(TapePoint InPointA, TapePoint InPointB)
	{
		PointA = InPointA;
		PointB = InPointB;
		Length = (PointB.Position - PointA.Position).Length * 0.9f;
	}
	public TapePoint PointA, PointB;
	public float Length;
}

public partial class GoalTapeEntity : Entity
{	
	public List<TapePoint> Points {get;set;}

	public List<TapeStick> Sticks {get;set;}

	public Mesh TapeMesh {get;set;}

	public Model TapeModel {get;set;}

	public SceneObject TapeSceneObject {get;set;}

	private List<SimpleVertex> vertices {get;set;}

	public override void Spawn()
	{
		base.Spawn();
		Points = new List<TapePoint>{};
		Sticks = new List<TapeStick>{};
		TapeMesh = new Mesh(Material.Load("materials/tw_goaltape.vmat"));
		TapeMesh.CreateVertexBuffer<SimpleVertex>( SimpleVertex.Layout );
		TapeModel = Model.Builder.AddMesh(TapeMesh).Create();
		TapeSceneObject = new SceneObject(Map.Scene, TapeModel, new Transform(Vector3.Zero));
		vertices = new List<SimpleVertex>();
	}

	public virtual void CreateRope(List<TapePoint> InPoints)
	{
		for (int i = 0; i < InPoints.Count; i++)
		{
			Points.Add(InPoints[i]);
			if (i - 1 >= 0)
			{
				TapeStick NewStick = new TapeStick(Points[i - 1], Points[i]);
				//NewStick.PointA = Points[i - 1];
				//NewStick.PointB = Points[i];
				//NewStick.Length = (NewStick.PointB.Position - NewStick.PointA.Position).Length;
				Sticks.Add(NewStick);
			}
		}

		UpdateRopeMesh(true);
		//mesh.LockVertexBuffer<SimpleVertex>( vertices =>
        //{
        //} );
	}

	public virtual void UpdateRopeMesh(bool first)
	{
		vertices.Clear();
		Vector3 Forward = Owner.Rotation.Forward;
		Vector3 Right = Owner.Rotation.Right;
		Vector3 Up = Owner.Rotation.Up;
		for (int i = 0; i < Sticks.Count; i++)
		{
			TapeStick CurStick = Sticks[i];
			Vector3 StickCentre = (CurStick.PointA.Position + CurStick.PointB.Position) / 2;
			Vector3 StickDir = (CurStick.PointA.Position - CurStick.PointB.Position).Normal;
			Vector3 StickNormal = Vector3.Cross(Vector3.Up, StickDir).Normal;
			float UVXStart = 1 - (float)i / (float)Sticks.Count;
			float UVXEnd = 1 - (float)(i + 1) / (float)Sticks.Count;
			vertices.Add( new SimpleVertex( CurStick.PointA.Position + (Up * 3), StickNormal, Right, new Vector2(UVXStart, 0) ));
			vertices.Add( new SimpleVertex( CurStick.PointA.Position - (Up * 3), StickNormal, Right, new Vector2(UVXStart, 1) ));
			vertices.Add( new SimpleVertex( CurStick.PointB.Position - (Up * 3), StickNormal, Right, new Vector2(UVXEnd, 1) ));
			vertices.Add( new SimpleVertex( CurStick.PointB.Position + (Up * 3), StickNormal, Right, new Vector2(UVXEnd, 0) ));
			vertices.Add( new SimpleVertex( CurStick.PointA.Position + (Up * 3), StickNormal, Right, new Vector2(UVXStart, 0) ));
			vertices.Add( new SimpleVertex( CurStick.PointB.Position - (Up * 3), StickNormal, Right, new Vector2(UVXEnd, 1) ));
		}
		if (first)
		{
			TapeMesh.SetVertexBufferSize(vertices.Count);
			TapeMesh.SetVertexRange(0, vertices.Count);
		}
		TapeMesh.SetVertexBufferData(vertices);
	}

	[Event.Frame]
	public virtual void SimulateTape()
	{
		MyGame GameEnt = Game.Current as MyGame;
		foreach (TapePoint p in Points)
		{
			if (!p.Anchored)
			{
				Vector3 PositionBeforeUpdate = p.Position;
				p.Velocity += GameEnt.StageTilt.Down * Time.Delta * 350f;
				p.Velocity += -p.Velocity * Time.Delta * 6f;
				p.Position += p.Velocity * Time.Delta;
				//p.PrevPosition = PositionBeforeUpdate;
			}
		}
		float TapeDelta = Math.Min(Time.Delta * 1000, 40);
		for (int i = 0; i < 3; i++)
		{
			foreach (TapeStick Stick in Sticks)
			{
				Vector3 StickCentre = (Stick.PointA.Position + Stick.PointB.Position) / 2;
				Vector3 StickDir = (Stick.PointA.Position - Stick.PointB.Position).Normal;
				if (!Stick.PointA.Anchored)
				{
					Vector3 OldPos = Stick.PointA.Position;
					Stick.PointA.Position = StickCentre + StickDir * Stick.Length / 2;
					Stick.PointA.Velocity = Stick.PointA.Velocity - ((OldPos - Stick.PointA.Position) * TapeDelta);
				}
				if (!Stick.PointB.Anchored)
				{
					Vector3 OldPos = Stick.PointB.Position;
					Stick.PointB.Position = StickCentre - StickDir * Stick.Length / 2;
					Stick.PointB.Velocity = Stick.PointB.Velocity - ((OldPos - Stick.PointB.Position) * TapeDelta);
				}
			}
		}
		foreach (TapePoint p in Points)
		{
			if (!p.Anchored && Local.Client.Pawn is Pawn)
			{
				Pawn Ball = Local.Client.Pawn as Pawn;
				if ((p.Position - Ball.ClientPosition).Length < 10)
				{
					Vector3 NewNormal = (p.Position - Ball.ClientPosition).Normal;
					NewNormal = (NewNormal * new Vector3(1, 1, 0.8f)).Normal;
					Vector3 DesiredPosition = Ball.ClientPosition + (NewNormal * 10);
					p.Velocity += (DesiredPosition - p.Position) * (Time.Delta * 1000);
					p.Position = DesiredPosition;
					Ball.ClientVelocity -= (NewNormal * 10f * Time.Delta);
				}
			}
			//DebugOverlay.Sphere(p.Position, 0.5f, new Color(255, 0, 0), Time.Delta, false);
		}
		UpdateRopeMesh(false);
	}

	//public override void DoRender(SceneObject Obj)
	//{
	//	if (Graphics.LayerType == SceneLayerType.Opaque)
	//	{
	//		return;
	//	}
	//	Vector3 Up = Vector3.Up;
	//	Material TapeMat = Material.Load("materials/tw_goaltape.vmat");
	//	VertexBuffer TapeVerts = new VertexBuffer();
	//	foreach (TapeStick Stick in Sticks)
	//	{
	//		TapeVerts.AddQuad(Stick.PointA.Position + (Up * 3), Stick.PointB.Position + (Up * 3), Stick.PointB.Position - (Up * 3), Stick.PointA.Position - (Up * 3));
	//	}
	//	TapeVerts.Draw(TapeMat);
	//}
	protected override void OnDestroy()
	{
		TapeSceneObject.Delete();
	}
}