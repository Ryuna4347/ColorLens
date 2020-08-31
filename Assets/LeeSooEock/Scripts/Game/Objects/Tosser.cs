using UnityEngine;

namespace SUOKI
{
	//보이지 않지만 이 게임의 오브젝트들은 결국 토스하는 기믹이 대부분
	//렌즈는 도착지점이 정해져있고

	//반사광은 어디서 들어왔냐에 따라서 보내는곳이다르다.

	public enum TosserKind
	{
		reflectLT,
		reflectRT,

		concaveHorizonL,
		concaveHorizonR,

		concaveVerticalT,
		concaveVerticalB,

		convexHorizonL,
		convexHorizonR,

		convexVerticalT,
		convexVerticalB,
	}


	public class Tosser
	{
		Coord _coord;
		MoveDir _entableDirs;			//진입가능한 방향.
		TosserKind _kind;

		public static Tosser New(TosserKind kind, Coord coord )
		{
			return new Tosser().Init( coord, kind );
		}

		public Tosser Init( Coord coord, TosserKind kind  )
		{
			_coord = coord;
			_kind = kind;
			return this;
		}

		public void Fin()
		{

		}

		public Coord GetCoord()
		{
			return _coord;
		}

		//진입시도전후가 같은좌표를 반환하면 안가면 됨.
		//public bool IsEntable( MoveDir enter )
		//{
		//	if ( (enter & _entableDirs) == 0 )
		//		return false;
		//	return true;
		//}

		//void _SetEntableDirs( MoveDir entables )
		//{
		//	this._entableDirs = entables;
		//}

		public Coord GetExitCoord(MoveDir enter, Coord enterCoord)
		{
			Coord targetC = _coord;
			switch ( _kind ) {
				case TosserKind.reflectLT:
					targetC =  GetReflectLT( enter );
					break;
				case TosserKind.reflectRT:
					targetC = GetReflectRT( enter );
					break;
				case TosserKind.concaveHorizonL:
					targetC =  GetConcaveHorizonL( enter );
					break;
				case TosserKind.concaveHorizonR:
					targetC = GetConcaveHorizonR( enter );
					break;
				case TosserKind.concaveVerticalT:
					targetC = GetConcaveVerticalT( enter );
					break;
				case TosserKind.concaveVerticalB:
					targetC = GetConcaveVerticalB( enter );
					break;
				case TosserKind.convexHorizonL:
					targetC = GetConvaxHoritonL( enter );
					break;
				case TosserKind.convexHorizonR:
					targetC = GetConvaxHoritonR( enter );
					break;
				case TosserKind.convexVerticalT:
					targetC = GetConvaxVerticalT( enter );
					break;
				case TosserKind.convexVerticalB:
					targetC = GetConvexVerticalB( enter );
					break;
				default:
					break;
			}
			if ( targetC == _coord )
				return enterCoord;
			return targetC;
		}

