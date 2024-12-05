using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SiloCanvas : MonoBehaviour
{

    public GameObject texbox;
    public Transform canvas;
    public Button exitBtn;
    public GameObject uiPrefab;
    private void Start() 
    {
        exitBtn.onClick.AddListener(closeCanvas);
    }



    public void updateCropInfo(List<Silo.siloData> data)
    {
        foreach(Transform child in canvas)
        {
            Destroy(child.gameObject);
        }
        foreach (Silo.siloData sd in data)
        {
            GameObject newUiElement = Instantiate(uiPrefab);
            newUiElement.transform.SetParent(canvas, false);
            newUiElement.GetComponentInChildren<Image>().sprite = sd.cropImage;
            newUiElement.GetComponentInChildren<TextMeshProUGUI>().text = sd.amount.ToString();

        }
    }

    private void closeCanvas()
    {
        gameObject.SetActive(false);
    }
}
