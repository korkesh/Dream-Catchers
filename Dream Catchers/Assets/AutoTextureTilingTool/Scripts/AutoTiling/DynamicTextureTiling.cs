using UnityEngine;
using System.Collections;

namespace AutoTiling {

	/// <summary>
	/// Dynamic texture tiling.
	/// This class will allow updating of the Mesh and the UV coordinates at runtime.
	/// Every time your GameObject gets resized, it will adjust the Texture accordingly.
	/// </summary>
	public class DynamicTextureTiling : AutoTextureTiling {

		void Update () {
			
			if (scaleX != transform.localScale.x || scaleY != transform.localScale.y || scaleZ != transform.localScale.z) {
				scaleX = transform.localScale.x;
				scaleY = transform.localScale.y;
				scaleZ = transform.localScale.z;
				CreateMeshAndUVs ();
			}

		}

	}

}