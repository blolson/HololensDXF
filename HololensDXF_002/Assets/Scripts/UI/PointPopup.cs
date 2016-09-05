using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;

public class PointPopup : MonoBehaviour {

    public GameObject UI;
    public GameObject moveIcon;
    public GameObject trashIcon;

    public float visualScale = 0.08f;
    private ARMakePoint popupPoint;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float modify = Vector3.Distance(Camera.main.transform.position, gameObject.transform.position) * visualScale;
        gameObject.transform.localScale = new Vector3(modify, modify, modify);
    }

    public void SetPoint(ARMakePoint _point)
    {
        Debug.Log("SetPoint " + _point);
        popupPoint = _point;

        if (popupPoint.ifLocked())
        {
            moveIcon.GetComponent<BoxCollider>().enabled = false;
            trashIcon.GetComponent<BoxCollider>().enabled = false;

            var color = moveIcon.GetComponent<RawImage>().color;
            color.a = 25f/255.0f;
            moveIcon.GetComponent<RawImage>().color = color;

            color = trashIcon.GetComponent<RawImage>().color;
            color.a = 25f / 255.0f;
            trashIcon.GetComponent<RawImage>().color = color;
        }
    }

    public ARMakePoint GetPoint(){return popupPoint;}

    public void OnTrash()
    {
        Debug.Log("Trashing " + popupPoint);
        if (popupPoint.ifLocked())
        {
            ARMakePointManager.Instance.Close();
            return;
        }

        Destroy(popupPoint.gameObject);
        ARMakePointManager.Instance.Close();
    }

    public void OnLock()
    {
        if(popupPoint.ifLocked())
            popupPoint.Lock(false);
        else
            popupPoint.Lock(true);

        ARMakePointManager.Instance.Close();
    }

    public void OnMove()
    {
        Debug.Log("OnMove");
        if(popupPoint.ifLocked())
        {
            ARMakePointManager.Instance.Close();
            return;
        }

        ARMakeManager.Instance.mode = ARMakeMode.PointMove;
        UI.SetActive(false);
        popupPoint.Move(true);
        popupPoint.GetCollider().enabled = false;
        GazeManager.Instance.RaycastLayerMask = ~ARMakePointManager.Instance.PointMove_RaycastLayers;
    }
}
