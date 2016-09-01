using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity;

public class ARMakePoint : MonoBehaviour {

    public Vector3 position;
    public List<ARMakeLine> lineList = new List<ARMakeLine>();
    //Blade: Not sure what "Root" does
    public bool IsStart;

    private TraceOutline tracer;
    private bool isMoving = false;
    private Vector3? moveDestination;

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
        if (isMoving)
        {
            //if (moveDestination != null)
            //    gameObject.transform.position = (Vector3)moveDestination;
            //else
            //    gameObject.transform.position = GazeManager.Instance.RoomPosition;

            gameObject.transform.position = GazeManager.Instance.RoomPosition;
            foreach (ARMakeLine _line in lineList)
            {
                _line.MoveAndRedraw();
            }
        }
	}

    public void OnGazeEnter()
    {
        if (ARMakeManager.Instance.mode == ARMakeMode.PointMove)
        {
            ARMakePointManager.Instance.UpdateMoveDestination(this);
            tracer.FadeIn();
        }
        else if (ARMakeManager.Instance.mode == ARMakeMode.AddLine)
        {
            ARMakeLineGuide.Instance.UpdateDestination(this);
            tracer.FadeIn();
        }
        else if (ARMakeManager.Instance.mode == ARMakeMode.Free)
        {
            tracer.FadeIn();
        }
    }

    public void OnGazeLeave()
    {
        //Debug.Log("OnGazeLeave: " + gameObject.name);
        if (ARMakeManager.Instance.mode == ARMakeMode.PointMove)
        {
            ARMakePointManager.Instance.UpdateMoveDestination();
            tracer.FadeOut();
        }
        else if (ARMakeManager.Instance.mode == ARMakeMode.AddLine)
        {
            ARMakeLineGuide.Instance.UpdateDestination();
            tracer.FadeOut();
        }
        else if (ARMakeManager.Instance.mode == ARMakeMode.Free)
        {
            tracer.FadeOut();
        }
    }

    public void RemoveLine(ARMakeLine _line)
    {
        Debug.Log(this + " Remove Line: " + _line);
        if(lineList.Contains(_line))
        {
            lineList.Remove(_line);
        }
    }

    public void AddLine(ARMakeLine _line, ARMakePoint _deletePoint = null)
    {
        foreach (ARMakeLine line in lineList)
        {
            Debug.Log(this + " " + line.gameObject + " " + _line.gameObject);
            if (line.gameObject == _line.gameObject)
            {
                Debug.LogError("This line already exists");
                return;
            }
        }

        Debug.Log(this + " " + lineList.Count + " " + _deletePoint + " " + _line);
        var _newLine = _line.AddPoint(this, _deletePoint);
        lineList.Add(_newLine);
        Debug.Log(this + " " + lineList.Count + " " + _deletePoint + " " + _line);

        foreach (ARMakeLine line in lineList)
        {
            Debug.Log(this + " " + line.gameObject);
        }

        //Debug.Log("Just created this line: " + _newLine + " from these points:");
    }

    public void Move(bool _state)
    {
        isMoving = _state;
    }

    public void SetDestination(ARMakePoint _point = null)
    {
        if (_point)
            moveDestination = _point.transform.position;
        else
            moveDestination = null;
    }

    void OnDestroy()
    {
        Debug.Log(gameObject.name + " Removing Points from " + lineList.Count);
        foreach (ARMakeLine _line in lineList)
        {
            Debug.Log(_line);
            if(_line != null)
                _line.RemovePoint(this);
        }
    }
}
