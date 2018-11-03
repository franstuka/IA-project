using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSound : MonoBehaviour {

    private AudioSource audioSource;
    public AudioClip sound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            audioSource.clip = sound;
            audioSource.Play();
        }
    }
}
