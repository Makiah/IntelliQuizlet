using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class QAPair : MonoBehaviour {

	public static List <QAPair> setQuestions = new List <QAPair> ();

	public static QAPair Create(string question, string answer)
	{
		if (SetManager.instance.qaPairPrefab != null)
		{
			GameObject createdObject = (GameObject)(Instantiate (SetManager.instance.qaPairPrefab, GameObject.Find ("ScrollContent").transform));
			//Make sure that this question goes above the add new button but after any older questions.  
			createdObject.transform.SetSiblingIndex (createdObject.transform.parent.childCount - 2);
			createdObject.GetComponent <QAPair> ().Initialize (question, answer);
			UpdateSetNum ();
			return createdObject.GetComponent <QAPair> ();
		} else
		{
			Debug.LogError ("QAPairPrefab not set!");
			return null;
		}
	}
		
	//IDEA: Let's have the QAPair control everything related to removal and addition of new QAPairs.  

	[HideInInspector] public int correctlyAnswered, incorrectlyAnswered;
	[SerializeField] private InputField questionField = null, answerField = null;

	public void Initialize(string question, string answer)
	{
		setQuestions.Add (this);
		this.questionField.text = question;
		this.answerField.text = answer;
	}

	//Maybe these'll do something later.  
	public void UpdateQuestion(string newQuestion)
	{
		questionField.text = newQuestion;
	}

	public string GetQuestion()
	{
		return questionField.text;
	}

	public void UpdateAnswer(string newAnswer)
	{
		answerField.text = newAnswer;
	}

	public string GetAnswer()
	{
		return answerField.text;
	}

	public void Remove()
	{
		setQuestions.Remove (this);
		Destroy (gameObject);
		UpdateSetNum ();
	}

	public override string ToString()
	{
		return questionField.text + "~" + answerField.text + "~" + correctlyAnswered + "~" + incorrectlyAnswered;
	}

	private static void UpdateSetNum()
	{
		SetManager.instance.numberOfSets.text = "" + setQuestions.Count + " terms";
	}

}
