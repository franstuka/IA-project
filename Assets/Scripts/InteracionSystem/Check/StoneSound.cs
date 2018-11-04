using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSound : MonoBehaviour {

    private AudioSource audioSource;
    public AudioClip sound;
    private bool ballCollided = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Scenario" && !ballCollided)
        {
            audioSource.clip = sound;
            audioSource.Play();
            ballCollided = true;
            Skeleton.soundHeard = true;
        }
    }
}
