using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SUOKI
{
    public class Exit : Unit
    {
        public Color color;

        public SpriteRenderer render;

        public override void Init()
        {
            base.Init();

            render.color = color;
        }

        public override void Fin()
        {
            base.Fin();
        }


        public void Enter()
        {

        }
    }
}
