using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;


[CustomEditor(typeof(SpeechBubbleGen))]
public class SpeechBubbleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SpeechBubbleGen generator = (SpeechBubbleGen)target;

        base.OnInspectorGUI();

		if (GUILayout.Button("Generate Speech Bubble"))
		{
			GenerateBubble(generator);
		}

        if(generator.clip != string.Empty)
        {
            generator.GetMeta();
        }
	}

    void GenerateBubble(SpeechBubbleGen gen)
    {
        gen.Generate();
    }
}
