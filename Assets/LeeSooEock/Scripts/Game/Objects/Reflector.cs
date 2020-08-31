using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SUOKI
{
    public enum ReflectKind
    {
        reflectLT,
        reflectRT,
    }

    public class Reflector : Unit
    {
        public ReflectKind reflectKind;

        Tosser tosser;

        public override void Init()
        {
            base.Init();

            switch ( reflectKind ) {
                case ReflectKind.reflectLT:
                    tosser = Tosser.New(TosserKind.reflectLT, GetCoord() );
                    break;
                case ReflectKind.reflectRT:
                    tosser = Tosser.New( TosserKind.reflectRT, GetCoord() );
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
