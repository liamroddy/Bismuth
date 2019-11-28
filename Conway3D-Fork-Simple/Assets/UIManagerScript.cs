using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour
{
    public GameObject GameManager;
	
	public Button StartButton;

	public InputField VerticalField;
	public InputField HorizontalField;
	public InputField DepthField;

	public InputField StarvationField;
	public InputField OverpopulationField;
	public InputField GenerationLowerField;
	public InputField GenerationUpperField;
	
	// Start is called before the first frame update
    void Start()
    {
        Button start = StartButton.GetComponent<Button>();
		start.onClick.AddListener(StartGame);
    }

	// parse the user-given values before passing them to the Game Manager
	void StartGame(){
		int y, x, z;
		y = x = z = -1;
		
		// if TryParse kicks up a fuss over any of the user's dimension inputs
		if (!(int.TryParse(VerticalField.text, out y) && int.TryParse(HorizontalField.text, out x) && int.TryParse(DepthField.text, out z)))
			y = -1; // automatically fail one of them

		int starve, overpop, genLower, genUpper;

		starve = overpop = genLower = genUpper = -1;

		// again, any invalid input and we automatically default a value to zero
		if (!(int.TryParse(StarvationField.text, out starve) && int.TryParse(OverpopulationField.text, out overpop) && int.TryParse(GenerationLowerField.text, out genLower)  && int.TryParse(GenerationUpperField.text, out genUpper)))
			starve = -1;


		GameManager.GetComponent<GameMangerScript>().StartGame(x, y, z, starve, overpop, genLower, genUpper);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
