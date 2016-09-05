using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity;

public class ARMakeDimension : MonoBehaviour {

    public GameObject colliderObject;

    private GameObject line;
    private BoxCollider boxCollider;
    private TraceOutline tracer;
    private TextMesh text;
    private bool isMoving = false;
    private const float metersToInches = 39.3701f;

    // Use this for initialization
    void Start()
    {
        tracer = gameObject.GetComponentInChildren<TraceOutline>();
        tracer.gameObject.SetActive(false);
        boxCollider = colliderObject.GetComponent<BoxCollider>();
        text = gameObject.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if (colliderObject != null && boxCollider.enabled)
        {
            float modify = Vector3.Distance(Camera.main.transform.position, colliderObject.transform.position);
            colliderObject.transform.localScale = new Vector3(modify, modify, modify);
        }
    }

    public BoxCollider GetCollider()
    {
        return boxCollider;
    }

    public void OnGazeEnter()
    {
        Debug.Log("OnGazeEnter " + gameObject.name + " " + ARMakeManager.Instance.mode);
        if (ARMakeManager.Instance.mode == ARMakeMode.Free)
        {
            if (line != null && line.GetComponent<ARMakeLine>() != null && line.GetComponent<ARMakeLine>().pointList.Count > 1)
            {
                ARMakeLine _line = line.GetComponent<ARMakeLine>();
                if (_line.pointList[0].ifLocked() && _line.pointList[1].ifLocked())
                {
                    return;
                }
            }
            else
                return;

            tracer.FadeIn();
        }
    }

    public void OnGazeLeave()
    {
        //Debug.Log("OnGazeLeave: " + gameObject.name);
        tracer.FadeOut();
    }

    public void MoveAndRedraw(float _distance, Vector3 _position, Vector3 _offset)
    {
        gameObject.transform.position = _position;
        gameObject.transform.Translate(_offset * 0.05f);

        if(text == null)
            text = gameObject.GetComponent<TextMesh>();

        text.text = (Mathf.Round(_distance * metersToInches * 100f) / 100f) + "<size=" + (text.fontSize * 5f / 6f) + ">in</size>";
    }

    public void SetLine(GameObject _line)
    {
        line = _line;
    }


    public GameObject GetLine()
    {
        return line;
    }

    public string GetText()
    {
        return text.text;

    }
    public void UpdateLineDistance(float _distance)
    {
        Debug.Log("UpdateLineDistance");
        line.GetComponent<ARMakeLine>().UpdateDistance(_distance);
    }
}
