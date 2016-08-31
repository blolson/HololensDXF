using UnityEngine;
using HoloToolkit.Unity;

/// <summary>
/// provide a tip text of current measure mode
/// </summary>
public class UnityDimensionValue : Singleton<UnityDimensionValue>
{
    private const string LineMode = "Line Mode";
    private const string PloygonMode = "Geometry Mode";
    private TextMesh text;
    private int fadeTime = 100;

    void Start()
    {
        text = GetComponent<TextMesh>();
        text.text = LineMode;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            // if you want log the position of mode tip text, just uncomment it.
            // Debug.Log("pos: " + gameObject.transform.position);
            text.text = LineMode;
        }

        var render = GetComponent<MeshRenderer>().material;
        fadeTime = 100;
        // fade tip text
        if (fadeTime == 0)
        {
            var color = render.color;
            fadeTime = 100;
            color.a = 1f;
            render.color = color;
            gameObject.SetActive(false);
        }
        else
        {
            var color = render.color;
            color.a -= 0.01f;
            render.color = color;
            fadeTime--;
        }
    }
}
