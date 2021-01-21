using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] checkMateClips;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomCheckMateSound()
    {
        int i = Random.Range(0, checkMateClips.Length);
        audioSource.clip = checkMateClips[i];
        audioSource.Play();
    }
}
