using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrailRenderer))]
public class TraceOutline : MonoBehaviour {

    public GameObject[] destinationArray;
    public float animationSpeed = 1f;
    public float lerpEnd = 1f;
    public float fadeOutSpeed = 0.1f;

    private TrailRenderer trailRenderer;
    private Bounds bounds;
    private float height;
    private float width;
    private float depth;
    private float lerp;
    private int destIter = 0;
    private int destTotal;
    private Vector3 curDest;

    private float currentFadeOutTime = 0;

    // Use this for initialization
    void Start () {
        if (gameObject.GetComponent<TrailRenderer>() != null && destinationArray.Length > 0)
        {
            trailRenderer = gameObject.GetComponent<TrailRenderer>();
            curDest = destinationArray[0].transform.position;
            return;
        }
        Debug.Log("Disabling TraceOutline, don't have a list of destination points to follow");
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

        if (gameObject.activeInHierarchy && currentFadeOutTime > 0)
        {
            var render = trailRenderer.material;
            var color = render.color;
            color.a -= Time.deltaTime * fadeOutSpeed;
            render.color = color;
            if (color.a <= 0f)
            {
                currentFadeOutTime = 0;
                gameObject.SetActive(false);
            }
        }
    }

    public void OnGazeEnter()
    {
        if (gameObject.activeInHierarchy)
        {
            currentFadeOutTime = 1;
        }
    }

}
