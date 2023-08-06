using ClearSky;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePlayer : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
	[SerializeField]
	private GameObject bubb;

    // Update is called once per frame
    void Update()
    {
        if (bubb.activeSelf && bubb.GetComponent<SpeechBubbleGen>().bubbleOn)
        {
            StartCoroutine(WaitPause());
        }
        else
        {
			player.GetComponent<PlayerController>().controlOn = true;
		}
    }

    private IEnumerator WaitPause()
    {
        yield return new WaitForSeconds(0.1f);
		player.GetComponent<PlayerController>().controlOn = false;
	}
}
