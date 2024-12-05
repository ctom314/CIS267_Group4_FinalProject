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
    }

    private CropObject[] crops;
    private List<siloData> siloDataList = new List<siloData>();
    private void Start() 
    {
        crops = Resources.LoadAll<CropObject>("Crops");

        foreach(var c in crops)
        {
            siloData sd = new siloData();
            sd.amount = 0;
            sd.name = c.name;
            sd.cropImage = c.cropIcon;
            siloDataList.Add(sd);
        }
    }

    public void addCrops(int cropGained, string cropName)
    {
        siloData sd = new siloData();
        sd.name = cropName;
        int id = siloDataList.FindIndex(sd => sd.name == cropName);
        sd.amount = siloDataList[id].amount;
        sd.amount += cropGained;
        siloDataList[id] = sd;
    }

    public void RemoveCrops(int cropGained, string cropName)
    {
        siloData sd = new siloData();
        sd.name = cropName;
        int id = siloDataList.FindIndex(sd => sd.name == cropName);
        sd.amount = siloDataList[id].amount;
        sd.amount -= cropGained;
        siloDataList[id] = sd;
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
