#region Info
// -----------------------------------------------------------------------
// SetSpriteSorting.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using UnityEditor;
using UnityEngine;
#endregion
[ExecuteInEditMode]
public class SetSpriteSorting : MonoBehaviour
{

	public string sortingLayerName;
	public int sortingOrder;

	private void Awake()
	{
		Refresh();
	}
    #if UNITY_EDITOR
	private void Update()
	{
		if (EditorApplication.isPlayingOrWillChangePlaymode)
		{
			//this.enabled = false;
		}
		else
		{
			// editor code here!
			Refresh();
		}
	}
    #endif
	private void Refresh()
	{
		transform.GetComponent<Renderer>().sortingLayerName = sortingLayerName;
		transform.GetComponent<Renderer>().sortingOrder = sortingOrder;
	}
}
