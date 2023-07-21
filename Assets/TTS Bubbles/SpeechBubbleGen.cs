using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using System.Reflection;
using UnityEditor.ShaderGraph.Serialization;

[RequireComponent(typeof(TextMeshPro))]
[RequireComponent(typeof(AudioSource))]
[ExecuteInEditMode]
public class SpeechBubbleGen : MonoBehaviour
{
    public Bubble bubble;
    public TriggerType trigger = TriggerType.Awake;

    [AssetPath.Attribute(typeof(TextAsset))]
    public string meta;

	public int indxColl;
	public int indxKey;
	public GameObject colliderParent;
	public Collider2D available2DCollider;
	public Collider available3DCollider;
	public bool customButtonInput;
	public string progressButton;
	public AudioMixerGroup audioMixer;

	private AudioClip currentClip;
	private string currentText;
	private int clipCount;
	private int currentBubbleIndx;
	private bool bubbleOn;
    private TextAsset textJSON;
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

	public void GetMeta()
    {
        bubble = new Bubble();
        textJSON = AssetPath.Load<TextAsset>(meta);
		bubble = JsonUtility.FromJson<Bubble>(textJSON.text);
		clipCount = bubble.count;
	}

	private string GetClipPath(int count)
	{
		int lastSlash = meta.Length - Reverse(meta).IndexOf("/");
		string path = meta.Substring(0, lastSlash) + bubble.bubble[count].audio;
		return path;
	}

	public static string Reverse(string s)
	{
		char[] charArray = s.ToCharArray();
		Array.Reverse(charArray);
		return new string(charArray);
	}

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		textBox = GetComponent<TextMeshPro>();
		GetMeta();
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
		currentClip = AssetPath.Load<AudioClip>(GetClipPath(n));
		currentText = bubble.bubble[n].speech;
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

		Debug.Log(clipCount);
		UpdateClipText(num - 1);
		audioSource.clip = currentClip;
		textBox.text = currentText;

		textBox.enabled = true;
		audioSource.Play();

		return ret;
	}
}
