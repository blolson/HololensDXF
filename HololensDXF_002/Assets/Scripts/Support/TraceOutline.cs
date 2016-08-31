using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrailRenderer))]
public class TraceOutline : MonoBehaviour {

    public GameObject[] destinationArray;
    public float animationSpeed = 1f;
    public float lerpEnd = 1f;
    public float fadeOutSpeed = 0.1f;
    public float fadeInSpeed = 2.0f;
    public bool fateOutOnGazeEnter = true;
    public bool activeOnSpawn = false;

    private TrailRenderer trailRenderer;
    private Bounds bounds;
    private float height;
    private float width;
    private float depth;
    private float lerp;
    private int destIter = 0;
    private int destTotal;
    private Vector3 curDest;

    private enum fadeState { fadeOut, fadeIn, inactive };
    private fadeState visualState = fadeState.inactive;

    // Use this for initialization
    void Start () {
        if (gameObject.GetComponent<TrailRenderer>() != null && destinationArray.Length > 0)
        {
            trailRenderer = gameObject.GetComponent<TrailRenderer>();
            curDest = destinationArray[0].transform.position;
            return;
        }
        Debug.Log("Disabling TraceOutline, don't have a list of destination points to follow");
        if(!activeOnSpawn)
            gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        lerp += Time.deltaTime * animationSpeed;
        transform.position = Vector3.Slerp(transform.position, new Vector3(curDest.x, curDest.y, curDest.z), lerp);

        if (lerp >= lerpEnd)
        {
            destIter = ++destIter >= destinationArray.Length ? destIter = 0 : destIter;
            curDest = destinationArray[destIter].transform.position;
            lerp = 0;
        }

        if (gameObject.activeInHierarchy && visualState == fadeState.fadeOut)
        {
            var render = trailRenderer.material;
            var color = render.color;
            color.a = Mathf.Max(color.a - (Time.deltaTime * fadeOutSpeed), 0);
            render.color = color;
            if (color.a <= 0f)
            {
                visualState = fadeState.inactive;
                gameObject.SetActive(false);
            }
        }
        else if (gameObject.activeInHierarchy && visualState == fadeState.fadeIn)
        {
            var render = trailRenderer.material;
            var color = render.color;
            color.a = Mathf.Min(color.a + (Time.deltaTime * fadeInSpeed), 1);
            render.color = color;
            if (color.a >= 1f)
            {
                visualState = fadeState.inactive;
            }
        }
    }

    public void OnGazeEnter()
    {
        if (gameObject.activeInHierarchy && fateOutOnGazeEnter)
        {
            visualState = fadeState.fadeOut;
        }
    }

    public void FadeOut()
    {
        if (gameObject.activeInHierarchy)
        {
            visualState = fadeState.fadeOut;
        }
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        if (trailRenderer == null)
        {
            trailRenderer = gameObject.GetComponent<TrailRenderer>();
            curDest = destinationArray[0].transform.position;
        }
        var render = trailRenderer.material;
        var color = render.color;
        color.a = 0f;
        render.color = color;
        visualState = fadeState.fadeIn;
    }

}
