using System;
using UnityEngine;


namespace SUOKI
{
	public interface IMovable
	{
		Coord GetCoord();
		bool IsMovable();
		void Move( Vector3 pos );
	}

	public interface ITossable
	{
		Coord Toss( Unit unit, MoveDir enterDir);
	}

	public interface IEventable
	{

	}

	public interface IBlockable
	{
		bool IsBlocked( Coord from, Coord to );
	}
}
