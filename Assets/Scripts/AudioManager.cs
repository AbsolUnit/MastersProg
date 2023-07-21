using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;
    void Start()
    {
		if (PlayerPrefs.HasKey("MasterVol")) { mixer.SetFloat("MasterVol", PlayerPrefs.GetFloat("MasterVol")); }
		if (PlayerPrefs.HasKey("MusicVol")) { mixer.SetFloat("MusicVol", PlayerPrefs.GetFloat("MusicVol")); }
		if (PlayerPrefs.HasKey("VoicesVol")) { mixer.SetFloat("VoicesVol", PlayerPrefs.GetFloat("VoicesVol")); }
	}
}