		public Coord GetReflectLT( MoveDir enter )
		{
			Coord coord = _coord;

			switch ( enter ) {
				case MoveDir.None:
					break;
				case MoveDir.left:
					coord.y -= 1;
					break;
				case MoveDir.right:
					coord.y += 1;
					break;
				case MoveDir.up:
					coord.x += 1;
					break;
				case MoveDir.down:
					coord.x -= 1;
					break;
				case MoveDir.leftDown:
					coord.x -= 1;
					coord.y -= 1;
					break;
				case MoveDir.rightDown:
					//block
					break;
				case MoveDir.leftUp:
					//block
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

		public Coord GetReflectRT(MoveDir enter )
		{
			Coord coord = _coord;

			switch ( enter ) {
				case MoveDir.None:
					break;
				case MoveDir.left:
					coord.y += 1;
					break;
				case MoveDir.right:
					coord.y -= 1;
					break;
				case MoveDir.up:
					coord.x -= 1;
					break;
				case MoveDir.down:
					coord.x += 1;
					break;
				case MoveDir.leftDown:
					//Block
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
					//block
					break;
				default:
					break;
			}
			return coord;
		}

		public Coord GetConcaveHorizonL( MoveDir enter )
		{
			Coord coord = _coord;

			switch ( enter ) {
				case MoveDir.None:
					break;
				case MoveDir.left:
				case MoveDir.right:
					//block
					break;
				case MoveDir.up:
				case MoveDir.leftUp:
				case MoveDir.rightUp:
					coord.x -= 1;
					coord.y -= 1;
					break;
				case MoveDir.down:
				case MoveDir.leftDown:
				case MoveDir.rightDown:
					coord.x -= 1;
					coord.y += 1;
					break;
				default:
					break;
			}
			return coord;
		}

		public Coord GetConcaveHorizonR( MoveDir enter )
		{
			Coord coord = _coord;

			switch ( enter ) {
				case MoveDir.None:
					break;
				case MoveDir.left:
				case MoveDir.right:
					//block
					break;
				case MoveDir.up:
				case MoveDir.leftUp:
				case MoveDir.rightUp:
					coord.x += 1;
					coord.y -= 1;
					break;
				case MoveDir.down:
				case MoveDir.leftDown:
				case MoveDir.rightDown:
					coord.x += 1;
					coord.y += 1;
					break;
				default:
					break;
			}
			return coord;
		}

		public Coord GetConcaveVerticalT( MoveDir enter )
		{
			Coord coord = _coord;

			switch ( enter ) {
				case MoveDir.None:
					break;
				case MoveDir.left:
				case MoveDir.leftUp:
				case MoveDir.leftDown:
					coord.x += 1;
					coord.y += 1;
					break;
				case MoveDir.right:
				case MoveDir.rightUp:
				case MoveDir.rightDown:
					coord.x -= 1;
					coord.y += 1;
					break;
				case MoveDir.up:
				case MoveDir.down:
					//block
					break;
				default:
					break;
			}
			return coord;
		}

		public Coord GetConcaveVerticalB( MoveDir enter )
		{
			Coord coord = _coord;

			switch ( enter ) {
				case MoveDir.None:
					break;
				case MoveDir.left:
				case MoveDir.leftUp:
				case MoveDir.leftDown:
					coord.x += 1;
					coord.y -= 1;
					break;
				case MoveDir.right:
				case MoveDir.rightUp:
				case MoveDir.rightDown:
					coord.x -= 1;
					coord.y -= 1;
					break;
				case MoveDir.up:
				case MoveDir.down:
					//block
					break;
				default:
					break;
			}
			return coord;
		}

		public Coord GetConvaxHoritonL( MoveDir enter )
		{
			Coord coord = _coord;

			switch ( enter ) {
				case MoveDir.None:
					break;
				case MoveDir.left:
				case MoveDir.right:
					//block
					break;
				case MoveDir.up:
				case MoveDir.rightUp:
				case MoveDir.leftUp:
					coord.x += 1;
					coord.y -= 1;
					break;
				case MoveDir.down:
				case MoveDir.leftDown:
				case MoveDir.rightDown:
					coord.x += 1;
					coord.y += 1;
					break;
				default:
					break;
			}
			return coord;
		}

		public Coord GetConvaxHoritonR( MoveDir enter )
		{
			Coord coord = _coord;

			switch ( enter ) {
				case MoveDir.None:
					break;
				case MoveDir.left:
				case MoveDir.right:
					//block
					break;
				case MoveDir.up:
				case MoveDir.rightUp:
				case MoveDir.leftUp:
					coord.x -= 1;
					coord.y -= 1;
					break;
				case MoveDir.down:
				case MoveDir.leftDown:
				case MoveDir.rightDown:
					coord.x -= 1;
					coord.y += 1;
					break;
				default:
					break;
			}
			return coord;
		}

		public Coord GetConvaxVerticalT( MoveDir enter )
		{
			Coord coord = _coord;
			switch ( enter ) {
				case MoveDir.None:
					break;
				case MoveDir.left:
				case MoveDir.leftUp:
				case MoveDir.leftDown:
					coord.x += 1;
					coord.y -= 1;
					break;
				case MoveDir.right:
				case MoveDir.rightUp:
				case MoveDir.rightDown:
					coord.x -= 1;
					coord.y -= 1;
					break;
				case MoveDir.up:
				case MoveDir.down:
					//block
					break;
				default:
					break;
			}
			return coord;
		}

		public Coord GetConvexVerticalB( MoveDir enter )
		{
			Coord coord = _coord;
			switch ( enter ) {
				case MoveDir.None:
					break;
				case MoveDir.left:
				case MoveDir.leftUp:
				case MoveDir.leftDown:
					coord.x += 1;
					coord.y += 1;
					break;
				case MoveDir.right:
				case MoveDir.rightUp:
				case MoveDir.rightDown:
					coord.x -= 1;
					coord.y += 1;
					break;
				case MoveDir.up:
				case MoveDir.down:
					//block
					break;
				default:
					break;
			}
			return coord;
		}
	}
}

