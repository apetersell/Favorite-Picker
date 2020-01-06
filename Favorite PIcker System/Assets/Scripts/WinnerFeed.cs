using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinnerFeed : MonoBehaviour
{
    public float lifeTime;
    public Text[] slots;
    private float timer;

    void Update()
    {
        //timer += Time.deltaTime;
        //if (timer > lifeTime)
        //{
        //    for (int i = 0; i < slots.Length; i++)
        //    {
        //        slots[i].text = "";
        //    }
        //}
    }

    public void UpdateFeed(string sent)
    {
        slots[3].text = slots[2].text;
        slots[2].text = slots[1].text;
        slots[1].text = slots[0].text;
        slots[0].text = sent;
        //timer = 0;
    }
}
