using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class SetManager : MonoBehaviour {

	public static SetManager instance;
	void Awake() {instance = this;}

	public GameObject qaPairPrefab = null;

	public Text title = null, numberOfSets = null;

	void Start() 
	{
		LoadQAPairs (HomeScreenManager.instance.obtainedPath);
	}

	public void AddEmptyQAPair()
	{
		QAPair.Create ("", "");
	}

	public void LoadQAPairs(string path)
	{
		try
		{
			string line;
			// Create a new StreamReader, tell it which file to read and what encoding the file
			// was saved as
			StreamReader theReader = new StreamReader (path, Encoding.Default);
			// Immediately clean up the reader after this block of code is done.
			// You generally use the "using" statement for potentially memory-intensive objects
			// instead of relying on garbage collection.
			// (Do not confuse this with the using directive for namespace at the 
			// beginning of a class!)
			string titleText = "";
			using (theReader)
			{
				//For some reason  a new string variable must be created.  
				titleText = theReader.ReadLine(); //There must be a line no matter what.  

				// While there's lines left in the text file, do this:
				do
				{
					try 
					{
						line = theReader.ReadLine ();
					} 
					catch (System.Exception e)
					{
						line = null;
					}

					if (line != null)
					{
						// Do whatever you need to do with the text line, it's a string now
						// In this example, I split it into arguments based on comma
						// deliniators, then send that array to DoStuff()
						if (line.Length > 1 && line.IndexOf("~") != -1)
						{
							string[] entries = line.Split ('~');
							string question = entries[0], answer = entries[1];
							Debug.Log("Creating QAPair " + question + ", " + answer);
							if (!(question.Equals("") && answer.Equals("")))
								QAPair.Create(question, answer);
						}
					}
				} while (line != null);
				// Done reading, close the reader and return true to broadcast success    
				theReader.Close ();
			}

			title.text = titleText;
		}
		// If anything broke in the try block, we throw an exception with information
		// on what didn't work
		catch (System.Exception e)
		{
			Debug.LogError ("Fatal error in Reading File!");
		}
	}

	public void SaveQAPairs()
	{
		try 
		{
			string toOutputToFile = title.text + "\n";
			for (int i = 0; i < QAPair.setQuestions.Count; i++)
			{
				toOutputToFile = toOutputToFile + QAPair.setQuestions[i].ToString() + "\n";
			}
			System.IO.File.WriteAllText(HomeScreenManager.instance.obtainedPath, toOutputToFile);
		} 
		catch (System.Exception e)
		{
			Debug.LogError("Couldn't write to file!");
		}
	}

}
