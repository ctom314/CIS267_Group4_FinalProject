using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Silo : MonoBehaviour
{
    public struct siloData
    {
        public string name;
        public int amount;
        public Sprite cropImage;
        public string cropType;
    }

    private CropObject[] crops;
    private List<siloData> siloDataList = new List<siloData>();
    private void Start()
    {
        crops = Resources.LoadAll<CropObject>("Crops");

        foreach (var c in crops)
        {
            siloData sd = new siloData();
            sd.amount = 0;
            sd.name = c.name;
            sd.cropImage = c.cropIcon;
            sd.cropType = c.name; // Set cropType based on the crop's name
            siloDataList.Add(sd);
        }
    }

    public void addCrops(int cropGained, string cropName)
    {
        int id = siloDataList.FindIndex(sd => sd.name == cropName);
        if (id != -1)
        {
            siloData sd = siloDataList[id];
            sd.amount += cropGained;
            siloDataList[id] = sd;
        }
    }

    public void RemoveCrops(int cropGained, string cropName)
    {
        int id = siloDataList.FindIndex(sd => sd.name == cropName);
        if (id != -1)
        {
            siloData sd = siloDataList[id];
            sd.amount = Mathf.Max(0, sd.amount - cropGained); // Prevent negative amounts
            siloDataList[id] = sd;
        }
    }

    public bool hasEnough(int cropsNeeded, string cropName)
    {
        bool allGood = false;
        if(siloDataList.Find(sd => sd.name == cropName).amount >= cropsNeeded)
        {
            allGood = true;
        }

        return allGood;
    }

    public List<siloData> getData()
    {
        return siloDataList;
    }
}
