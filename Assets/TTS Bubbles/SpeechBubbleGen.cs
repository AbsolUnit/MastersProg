using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
[RequireComponent(typeof(AudioSource))]
public class SpeechBubbleGen : MonoBehaviour
{
    public Bubble bubble;
    public TriggerType trigger = TriggerType.Awake;

    [AssetPath.Attribute(typeof(AudioClip))]
    public string clip;

	[AssetPath.Attribute(typeof(Collider2D))]
	public string collider2D;

	private AudioClip audioClip;
    private TextAsset textJSON;
    private string metaPath;
    private TextMeshPro textBox;
    private AudioSource audioSource;

    public void Generate()
    {
		audioClip = AssetPath.Load<AudioClip>(clip);
        textBox = GetComponent<TextMeshPro>();
        textBox.text = bubble.text;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
	}

    public void GetMeta()
    {
        bubble = new Bubble();
        int lastSlash = clip.Length - Reverse(clip).IndexOf("/");
		metaPath = clip.Substring(0, lastSlash) + "meta_" + clip.Substring(lastSlash, clip.IndexOf(".", lastSlash) - lastSlash) + ".json";
        textJSON = AssetPath.Load<TextAsset>(metaPath);
        bubble = JsonUtility.FromJson<Bubble>(textJSON.text);
	}

	public static string Reverse(string s)
	{
		char[] charArray = s.ToCharArray();
		Array.Reverse(charArray);
		return new string(charArray);
	}
}
