using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SUOKI
{
    public class Game : MonoBehaviour
    {
		public StageController con;

		Coroutine playCo = null;

		public void Start()
		{
			Play();
		}

		public void Play()
		{
			Log.Seq.I( $"Game Play()" );

			if ( playCo != null ) StopCoroutine( playCo );
			playCo = StartCoroutine( PlayCo() );
		}

		IEnumerator PlayCo()
		{
			yield return null;
			StageMan.In.Init( con );
		}
	}
}

