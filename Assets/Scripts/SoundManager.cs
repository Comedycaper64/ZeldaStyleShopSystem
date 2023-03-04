using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;

    private float masterVolume;
    private float musicVolume;
    private float sfxVolume;

    private void Awake()
    {
        if (Instance != null)
        {
            //Debug.LogError("There's more than one SoundManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        masterVolume = 1f;
        musicVolume = 0.25f;
        sfxVolume = 1f;
    }

    public float GetMusicVolume()
    {
        return masterVolume * musicVolume;
    }

    public float GetSoundEffectVolume()
    {
        return masterVolume * sfxVolume;
    }
}
