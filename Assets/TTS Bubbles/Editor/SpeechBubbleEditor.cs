using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UI;


[CustomEditor(typeof(SpeechBubbleGen))]
public class SpeechBubbleEditor : Editor
{
	private SpeechBubbleGen generator;
	private List<Collider2D> collider2DArr = new List<Collider2D>();
	private List<Collider> collider3DArr = new List<Collider>();
	private List<string> colliderStringArr = new List<string>();
	private string[] keyCodes;

	private SerializedProperty indxColl; //int
	private SerializedProperty indxKey; //int
	private SerializedProperty customButtonInput; //bool
	private SerializedProperty progressButton; //string
	private SerializedProperty bubble; //Bubble
	private SerializedProperty clipCount; //int
	private SerializedProperty trigger; //TriggerType
	private SerializedProperty meta; //string
	private SerializedProperty audioClip; //AudioClip
	private SerializedProperty colliderParent; //GameObject
	private SerializedProperty textJSON; //TextAsset
	private SerializedProperty metaPath; //string
	private SerializedProperty textBox; //TextMeshPro
	private SerializedProperty audioSource; //AudioSource
	private SerializedProperty audioMixer; //AudioMixerGroup
	private SerializedProperty clips; //Array
	private SerializedProperty visuals; //GameObject
	private SerializedProperty animateText; //bool
	private SerializedProperty fontSize; //float
	private SerializedProperty order; //ArrayList
	private SerializedProperty childOp; //bool
	private SerializedProperty child; //SpeechBubbleGen
	private SerializedProperty loopLast; //bool
	private SerializedProperty loopAll; //bool
	private SerializedProperty buttonMode; //bool
	private SerializedProperty buttons; //Button[]
	private SerializedProperty mute; //bool
	private SerializedProperty oneTime; //bool
	private SerializedProperty autoPlay; //bool

	private void OnEnable()
	{
		indxColl = serializedObject.FindProperty("indxColl");
		indxKey = serializedObject.FindProperty("indxKey");
		customButtonInput = serializedObject.FindProperty("customButtonInput");
		progressButton = serializedObject.FindProperty("progressButton");
		bubble = serializedObject.FindProperty("bubble");
		clipCount = serializedObject.FindProperty("clipCount");
		trigger = serializedObject.FindProperty("trigger");
		meta = serializedObject.FindProperty("meta");
		audioClip = serializedObject.FindProperty("audioClip");
		colliderParent = serializedObject.FindProperty("colliderParent");
		textJSON = serializedObject.FindProperty("textJSON");
		metaPath = serializedObject.FindProperty("metaPath");
		textBox = serializedObject.FindProperty("textBox");
		audioSource = serializedObject.FindProperty("audioSource");
		audioMixer = serializedObject.FindProperty("audioMixer");
		clips = serializedObject.FindProperty("clips");
		visuals = serializedObject.FindProperty("visuals");
		animateText = serializedObject.FindProperty("animateText");
		fontSize = serializedObject.FindProperty("fontSize");
		order = serializedObject.FindProperty("order");
		childOp = serializedObject.FindProperty("childOp");
		child = serializedObject.FindProperty("child");
		loopLast = serializedObject.FindProperty("loopLast");
		loopAll = serializedObject.FindProperty("loopAll");
		buttonMode = serializedObject.FindProperty("buttonMode");
		buttons = serializedObject.FindProperty("buttons");
		mute = serializedObject.FindProperty("mute");
		oneTime = serializedObject.FindProperty("oneTime");
		autoPlay = serializedObject.FindProperty("autoPlay");
		GetKeyCodes();
	}

