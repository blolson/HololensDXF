//#define ARMAKEDEBUG

using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity;

public class ARMakePoint : MonoBehaviour {

    public Vector3 position;
    public List<ARMakeLine> lineList = new List<ARMakeLine>();
    //Blade: Not sure what "Root" does
    public bool IsStart = false;
    public GameObject colliderObject;
    public GameObject lockObject;

    private BoxCollider boxCollider;
    private TraceOutline tracer;
    private bool isMoving, isLocked = false;
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
        boxCollider = colliderObject.GetComponent<BoxCollider>();
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
        if(colliderObject != null && boxCollider.enabled)
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
#if ARMAKEDEBUG
        Debug.Log(this + " Remove Line: " + _line);
#endif
        if(lineList.Contains(_line))
        {
            lineList.Remove(_line);
        }
    }

    public void AddLine(ARMakeLine _line, ARMakePoint _deletePoint = null)
    {
        foreach (ARMakeLine line in lineList)
        {
#if ARMAKEDEBUG
            Debug.Log(this + " " + line.gameObject + " " + _line.gameObject); 
#endif
            if (line.gameObject == _line.gameObject)
            {
                Debug.LogError("This line already exists");
                return;
            }
        }

#if ARMAKEDEBUG
        Debug.Log(this + " " + lineList.Count + " " + _deletePoint + " " + _line); 
#endif
        var _newLine = _line.AddPoint(this, _deletePoint);
        lineList.Add(_newLine);
#if ARMAKEDEBUG
        Debug.Log(this + " " + lineList.Count + " " + _deletePoint + " " + _line); 
#endif

        foreach (ARMakeLine line in lineList)
        {
#if ARMAKEDEBUG
            Debug.Log(this + " " + line.gameObject);
#endif
        }

        //Debug.Log("Just created this line: " + _newLine + " from these points:");
    }

    public void Move(bool _state)
    {
        isMoving = _state;
    }

    public void RedrawLines()
    {
        foreach (ARMakeLine _line in lineList)
        {
            _line.MoveAndRedraw();
        }
    }

    public void Lock(bool _state)
    {
        isLocked = _state;
        if(isLocked)
        {
            var renderer = gameObject.GetComponent<MeshRenderer>().material;
            var color = renderer.color;
            color.r = 255f;
            color.g = 255f;
            color.b = 0f;
            color.a = 255f;
            renderer.color = color;
            lockObject.SetActive(true);
        }
        else
        {
            var renderer = gameObject.GetComponent<MeshRenderer>().material;
            var color = renderer.color;
            color.r = 0;
            color.g = 255f;
            color.b = 0f;
            color.a = 255f;
            renderer.color = color;
            lockObject.SetActive(false);
        }
    }

    public bool ifLocked() { return isLocked; }

    public void SetDestination(ARMakePoint _point = null)
    {
        if (_point)
            moveDestination = _point.transform.position;
        else
            moveDestination = null;
    }

    void OnDestroy()
    {
#if ARMAKEDEBUG
        Debug.Log(gameObject.name + " Removing Points from " + lineList.Count); 
#endif
        foreach (ARMakeLine _line in lineList)
        {
#if ARMAKEDEBUG
            Debug.Log(_line);
#endif
            if (_line != null)
                _line.RemovePoint(this);
        }
    }
}
