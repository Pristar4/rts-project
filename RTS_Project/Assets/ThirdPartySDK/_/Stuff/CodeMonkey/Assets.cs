#region Info
// -----------------------------------------------------------------------
// Assets.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEngine;
#endregion

namespace CodeMonkey
{

	/*
	 * Global Asset references
	 * Edit Asset references in the prefab CodeMonkey/Resources/CodeMonkeyAssets
	 * */
	public class Assets : MonoBehaviour
	{

		// Internal instance reference
		private static Assets _i;


		// All references

		public Sprite s_White;
		public Sprite s_Circle;

		public Material m_White;

		// Instance reference
		public static Assets i
		{
			get
			{
				if (_i == null)
					_i = Instantiate(
							Resources.Load<Assets>("CodeMonkeyAssets"));
				return _i;
			}
		}
	}

}
