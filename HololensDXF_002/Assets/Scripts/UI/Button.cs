using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

public class Button : MonoBehaviour {

    public List<OnTapEvent> tapEvents = new List<OnTapEvent>();
    public TraceOutline tracer;
    private RawImage image;
    private float originalAlpha;

    [System.Serializable]
    public struct OnTapEvent
    {
        [Tooltip("For readability")]
        public string eventName;
        [Tooltip("The UnityEvent to be invoked when the state is changed.")]
        public UnityEvent response;
    }

    // Use this for initialization
    void Start () {
        image = GetComponent<RawImage>();
        originalAlpha = image.color.a;
        tracer.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnSelect()
    {
        foreach (OnTapEvent ote in tapEvents)
            ote.response.Invoke();
    }

    public void OnGazeEnter()
    {
        tracer.gameObject.SetActive(true);

        var color = image.color;
        color.a = 175.0f/255.0f;
        image.color = color;
    }

    public void OnGazeLeave()
    {
        tracer.gameObject.SetActive(false);
        
        var color = image.color;
        color.a = originalAlpha;
        image.color = color;
    }
}
