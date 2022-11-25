using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicUI : MonoBehaviour
{
    public Image mic;

    public void Show(bool enabled)
    {
        mic.enabled = enabled;
    }
}
