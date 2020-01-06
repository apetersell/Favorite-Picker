using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementButton : MonoBehaviour
{
    public Manager manager;
    public Text textComponent;
    public Image BG;
    public int index;

    public void makeSelection()
    {
        bool chosen = manager.selections.Contains(index);
        if (chosen)
        {
            manager.selections.Remove(index);
            BG.color = Color.white;
        }
        else
        {
            manager.selections.Add(index);
            BG.color = Color.green;
        }
    }
}
