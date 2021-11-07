#region Info
// -----------------------------------------------------------------------
// ShieldField.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using System;
using UnityEngine;
#endregion

namespace TopDownShooter
{
	public class ShieldField : MonoBehaviour
	{

		public ShieldFieldTransformerLink[] shieldFieldTransformerLinkArray;


		private void Start()
		{
			foreach (var shieldFieldTransformerLink in
					shieldFieldTransformerLinkArray)
				shieldFieldTransformerLink.shieldFieldTransformer.OnDestroyed
						+= ShieldFieldTransformer_OnDestroyed;
		}

		private void
				ShieldFieldTransformer_OnDestroyed(object sender, EventArgs e)
		{
			var shieldFieldTransformer = sender as ShieldFieldTransformer;
			var shieldFieldTransformerLink
					= GetShieldFieldTransformerLink(shieldFieldTransformer);
			foreach (var powerLine in shieldFieldTransformerLink
					.shieldFieldTransformerPowerLines
					.powerLineArray) Destroy(powerLine.gameObject);
			TestAllTransformersDead();
		}

		public ShieldFieldTransformerLink GetShieldFieldTransformerLink(
				ShieldFieldTransformer shieldFieldTransformer)
		{
			foreach (var shieldFieldTransformerLink in
					shieldFieldTransformerLinkArray)
				if (shieldFieldTransformerLink.shieldFieldTransformer ==
				    shieldFieldTransformer)
					return shieldFieldTransformerLink;
			return null;
		}

		private void TestAllTransformersDead()
		{
			var allDead = true;
			foreach (var shieldFieldTransformerLink in
					shieldFieldTransformerLinkArray)
				if (shieldFieldTransformerLink.shieldFieldTransformer.IsAlive())
				{
					allDead = false;
					break;
				}

			if (allDead) // All transformers are dead!
				Destroy(gameObject);
		}

		[Serializable]
		public class ShieldFieldTransformerPowerLines
		{
			public Transform[] powerLineArray;
		}

		[Serializable]
		public class ShieldFieldTransformerLink
		{
			public ShieldFieldTransformer shieldFieldTransformer;
			public ShieldFieldTransformerPowerLines
					shieldFieldTransformerPowerLines;
		}
	}
}
