using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QuizToggle : MonoBehaviour {

	public static QuizToggle instance;
	void Awake() {instance = this;}

	[SerializeField] private Canvas quizCanvas = null, editCanvas = null;
	private Text textComponent;

	public bool InQuizMode = true;

	void Start()
	{
		textComponent = transform.GetChild(0).GetComponent <Text> ();
		ToggleQuizMode ();
	}

	public void ToggleQuizMode()
	{
		InQuizMode = !InQuizMode;
		if (InQuizMode)
		{
			quizCanvas.gameObject.SetActive (true);
			editCanvas.gameObject.SetActive (false);
			textComponent.text = "Exit Quiz Mode";
			QuizManager.instance.Activate ();
			GetComponent <Image> ().color = Color.red;
		} else
		{
			quizCanvas.gameObject.SetActive (false);
			editCanvas.gameObject.SetActive (true);
			textComponent.text = "Enter Quiz Mode";
			QuizManager.instance.Deactivate ();
			GetComponent <Image> ().color = Color.green;
		}
	}

}