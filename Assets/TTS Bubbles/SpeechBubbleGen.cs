using System;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshPro))]
[RequireComponent(typeof(AudioSource))]
[ExecuteInEditMode]
public class SpeechBubbleGen : MonoBehaviour
{
    public Bubble bubble;
	public int clipCount;
	public TriggerType trigger = TriggerType.Awake;
	public bool animateText;
	public float fontSize;
	public List<string> order;
	public bool loopLast;
	public bool loopAll;
	public bool buttonMode;
	public Button[] buttons;
	public bool mute;
	public bool oneTime;
	public GameObject pausePlayer;
	public bool autoPlay;
	public bool bubbleOn;

	public bool childOp;
	public SpeechBubbleGen child;

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
	
    private TextMeshPro textBox;
    private AudioSource audioSource;
	private Vector3 pos;
	private Quaternion rot;
	private bool flag;

	public void Generate()
    {
		audioSource = GetComponent<AudioSource>();
		audioSource.outputAudioMixerGroup = audioMixer;
		textBox = GetComponent<TextMeshPro>();
		UpdateClipText(0);
		if (order.Count == 0)
		{
			ResetOrder();
		}
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

	public void ResetOrder()
	{
		int childCount = 0;
		if (childOp && child != null)
		{
			childCount = child.order.Count;
		}
		order = new List<string>();
		for (int i = 0; i < clipCount; i++)
		{
			order.Add(i.ToString());
		}
		if (childCount != 0)
		{
			child.Generate();
			foreach (var bubble in child.order)
			{
				order.Add("Child" + bubble.ToString());
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
					NextBubble();
				}
			}
		}
	}

	public void NextBubble()
	{
		if (currentBubbleIndx < order.Count - 1)
		{
			PlayBubble(currentBubbleIndx + 1);
		}
		else
		{
			if (oneTime)
			{
				TurnOff();
				this.gameObject.SetActive(false);
			}

			if (loopLast)
			{
				PlayBubble(currentBubbleIndx);
			}
            else if (loopAll)
            {
				PlayBubble(0);
            }

            else
			{
				TurnOff();
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
		bubbleOn = true;
		

		if (buttonMode)
		{
			foreach (Button button in buttons)
			{
				button.enabled = false;
			}
		}

		bool ret;
		if (num > order.Count - 1)
		{
			num = order.Count - 1;
			ret = false;
		}
		else if (num < 0)
		{
			num = 0;
			ret = false;
		}
		else
		{
			ret = true;
		}

		string bubb = order[num];
		int indx = int.Parse(bubb.Substring(bubb.Length - 1));

		currentBubbleIndx = num;

		if (bubb.Contains("Child"))
		{
			int level = Regex.Matches(bubb, "Child").Count;
			SpeechBubbleGen p = this;
			for (int i=0; i<level; i++)
			{
				SpeechBubbleGen c = p.child;
				p = c;
			}
			p.PlayBubble(indx);

			if (buttonMode || autoPlay)
			{
				StartCoroutine(WaitNext());
			}
			else if (oneTime)
			{
				TurnOff();
				this.gameObject.SetActive(false);
			}
			return ret;
		}

		UpdateClipText(indx);
		
		if (!animateText)
		{
			textBox.enabled = true;
			if (buttonMode)
			{
				foreach (Button button in buttons)
				{
					button.enabled = true;
				}
			}
			if (autoPlay)
			{
				StartCoroutine(WaitNext());
			}
			else if (oneTime)
			{
				TurnOff();
				this.gameObject.SetActive(false);
			}
		}
		else
		{
			StartCoroutine(TextAnimation(num));
		}
		
		visuals.SetActive(true);
		if (!mute)
		{
			audioSource.Play();
		}
		
		return ret;
	}

	private IEnumerator WaitNext()
	{
		yield return new WaitForSeconds(audioSource.clip.length + 1);
		NextBubble();
	}

	private IEnumerator TextAnimation(int indx)
	{
		string text = textBox.text;
		float length = audioSource.clip.length;
		float secPerChar = (length / text.Length);
		textBox.text = "";
		textBox.enableAutoSizing = false;
		textBox.fontSize = fontSize;
		textBox.enabled = true;
		foreach (char letter in text)
		{
			if (indx == currentBubbleIndx) {
				textBox.text += letter;
				yield return new WaitForSeconds(secPerChar);
			}
			else
			{
				textBox.text = text;
				break;
			}
		}
		if (buttonMode)
		{
			foreach (Button button in buttons)
			{
				button.enabled = true;
			}
		}
		if (autoPlay)
		{
			yield return new WaitForSeconds(1);
			NextBubble();
		}
		else if (oneTime)
		{
			TurnOff();
			this.enabled = false;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (trigger == TriggerType.Collider2D)
		{
			if (collision == available2DCollider)
			{
				PlayBubble(0);
			}
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (trigger == TriggerType.Collider2D)
		{
			if (collision == available2DCollider && bubbleOn == false)
			{
				PlayBubble(0);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (trigger == TriggerType.Collider2D)
		{
			if (collision == available2DCollider)
			{
				TurnOff();
			}
		}
	}

	

	private void OnTriggerEnter(Collider collision)
	{
		if (trigger == TriggerType.Collider3D)
		{
			if (collision == available3DCollider)
			{
				PlayBubble(0);
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (trigger == TriggerType.Collider3D)
		{
			if (other == available3DCollider && bubbleOn == false)
			{
				PlayBubble(0);
			}
		}
	}

	private void OnTriggerExit(Collider collision)
	{
		if (trigger == TriggerType.Collider3D)
		{
			if (collision == available3DCollider)
			{
				TurnOff();
			}
		}
	}

	public void TurnOff()
	{
		SpeechBubbleGen p = this;
		while (p.childOp && p.child != null)
		{
			p.currentBubbleIndx = 0;
			p.textBox.enabled = false;
			p.visuals.SetActive(false);
			p.audioSource.Stop();
			p.bubbleOn = false;
			p.flag = false;
			SpeechBubbleGen c = p.child;
			c.currentBubbleIndx = 0;
			c.textBox.enabled = false;
			c.visuals.SetActive(false);
			c.audioSource.Stop();
			c.bubbleOn = false;
			c.flag = false;
			p = c;
		}
	}
}
