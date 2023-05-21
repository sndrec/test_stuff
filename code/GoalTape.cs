using Sandbox;
using Editor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

public class TapePoint
{
	public TapePoint( Vector3 InPosition, bool InAnchored )
	{
		Position = InPosition;
		OldPosition = InPosition;
		Velocity = Vector3.Zero;
		Anchored = InAnchored;
		Twist = 0;
	}
	public Vector3 Position, OldPosition, Velocity;
	public float Twist;
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
		TapeSceneObject = new SceneObject(Game.SceneWorld, TapeModel, new Transform(Vector3.Zero));
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
			Vector3 StartNormal = -Forward;
			Vector3 EndNormal = -Forward;
			Vector3 StartUp = Up;
			Vector3 EndUp = Up;
			if (i > 0) // true if this is not the first stick
			{
				TapeStick PrevStick = Sticks[i - 1];
				if (CurStick.PointA == PrevStick.PointB) //true if point A is connected to the previous stick
				{
					Vector3 PrevStickDir = (PrevStick.PointA.Position - PrevStick.PointB.Position).Normal;
					Vector3 AvgDir = (StickDir + PrevStickDir).Normal;
					StartUp = Vector3.Cross( Owner.Rotation.Forward, AvgDir ).Normal;
					StartNormal = Vector3.Cross( StartUp, AvgDir ).Normal;
					StartUp *= Rotation.FromAxis( AvgDir, CurStick.PointA.Twist );
					StartNormal *= Rotation.FromAxis( AvgDir, CurStick.PointA.Twist );
					CurStick.PointA.Twist = MathX.Lerp( CurStick.PointA.Twist, PrevStick.PointB.Twist, Time.Delta );
					CurStick.PointA.Twist = MathX.Lerp( CurStick.PointA.Twist, CurStick.PointB.Twist, Time.Delta );
				}
				else // true if point A is NOT connected to the previous stick
				{
					StartUp = Vector3.Cross( Owner.Rotation.Forward, StickDir ).Normal;
					StartNormal = Vector3.Cross( StartUp, StickDir ).Normal;
					StartUp *= Rotation.FromAxis( StickDir, CurStick.PointA.Twist );
					StartNormal *= Rotation.FromAxis( StickDir, CurStick.PointA.Twist );
					CurStick.PointA.Twist = MathX.Lerp( CurStick.PointA.Twist, 45, Time.Delta * 3 );
				}
			}else // if this IS the first stick...
			{
				if ( CurStick.PointA.Anchored )
				{
					CurStick.PointA.Twist = 0;
					CurStick.PointB.Twist = MathX.Lerp( CurStick.PointB.Twist, CurStick.PointA.Twist, Time.Delta );
				}
				else
				{
					CurStick.PointA.Twist = MathX.Lerp( CurStick.PointA.Twist, 45, Time.Delta * 3 );
					CurStick.PointB.Twist = MathX.Lerp( CurStick.PointB.Twist, CurStick.PointA.Twist, Time.Delta );
				}
			}
			if ( i+1 < Sticks.Count ) // true if this is not the last stick
			{
				TapeStick NextStick = Sticks[i + 1];
				if ( CurStick.PointB == NextStick.PointA ) // true if point B is connected to the next stick
				{
					Vector3 NextStickDir = (NextStick.PointA.Position - NextStick.PointB.Position).Normal;
					Vector3 AvgDir = (StickDir + NextStickDir).Normal;
					EndUp = Vector3.Cross( Owner.Rotation.Forward, AvgDir ).Normal;
					EndNormal = Vector3.Cross( EndUp, AvgDir ).Normal;
					EndUp *= Rotation.FromAxis( AvgDir, CurStick.PointB.Twist );
					EndNormal *= Rotation.FromAxis( AvgDir, CurStick.PointB.Twist );
					CurStick.PointB.Twist = MathX.Lerp( CurStick.PointB.Twist, NextStick.PointA.Twist, Time.Delta );
					CurStick.PointB.Twist = MathX.Lerp( CurStick.PointB.Twist, CurStick.PointA.Twist, Time.Delta );
				}
				else // true if point B is NOT connected to the next stick
				{
					EndUp = Vector3.Cross( Owner.Rotation.Forward, StickDir ).Normal;
					EndNormal = Vector3.Cross( EndUp, StickDir ).Normal;
					EndUp *= Rotation.FromAxis( StickDir, CurStick.PointB.Twist );
					EndNormal *= Rotation.FromAxis( StickDir, CurStick.PointB.Twist );
					CurStick.PointB.Twist = MathX.Lerp( CurStick.PointB.Twist, -45, Time.Delta * 3 );
				}
			}else // if this IS the last stick...
			{
				if ( CurStick.PointB.Anchored )
				{
					CurStick.PointB.Twist = 0;
					CurStick.PointA.Twist = MathX.Lerp( CurStick.PointB.Twist, CurStick.PointA.Twist, Time.Delta );
				}
				else
				{
					CurStick.PointB.Twist = MathX.Lerp( CurStick.PointA.Twist, -45, Time.Delta * 3 );
					CurStick.PointA.Twist = MathX.Lerp( CurStick.PointB.Twist, CurStick.PointA.Twist, Time.Delta );
				}
			}
			float UVXStart = 1 - (float)i / (float)Sticks.Count;
			float UVXEnd = 1 - (float)(i + 1) / (float)Sticks.Count;
			vertices.Add( new SimpleVertex( CurStick.PointA.Position + (StartUp * 3), StartNormal, Right, new Vector2(UVXStart, 0) ));
			vertices.Add( new SimpleVertex( CurStick.PointA.Position - (StartUp * 3), StartNormal, Right, new Vector2(UVXStart, 1) ));
			vertices.Add( new SimpleVertex( CurStick.PointB.Position - (EndUp * 3), EndNormal, Right, new Vector2(UVXEnd, 1) ));
			vertices.Add( new SimpleVertex( CurStick.PointB.Position + (EndUp * 3), EndNormal, Right, new Vector2(UVXEnd, 0) ));
			vertices.Add( new SimpleVertex( CurStick.PointA.Position + (StartUp * 3), StartNormal, Right, new Vector2(UVXStart, 0) ));
			vertices.Add( new SimpleVertex( CurStick.PointB.Position - (EndUp * 3), EndNormal, Right, new Vector2(UVXEnd, 1) ));
		}
		if (first)
		{
			TapeMesh.SetVertexBufferSize(vertices.Count);
			TapeMesh.SetVertexRange(0, vertices.Count);
		}
		TapeMesh.SetVertexBufferData(vertices);
	}

	[GameEvent.Client.Frame]
	public virtual void SimulateTape()
	{
		MyGame GameEnt = GameManager.Current as MyGame;
		foreach (TapePoint p in Points)
		{
			p.OldPosition = p.Position;
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
			if (!p.Anchored && Game.LocalClient.Pawn is Pawn)
			{
				Pawn Ball = Game.LocalClient.Pawn as Pawn;
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
			if ( !p.Anchored )
			{
				TraceResult ParticleTrace = Trace.Sphere( 1f, p.OldPosition, p.Position ).WithTag( "solid" ).WithoutTags( "smbtrigger" ).IncludeClientside( true ).Run();
				if ( ParticleTrace.Hit )
				{
					TraceResult DepenPoint = Trace.Ray( p.Position, ParticleTrace.HitPosition ).WithTag( "solid" ).WithoutTags( "smbtrigger" ).IncludeClientside( true ).Run();
					p.Position = DepenPoint.HitPosition + (ParticleTrace.Normal * 1.01f);
					p.Velocity += ParticleTrace.Normal * Vector3.Dot( Velocity, ParticleTrace.Normal ) * -1.1f;
					p.Twist = MathX.Lerp( p.Twist, 90 * Math.Sign(p.Twist), Time.Delta * 12 );
				}
			}
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
