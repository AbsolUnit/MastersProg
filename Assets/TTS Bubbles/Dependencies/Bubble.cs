using System;
using UnityEngine;

[Serializable]
public class Speech
{
	[TextArea]
	public string speech;
}

[Serializable]
public class Bubble
{
	public Speech[] bubble;
	public string model;
}
