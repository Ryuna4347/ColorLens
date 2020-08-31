using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SUOKI
{
	public class StageController : MonoBehaviour
	{
		public MapController map;
		public InputController input;

		public void Init()
		{
			map.Init();
			input.Init();
		}

		public void Fin()
		{
			map.Fin();
			input.Fin();
		}


	}
}

