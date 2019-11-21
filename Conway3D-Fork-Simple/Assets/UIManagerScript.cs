using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour
{
    public GameObject gameManager;
	
	public Button RestartButton;

	public InputField verticalField;
	public InputField horizontalField;
	public InputField zertizontalField;

	public InputField StarvationField;
	public InputField OverpopulationField;
	public InputField GenerationLowerField;
	public InputField GenerationUpperField;
	
	// Start is called before the first frame update
    void Start()
    {
        Button restart = RestartButton.GetComponent<Button>();
		restart.onClick.AddListener(restartGame);
    }

	void restartGame(){
		int vert, hor, zert;
		vert = hor = zert = -1;
		
		// if TryParse kicks up a fuss over any of the user's dimension inputs
		if (!(int.TryParse(verticalField.text, out vert) && int.TryParse(horizontalField.text, out hor) && int.TryParse(zertizontalField.text, out zert)))
			vert = -1; // atomatically fail the next if statement

		int starve, overpop, genLower, genUpper;

		starve = overpop = genLower = genUpper = -1;

		if (!(int.TryParse(StarvationField.text, out starve) && int.TryParse(OverpopulationField.text, out overpop) && int.TryParse(GenerationLowerField.text, out genLower)  && int.TryParse(GenerationUpperField.text, out genUpper)))
			vert = -1; // atomatically fail the next if statement



		//gameManager.restartGame(hor, vert, zert);
		gameManager.GetComponent<GameMangerScript>().restartGame(hor, vert, zert, starve, overpop, genLower, genUpper);

		//ScriptName scriptToAccess = gameManager.GetComponent<ScriptName>();
		// get the script on the object (make sure the script is a public class)      
		
		//scriptToAccess.restartGame(hor, vert, zert);
		// calls the method in the script on the other object.

/*
		int vert = int.Parse(verticalField.text);
		int hor = int.Parse(horizontalField.text);
		int zert = int.Parse(zertizontalField.text);*/
		
		// if none of our three dimension fields are blank
		/*
		if(vert>0 && hor>0 && zert>0) {
			gridDepth=zert;
			gridHeight = vert;
			gridWidth = hor;
			
			// set up and start
			Debug.Log ("Dimensions valid!! " + hor + "x" + vert + "x" + zert);
			SetupGame(gridWidth, gridHeight, gridDepth);
			RandomiseState();
		}
		else
			Debug.Log ("Dimension input invalid!");

			*/
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
