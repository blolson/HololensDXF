using UnityEngine;
using System.Collections;

public class BubbleUpOnGazeEnter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnGazeEnter()
    {
        Debug.Log("OnGazeEnter " + gameObject.name);
        transform.parent.SendMessageUpwards("OnGazeEnter", SendMessageOptions.DontRequireReceiver);
    }

    public void OnGazeLeave()
    {
        Debug.Log("OnGazeLeave " + gameObject.name);
        transform.parent.SendMessageUpwards("OnGazeLeave", SendMessageOptions.DontRequireReceiver);
    }
}
