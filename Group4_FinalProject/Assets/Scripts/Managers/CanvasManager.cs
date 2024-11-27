using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject CropSelector;
    public GameObject siloCanvas;


    private void Start() {
    }

    public void startCropSelectorCanvas(fieldData fd)
    {
        Time.timeScale = 0;
        CropSelector.SetActive(true);
        CropSelector.GetComponent<LoadCrops>().fd = fd;
    }

    public void startSiloCanvas(List<Silo.siloData> data)
    {
        siloCanvas.SetActive(true);
        siloCanvas.GetComponent<SiloCanvas>().updateCropInfo(data);
    }
}
