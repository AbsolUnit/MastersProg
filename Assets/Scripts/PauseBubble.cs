using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseBubble : MonoBehaviour
{

    [SerializeField]
    private GameObject waitable;

    [SerializeField]
    private SpeechBubbleGen[] bubbles;

	// Start is called before the first frame update
	void Start()
    {
        if (waitable.activeSelf == true)
        {
            foreach (SpeechBubbleGen bubb in bubbles)
            {
                bubb.enabled = false;
            }
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (waitable.activeSelf == true)
		{
			foreach (SpeechBubbleGen bubb in bubbles)
			{
				bubb.enabled = false;
			}
		}
        else
        {
			foreach (SpeechBubbleGen bubb in bubbles)
			{
				bubb.enabled = true;
			}
		}
	}
}