	public override void OnInspectorGUI()
	{
		serializedObject.UpdateIfRequiredOrScript();

		generator = (SpeechBubbleGen)target;

		EditorGUILayout.PropertyField(meta, new GUIContent("Meta Data", "The meta data generated by the VocaliseNow generator"));

		if ((TextAsset)meta.GetUnderlyingValue())
		{
			GetMeta();
			GatherClips();
		}
		else
		{
			EditorGUILayout.HelpBox("Please provide a valid meta data file", MessageType.Warning);
		}

		EditorGUILayout.PropertyField(audioMixer, new GUIContent("Audio Mixer", "The desired output audio mixer group"));
		EditorGUILayout.PropertyField(visuals, new GUIContent("Visuals", "The art for the speech bubble"));

		EditorGUILayout.Space(10);

		EditorGUILayout.PropertyField(oneTime, new GUIContent("Play once", "Will disable bubble then finished playing, or finished order if combined with autoplay"));
		EditorGUILayout.PropertyField(autoPlay, new GUIContent("Auto Play", "Will automatically play the bubble until finished or stopped"));
		EditorGUILayout.PropertyField(animateText, new GUIContent("Animate Text", "Do you want to use animated text?"));
		EditorGUI.indentLevel++;

		if (animateText.boolValue == true)
		{
			EditorGUILayout.PropertyField(fontSize, new GUIContent("Font Size", "Textbox auto size option does not work with animated text"));
		}

		EditorGUI.indentLevel--;
		EditorGUILayout.PropertyField(buttonMode, new GUIContent("Button Mode", "This will play all bubbles marked 'Child' automatically"));
		if (buttonMode.boolValue)
		{
			EditorGUILayout.PropertyField(buttons, new GUIContent("Button", "The Button that activates NextBubble() and any other naffecting buttons"));
			if (buttons.arraySize == 0)
			{
				EditorGUILayout.HelpBox("Please provide a valid Button", MessageType.Warning);
			}
		}
		EditorGUILayout.PropertyField(mute, new GUIContent("Mute Audio", "Do you with to mute the audio of this bubble?"));
		

		EditorGUILayout.PropertyField(childOp, new GUIContent("Optional Child", "Do you wish to link another bubble to this one?"));
		EditorGUI.indentLevel++;

		if (childOp.boolValue == true)
		{
			EditorGUILayout.PropertyField(child, new GUIContent("Child", "The SpeechBubbleGen component of the child bubble"));
		}

		EditorGUI.indentLevel--;

		EditorGUILayout.Space(10);  

		EditorGUILayout.PropertyField(trigger, new GUIContent("Bubble Trigger", "How do you want the speech bubble to be triggered"));
		EditorGUI.indentLevel++;

		if (generator.trigger == TriggerType.Collider2D || generator.trigger == TriggerType.Collider3D)
		{
			EditorGUILayout.PropertyField(colliderParent, new GUIContent("Parent Object", "The object that contains the collider you wish to use as a trigger"));

			if (generator.trigger == TriggerType.Collider2D && colliderParent.objectReferenceValue != null)
			{
				//EditorGUILayout.PropertyField(colliderTwoD, new GUIContent("2D Collider", "The 2D collider you wish to use as a trigger"));
				ContructColliderArray((GameObject)colliderParent.objectReferenceValue, true);
				indxColl.intValue = EditorGUILayout.Popup(new GUIContent("2D Collider", "The 2D collider you wish to use as a trigger"), indxColl.intValue, colliderStringArr.ToArray());
				if (collider2DArr.Count() != 0)
				{
					generator.available2DCollider = collider2DArr[indxColl.intValue];
				}
			}

			if (generator.trigger == TriggerType.Collider3D && colliderParent.objectReferenceValue != null)
			{
				//EditorGUILayout.PropertyField(colliderThreeD, new GUIContent("3D Collider", "The 3D collider you wish to use as a trigger"));
				ContructColliderArray((GameObject)colliderParent.objectReferenceValue, false);
				indxColl.intValue = EditorGUILayout.Popup(new GUIContent("3D Collider", "The 3D collider you wish to use as a trigger"), indxColl.intValue, colliderStringArr.ToArray());
				if (collider3DArr.Count() != 0)
				{
					generator.available3DCollider = collider3DArr[indxColl.intValue];
				}
			}

			if (colliderParent.objectReferenceValue == null)
			{
				EditorGUILayout.HelpBox("Please select a parent for the collider, or select a different trigger option", MessageType.Warning);
			}

			EditorGUILayout.PropertyField(customButtonInput, new GUIContent("Custom Key Code Input?", "Would you like to type the KeyCode you wish?"));
			if (customButtonInput.boolValue)
			{
				EditorGUILayout.PropertyField(progressButton, new GUIContent("Progress Button", "The KeyCode of the progress button you wish to use \n NOTE: access using the 'progressButton' variable"));
			}
			else
			{
				indxKey.intValue = EditorGUILayout.Popup(new GUIContent("Progress Button", "The KeyCode of the progress button you wish to use \n NOTE: access using the 'progressButton' variable"), indxKey.intValue, keyCodes);
				progressButton.stringValue = keyCodes[indxKey.intValue];
			}
		}

		EditorGUILayout.Space(10);

		EditorGUI.indentLevel--;

		EditorGUILayout.LabelField(new GUIContent("Bubbles:", "All the paragraphs in this meta file"));
		EditorGUI.indentLevel++;
		for (int i = 0; i < generator.bubble.bubble.Length; i++)
		{
			ReadOnlyTextField(true, new GUIContent("Text " + i.ToString() + ":", "Text generated from meta data"), generator.bubble.bubble[i].speech);
		}
		ReadOnlyTextField(false, new GUIContent("Voice Model:", "Model name generated from meta data"), generator.bubble.model);
		EditorGUI.indentLevel--;

		EditorGUILayout.Space(10);

		EditorGUILayout.PropertyField(loopLast, new GUIContent("Loop Last", "Do you want the last bubble in the order below to loop?"));
		EditorGUILayout.PropertyField(loopAll, new GUIContent("Loop All", "Do you want the bubbles below to loop when you reach the end?"));

		if (loopAll.boolValue == true && loopLast.boolValue == true)
		{
			EditorGUILayout.HelpBox("Please only select one loop option", MessageType.Warning);
		}

		EditorGUILayout.PropertyField(order, new GUIContent("Bubble Order", "Select the order of the speech bubbles"));

		EditorGUILayout.BeginHorizontal();
		float buttonWidth = 150;
		float buttonGap = 20;
		GUILayout.Space(Screen.width/2 - buttonWidth - buttonGap);
		if (GUILayout.Button("Reset Bubble", GUILayout.Width(buttonWidth), GUILayout.Height(25)))
		{
			GetMeta();
			GatherClips();
			serializedObject.ApplyModifiedProperties();
			generator.Generate();
		}
		GUILayout.Space(buttonGap);
		if (GUILayout.Button("Reset Order", GUILayout.Width(buttonWidth), GUILayout.Height(25)))
		{
			generator.ResetOrder();
		}
		EditorGUILayout.EndHorizontal();

		serializedObject.ApplyModifiedProperties();

	}

