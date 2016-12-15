using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TabbingBehaviour : MonoBehaviour {

	EventSystem system;

	void Start()
	{
		OnLevelWasLoaded ();
	}

	void OnLevelWasLoaded()
	{
		DontDestroyOnLoad (this.gameObject);
		system = EventSystem.current;// EventSystemManager.currentSystem;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab) && QuizToggle.instance.InQuizMode == false)
		{
			GameObject current = system.currentSelectedGameObject;

			if (current != null)
			{
				InputField currentInputField = current.GetComponent<InputField> ();
				if (currentInputField != null)
				{
					InputField newInputField = null;
					if (currentInputField.gameObject.name.Equals ("Question"))
						newInputField = currentInputField.gameObject.transform.parent.GetChild (1).GetComponent <InputField> ();
					else if (currentInputField.name.Equals ("Answer"))
					{
						int qaPairChildIndex = currentInputField.transform.parent.GetSiblingIndex ();
						int totalNumberOfChildren = currentInputField.transform.parent.parent.childCount - 1;
						if (qaPairChildIndex < totalNumberOfChildren - 1)
							newInputField = currentInputField.gameObject.transform.parent.parent.GetChild (qaPairChildIndex + 1).FindChild ("Question").GetComponent <InputField> ();
						else
							newInputField = QAPair.Create ("", "").transform.FindChild("Question").GetComponent <InputField> ();
					}

					if (newInputField != null)
					{
						system.SetSelectedGameObject (newInputField.gameObject, new BaseEventData (system)); 
						newInputField.OnPointerClick (new PointerEventData (system));

						//Find the tap in the input field that was last selected.  
						int indexOfTab = currentInputField.text.IndexOf("\t");
						string newText = "";
						if (indexOfTab > 0) //Check to make sure that is not -1, and greater than 0 in one go.  
						{
							newText = currentInputField.text.Substring (0, indexOfTab);
							if (indexOfTab < currentInputField.text.Length - 1)
								newText = newText + currentInputField.text.Substring (indexOfTab + 1);
						}

						currentInputField.text = newText;
					}
					
				}
			} 

		}
	}

	void CreateNewTermIfSceneAppropriate()
	{
		if (SceneManager.GetActiveScene ().buildIndex == 1)
		{
			QAPair newTerm = QAPair.Create ("", "");
			InputField inputfield = newTerm.transform.GetChild(0).GetComponent<InputField> ();
			inputfield.OnPointerClick (new PointerEventData (system));  //if it's an input field, also set the text caret
			system.SetSelectedGameObject (newTerm.GetComponentInChildren<Selectable> ().gameObject, new BaseEventData (system)); 
		}
	}
}
