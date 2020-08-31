using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SUOKI
{
    public class Unit : Actor
    {
        public virtual void Init()
        {

        }

        public virtual void Fin()
        {

        }

        //이 유닛이 있는 타일에 진입하려할때
        public virtual bool Enter(Unit unit, MoveDir enterDir )
        {
            return false;
        }

        public Coord GetCoord()
        {
            return new Coord( (int)localPosition.x, (int)localPosition.y );
        }
    }
}
