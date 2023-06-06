using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using System.Reflection;

[RequireComponent(typeof(TextMeshPro))]
[RequireComponent(typeof(AudioSource))]
public class SpeechBubbleGen : MonoBehaviour
{
    public Bubble bubble;
    public TriggerType trigger = TriggerType.Awake;

    [AssetPath.Attribute(typeof(AudioClip))]
    public string clip;

	public int indx;
	public GameObject colliderParent;
	public Collider2D available2DCollider;
	public Collider available3DCollider;

	private AudioClip audioClip;
    private TextAsset textJSON;
    private string metaPath;
    private TextMeshPro textBox;
    private AudioSource audioSource;

	public void Generate()
    {
		audioClip = AssetPath.Load<AudioClip>(clip);
        textBox = GetComponent<TextMeshPro>();
        textBox.text = bubble.bubble[0].speech;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        if (trigger == TriggerType.Awake)
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
        int lastSlash = clip.Length - Reverse(clip).IndexOf("/");
		metaPath = clip.Substring(0, lastSlash) + "meta_" + clip.Substring(lastSlash, clip.IndexOf(".", lastSlash) - lastSlash) + ".json";
        textJSON = AssetPath.Load<TextAsset>(metaPath);
		bubble = JsonUtility.FromJson<Bubble>(textJSON.text) ;
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
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (trigger == TriggerType.Collider2D)
		{
			if (collision == available2DCollider)
			{
				PlayBubble();
			}
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (trigger == TriggerType.Collider3D)
		{
			if (collision == available3DCollider)
			{
				PlayBubble();
			}
		}
	}

	public void PlayBubble()
	{
		textBox.enabled = true;
		audioSource.Play();
	}
}
