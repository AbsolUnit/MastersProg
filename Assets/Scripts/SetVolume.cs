using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    [SerializeField] private string sliderName;

    public void SetVal(float val)
    {
        mixer.SetFloat(sliderName + "Vol", Mathf.Log10(val) * 20);
        PlayerPrefs.SetFloat(sliderName + "Vol", Mathf.Log10(val) * 20);
    }

	private void Start()
	{
		if (PlayerPrefs.HasKey(sliderName + "Vol"))
        {
            gameObject.GetComponent<Slider>().value = Mathf.Pow(10, PlayerPrefs.GetFloat(sliderName + "Vol")/20);
		}
	}
}
