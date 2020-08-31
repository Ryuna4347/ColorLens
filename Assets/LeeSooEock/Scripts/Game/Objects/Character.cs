using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SUOKI
{
    public class Character : Unit, IMovable
    {
        public Color color;
        public int moveCount;

        public SpriteRenderer render;

        public override void Init()
        {
            base.Init();

            Set( color );
        }

        public override void Fin()
        {
            base.Fin();
        }

        public void Set( Color color )
        {
            this.color = color;
            render.color = color;

            if ( color.r >= 1f && color.g >= 1f && color.b >= 1f )
                moveCount = 1;
            else  if ( color.r >= 1f && color.b <= 0f) {
                moveCount = 1;
            }
            else if ( color.g >= 1f && color.r <= 0f ) {
                moveCount = 2;
            }
            else if( color.b >= 1f && color.g <= 0f ) {
                moveCount = 3;
            }
        }

        public int GetMoveCount()
        {
            return moveCount;
        }

        public void Move( Vector3 pos )
        {
            moveCount--;
            localPosition = pos;
        }

        public void Toss(Vector3 pos )
        {

        }

        public new Coord GetCoord()
        {
            return base.GetCoord();
        }

        public bool IsMovable()
        {
            return moveCount > 0;
        }
    }
}
