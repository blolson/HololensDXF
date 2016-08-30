using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity;

public class TipManager : MonoBehaviour {


    public List<TipControl> tipList;
    public List<DirectionIndicator> dirList;
    public float arrowTimer = 30;

    private TipControl curTip;
    private int tipIter = -1;
    private float curArrowTimer;

    // Use this for initialization
    void Start () {

        foreach (TipControl tc in tipList)
        {
            DirectionIndicator di = tc.GetComponent<DirectionIndicator>();
            if (di != null)
            {
                dirList.Add(di);
                di.enabled = false;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(curArrowTimer > 0)
        {
            curArrowTimer -= Time.deltaTime;
            if (curArrowTimer <= 0)
            {
                dirList[tipIter].enabled = true;
            }
        }

    }

    public void CycleTips()
    {
        if (tipIter == -1)
        {
            tipList[++tipIter].FadeIn(1.0f);
            return;
        }

        tipList[tipIter].FadeOut(0f);
        tipIter = ++tipIter >= tipList.Count ? 0 : tipIter;
        tipList[tipIter].FadeIn(1.0f);

        curTip = tipList[tipIter];

        curArrowTimer = arrowTimer;
    }

    public void RestartHintArrowTimer(TipControl responder)
    {
        //if(responder == curTip)
        //{
        //    curArrowTimer = arrowTimer;
        //}
    }
}
