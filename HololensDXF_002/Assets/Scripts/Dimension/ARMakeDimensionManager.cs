using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity;

public class ARMakeDimensionManager : Singleton<ARMakeDimensionManager>
{
    public static string keyboardText = "";

    private TouchScreenKeyboard keyboard;
    private ARMakeDimension activeDimension;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (keyboard != null && TouchScreenKeyboard.visible == false)
        {
            if (keyboard.done == true)
            {
                keyboardText = keyboard.text;
                keyboard = null;

                float x = -1;
                if (float.TryParse(keyboardText, out x) && x > 0f)
                {
                    activeDimension.UpdateLineDistance(x);
                }
                else
                {
                    Debug.Log("Keyboard re-opened " + keyboardText);
                    keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumbersAndPunctuation, false, false, false, false, "You need to enter a # for the distance");
                }
            }
        }
    }

    public void EditDimension(ARMakeDimension _dimension)
    {
        Debug.Log("EditDimension");
        if (_dimension.GetLine() != null && _dimension.GetLine().GetComponent<ARMakeLine>() != null && _dimension.GetLine().GetComponent<ARMakeLine>().pointList.Count > 1)
        {
            ARMakeLine _line = _dimension.GetLine().GetComponent<ARMakeLine>();
            if (_line.pointList[0].ifLocked() && _line.pointList[1].ifLocked())
            {
                return;
            }
        }
        else
            return;

#if UNITY_EDITOR
        keyboard = TouchScreenKeyboard.Open("12", TouchScreenKeyboardType.NumbersAndPunctuation, false, false, false, false, "Enter a new distance");
        activeDimension = _dimension;
#elif !UNITY_EDITOR
        Debug.Log("Keyboard open");
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumbersAndPunctuation, false, false, false, false, "Enter a new distance");

        activeDimension = _dimension;
#endif
    }
}
