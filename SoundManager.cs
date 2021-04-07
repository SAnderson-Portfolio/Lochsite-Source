using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource efxSource;
    private AudioSource musicSource;
    public static SoundManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

       
    }
    // Start is called before the first frame update
    void Start()
    {
        efxSource = gameObject.AddComponent<AudioSource>() as AudioSource;
        musicSource = gameObject.AddComponent<AudioSource>() as AudioSource;

        efxSource.volume = PlayerPrefs.GetFloat("EFX Volume", 1);
        musicSource.volume = PlayerPrefs.GetFloat("Music Volume", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("Music Volume", musicSource.volume);
        PlayerPrefs.SetFloat("EFX Volume", efxSource.volume);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.mute)
            musicSource.mute = false;
        //Set the sound to parameter
        musicSource.clip = clip;
        if (!musicSource.isPlaying)
        {
            //Play the sound
            musicSource.Play();
            musicSource.mute = false;
        }
    }//Plays music clips

    public void StopMusic()
    {
        if(musicSource.isPlaying)
        {
            musicSource.mute = true;
           // musicSource.Stop();

        }
      
    }//Stops any currently playing music.
    public void PlayClipOneShot(AudioClip clip)
    {
        //Play the sound
        efxSource.PlayOneShot(clip);
    }//Plays a sound as a one shot allowing multiple sounds to be played at the same time.

    public void PlayClip(AudioClip clip)
    {
        //Set the sound to parameter
        efxSource.clip = clip;
        if(!efxSource.isPlaying)
        {
            //Play the sound
            efxSource.Play();
        }
        
    }//Plays a sound and doesn't let another play until it's finished.

    public void StopClip(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Stop();
    }//Stops the currently playing sound.

    public void RandomClip(params AudioClip[] clips)
    {
        if(!efxSource.isPlaying)
        {
            int randomIndex = Random.Range(0, clips.Length);

            efxSource.clip = clips[randomIndex];

            efxSource.Play();
        }
    }//Plays a random sound from a list of sounds that have been passed in.

    public void RandomClipOneShot(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        
        efxSource.PlayOneShot(clips[randomIndex]);
    }//Plays a random sound from a list of sounds that have been passed in as a oneshot.

    public void SetMusicVolume(float newValue)
    {
        musicSource.volume = newValue;
    }//Sets the volume of music

    public void SetEFXVolume(float newValue)
    {
        efxSource.volume = newValue;
    }//Sets the volume of sound effects

    public float GetMusicVolume()
    {
            return musicSource.volume;
    }//Returns the current music volume

    public float GetEFXVolume()
    {
            return efxSource.volume;
    }//Returns the current effect volume.
}
