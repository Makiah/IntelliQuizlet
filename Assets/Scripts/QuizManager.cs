using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour {

	public static QuizManager instance;
	void Awake() {instance = this;}

	public InputField questionField, userAnswerField, actualAnswerField;
	public Button iGiveUpButton;
	public Slider percentageStudyingComplete, percentageQuestionComplete;

	public List <QAPair> questionOrder;

	public void Activate()
	{
		//Establish question order
		questionOrder = new List <QAPair> ();

		List <QAPair> originalList = new List <QAPair> ();
		foreach (QAPair qa in QAPair.setQuestions)
			originalList.Add (qa);
		//Remove until order established.  
		while (originalList.Count > 0)
		{
			int chosenIndex = Random.Range (0, originalList.Count);
			questionOrder.Add (originalList [chosenIndex]);
			originalList.RemoveAt (chosenIndex);
		}

		//Start the actual question without removing the first one.  
		questionField.text = questionOrder [0].GetQuestion ();
		actualAnswerField.text = questionOrder [0].GetAnswer ();
		actualAnswerKeywords = DeterminePhraseKeywords (actualAnswerField.text);
		userAnswerField.interactable = true;
		SetButtonState (ButtonState.IGIVEUP);
	}
		
	private string[] actualAnswerKeywords;

	//Update slider, check using improved technique compared to Quizlet, etc.  
	public void OnUserAnswerChanged()
	{
		string[] userAnswerKeywords = DeterminePhraseKeywords (userAnswerField.text);

		//Determine the degree of similarity between the two. 
		int similarities = 0;
		foreach (string s in actualAnswerKeywords)
		{
			foreach (string a in userAnswerKeywords)
			{
				if (s.Equals (a))
				{
					similarities++;
					break;
				}
			}
		}

		//Calculate degree of similarity.  
		float degreeOfSimilarity = ((float) (similarities)) / ((float) (actualAnswerKeywords.Length));
		percentageQuestionComplete.value = degreeOfSimilarity;
		percentageQuestionComplete.transform.FindChild ("Fill Area").GetChild (0).GetComponent <Image> ().color = Color.Lerp (Color.red, Color.green, degreeOfSimilarity);

		if (degreeOfSimilarity >= 0.8)
		{
			OnQuestionComplete ();
			if (indexOfDuplicatedQuestion == -1) //This would imply that the user did NOT click "I GIVE UP".  
				SetButtonState(ButtonState.IWASWRONG);
		}
	}

	public string[] DeterminePhraseKeywords(string phrase) 
	{
		//Make lowercase.  
		phrase = phrase.ToLower();

		//Eliminate end or beginning spaces.  
		phrase.Trim();

		//Get rid of any punctuation. 
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		foreach (char c in phrase)
		{
			if (!char.IsPunctuation(c))
				sb.Append(c);
		}
		phrase = sb.ToString();

		//Eliminate any double spaces and replace with single spaces.  
		while(phrase.Contains("  ")) phrase = phrase.Replace("  ", " ");

		//Eliminate everything in parentheses.  
		if (phrase.IndexOf ("(") != -1 && phrase.IndexOf (")") != -1)
		{
			phrase = phrase.Substring (0, phrase.IndexOf ("(")) + phrase.Substring(phrase.IndexOf (")") + 1);
		}

		//Separate string into individual words.  
		List <string> keywords = new List <string> ();

		string[] splitString = phrase.Split (' ');
		foreach (string s in splitString)
			keywords.Add (s);

		//Remove all predetermined unnecessary words.  
		string[] unnecessaryPhrases = {"the", "a", "an", "almost", "exactly", "etc", "etc.", "are", "is", "aspect", "as", "because", "basically", "completely", "for", "when"};

		foreach (string un in unnecessaryPhrases)
		{
			for (int i = 0; i < keywords.Count; i++)
			{
				if (keywords[i].Equals(un))
					keywords.RemoveAt (i);
			}
		}

		//Remove all empty strings (that somehow pop every now and then)
		for (int i = 0; i < keywords.Count; i++)
			if (keywords [i].Equals (""))
				keywords.RemoveAt (i);

		return keywords.ToArray();
	}

	//Make it possible to edit the question.  
	public void OnToggleQuestionFieldEditability()
	{
		questionField.interactable = !questionField.interactable;
	}

	//Update the QAPair class accordingly.  
	public void OnEditRealQuestion()
	{
		questionOrder [0].UpdateQuestion (questionField.text);
	}

	//Make it possible to edit the answer.  
	public void OnToggleAnswerFieldEditability()
	{
		actualAnswerField.interactable = !actualAnswerField.interactable;
	}

	//Change the QAPair class accordingly.  
	public void OnEditRealAnswer()
	{
		questionOrder [0].UpdateAnswer (actualAnswerField.text);
	}

	//Move the question and the user answer out of view, move the actual answer to the top of the screen, and then have the user type ofut the answer exactly as it appears
	int indexOfDuplicatedQuestion = -1;

	public void OnButtonPress()
	{
		switch (state)
		{
		case ButtonState.IGIVEUP:
			//Slide up the actual answer field so that it can be copied down.  
			actualAnswerField.transform.GetComponent <Animator> ().SetTrigger ("SlideUp");
			//Add this question to the list at a random point again.  
			questionOrder [0].incorrectlyAnswered++;
			indexOfDuplicatedQuestion = Random.Range (1, questionOrder.Count);
			questionOrder.Insert (indexOfDuplicatedQuestion, questionOrder [0]);
			//Update the slider.  
			UpdatePercentageCompleteSlider ();
			SetButtonState (ButtonState.IWASRIGHT);
			break;
		case ButtonState.IWASRIGHT: 
			//Since I Give Up must have been pressed to get here, remove the extra addition from the lsit.  
			questionOrder.RemoveAt (indexOfDuplicatedQuestion);
			indexOfDuplicatedQuestion = -1;
			//Slide the actual answer field back down.  
			actualAnswerField.GetComponent <Animator> ().SetTrigger ("SlideDown");
			OnQuestionComplete ();
			break;
		case ButtonState.IWASWRONG: 
			//Slide up the actual answer field so that it can be copied down.  
			actualAnswerField.transform.GetComponent <Animator> ().SetTrigger ("SlideUp");
			//Add this question to the list at a random point again.  
			questionOrder [0].incorrectlyAnswered++;
			questionOrder.Insert (Random.Range (1, questionOrder.Count), questionOrder [0]);
			//Update the slider.  
			break;
		}
	}

	public void OnQuestionComplete()
	{
		//Prevent the user from changing their question.  
		userAnswerField.interactable = false;

		//Like quizlet.  
		userAnswerField.textComponent.color = Color.green;

		userAnswerField.GetComponent <Animator> ().SetTrigger ("SlideUp");

		Invoke("OnNextQuestion", 2f);
	}

	//Update percentage complete slider.  
	private void UpdatePercentageCompleteSlider()
	{
		//Calculate percent complete
		float percentComplete = (float) (QAPair.setQuestions.Count - questionOrder.Count) / QAPair.setQuestions.Count;
		percentageStudyingComplete.value = percentComplete;
		//Update the color.  
		percentageStudyingComplete.transform.FindChild ("Fill Area").GetChild (0).GetComponent <Image> ().color = Color.Lerp (Color.red, Color.green, percentComplete);
		//Update the text
		percentageStudyingComplete.transform.FindChild("Fill Area").GetChild(0).GetChild(0).GetComponent <Text> ().text = "" + ((int) ((percentComplete * 100))) + "%";
	}


	//Reset everything to its original state, and ask another question.  
	public void OnNextQuestion()
	{
		//Remove question just done.  
		questionOrder.RemoveAt (0);

		if (questionOrder.Count > 0)
		{
			//Update fields with new question and answer.  
			questionField.text = questionOrder [0].GetQuestion ();
			actualAnswerField.text = questionOrder [0].GetAnswer ();

			//Determine the keywords for the new answer once rather than a shit ton of times.  
			actualAnswerKeywords = DeterminePhraseKeywords (actualAnswerField.text);

			//Reset user answer field.  
			userAnswerField.interactable = true;
			userAnswerField.textComponent.color = Color.green;
			userAnswerField.GetComponent <Animator> ().SetTrigger ("SlideDown");
			userAnswerField.text = "";
			userAnswerField.textComponent.color = Color.black;

			//Update both sliders.  
			OnUserAnswerChanged ();
			UpdatePercentageCompleteSlider ();

			//Reset the button to its original state.  
			SetButtonState (ButtonState.IGIVEUP);
		} else
		{
			QuizToggle.instance.ToggleQuizMode ();
		}
	}
		
	//Reset the list, get ready for when the user starts to ask again.  
	public void Deactivate()
	{
		questionOrder = null;
	}

	//Button States
	enum ButtonState {IWASRIGHT, IWASWRONG, IGIVEUP}

	private ButtonState state;

	private void SetButtonState(ButtonState state)
	{
		this.state = state;
		switch (state)
		{
		case ButtonState.IWASRIGHT: 
			iGiveUpButton.interactable = true;
			iGiveUpButton.GetComponent <Image> ().color = Color.green;
			iGiveUpButton.transform.GetChild (0).GetComponent <Text> ().text = "I WAS RIGHT!";
			break;
		case ButtonState.IWASWRONG: 
			iGiveUpButton.interactable = true;
			iGiveUpButton.GetComponent <Image> ().color = Color.red;
			iGiveUpButton.transform.GetChild (0).GetComponent <Text> ().text = "I WAS WRONG!";
			break;
		case ButtonState.IGIVEUP: 
			iGiveUpButton.interactable = true;
			iGiveUpButton.GetComponent <Image> ().color = Color.gray;
			iGiveUpButton.transform.GetChild (0).GetComponent <Text> ().text = "I GIVE UP!";
			break;
		}
	}

}
