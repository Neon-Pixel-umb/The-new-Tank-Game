using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance; // Singleton instance

    [Header("Sound Effects")]
    public AudioClip shootSound;
    public AudioClip[] activeItemSounds; // Array for active item sounds
    public AudioClip NukeSound;
    public AudioClip FlashBang;
    public AudioClip Good;
    public AudioClip takeDamageSound;
    public AudioClip Coin;
    public AudioClip Register;

    [Header("Music")]
    public AudioClip[] musicTracks; // Array of music tracks for background music
    public AudioSource sfxSource;
    public AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps the sound manager across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ensure AudioSource components are attached
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = false; // Ensure music tracks loop through the playlist
            musicSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        PlayRandomMusicTrack();
    }

    private void PlayRandomMusicTrack()
    {
        if (musicTracks.Length > 0)
        {
            AudioClip selectedTrack = musicTracks[Random.Range(0, musicTracks.Length)];
            musicSource.clip = selectedTrack;
            musicSource.Play();
            StartCoroutine(WaitForTrackToEnd());
        }
    }

    private IEnumerator WaitForTrackToEnd()
    {
        while (musicSource.isPlaying)
        {
            yield return null;
        }
        PlayRandomMusicTrack(); // Plays a random track when the current track ends
    }

    // Plays a specified sound effect
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Called when shooting
    public void PlayShootSound()
    {
        PlaySound(shootSound);
    }

    // Called when an active item is used, with item index to determine sound
    public void PlayActiveItemSound(int itemIndex)
    {
        if (itemIndex >= 0 && itemIndex < activeItemSounds.Length)
        {
            PlaySound(activeItemSounds[itemIndex]);
        }
    }

    // Called when taking damage
    public void PlayTakeDamageSound()
    {
        PlaySound(takeDamageSound);
    }

    public void PlayNuke()
    {
        PlaySound(NukeSound);
    }

    public void PlayFlash()
    {
        PlaySound(FlashBang);
    }

    public void PlayGood()
    {
        PlaySound(Good);
    }

    public void PlayCoin()
    {
        PlaySound(Coin);
    }
    public void PlayRegister()
    {
        PlaySound(Register);
    }
}
