using Cinemachine.PostFX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public float dayLengthMinutes = 7;
    public float nightLengthMinutes = 7;
    public bool isDay = true;

    // Time UI
    public Slider timeSlider;
    public Image timeStart;
    public Image timeEnd;
    public TextMeshProUGUI dayCount;
    public GameObject sliderObj;

    // Season UI
    public TextMeshProUGUI seasonText;

    // Slider Colors
    public Color dayColor;
    public Color nightColor;

    // Time sprites
    public Sprite sun;
    public Sprite moon;

    // Post Processing
    public CinemachinePostProcessing postProcessing;

    // Music Manager
    public MusicManager musicManager;

    // Time lengths in seconds
    private float dayLength;
    private float nightLength;

    // Time vals
    private int curTime = 0;

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
        timeSlider.interactable = false;

        // Setup time
        if (isDay)
        {
            // Set time to day
            timeSlider.maxValue = dayLength;
            swapTimeSprites();
            swapSliderColors();
            postProcessing.enabled = false;

            //Start day music
            musicManager.PlayDayTrack();
        }
        else
        {
            // Set time to night
            timeSlider.maxValue = nightLength;
            swapTimeSprites();
            swapSliderColors();
            postProcessing.enabled = true;

            //Start night music
            musicManager.PlayNightTrack();
        }

        // Setup season text
        updateSeasonDisplay();

        StartCoroutine(incrementTime());
    }
    private void updateTime()
    {

        // Check if day/night has ended
        if (isDay && dayEnded())
        {
            // Switch to night
            isDay = false;
            curTime = 0;
            timeSlider.maxValue = nightLength;
            updateSlider();
            swapTimeSprites();
            swapSliderColors();

            //Switch to night music
            musicManager.PlayNightTrack();

            // Enable PP
            postProcessing.enabled = true;
        }
        else if (!isDay && nightEnded())
        {
            // Switch to day
            isDay = true;
            curTime = 0;
            timeSlider.maxValue = dayLength;
            updateSlider();
            swapTimeSprites();
            swapSliderColors();

            //Switch to day music
            musicManager.PlayDayTrack();

            // Disable PP
            postProcessing.enabled = false;

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

        // If season has ended, increment season
        if (PersistentData.instance.getSeasonDay() >= PersistentData.instance.getSeasonLength(PersistentData.instance.getSeasonId()))
        {

            // Increment season
            PersistentData.instance.incrementSeason();

            // Update season text
            updateSeasonDisplay();
        }
        else
        {
            // No season change, increment days this season
            PersistentData.instance.incrementSeasonDay();
        }
    }

    // Update season text
    private void updateSeasonDisplay()
    {
        seasonText.SetText(PersistentData.instance.getSeasonName());
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

    public void SkipToNight()
    {
        // Switch to night
        isDay = false;
        curTime = 0;
        timeSlider.maxValue = nightLength;
        updateSlider();
        swapTimeSprites();
        swapSliderColors();

        // Switch music to night track
        musicManager.PlayNightTrack();

        // Enable post-processing (if applicable)
        postProcessing.enabled = true;
    }
}
