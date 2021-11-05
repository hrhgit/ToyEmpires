using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_MultipleChartsManager : MonoBehaviour
{
    public List<GameObject> chartsList;

    public void OpenChart(GameObject chart)
    {
        foreach (GameObject c in chartsList)
        {
            c.SetActive(false);
        }
        chart.SetActive(true);
    }
}
