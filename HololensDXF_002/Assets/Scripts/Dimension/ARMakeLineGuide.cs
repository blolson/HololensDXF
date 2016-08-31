using UnityEngine;
using HoloToolkit.Unity;

public class ARMakeLineGuide : Singleton<ARMakeLineGuide>
{
    public LineRenderer guide;
    public GameObject originalDestination;
    private GameObject source;
    private GameObject destination;

    // Use this for initialization
    void Start () {
        destination = originalDestination;
    }
	
	// Update is called once per frame
	void Update () {
        if (guide.enabled)
        {
            guide.SetPosition(1, destination.transform.position);
        }
    }

    public void UpdateSource(ARMakePoint _source = null)
    {
        //will update this ifelse later if necessary
        if(_source != null)
            source = _source.gameObject;
        else
            source = _source.gameObject;

        guide.SetPosition(1, destination.transform.position);
        guide.SetPosition(0, source.transform.position);
        guide.enabled = true;
    }

    public void UpdateDestination(ARMakePoint _destination = null)
    {
        if (_destination != null)
            destination = _destination.gameObject;
        else
            destination = originalDestination;

        guide.SetPosition(1, destination.transform.position);
        guide.SetPosition(0, source.transform.position);
        guide.enabled = true;
    }

    public void EndGuide()
    {
        guide.enabled = false;
    }
}
