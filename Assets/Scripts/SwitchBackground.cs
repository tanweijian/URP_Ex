using System;
using UnityEngine;
using UnityEngine.UI;

public class SwitchBackground : MonoBehaviour
{
    private enum BGType
    {
        None,
        Black,
        Red,
        Green,
    }

    private BGType bgType;

    public RawImage mBackground;
    public Button mSwitchBtn;

    private void Start()
    {
        mSwitchBtn.onClick.AddListener(SwitchBackgroundColor);
    }

    private void SwitchBackgroundColor()
    {
        bgType = (BGType)((int)(bgType + 1) % 4);
        switch (bgType)
        {
            case BGType.None:
                mBackground.color = Color.white;
                break;
            case BGType.Black:
                mBackground.color = Color.black;
                break;
            case BGType.Red:
                mBackground.color = Color.red;
                break;
            case BGType.Green:
                mBackground.color = Color.green;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}