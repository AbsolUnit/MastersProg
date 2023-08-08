using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseBubble : MonoBehaviour
{

    [SerializeField]
    private GameObject waitable;

    [SerializeField]
    private GameObject[] bubbles;

	// Start is called before the first frame update
	void Start()
    {
        if (waitable.activeSelf == true)
        {
            foreach (GameObject bubb in bubbles)
            {
                bubb.SetActive(false);
            }
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (waitable.activeSelf == true)
		{
			foreach (GameObject bubb in bubbles)
			{
				bubb.SetActive(false);
			}
		}
        else
        {
			foreach (GameObject bubb in bubbles)
			{
				bubb.SetActive(true);
			}
		}
	}
}
