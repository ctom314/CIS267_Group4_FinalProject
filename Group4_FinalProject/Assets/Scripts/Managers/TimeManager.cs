using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public int dayLengthMinutes = 7;
    public int nightLengthMinutes = 7;

    // Time UI
    public Slider timeSlider;
    public Image timeStart;
    public Image timeEnd;
    public TextMeshProUGUI dayCount;

    // Slider Colors
    public Color dayColor;
    public Color nightColor;

    // Time sprites
    public Sprite sun;
    public Sprite moon;

    // Time lengths in seconds
    private int dayLength;
    private int nightLength;

    // Time vals
    private int curTime = 0;
    private bool isDay = true;

    private PauseManager pm;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PauseManager>();

        // Convert minutes to seconds
        dayLength = dayLengthMinutes * 60;
        nightLength = nightLengthMinutes * 60;

        dayCount.text = "Day " + PersistentData.instance.getDay();

        // Setup slider
        timeSlider.maxValue = dayLength;
        timeSlider.fillRect.GetComponent<Image>().color = dayColor;

        StartCoroutine(incrementTime());
    }
    private void updateTime()
    {

        // Check if day/night has ended
        if (isDay && dayEnded())
        {
            isDay = false;
            curTime = 0;
            timeSlider.maxValue = nightLength;
            updateSlider();
            swapTimeSprites();
            swapSliderColors();
        }
        else if (!isDay && nightEnded())
        {
            isDay = true;
            curTime = 0;
            timeSlider.maxValue = dayLength;
            updateSlider();
            swapTimeSprites();
            swapSliderColors();

            // Increment to next day
            incrementDay();
        }

        updateSlider();
    }

    private void swapTimeSprites()
    {
        // Switch the sprites of the time images based on time of day
        if (isDay)
        {
            timeStart.sprite = sun;
            timeEnd.sprite = moon;
        }
        else if (!isDay)
        {
            timeStart.sprite = moon;
            timeEnd.sprite = sun;
        }
    }

    private void swapSliderColors()
    {
        // Change the color of the slider fill based on time of day

        if (isDay)
        {
            timeSlider.fillRect.GetComponent<Image>().color = dayColor;
        }
        else if (!isDay)
        {
            timeSlider.fillRect.GetComponent<Image>().color = nightColor;
        }
    }

    private void updateSlider()
    {
        // Update slider value
        timeSlider.value = curTime;
    }

    private bool dayEnded()
    {
        return curTime >= dayLength;
    }

    private bool nightEnded()
    {
        return curTime >= nightLength;
    }

    private void incrementDay()
    {
        PersistentData.instance.incrementDay();
        dayCount.text = "Day " + PersistentData.instance.getDay();
    }

    private IEnumerator incrementTime()
    {
        while (!pm.isPaused)
        {
            // Wait 1 second
            yield return new WaitForSeconds(1);

            // Increment time
            curTime++;

            updateTime();
        }
    }
}
