using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [Header("Music Tracks")]

    public AudioClip dayMusic;
    public AudioClip nightMusic;

    [Header("Volume Controls")]

    [Range(0f, 1f)] public float dayMusicVolume = 0.2f;
    [Range(0f, 1f)] public float nightMusicVolume = 0.2f;
    [Range(0.1f, 5f)] public float crossfadeDuration = 3f;

    private AudioSource audioSource1;
    private AudioSource audioSource2;
    private bool isFading = false;

    void Awake()
    {
        var existingInstances = FindObjectsOfType<MusicManager>();
        if (existingInstances.Length > 1)
        {
            Debug.Log("Duplicate MusicManager detected, destroying...");
            Destroy(gameObject);
            return;
        }

        Debug.Log("MusicManager initialized and set to DontDestroyOnLoad");
        DontDestroyOnLoad(gameObject);

        InitializeAudioSources();

        // Subscribe to scene changes only for the first instance
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Trigger ResetMusic on initial scene
        if (SceneManager.GetActiveScene().isLoaded)
        {
            Debug.Log("Initial scene already loaded. Calling ResetMusic.");
            ResetMusic(SceneManager.GetActiveScene().name);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}, LoadSceneMode: {mode}");

        // Avoid unnecessary duplicate handling
        if (scene.name == "MainMenu")
        {
            StopMusic();
            Debug.Log("Music stopped for MainMenu.");
            return;
        }

        ResetMusic(scene.name);
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene changes to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PlayDayTrack()
    {
        InitializeAudioSources(); // Ensure audio sources are initialized

        if (dayMusic == null)
        {
            Debug.LogError("Day music is not assigned!");
            return;
        }

        if (!isFading && (audioSource1.clip != dayMusic && audioSource2.clip != dayMusic))
        {
            Debug.Log("Starting day track...");
            StartCoroutine(CrossfadeTracks(dayMusic, dayMusicVolume));
        }
    }

    public void PlayNightTrack()
    {
        InitializeAudioSources(); // Ensure audio sources are initialized

        if (nightMusic == null)
        {
            Debug.LogError("Night music is not assigned!");
            return;
        }

        if (!isFading && (audioSource1.clip != nightMusic && audioSource2.clip != nightMusic))
        {
            StartCoroutine(CrossfadeTracks(nightMusic, nightMusicVolume));
        }
    }

    public void StopMusic()
    {
        audioSource1.Stop();
        audioSource2.Stop();
    }

    private IEnumerator CrossfadeTracks(AudioClip newTrack, float newVolume)
    {
        if (audioSource1 == null || audioSource2 == null)
        {
            Debug.LogWarning("Audio sources are null. Reinitializing.");
            InitializeAudioSources();
        }

        isFading = true;

        AudioSource activeSource = audioSource1.isPlaying ? audioSource1 : audioSource2;
        AudioSource idleSource = activeSource == audioSource1 ? audioSource2 : audioSource1;

        idleSource.clip = newTrack;
        idleSource.volume = 0f;
        idleSource.Play();

        float elapsedTime = 0f;
        while (elapsedTime < crossfadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / crossfadeDuration;

            activeSource.volume = Mathf.Lerp(activeSource.volume, 0f, t);
            idleSource.volume = Mathf.Lerp(0f, newVolume, t);

            yield return null;
        }

        activeSource.Stop();
        activeSource.volume = 0f;
        idleSource.volume = newVolume;

        isFading = false;
    }

    public void PauseMusic()
    {
        if (audioSource1.isPlaying)
        {
            audioSource1.Pause();
        }
        if (audioSource2.isPlaying)
        {
            audioSource2.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (!audioSource1.isPlaying && audioSource1.clip != null)
        {
            audioSource1.Play();
        }
        if (!audioSource2.isPlaying && audioSource2.clip != null)
        {
            audioSource2.Play();
        }
    }

    public void FadeOutMusic(float duration)
    {
        StartCoroutine(FadeOut(duration));
    }

    public void FadeInMusic(AudioClip newTrack, float newVolume, float duration)
    {
        StartCoroutine(FadeIn(newTrack, newVolume, duration));
    }

    private IEnumerator FadeOut(float duration)
    {
        AudioSource activeSource = audioSource1.isPlaying ? audioSource1 : audioSource2;

        float startVolume = activeSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            activeSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        activeSource.volume = 0f;
        activeSource.Stop();
    }

    private IEnumerator FadeIn(AudioClip newTrack, float newVolume, float duration)
    {
        AudioSource idleSource = audioSource1.isPlaying ? audioSource2 : audioSource1;

        idleSource.clip = newTrack;
        idleSource.volume = 0f;
        idleSource.Play();

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            idleSource.volume = Mathf.Lerp(0f, newVolume, elapsed / duration);
            yield return null;
        }

        idleSource.volume = newVolume;
    }

    public void ResetMusic(string currentScene)
    {
        Debug.Log($"ResetMusic called for scene: {currentScene}");

        InitializeAudioSources(); // Reinitialize audio sources if needed

        if (currentScene == "SpringMap")
        {
            Debug.Log("Playing day track for SpringMap");
            PlayDayTrack();
        }
        else if (currentScene == "NightScene")
        {
            Debug.Log("Playing night track for NightScene");
            PlayNightTrack();
        }
        else if (currentScene == "MainMenu")
        {
            Debug.Log("Stopping music for MainMenu");
            StopMusic(); // No music in main menu
        }
        else
        {
            Debug.LogWarning($"No matching music found for scene: {currentScene}");
        }
    }

    public void InitializeAudioSources()
    {
        if (audioSource1 == null)
        {
            audioSource1 = gameObject.AddComponent<AudioSource>();
            audioSource1.loop = true;
            audioSource1.playOnAwake = false;
            Debug.Log("Recreated audioSource1");
        }

        if (audioSource2 == null)
        {
            audioSource2 = gameObject.AddComponent<AudioSource>();
            audioSource2.loop = true;
            audioSource2.playOnAwake = false;
            Debug.Log("Recreated audioSource2");
        }
    }

    public void QuitToMainMenu()
    {
        Debug.Log("Quitting to Main Menu...");
        Time.timeScale = 1; // Reset time scale in case of pause
        PlayerMovement.canMove = true; // Ensure player movement resets

        SceneManager.LoadScene("MainMenu");

        // Ensure MusicManager resets properly
        if (FindObjectOfType<MusicManager>() != null)
        {
            FindObjectOfType<MusicManager>().ResetMusic("MainMenu");
        }
    }
}