using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SUOKI
{
    public class ConcaveLens : Unit
    {
        public LendDirKind lensDir;

        Tosser tosser1;
        Tosser tosser2;

        public override void Init()
        {
            base.Init();

            Coord coord;
            switch ( lensDir ) {
                case LendDirKind.horizontal:
                    tosser1 = Tosser.New( TosserKind.concaveHorizonL, GetCoord() );
                    coord = new Coord( GetCoord().x + 1, GetCoord().y );
                    tosser2 = Tosser.New( TosserKind.concaveHorizonR, coord );
                    StageMan.In.con.map.SetUnitInTile( this, coord );
                    break;
                case LendDirKind.vertical:
                    tosser1 = Tosser.New( TosserKind.concaveVerticalT, GetCoord() );
                    coord = new Coord( GetCoord().x, GetCoord().y - 1 );
                    tosser2 = Tosser.New( TosserKind.concaveVerticalB, coord );
                    StageMan.In.con.map.SetUnitInTile( this, coord );
                    break;
                default:
                    break;
            }
        }

        public override void Fin()
        {
            base.Fin();
        }
    }
}
