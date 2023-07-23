using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Footsteps : MonoBehaviour
{
    public AudioClip[] stepSounds;
	private AudioSource source;

	private void Start()
	{
		source = gameObject.GetComponent<AudioSource>();
	}

	public void Step()
    {
		source.clip = stepSounds[Random.Range(0, stepSounds.Length)];
		source.Play();
    }
}
