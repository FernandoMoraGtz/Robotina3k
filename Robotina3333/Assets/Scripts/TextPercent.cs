using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPercent : MonoBehaviour
{
    Text percentageText;
    // Start is called before the first frame update
    void Start()
    {
        percentageText = GetComponent<Text>();
    }

    public void updateText(int percent)
    {
        percentageText.text = percent + "%";
    }

    public void updateMapText(int percent)
    {
        percentageText.text = "Battery: " + percent + "%";
    }
}
