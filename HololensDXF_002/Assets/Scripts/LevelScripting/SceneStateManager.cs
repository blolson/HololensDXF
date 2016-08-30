using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using HoloToolkit.Unity;
using System.Collections;

public partial class SceneStateManager : Singleton<SceneStateManager> {

    public KeywordManager keywordManager;
    public enum SceneStates { Start, Scan, ScanGood, Generate, GenerateStart, Edit, Email, Restart };
    public SceneStates currentState;

    [System.Serializable]
    public struct StateKeywords
    {
        [Tooltip("The keyword to recognize.")]
        public string Keyword;
        [Tooltip("The state where this keyword is active.")]
        public SceneStates activeState;
        [Tooltip("The KeyCode to recognize.")]
        public KeyCode KeyCode;
        [Tooltip("The UnityEvent to be invoked when the keyword is recognized.")]
        public UnityEvent Response;
    }

    [System.Serializable]
    public struct StateChangeHooks
    {
        [Tooltip("For readability")]
        public string eventName;
        [Tooltip("When this state is entered, this event will be triggered.")]
        public SceneStates activeState;
        [Tooltip("The UnityEvent to be invoked when the state is changed.")]
        public UnityEvent response;
    }

    public List<StateKeywords> StateKeyRecList = new List<StateKeywords>();
    public StateChangeHooks[] StateEventHooks;

    // Use this for initialization
    void Start () {
        StartCoroutine("StartFirstFrame");
    }
	
	// Update is called once per frame
	void Update () {

	}

    IEnumerator StartFirstFrame()
    {
        yield return null;
        currentState = SceneStates.Start;
        List<KeywordManager.KeywordAndResponse> newWordList = new List<KeywordManager.KeywordAndResponse>();
        foreach (StateKeywords skw in StateKeyRecList)
        {
            if (skw.activeState == currentState)
            {
                newWordList.Add(new KeywordManager.KeywordAndResponse(skw.Keyword, skw.KeyCode, skw.Response));
            }
        }
        keywordManager.UpdateKeywordRecognizer(newWordList.ToArray());
        keywordManager.StartKeywordRecognizer();
    }

    // Update is called once per frame
    public void Progress(SceneStates nextState = SceneStates.Start)
    {
        if (nextState == SceneStates.Start)
            currentState++;
        else
            currentState = nextState;


        Debug.Log(currentState);

        foreach (StateChangeHooks sch in StateEventHooks)
        {
            if(sch.activeState == currentState)
                sch.response.Invoke();
        }




        List<KeywordManager.KeywordAndResponse> newWordList = new List<KeywordManager.KeywordAndResponse>();
        foreach (StateKeywords skw in StateKeyRecList)
        {
            if (skw.activeState == currentState)
            {
                newWordList.Add(new KeywordManager.KeywordAndResponse(skw.Keyword, skw.KeyCode, skw.Response));
            }
        }

        keywordManager.UpdateKeywordRecognizer(newWordList.ToArray());
    }
}
