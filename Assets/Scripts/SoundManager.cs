using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public AudioSource efxSource;                   //Drag a reference to the audio source which will play the sound effects.
    public AudioSource musicSource;                 //Drag a reference to the audio source which will play the music.
    public static SoundManager instance = null;     //Allows other scripts to call functions from SoundManager.				
    public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
    public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.

    public AudioClip MoveSound1;
    public AudioClip MoveSound2;
    public AudioClip EatSound1;
    public AudioClip EatSound2;
    public AudioClip DrinkSound1;
    public AudioClip DrinkSound2;
    public AudioClip GameOverSound;
    public AudioClip AttackSound1;
    public AudioClip AttackSound2;
    public AudioClip ChopSound1;
    public AudioClip ChopSound2;
    public AudioClip Dyingsound; 

    public AudioClip[] clips;

    void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);

        clips = new AudioClip[] {
            MoveSound1,
            MoveSound2,
            EatSound1,
            EatSound2,
            DrinkSound1,
            DrinkSound2,
            GameOverSound,
            AttackSound1,
            AttackSound2,
            ChopSound1,
            ChopSound2,
            Dyingsound,
        };
    }

    public void PlaySingle(int index)
    {
        PlaySingle(clips[index]);
    }

    public void RandomizeSfx(params int[] indices)
    {
        AudioClip[] array = new AudioClip[indices.Length];

        for (int i = 0; i < indices.Length; i++)
            array[i] = clips[indices[i]];

        RandomizeSfx(array);
    }

    public void PlaySingle(AudioClip clip)
    {
        efxSource.PlayOneShot(clip);
    }


    //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    public void RandomizeSfx(params AudioClip[] clips)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        efxSource.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        efxSource.clip = clips[randomIndex];

        //Play the clip.
        efxSource.Play();
    }
}

