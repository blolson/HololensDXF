using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using HoloToolkit.Unity;

public class TipControl : MonoBehaviour {

    public Text[] textArray;
    public RawImage[] imageArray;
    public TrailRenderer trail;
    public DirectionIndicator dirIndicator;
    public TipManager tipManager;

    private enum FadeState {fadeIn, fadeOut, inactive};
    private FadeState currentState;
    private float delay = 0f;

    // Use this for initialization
    void Start () {
        dirIndicator = GetComponent<DirectionIndicator>();
	}

    public void OnGazeEnter()
    {
        if (dirIndicator != null && dirIndicator.DoesDirectionIndicatorExist())
            dirIndicator.enabled = false;
    }

    public void OnGazeLeave()
    {
        tipManager.RestartHintArrowTimer(this);
    }

    // Update is called once per frame
    void Update () {
        if (currentState == FadeState.fadeOut)
        {
            var render = trail.material;
            var color = render.color;
            color.a = Mathf.Max(color.a - Time.deltaTime, 0);
            render.color = color;
            if (color.a <= 0f)
            {
                currentState = FadeState.inactive;
                gameObject.SetActive(false);
            }
        }
        else if (currentState == FadeState.fadeIn)
        {
            var render = trail.material;
            var color = render.color;
            color.a = Mathf.Min(color.a + Time.deltaTime, 1);
            render.color = color;
            if (color.a >= 1f)
            {
                currentState = FadeState.inactive;
            }
        }
    }

    public void FadeOut(float _delay = 0f)
    {
        StopCoroutine("_FadeIn");

        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        delay = _delay;
        for (int i = 0; i < textArray.Length; i++)
            textArray[i].CrossFadeAlpha(1, 0f, false);

        for (int i = 0; i < imageArray.Length; i++)
            imageArray[i].CrossFadeAlpha(1, 0f, false);

        var render = trail.material;
        var color = render.color;
        color.a = 1f;
        render.color = color;

        StartCoroutine("_FadeOut");
    }

    IEnumerator _FadeOut()
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < textArray.Length; i++)
            textArray[i].CrossFadeAlpha(0, 1.0f, false);

        for (int i = 0; i < imageArray.Length; i++)
            imageArray[i].CrossFadeAlpha(0, 1.0f, false);

        currentState = FadeState.fadeOut;
    }

    public void FadeIn(float _delay = 0f)
    {
        if (!SceneStateManager.Instance.TIPS_ENABLED)
            return;

        StopCoroutine("_FadeOut");
        gameObject.transform.position = Camera.main.transform.forward * 1.5f;

        delay = _delay;
        if(!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        for (int i = 0; i < textArray.Length; i++)
            textArray[i].CrossFadeAlpha(0, 0f, false);

        for (int i = 0; i < imageArray.Length; i++)
            imageArray[i].CrossFadeAlpha(0, 0f, false);

        var render = trail.material;
        var color = render.color;
        color.a = 0;
        render.color = color;

        StartCoroutine("_FadeIn");
    }

    IEnumerator _FadeIn()
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < textArray.Length; i++)
            textArray[i].CrossFadeAlpha(1, 1.0f, false);

        for (int i = 0; i < imageArray.Length; i++)
            imageArray[i].CrossFadeAlpha(1, 1.0f, false);

        currentState = FadeState.fadeIn;
    }
}
