using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SiloCanvas : MonoBehaviour
{

    public GameObject texbox;
    public Transform canvas;
    private void Start() 
    {
        
    }



    public void updateCropInfo(List<Silo.siloData> data)
    {
        foreach(Transform child in canvas)
        {
            Destroy(child.gameObject);
        }
        foreach (Silo.siloData sd in data)
        {
            GameObject newUiElement = Instantiate(texbox, canvas);
            texbox.GetComponent<TextMeshProUGUI>().text = sd.name + ": " + sd.amount;

        }
    }
}
