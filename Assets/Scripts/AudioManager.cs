using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;
        
        [HideInInspector]
        public AudioSource source;
    }
    
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Settings")]
    public Sound[] sounds;
    public Sound[] music;
    
    [Header("Volume Controls")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 0.75f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.75f;
    
    private Sound currentMusic;
    private Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();
    private Dictionary<string, Sound> musicDictionary = new Dictionary<string, Sound>();
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Initialize all sounds
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume * sfxVolume * masterVolume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            
            soundDictionary[s.name] = s;
        }
        
        // Initialize all music tracks
        foreach (Sound m in music)
        {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.clip;
            m.source.volume = m.volume * musicVolume * masterVolume;
            m.source.pitch = m.pitch;
            m.source.loop = true; // Music should always loop
            
            musicDictionary[m.name] = m;
        }
        
        // Load saved volume settings
        LoadVolumeSettings();
    }
    
    private void Start()
    {
        // Play default music if specified
        if (music.Length > 0)
        {
            PlayMusic(music[0].name);
        }
    }
    
    public void PlaySound(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound s))
        {
            s.source.Play();
        }
        else
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
    }
    
    public void PlayMusic(string name)
    {
        // Stop current music if playing
        if (currentMusic != null && currentMusic.source.isPlaying)
        {
            currentMusic.source.Stop();
        }
        
        if (musicDictionary.TryGetValue(name, out Sound m))
        {
            m.source.Play();
            currentMusic = m;
        }
        else
        {
            Debug.LogWarning("Music: " + name + " not found!");
        }
    }
    
    public void StopMusic()
    {
        if (currentMusic != null && currentMusic.source.isPlaying)
        {
            currentMusic.source.Stop();
        }
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        UpdateAllVolumes();
        SaveVolumeSettings();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        UpdateMusicVolumes();
        SaveVolumeSettings();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        UpdateSFXVolumes();
        SaveVolumeSettings();
    }
    
    private void UpdateAllVolumes()
    {
        UpdateMusicVolumes();
        UpdateSFXVolumes();
    }
    
    private void UpdateMusicVolumes()
    {
        foreach (Sound m in music)
        {
            if (m.source != null)
            {
                m.source.volume = m.volume * musicVolume * masterVolume;
            }
        }
    }
    
    private void UpdateSFXVolumes()
    {
        foreach (Sound s in sounds)
        {
            if (s.source != null)
            {
                s.source.volume = s.volume * sfxVolume * masterVolume;
            }
        }
    }
    
    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }
    
    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        
        UpdateAllVolumes();
    }
}
