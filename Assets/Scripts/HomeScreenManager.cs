using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

public class HomeScreenManager : MonoBehaviour 
{
	//Singleton
	public static HomeScreenManager instance;
	void Awake() {instance = this;}

	[SerializeField] private InputField path;
	[SerializeField] private InputField setName;
	[SerializeField] private Button ready;

	private enum ButtonState {DISABLED, CREATENEW, LOADOLD}
	private ButtonState buttonState = ButtonState.DISABLED;

	void Start()
	{
		UpdateButtonState ();
		DontDestroyOnLoad (this.gameObject);
	}

	public void UpdateButtonState()
	{
		switch (buttonState)
		{
		case ButtonState.CREATENEW:
			ready.GetComponent <Image> ().color = Color.red;
			ready.transform.GetChild (0).GetComponent <Text> ().text = "Create";
			ready.interactable = true;
			setName.gameObject.SetActive (true);
			break;
		case ButtonState.LOADOLD:
			ready.GetComponent <Image> ().color = Color.green;
			ready.transform.GetChild (0).GetComponent <Text> ().text = "Load";
			ready.interactable = true;
			setName.gameObject.SetActive (false);
			break;
		case ButtonState.DISABLED:
			ready.GetComponent <Image> ().color = Color.gray;
			ready.transform.GetChild (0).GetComponent <Text> ().text = "Please Input Valid Path and Title";
			ready.interactable = false;
			setName.gameObject.SetActive (true);
			break;
		}
	}

	public void OnPathChanged()
	{
		ButtonState newButtonStatus = buttonState;

		if (path.text.Equals ("") == false)
		{
			//Short-circuiting prevents errors in this case.  
			if (path.text.Length > 7 && path.text.Substring (path.text.Length - 4).Equals (".txt"))
			{
				// Handle any problems that might arise when reading the text
				try
				{
					// Create a new StreamReader, tell it which file to read and what encoding the file
					// was saved as
					StreamReader theReader = new StreamReader (path.text, Encoding.Default);
					// Immediately clean up the reader after this block of code is done.
					// You generally use the "using" statement for potentially memory-intensive objects
					// instead of relying on garbage collection.
					// (Do not confuse this with the using directive for namespace at the 
					// beginning of a class!)
					using (theReader)
					{
						theReader.ReadLine ();
						// Done reading, close the reader and return true to broadcast success    
						theReader.Close ();
					}
					newButtonStatus = ButtonState.LOADOLD;
				}
				// If anything broke in the try block, we throw an exception with information on what didn't work
				catch (System.Exception e)
				{
					if (setName.text.Equals ("") == false)
						newButtonStatus = ButtonState.CREATENEW;
					else
						newButtonStatus = ButtonState.DISABLED;
				}
			} else
			{
				newButtonStatus = ButtonState.DISABLED;
			}
		} 
		else
		{
			newButtonStatus = ButtonState.DISABLED;
		}

		if (newButtonStatus != buttonState)
		{
			buttonState = newButtonStatus;
			UpdateButtonState ();
		}

	}

	public void OnTitleChanged()
	{
		OnPathChanged (); //Just update this so that if the title was originally null it becomes possible to edit.  
	}

	[HideInInspector] public string obtainedPath;

	public void OnReadyPressed()
	{
		if (buttonState != ButtonState.DISABLED)
		{
			obtainedPath = path.text;

			if (buttonState == ButtonState.CREATENEW)
			{
				System.IO.File.WriteAllText(path.text, setName.text + "\n");
				Debug.Log ("Created new file at " + path.text + " with title " + setName.text);
			}

			SceneManager.LoadScene (1);
		}
	}


}
