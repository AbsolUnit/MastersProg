using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using System.Reflection;
using UnityEditor.ShaderGraph.Serialization;

[RequireComponent(typeof(TextMeshPro))]
[RequireComponent(typeof(AudioSource))]
public class SpeechBubbleGen : MonoBehaviour
{
    public Bubble bubble;
    public TriggerType trigger = TriggerType.Awake;

    [AssetPath.Attribute(typeof(TextAsset))]
    public string meta;

	public int indx;
	public GameObject colliderParent;
	public Collider2D available2DCollider;
	public Collider available3DCollider;

	private AudioClip currentClip;
	private string currentText;
	private int clipCount;
	private int currentClipCount = 0;
    private TextAsset textJSON;
    private TextMeshPro textBox;
    private AudioSource audioSource;

	public void Generate()
    {
		UpdateClipText();

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

	private void UpdateClipText()
	{
		currentClip = AssetPath.Load<AudioClip>(GetClipPath(currentClipCount));
		currentText = bubble.bubble[currentClipCount].speech;
	}

	public void PlayBubble()
	{
		UpdateClipText();
		audioSource.clip = currentClip;
		textBox.text = currentText;

		textBox.enabled = true;
		audioSource.Play();

		currentClipCount++;
	}
}