	void GetMeta()
	{
		bubble.SetUnderlyingValue(new Bubble());
		bubble.SetUnderlyingValue(JsonUtility.FromJson<Bubble>(((TextAsset)meta.GetUnderlyingValue()).text));
		clipCount.intValue = ((Bubble)bubble.GetUnderlyingValue()).count;
	}

	void GatherClips()
	{
		List<AudioClip> temp = new List<AudioClip>();
		for(int i=0; i< generator.clipCount; i++)
		{
			AudioClip clip = (AudioClip)AssetDatabase.LoadAssetAtPath(GetClipPath(i), typeof(AudioClip));
			temp.Add(clip);
		}
		clips.SetUnderlyingValue(temp.ToArray());
	}

	string GetClipPath(int count)
	{
		string metaPath = AssetDatabase.GetAssetPath(generator.meta);
		int lastSlash = metaPath.Length - Reverse(metaPath).IndexOf("/");
		string path = metaPath.Substring(0, lastSlash) + generator.bubble.bubble[count].audio;
		return path;
	}

	static string Reverse(string s)
	{
		char[] charArray = s.ToCharArray();
		Array.Reverse(charArray);
		return new string(charArray);
	}

	void ReadOnlyTextField(bool area, GUIContent label, string text, float height = 40)
	{
		if (area)
		{
			EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
			GUILayout.BeginScrollView(new Vector2(0,0), GUILayout.Height(height));
			{
				EditorStyles.textField.wordWrap = true;
				GUILayout.TextArea(text, EditorStyles.textField, GUILayout.ExpandHeight(true));
			}
			GUILayout.EndScrollView();
		}
		else
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
				EditorGUILayout.SelectableLabel(text, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	void ContructColliderArray(GameObject parent, bool twoD)
	{
		colliderStringArr = new List<string>();
		collider2DArr = new List<Collider2D>();
		collider3DArr = new List<Collider>();
		if (twoD)
		{
			foreach(Collider2D col in parent.GetComponents<Collider2D>())
			{
				colliderStringArr.Add(col.GetType().ToString());
				collider2DArr.Add(col);
			}
		}
		else
		{
			foreach (Collider col in parent.GetComponents<Collider>())
			{
				colliderStringArr.Add(col.GetType().ToString());
				collider3DArr.Add(col);
			}
		}
	}

	private void GetKeyCodes()
	{
		keyCodes = new string[] {
			"None",
			"Backspace",
			"Delete",
			"Tab",
			"Clear",
			"Return",
			"Pause",
			"Escape",
			"Space",
			"Keypad0",
			"Keypad1",
			"Keypad2",
			"Keypad3",
			"Keypad4",
			"Keypad5",
			"Keypad6",
			"Keypad7",
			"Keypad8",
			"Keypad9",
			"KeypadPeriod",
			"KeypadDivide",
			"KeypadMultiply",
			"KeypadMinus",
			"KeypadPlus",
			"KeypadEnter",
			"KeypadEquals",
			"UpArrow",
			"DownArrow",
			"RightArrow",
			"LeftArrow",
			"Insert",
			"Home",
			"End",
			"PageUp",
			"PageDown",
			"F1",
			"F2",
			"F3",
			"F4",
			"F5",
			"F6",
			"F7",
			"F8",
			"F9",
			"F10",
			"F11",
			"F12",
			"F13",
			"F14",
			"F15",
			"Alpha0",
			"Alpha1",
			"Alpha2",
			"Alpha3",
			"Alpha4",
			"Alpha5",
			"Alpha6",
			"Alpha7",
			"Alpha8",
			"Alpha9",
			"Exclaim",
			"DoubleQuote",
			"Hash",
			"Dollar",
			"Percent",
			"Ampersand",
			"Quote",
			"LeftParen",
			"RightParen",
			"Asterisk",
			"Plus",
			"Comma",
			"Minus",
			"Period",
			"Slash",
			"Colon",
			"Semicolon",
			"Less",
			"Equals",
			"Greater",
			"Question",
			"At",
			"LeftBracket",
			"Backslash",
			"RightBracket",
			"Caret",
			"Underscore",
			"BackQuote",
			"A",
			"B",
			"C",
			"D",
			"E",
			"F",
			"G",
			"H",
			"I",
			"J",
			"K",
			"L",
			"M",
			"N",
			"O",
			"P",
			"Q",
			"R",
			"S",
			"T",
			"U",
			"V",
			"W",
			"X",
			"Y",
			"Z",
			"LeftCurlyBracket",
			"Pipe",
			"RightCurlyBracket",
			"Tilde",
			"Numlock",
			"CapsLock",
			"ScrollLock",
			"RightShift",
			"LeftShift",
			"RightControl",
			"LeftControl",
			"RightAlt",
			"LeftAlt",
			"LeftMeta",
			"LeftCommand",
			"LeftApple",
			"LeftWindows",
			"RightMeta",
			"RightCommand",
			"RightApple",
			"RightWindows",
			"AltGr",
			"Help",
			"Print",
			"SysReq",
			"Break",
			"Menu",
			"Mouse0",
			"Mouse1",
			"Mouse2",
			"Mouse3",
			"Mouse4",
			"Mouse5",
			"Mouse6",
			"JoystickButton0",
			"JoystickButton1",
			"JoystickButton2",
			"JoystickButton3",
			"JoystickButton4",
			"JoystickButton5",
			"JoystickButton6",
			"JoystickButton7",
			"JoystickButton8",
			"JoystickButton9",
			"JoystickButton10",
			"JoystickButton11",
			"JoystickButton12",
			"JoystickButton13",
			"JoystickButton14",
			"JoystickButton15",
			"JoystickButton16",
			"JoystickButton17",
			"JoystickButton18",
			"JoystickButton19",
			"Joystick1Button0",
			"Joystick1Button1",
			"Joystick1Button2",
			"Joystick1Button3",
			"Joystick1Button4",
			"Joystick1Button5",
			"Joystick1Button6",
			"Joystick1Button7",
			"Joystick1Button8",
			"Joystick1Button9",
			"Joystick1Button10",
			"Joystick1Button11",
			"Joystick1Button12",
			"Joystick1Button13",
			"Joystick1Button14",
			"Joystick1Button15",
			"Joystick1Button16",
			"Joystick1Button17",
			"Joystick1Button18",
			"Joystick1Button19",
			"Joystick2Button0",
			"Joystick2Button1",
			"Joystick2Button2",
			"Joystick2Button3",
			"Joystick2Button4",
			"Joystick2Button5",
			"Joystick2Button6",
			"Joystick2Button7",
			"Joystick2Button8",
			"Joystick2Button9",
			"Joystick2Button10",
			"Joystick2Button11",
			"Joystick2Button12",
			"Joystick2Button13",
			"Joystick2Button14",
			"Joystick2Button15",
			"Joystick2Button16",
			"Joystick2Button17",
			"Joystick2Button18",
			"Joystick2Button19",
			"Joystick3Button0",
			"Joystick3Button1",
			"Joystick3Button2",
			"Joystick3Button3",
			"Joystick3Button4",
			"Joystick3Button5",
			"Joystick3Button6",
			"Joystick3Button7",
			"Joystick3Button8",
			"Joystick3Button9",
			"Joystick3Button10",
			"Joystick3Button11",
			"Joystick3Button12",
			"Joystick3Button13",
			"Joystick3Button14",
			"Joystick3Button15",
			"Joystick3Button16",
			"Joystick3Button17",
			"Joystick3Button18",
			"Joystick3Button19",
			"Joystick4Button0",
			"Joystick4Button1",
			"Joystick4Button2",
			"Joystick4Button3",
			"Joystick4Button4",
			"Joystick4Button5",
			"Joystick4Button6",
			"Joystick4Button7",
			"Joystick4Button8",
			"Joystick4Button9",
			"Joystick4Button10",
			"Joystick4Button11",
			"Joystick4Button12",
			"Joystick4Button13",
			"Joystick4Button14",
			"Joystick4Button15",
			"Joystick4Button16",
			"Joystick4Button17",
			"Joystick4Button18",
			"Joystick4Button19",
			"Joystick5Button0",
			"Joystick5Button1",
			"Joystick5Button2",
			"Joystick5Button3",
			"Joystick5Button4",
			"Joystick5Button5",
			"Joystick5Button6",
			"Joystick5Button7",
			"Joystick5Button8",
			"Joystick5Button9",
			"Joystick5Button10",
			"Joystick5Button11",
			"Joystick5Button12",
			"Joystick5Button13",
			"Joystick5Button14",
			"Joystick5Button15",
			"Joystick5Button16",
			"Joystick5Button17",
			"Joystick5Button18",
			"Joystick5Button19",
			"Joystick6Button0",
			"Joystick6Button1",
			"Joystick6Button2",
			"Joystick6Button3",
			"Joystick6Button4",
			"Joystick6Button5",
			"Joystick6Button6",
			"Joystick6Button7",
			"Joystick6Button8",
			"Joystick6Button9",
			"Joystick6Button10",
			"Joystick6Button11",
			"Joystick6Button12",
			"Joystick6Button13",
			"Joystick6Button14",
			"Joystick6Button15",
			"Joystick6Button16",
			"Joystick6Button17",
			"Joystick6Button18",
			"Joystick6Button19",
			"Joystick7Button0",
			"Joystick7Button1",
			"Joystick7Button2",
			"Joystick7Button3",
			"Joystick7Button4",
			"Joystick7Button5",
			"Joystick7Button6",
			"Joystick7Button7",
			"Joystick7Button8",
			"Joystick7Button9",
			"Joystick7Button10",
			"Joystick7Button11",
			"Joystick7Button12",
			"Joystick7Button13",
			"Joystick7Button14",
			"Joystick7Button15",
			"Joystick7Button16",
			"Joystick7Button17",
			"Joystick7Button18",
			"Joystick7Button19",
			"Joystick8Button0",
			"Joystick8Button1",
			"Joystick8Button2",
			"Joystick8Button3",
			"Joystick8Button4",
			"Joystick8Button5",
			"Joystick8Button6",
			"Joystick8Button7",
			"Joystick8Button8",
			"Joystick8Button9",
			"Joystick8Button10",
			"Joystick8Button11",
			"Joystick8Button12",
			"Joystick8Button13",
			"Joystick8Button14",
			"Joystick8Button15",
			"Joystick8Button16",
			"Joystick8Button17",
			"Joystick8Button18",
			"Joystick8Button19"
		};
	}
}
