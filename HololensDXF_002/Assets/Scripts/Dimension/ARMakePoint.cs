using UnityEngine;
using System.Collections.Generic;

public class ARMakePoint : MonoBehaviour {

    public Vector3 position;
    public List<ARMakeLine> lineList = new List<ARMakeLine>();
    //Blade: Not sure what "Root" does
    public bool IsStart;

    private TraceOutline tracer;

    public ARMakePoint()
    {
        
    }

    public ARMakePoint(Vector3 pos)
    {
        position = pos;
    }

    // Use this for initialization
    void Start () {
        tracer = gameObject.GetComponentInChildren<TraceOutline>();
        tracer.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnGazeEnter()
    {
        if (DimensionManager.Instance.mode == ARMakeMode.AddLine)
        {
            ARMakeLineGuide.Instance.UpdateDestination(this);
            tracer.FadeIn();
        }
        else if (DimensionManager.Instance.mode == ARMakeMode.Free)
        {
            tracer.FadeIn();
        }
    }

    public void OnGazeLeave()
    {
        Debug.Log("OnGazeLeave: " + gameObject.name);
        if (DimensionManager.Instance.mode == ARMakeMode.AddLine)
        {
            ARMakeLineGuide.Instance.UpdateDestination();
            tracer.FadeOut();
        }
        else if (DimensionManager.Instance.mode == ARMakeMode.Free)
        {
            tracer.FadeOut();
        }
    }

    public void RemoveLine(ARMakeLine _line)
    {
        if(lineList.Contains(_line))
        {
            lineList.Remove(_line);
        }
    }

    public void AddLine(ARMakeLine _line, ARMakePoint _deletePoint = null)
    {
        if (lineList.Contains(_line))
        {
            Debug.LogError("This line already exists");
            return;
        }
        else
        {
            Debug.Log(this + " " + _deletePoint);
            var _newLine = _line.AddPoint(this, _deletePoint);
            lineList.Add(_newLine);
            //Debug.Log("Just created this line: " + _newLine + " from these points:");
        }
    }

    void OnDestroy()
    {
        //Debug.Log("Removing Points");
        foreach (ARMakeLine _line in lineList)
        {
            if(_line != null)
                _line.RemovePoint(this);
        }
    }
}
