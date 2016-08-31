using UnityEngine;
using System.Collections.Generic;

public class ARMakePoint : MonoBehaviour {

    public Vector3 position;
    public List<ARMakeLine> lineList = new List<ARMakeLine>();
    //Blade: Not sure what "Root" does
    public GameObject Root;
    public bool IsStart;

    public ARMakePoint()
    {
        
    }

    public ARMakePoint(Vector3 pos)
    {
        position = pos;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnGazeEnter()
    {
        ARMakeLineGuide.Instance.UpdateDestination(this);
    }

    public void OnGazeLeave()
    {
        ARMakeLineGuide.Instance.UpdateDestination();
    }

    // Update is called once per frame
    public void RemoveLine(ARMakeLine _line)
    {
        if(lineList.Contains(_line))
        {
            lineList.Remove(_line);
        }
    }

    // Update is called once per frame
    void OnDestroy()
    {
        foreach (ARMakeLine _line in lineList)
        {
            _line.RemovePoint(this);
        }
    }
}
