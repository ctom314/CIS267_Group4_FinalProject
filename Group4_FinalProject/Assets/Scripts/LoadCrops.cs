using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadCrops : MonoBehaviour
{
    public Transform canvas;
    public GameObject cropsUIElement;
    public fieldData fd;
    private PersistentData pd;

    private void Start() 
    {
        CropObject[] crops = Resources.LoadAll<CropObject>("Crops");

        foreach(Transform child in canvas)
        {
            Destroy(child.gameObject);
        }

        foreach(CropObject c in crops)
        {
            GameObject newUiElement = Instantiate(cropsUIElement, canvas);

            Image cropSprite = newUiElement.GetComponent<Image>();
            if(cropSprite != null)
            {
                cropSprite.sprite = c.cropIcon;
            }

            Button uiButton = newUiElement.GetComponent<Button>();
            if(uiButton != null)
            {
                CropObject thisItem = c;
                uiButton.onClick.AddListener(() => plantCrop(thisItem));
            }
        }
    }

    private void Update() {
        if(gameObject.activeInHierarchy)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
                Time.timeScale = 1;

            }
        }
    }

    private void plantCrop(CropObject c)
    {
        gameObject.SetActive(false);
        fd.cropInfo = c;
        fd.dayPlanted = PersistentData.instance.getDay();
        fd.isPlanted = true;
        Time.timeScale = 1;
    }
}
