using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Crops", menuName = "Crops/Crop objects")]
public class CropObject : ScriptableObject
{
    public Sprite cropIcon;
    public string cropName;
    public float growTime;
    public int harvestAmount;
}
