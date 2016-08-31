using UnityEngine;
using System.Collections.Generic;

public class ARMakeLine : MonoBehaviour {

    public List<ARMakePoint> pointList = new List<ARMakePoint>();
    public GameObject Root;
    public float Distance;

    public ARMakeLine(Vector3 Point1, Vector3 Point2, GameObject root, float dist)
    {
        pointList.Add(new ARMakePoint(Point1));
        pointList.Add(new ARMakePoint(Point2));
        Root = root;
        Distance = dist;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Update is called once per frame
    public void RemovePoint(ARMakePoint _point)
    {
        if (pointList.Contains(_point))
        {
            pointList.Remove(_point);
        }

        if(pointList.Count < 2)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void OnDestroy()
    {
        foreach (ARMakePoint _point in pointList)
        {
            _point.RemoveLine(this);
        }
    }
}
