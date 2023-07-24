using System;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEditor;

[RequireComponent(typeof(TextMeshPro))]
[RequireComponent(typeof(AudioSource))]
[ExecuteInEditMode]
public class SpeechBubbleGen : MonoBehaviour
{
    public Bubble bubble;
	public int clipCount;
	public TriggerType trigger = TriggerType.Awake;

    public TextAsset meta;

	public int indxColl;
	public int indxKey;
	public GameObject colliderParent;
	public Collider2D available2DCollider;
	public Collider available3DCollider;
	public bool customButtonInput;
	public string progressButton;
	public AudioMixerGroup audioMixer;
	public AudioClip[] clips;
	public GameObject visuals;

	private AudioClip currentClip;
	private string currentText;
	private int currentBubbleIndx;
	private bool bubbleOn;
    private TextMeshPro textBox;
    private AudioSource audioSource;

	public void Generate()
    {
		audioSource = GetComponent<AudioSource>();
		audioSource.outputAudioMixerGroup = audioMixer;
		textBox = GetComponent<TextMeshPro>();
		UpdateClipText(0);
		
		if (trigger == TriggerType.Function)
		{
			textBox.enabled = false;
			audioSource.playOnAwake = false;
		}
		else if (trigger == TriggerType.Awake)
        {
			textBox.enabled = true;
			audioSource.playOnAwake = true;
        }
		else if (trigger == TriggerType.Collider2D)
		{
			textBox.enabled = false;
			audioSource.playOnAwake = false;
			if (gameObject.GetComponent<BoxCollider>() != null)
			{
				DestroyImmediate(gameObject.GetComponent<BoxCollider>());
			}
			if (gameObject.GetComponent<BoxCollider2D>() == null)
			{
				gameObject.AddComponent<BoxCollider2D>();
				gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
			}
			else
			{
				gameObject.GetComponent<BoxCollider2D>().enabled = true;
			}
		}
		else if (trigger == TriggerType.Collider3D)
		{
			textBox.enabled = false;
			audioSource.playOnAwake = false;
			if (gameObject.GetComponent<BoxCollider2D>() != null)
			{
				DestroyImmediate(gameObject.GetComponent<BoxCollider2D>());
			}
			if (gameObject.GetComponent<BoxCollider>() == null)
			{
				gameObject.AddComponent<BoxCollider>();
				gameObject.GetComponent<BoxCollider>().isTrigger = true;
			}
			else
			{
				gameObject.GetComponent<BoxCollider>().enabled = true;
			}
		}
	}

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		textBox = GetComponent<TextMeshPro>();
		visuals.SetActive(false);
		Generate();
	}

	private void Update()
	{
		if (bubbleOn)
		{
			if (progressButton != "None" && progressButton != "")
			{
				if (Input.GetKeyDown(progressButton.ToLower()))
				{
					if (currentBubbleIndx < clipCount)
					{
						PlayBubble(currentBubbleIndx + 1);
					}
					else
					{
						currentBubbleIndx = 0;
						textBox.enabled = false;
						visuals.SetActive(false);
						audioSource.Stop();
						bubbleOn = false;
					}
				}
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (trigger == TriggerType.Collider2D)
		{
			if (collision == available2DCollider)
			{
				PlayBubble(1);
				bubbleOn = true;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (trigger == TriggerType.Collider2D)
		{
			if (collision == available2DCollider)
			{
				textBox.enabled = false;
				audioSource.Stop();
				bubbleOn = false;
			}
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (trigger == TriggerType.Collider3D)
		{
			if (collision == available3DCollider)
			{
				PlayBubble(1);
				bubbleOn = true;
			}
		}
	}

	private void OnTriggerExit(Collider collision)
	{
		if (trigger == TriggerType.Collider3D)
		{
			if (collision == available3DCollider)
			{
				textBox.enabled = false;
				audioSource.Stop();
				bubbleOn = false;
			}
		}
	}

	private void UpdateClipText(int n)
	{
		currentClip = clips[n];
		currentText = bubble.bubble[n].speech;
		audioSource.clip = currentClip;
		textBox.text = currentText;
	}

	public bool PlayBubble(int num)
	{
		bool ret;
		if (num > clipCount)
		{
			num = clipCount;
			ret = false;
		}
		else if (num < 1)
		{
			num = 1;
			ret = false;
		}
		else
		{
			ret = true;
		}

		currentBubbleIndx = num;

		UpdateClipText(num - 1);

		textBox.enabled = true;
		visuals.SetActive(true);
		audioSource.Play();

		return ret;
	}
}
