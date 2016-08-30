using UnityEngine;
using System.Collections.Generic;

public class ForwardOnGazeEnter : MonoBehaviour {

    public List<GameObject> ForwardObjects;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnGazeEnter()
    {
        foreach (GameObject go in ForwardObjects)
        {
            go.SendMessage("OnGazeEnter", SendMessageOptions.DontRequireReceiver);
        }
    }
}
