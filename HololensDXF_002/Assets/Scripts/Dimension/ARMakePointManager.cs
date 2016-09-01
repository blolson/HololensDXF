using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class ARMakePointManager : Singleton<ARMakePointManager>
{
    public GameObject PopupPrefab;

    private GameObject popup;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnHold()
    {
        ARMakeManager.Instance.mode = ARMakeMode.PointPopup;

        if (GazeManager.Instance.HitInfo.collider != null)
        {
            ARMakePoint popupPoint = GazeManager.Instance.HitInfo.collider.gameObject.GetComponent<ARMakePoint>();
            popup = (GameObject)Instantiate(PopupPrefab, popupPoint.transform.position, Quaternion.identity);
            popup.GetComponent<PointPopup>().popupPoint = popupPoint;
        }
    }

    public void UpdateMoveDestination(ARMakePoint _point = null)
    {
        if (popup != null)
            popup.GetComponent<PointPopup>().popupPoint.SetDestination(_point);
    }

    public void Close()
    {
        Debug.Log("\n");
        Debug.Log("\n");
        Debug.Log("CLOSE");
        Debug.Log("\n");
        Debug.Log("\n");
        if (ARMakeManager.Instance.mode == ARMakeMode.PointMove)
        {
            popup.GetComponent<PointPopup>().popupPoint.Move(false);

            if (GazeManager.Instance.HitInfo.collider != null)
            {
                ARMakePoint oldPoint = GazeManager.Instance.HitInfo.collider.gameObject.GetComponent<ARMakePoint>();
                if (oldPoint != null)
                {
                    ARMakePoint point = popup.GetComponent<PointPopup>().popupPoint;

                    point.transform.position = oldPoint.transform.position;
                    foreach (ARMakeLine oldLine in oldPoint.lineList)
                    {
                        bool sameLine = false;
                        foreach (ARMakeLine _line in point.lineList)
                        {
                            Debug.Log("CHECKING LINE " + _line.pointList[0] + " " + _line.pointList[1] + " " + oldLine.pointList[0] + " " + oldLine.pointList[1] + " " + oldPoint.gameObject.name);
                            if (_line.pointList.Count > 1)
                            {
                                if (oldLine.pointList.Contains(_line.pointList[1]) || oldLine.pointList.Contains(_line.pointList[0]))
                                {
                                    Debug.Log("SAME LINE, Ignore...");
                                    sameLine = true;
                                }
                            }
                        }

                        if(sameLine)
                            continue;
                        else
                            point.AddLine(oldLine, oldPoint);
                    }
                    Destroy(oldPoint.gameObject);

                    foreach (ARMakeLine _line in point.lineList)
                    {
                        _line.MoveAndRedraw();
                    }
                }
            }
            popup.GetComponent<PointPopup>().popupPoint.gameObject.GetComponent<BoxCollider>().enabled = true;
        }

        ARMakeManager.Instance.mode = ARMakeMode.Free;
        if(popup != null)
            Destroy(popup);
    }
}
