namespace SUOKI
{
	[System.Serializable]
#pragma warning disable CS0660 // 형식은 == 연산자 또는 != 연산자를 정의하지만 Object.Equals(object o)를 재정의하지 않습니다.
#pragma warning disable CS0661 // 형식은 == 연산자 또는 != 연산자를 정의하지만 Object.GetHashCode()를 재정의하지 않습니다.
	public struct Coord
#pragma warning restore CS0661 // 형식은 == 연산자 또는 != 연산자를 정의하지만 Object.GetHashCode()를 재정의하지 않습니다.
#pragma warning restore CS0660 // 형식은 == 연산자 또는 != 연산자를 정의하지만 Object.Equals(object o)를 재정의하지 않습니다.
	{
		public int x;
		public int y;
		public Coord( int _x, int _y )
		{
			x = _x;
			y = _y;
		}

		public static bool operator ==( Coord c1, Coord c2f )
		{
			return c1.x == c2f.x && c1.y == c2f.y;
		}
		public static bool operator !=( Coord c1, Coord c2f )
		{
			return !(c1 == c2f);
		}

		public static Coord NewRandom( int minX, int maxX, int minY, int maxY )
		{
			int x = UnityEngine.Random.Range( minX, maxX );
			int y = UnityEngine.Random.Range( minY, maxY );
			return new Coord( x, y );
		}
	}
}

