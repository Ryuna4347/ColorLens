using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SUOKI
{
    public class InputController : MonoBehaviour
    {

        public void Init()
        {

        }

        public void Fin()
        {

        }

        private void Update()
        {
            Move( CheckInput() );
        }

        InputDir CheckInput()
        {
            if ( Input.GetKeyDown( KeyCode.W ) )
                return InputDir.up;
            else if ( Input.GetKeyDown( KeyCode.A ) )
                return InputDir.left;
            else if ( Input.GetKeyDown( KeyCode.S ) )
                return InputDir.down;
            else if ( Input.GetKeyDown( KeyCode.D ) )
                return InputDir.right;
            return InputDir.None;
        }

        public void Move( InputDir dir )
        {
            if ( dir == InputDir.None )
                return;

            StageMan.In.InputMove( dir );
        }
    }
}
