using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerType
{
    [Tooltip("Simply call the function PlayBubble(num) where 'num' is the number of the message in the bubble you wish to play")]
    Function,
    [Tooltip("This allows you to simple activate the to play automatically.\nNOTE: will only play the first message in a multi-message bubble")]
    Awake,
	[Tooltip("Select a 2D collider in a parent object to activate the bubble, and a progression button to rotate through the messages")]
	Collider2D,
	[Tooltip("Select a 3D collider in a parent object to activate the bubble, and a progression button to rotate through the messages")]
	Collider3D
}
