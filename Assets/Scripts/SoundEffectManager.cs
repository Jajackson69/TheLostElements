using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;      // for footsteps, spells, UI etc.
    [SerializeField] private AudioSource musicSource;    // for BGM only

    [Header("Sound Library")]
    [SerializeField] private SoundEffectLibrary soundEffectLibrary;

    [Header("Background Music")]
    [SerializeField] private AudioClip backgroundMusic;  // drag your Moody Dungeon here

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // auto-assign audio sources if not set
        if (sfxSource == null || musicSource == null)
        {
            var sources = GetComponents<AudioSource>();
            if (sources.Length == 0)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                musicSource = gameObject.AddComponent<AudioSource>();
            }
            else if (sources.Length == 1)
            {
                sfxSource = sources[0];
                musicSource = gameObject.AddComponent<AudioSource>();
            }
            else
            {
                sfxSource = sources[0];
                musicSource = sources[1];
            }
        }

        // music settings
        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }

    private void Start()
    {
        // just start the one background track and let it loop forever
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    // ------------ SFX ------------

    // pitch is optional, defaults to 1
    public static void Play(string soundName, float pitch = 1f)
    {
        if (Instance == null || Instance.soundEffectLibrary == null || Instance.sfxSource == null)
            return;

        AudioClip clip = Instance.soundEffectLibrary.GetRandomClip(soundName);
        if (clip == null) return;

        Instance.sfxSource.pitch = pitch;
        Instance.sfxSource.PlayOneShot(clip);
    }

    public static void SetSfxVolume(float volume)
    {
        if (Instance == null || Instance.sfxSource == null) return;
        Instance.sfxSource.volume = volume;
    }

    public static void SetMusicVolume(float volume)
    {
        if (Instance == null || Instance.musicSource == null) return;
        Instance.musicSource.volume = volume;
    }
}
