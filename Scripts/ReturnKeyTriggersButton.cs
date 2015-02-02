/// Credit Melang 
/// Sourced from - http://forum.unity3d.com/members/melang.593409/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ReturnKeyTriggersButton : MonoBehaviour {

	EventSystem system;
	InputField field;

	public UnityEngine.UI.Button button;
	public bool highlight = true;
	public float highlightDuration = 0.2f;

	void Start ()
	{

		system = EventSystemManager.currentSystem;

		field = GetComponent<InputField>();

		field.onSubmit.AddListener(new UnityEngine.Events.UnityAction<string>(OnSubmitField));

	}
	
	
	void OnSubmitField(string value)
	{

		if (highlight) button.OnPointerEnter(new PointerEventData(system));
		button.OnPointerClick(new PointerEventData(system));

		if (highlight) Invoke("RemoveHighlight", highlightDuration);


	}

	void RemoveHighlight()
	{
		button.OnPointerExit(new PointerEventData(system));

	}
}
