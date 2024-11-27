using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectData : MonoBehaviour
{
    //1 - Field
    //2 - Silo
    public int objectId;
    Silo siloData;
    private CanvasManager cm;

    private void Start() {
        cm  = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CanvasManager>();
        siloData = GameObject.FindGameObjectWithTag("Silo").GetComponent<Silo>();
    }

    public void runLogic()
    {
        if(objectId == 1)
        {
            getFieldData();
        }
        else if(objectId == 2)
        {
            openSiloCanvas();
        }
    }

    private void getFieldData()
    {
        CropObject selectedCrop;
        fieldData fd;
        fd = gameObject.GetComponent<fieldData>();
        selectedCrop = gameObject.GetComponent<fieldData>().cropInfo;
        if(fd != null && selectedCrop != null)
        {
            if(fd.isPlanted && PersistentData.instance.getDay() > selectedCrop.growTime + fd.dayPlanted)
            {
                //harvest
                siloData.addCrops(selectedCrop.harvestAmount, selectedCrop.name);
                fd.isPlanted = false;
                fd.cropInfo = null;
                fd.dayPlanted = -1;

            }
            else if(!fd.isPlanted)
            {
                //Open canvas to plant crops
                cm.startCropSelectorCanvas(fd);
            }

        }
        else if(fd != null && selectedCrop == null)
        {
            cm.startCropSelectorCanvas(fd);
        }
    }

    private void openSiloCanvas()
    {
        siloData = gameObject.GetComponent<Silo>();

        if(siloData != null)
        {
            cm.startSiloCanvas(siloData.getData());

        }
    
    }
}
