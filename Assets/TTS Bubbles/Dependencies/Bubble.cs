using System;
using UnityEngine;

[Serializable]
public class Speech
{
	[TextArea]
	public string speech;
	public string audio;
}

[Serializable]
public class Bubble
{
	public Speech[] bubble;
	public string model;
	public int count;
}
