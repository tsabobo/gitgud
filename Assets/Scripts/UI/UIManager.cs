using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public GameObject selectWindow;

    public void OpenSelectWindow(bool open)
    {
        selectWindow.SetActive(open);
    }
}
