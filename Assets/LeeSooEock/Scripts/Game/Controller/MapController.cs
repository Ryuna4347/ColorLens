using SUOKI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SUOKI
{
	public enum InputDir
	{
		None,
		left,
		right,
		up,
		down,
	}

	public enum MoveDir
	{
		None =			0,
		left =			1 << 1,
		right =			1 << 2,
		up =			1 << 3,
		down =			1 << 4,
		leftDown =		1 << 5,
		rightDown =		1 << 6,
		leftUp =		1 << 7,
		rightUp =		1 << 8,
	}

	public class MapController : MonoBehaviour
	{
		public struct Boundary
		{
			public int left;
			public int right;
			public int top;
			public int bottom;

			public int width => right - left + 1;
			public int height => top - bottom + 1;

			public void Init()
			{
				left = 9999;
				right = -9999;
				top = -9999;
				bottom = 9999;
			}
		}

		public class Tile
		{
			public Coord coord;
			public Unit unit;

			public Tile( int x, int y, Unit unit = null )
			{
				coord.x = x;
				coord.y = y;
				this.unit = unit;
			}
		}

		public Tilemap tilemap;
		public List<Unit> units;
		public Tile[,] tiles;
		Boundary _bound;

		public void Init()
		{
			MakeGrid();
#if UNITY_EDITOR
			PrintTile();
#endif
		}


		public void Fin()
		{

		}

		public void MakeGrid()
		{
			var childs = tilemap.GetComponentsInChildren<Unit>();
			_bound = ConvertoTileObjects( childs );

			_PushToZeroPos( _bound );

			tiles = new Tile[_bound.width, _bound.height];
			for ( int y = 0; y < _bound.height; y++ ) {
				for ( int x = 0; x < _bound.width; x++ ) {
					tiles[x, y] = new Tile( x, y, null );
				}
			}

			foreach ( var unit in units ) {
				var pos = unit.localPosition;
#if UNITY_EDITOR
				if ( pos.x > _bound.width || pos.y > _bound.height ) {
					Log.to.E( $"Wrong {unit.name} {pos.x} {_bound.width} {pos.y} {_bound.height}" );
				}
#endif
				tiles[(int)pos.x, (int)pos.y].unit = unit;
			}
		}

		public void PrintTile()
		{
			foreach ( var unit in units ) {
				Log.to.I( $"{unit.name} - Pos : {unit.transform.localPosition} " );
			}
			for ( int y = 0; y < _bound.height; y++ ) {
				for ( int x = 0; x < _bound.width; x++ ) {

					var tile = tiles[x, y];
					if ( tile.unit != null ) {
						Log.to.I( $"x: {x} y: {y}  obj = {tile.unit.name}" );
					} else {
						Log.to.I( $"x: {x} y: {y}  obj = Empty" );
					}
				}
			}
		}

		public Boundary ConvertoTileObjects( Unit[] units )
		{
			Boundary bd = new Boundary();
			bd.Init();

			//오브젝트 타일맵의 부모를 0.5 올리고 자식들을 다 0.5 내려서 자식들의 로컬좌표계와 타일좌표계를 동일시
			tilemap.transform.localPosition += new Vector3( 0.5f, 0.5f, 0f );

			foreach ( var unit in units ) {
				//좌하단으로 밀어서 0.5 정수단위로 그리드를 사용할수있게한다.
				unit.localPosition -= new Vector3( 0.5f, 0.5f, 0 );
				if ( unit.position.x < bd.left ) {
					bd.left = (int)unit.position.x;
				}
				if ( unit.position.x > bd.right ) {
					bd.right = (int)unit.position.x;
				}
				if ( unit.position.y > bd.top ) {
					bd.top = (int)unit.position.y;
				}
				if ( unit.position.y < bd.bottom ) {
					bd.bottom = (int)unit.position.y;
				}
				this.units.Add( unit );
			}
			return bd;
		}

		void _PushToZeroPos( Boundary bd )
		{
			if ( bd.left < 0 ) {
				_PushAllObjects( InputDir.right, Mathf.Abs( bd.left ) );
			}

			if ( bd.bottom < 0 ) {
				_PushAllObjects( InputDir.up, Mathf.Abs( bd.bottom ) );
			}

			if ( bd.left > 0 ) {
				_PushAllObjects( InputDir.left, bd.left );
			}

			if ( bd.bottom > 0 ) {
				_PushAllObjects( InputDir.down, bd.bottom );
			}
		}

		Vector3 _GetPushVector(InputDir dir, int amount )
		{
			Vector3 pushVector = Vector3.zero;
			switch ( dir ) {
				case InputDir.left:
					pushVector = Vector3.left * amount;
					break;
				case InputDir.right:
					pushVector = Vector3.right * amount;
					break;
				case InputDir.up:
					pushVector = Vector3.up * amount;
					break;
				case InputDir.down:
					pushVector = Vector3.down * amount;
					break;
				default:
					break;
			}
			return pushVector;
		}

		void _PushAllObjects( InputDir dir, int amount )
		{
			Vector3 pushVec = _GetPushVector( dir, amount );
			foreach ( var item in units ) {
				item.transform.localPosition += pushVec;
			}
		}

		//여러칸먹는 유닛용도
		public void SetUnitInTile(Unit unit, Coord coord )
		{
			tiles[coord.x, coord.y].unit = unit;
		}

		public void AddUnit( Unit unit )
		{
			units.Add( unit );
			tiles[unit.GetCoord().x, unit.GetCoord().y].unit = unit;
		}

		public void RemoveUnit( Unit unit )
		{
			units.Remove( unit );
			tiles[unit.GetCoord().x, unit.GetCoord().y].unit = null;
		}

		public void MoveUnit( Coord from, Coord to)
		{
			var unit = tiles[from.x, from.y].unit;
			tiles[to.x, to.y].unit = unit;
			tiles[from.x, from.y].unit = null;
		}

		public bool TryMoveTo(IMovable movable, InputDir inputDir )
		{
			Coord from = movable.GetCoord();
			Coord dest = GetDestination( from, inputDir );
			var tile = tiles[dest.x, dest.y];
			if( tile.unit == null ) {
				MoveUnit( from, dest );
				movable.Move( ToVec(dest) );
				return true;
			}
			return false;
		}



		public Coord GetDestination(Coord coord, InputDir inputDir )
		{
			switch ( inputDir ) {
				case InputDir.None:
					break;
				case InputDir.left:
					coord.x -= 1;
					break;
				case InputDir.right:
					coord.x += 1;
					break;
				case InputDir.up:
					coord.y += 1;
					break;
				case InputDir.down:
					coord.y -= 1;
					break;
				default:
					break;
			}
			return coord;
		}

		public Coord GetDestination( Coord coord, MoveDir moveDir )
		{
			switch ( moveDir ) {
				case MoveDir.None:
					break;
				case MoveDir.left:
					coord.x -= 1;
					break;
				case MoveDir.right:
					coord.x += 1;
					break;
				case MoveDir.up:
					coord.y += 1;
					break;
				case MoveDir.down:
					coord.y -= 1;
					break;
				case MoveDir.leftDown:
					coord.x -= 1;
					coord.y -= 1;
					break;
				case MoveDir.rightDown:
					coord.x += 1;
					coord.y -= 1;
					break;
				case MoveDir.leftUp:
					coord.x -= 1;
					coord.y += 1;
					break;
				case MoveDir.rightUp:
					coord.x += 1;
					coord.y += 1;
					break;
				default:
					break;
			}
			return coord;
		}



		Vector3 ToVec( Coord coord )
		{
			return new Vector3( coord.x, coord.y, 0 );
		}

		Coord ToCoord( Vector3 vec )
		{
			return new Coord( (int)vec.x, (int)vec.y );
		}

		MoveDir InputToMove(InputDir input )
		{
			switch ( input ) {
				case InputDir.None:
					return MoveDir.None;
				case InputDir.left:
					return MoveDir.left;
				case InputDir.right:
					return MoveDir.right;
				case InputDir.up:
					return MoveDir.up;
				case InputDir.down:
					return MoveDir.down;
				default:
					break;
			}
			return MoveDir.None;
		}



	}
}


//public void AddTosser( Tosser tosser )
//{
//	Coord coord = tosser.GetCoord();
//	tiles[coord.x, coord.y].tosser = tosser;
//}

//public void RemoveTosser(Tosser tosser)
//{
//	Coord coord = tosser.GetCoord();
//	tiles[coord.x, coord.y].tosser = null;
//}

