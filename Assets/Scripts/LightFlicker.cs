using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
	[Tooltip("Minimum random light intensity")]
	public float minIntensity = 0f;
	[Tooltip("Maximum random light intensity")]
	public float maxIntensity = 1f;
	[Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
	[Range(1, 50)]
	public int smoothing = 5;

	private Light2D twoDlight;

	Queue<float> smoothQueue;
	float lastSum = 0;

	public void Reset()
	{
		smoothQueue.Clear();
		lastSum = 0;
	}

	void Start()
	{
		smoothQueue = new Queue<float>(smoothing);
		// External or internal light?
		twoDlight = GetComponent<Light2D>();
	}

	void Update()
	{
		// pop off an item if too big
		while (smoothQueue.Count >= smoothing)
		{
			lastSum -= smoothQueue.Dequeue();
		}

		// Generate random new item, calculate new average
		float newVal = Random.Range(minIntensity, maxIntensity);
		smoothQueue.Enqueue(newVal);
		lastSum += newVal;

		// Calculate new smoothed average
		twoDlight.intensity = lastSum / (float)smoothQueue.Count;
	}

}
