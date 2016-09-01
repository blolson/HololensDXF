using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class PointPopup : MonoBehaviour {

    public ARMakePoint popupPoint;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTrash()
    {
        Debug.Log("Trashing " + popupPoint);
        Destroy(popupPoint.gameObject);
        ARMakePointManager.Instance.Close();
    }

    public void OnLock()
    {

    }

    public void OnMove()
    {
        Debug.Log("OnMove");
        ARMakeManager.Instance.mode = ARMakeMode.PointMove;
        popupPoint.Move(true);
        popupPoint.gameObject.GetComponent<BoxCollider>().enabled = false;
    }
}
