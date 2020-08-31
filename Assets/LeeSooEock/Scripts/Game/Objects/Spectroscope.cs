using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SUOKI
{
    public class Spectroscope : Unit
    {
        public void Enter(MoveDir enterDir)
        {
            //R, G, B 순서로 위에서아래 순으로 쪼개서 생성.
            //White => ( 255 , 255 , 255) 니까 3개로 분리됨
            // 255 , 0 , 0
            // 0 , 255, 0
            // 0 , 0, 255

            //케릭터끼리만나면 서로의 255를 합침

            //분광기를 만나면 자기가가진 R,G,B를 분할함.

            switch ( enterDir ) {
                case MoveDir.None:
                    break;
                case MoveDir.left:
                case MoveDir.leftDown:
                case MoveDir.leftUp:
                    //오른쪽으로 3분할.
                    break;

                case MoveDir.right:
                case MoveDir.rightDown:
                case MoveDir.rightUp:
                    //왼쪽으로 3분할.
                    break;

                case MoveDir.up:
                    //아래로 3분할
                    break;
                case MoveDir.down:
                    //위로 3분할
                    break;
                default:
                    break;
            }

        }
    }
}
