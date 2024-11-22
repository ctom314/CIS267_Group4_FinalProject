using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class HealthManager : MonoBehaviour
{
    // Hearts Display
    public Image[] livesDisplay;

    // Hearts Images
    public Sprite heart_full;
    public Sprite heart_empty;

    // Immunity Frames
    public float immunityDuration = 1.0f;
    private bool dealDamage = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void updateLivesDisplay()
    {
        // Set lives images based on num lives
        for (int i = 0; i < livesDisplay.Length; i++)
        {
            if (i < PersistentData.instance.getPlayerHP())
            {
                livesDisplay[i].sprite = heart_full;
            }
            else
            {
                livesDisplay[i].sprite = heart_empty;
            }
        }
    }

    public bool getDealDamage()
    {
        return dealDamage;
    }

    public IEnumerator immunityFrames()
    {
        dealDamage = false;
        yield return new WaitForSeconds(immunityDuration);
        dealDamage = true;
    }
}
