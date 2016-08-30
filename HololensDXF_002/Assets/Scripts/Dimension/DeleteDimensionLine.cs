using UnityEngine;
using System.Collections;

public class DeleteDimensionLine : MonoBehaviour
{
    /// <summary>
    /// when tip text is tapped, destroy this tip and relative objects.
    /// </summary>
	public void OnSelect()
    {
        var parent = gameObject.transform.parent.gameObject;
        if (parent != null)
        {
            Destroy(parent);
        }
    }
}
