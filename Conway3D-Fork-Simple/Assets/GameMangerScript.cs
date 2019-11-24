using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMangerScript : MonoBehaviour
{
    public GameObject cubeBase;
	public GameObject cube;
	private bool[, , ,] state; // [x][y][z][LAYER] - layer: whether current render or next
	private int gridWidth;
	private int gridHeight;
	private int gridDepth;
	private float stepTime; // time between grid update
	private int cubeLen;
	private Vector3 worldCentre;
	private GameObject[, ,] cubeArray;

	int starvationValue;
	int overpopulationValue;
	int generationLowerValue;
	int generationUpperValue;
	

	// Start is called before the first frame update
    void Start()
    {
		Camera.main.transform.position = new Vector3(50f, 50f, 50f);
        Camera.main.transform.LookAt(new Vector3(0,0,0));

		stepTime = 1f;

		cubeLen = 2;
		
		SetupGame(5, 5, 5);
		RandomiseState();

		// draw lines to show the outside of our matrix
		DrawMatrixBorder();

		StartCoroutine("GameStep");
    }

	private void SetupGame(int matrixW, int matrixH, int matrixD) {
		gridWidth = matrixW;
		gridHeight = matrixH;
		gridDepth = matrixD;

		worldCentre = new Vector3(-(gridWidth/2)*cubeLen, -(gridHeight/2)*cubeLen, -(gridDepth/2)*cubeLen);
		state = new bool[gridWidth, gridHeight, gridDepth, 2];

		if (cubeArray != null)
		{
			foreach (GameObject cube in cubeArray)
				Object.Destroy(cube);
		}

		cubeArray = new GameObject[gridWidth, gridHeight, gridDepth];
	}

	private void RandomiseState() {
		for (int x=0; x<gridWidth; x++) {
			for (int y=0; y<gridHeight; y++) {
				for (int z=0; z<gridDepth; z++) {	
					if (Random.Range(0,1f) <= 0.5)
						state[x, y, z, 0] = true;
					else
						state[x, y, z, 0] = false;

					state[x, y, z, 1] = false;
				}
			}
		}
	}

	private IEnumerator GameStep() {
        while (true) {
			// find out how many neighbours each cell has
			for (int x=0; x<gridWidth; x++) {
				for (int y=0; y<gridHeight; y++) {
					for (int z=0; z<gridDepth; z++) {	
						int cnt = countNeighbours(x,y,z);

						// apply life/death rules on NEXT LAYER
						if (cnt < starvationValue || cnt > overpopulationValue) // if starvation/ overpopulation values met
							state[x,y,z,1] = false; // kill the cell
						if (cnt > generationLowerValue && cnt < generationUpperValue) // if the range of values for spontaneous gen met
							state[x,y,z,1] = true; // IT'S ALIIIVVVEEEE!!!!

						if (state[x,y,z,1]) {
							if (cubeArray[x,y,z] == null) {  // if there's not already a cube in place from last cycle
								cubeArray[x,y,z] = GameObject.CreatePrimitive(PrimitiveType.Cube);;
								cube = cubeArray[x,y,z];
								cube.transform.localScale = new Vector3(cubeLen, cubeLen, cubeLen);
								cube.transform.position = new Vector3(worldCentre.x+(x*cubeLen), worldCentre.y+(y*cubeLen), worldCentre.z+(z*cubeLen));
								

								float hue = ((float)y/(float)gridHeight); // entire rainbow should be run through from bottom floor to top
								//float val = (((float)x/(float)gridWidth)/2f + .5f); // smaller range for value, don't want a mass of hard-to-distinguish black cubes
								cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(hue, 1f, 1f);
							}
						}
						else { // if it's a dead cell this cycle
							if (cubeArray[x,y,z] != null) // if there's a cube here from last run
								Object.Destroy(cubeArray[x,y,z]);
						}
					}
				}
			}
			// make current layer become next
			for (int x=0; x<gridWidth; x++) {
				for (int y=0; y<gridHeight; y++) {
					for (int z=0; z<gridDepth; z++) {	
						state[x,y,z,0] = state[x,y,z,1];

					}
				}
			}
			yield return new WaitForSeconds(stepTime); // wait stepTime seconds before recalculating and re-rendering all over again
		}
	}

	private int countNeighbours(int x, int y, int z) {
		// return number of neighbours
		int cnt = 0;

		int n = 0;

		// find all 26 neigbours (and also self,see below)
		for (int xx=-1; xx<2; xx++) {
				for (int yy=-1; yy<2; yy++) {
					for (int zz=-1; zz<2; zz++) {	
						if (isAlive(x+xx, y+yy, z+zz))
							cnt++;
						n++;
					}
				}
		}
			
		if (isAlive(x, y, z)) // if true then we counted the block itself above
			cnt--; // so uncount self

		return cnt;
	}

	private bool isAlive(int x, int y, int z) {
		// Wrap values if off grid
		if (x < 0)
			x = (gridWidth-1);
		if (x >= gridWidth)
			x = 0;

		if (y < 0)
			y = (gridHeight-1);
		if (y >= gridHeight)
			y = 0;

		if (z < 0)
			z = (gridDepth-1);
		if (z >= gridDepth)
			z = 0;
		
		// if provided cell (wrapped) is alive, return true
		if (state[x,y,z,0] == true)
			return true;
		
		return false;
	}

	void DrawMatrixBorder() {
		// //Color lineCol = new Color(1, 1, 1);
		// //DrawLine(worldCentre + Vector3 ((cubeLen*gridWidth)/2), worldCentre - Vector3 ((cubeLen*gridWidth)/2), Color lineCol);
		// float xDist = (cubeLen*gridWidth);
		// float yDist = (cubeLen*gridHeight);
		// float zDist = (cubeLen*gridDepth);
		// Vector3 zero = worldCentre;//new Vector3(0,0,0); // vector3.zero shorthand

		// // DONE
		// DrawLine(new Vector3(zero.x - xDist, zero.y - yDist, zero.z + zDist), new Vector3(zero.x - xDist, zero.y + yDist, zero.z + zDist));
		// DrawLine(new Vector3(zero.x - xDist, zero.y + yDist, zero.z - zDist), new Vector3(zero.x - xDist, zero.y + yDist, zero.z + zDist));
		// DrawLine(new Vector3(zero.x - xDist, zero.y + yDist, zero.z - zDist), new Vector3(zero.x + xDist, zero.y + yDist, zero.z - zDist));
		// DrawLine(new Vector3(zero.x + xDist, zero.y + yDist, zero.z - zDist), new Vector3(zero.x + xDist, zero.y - yDist, zero.z - zDist));

		// // NOT DONE:
		// DrawLine(new Vector3(zero.x - xDist, zero.y - yDist, zero.z + zDist), new Vector3(zero.x - xDist, zero.y + yDist, zero.z + zDist));
		// DrawLine(new Vector3(zero.x - xDist, zero.y - yDist, zero.z + zDist), new Vector3(zero.x - xDist, zero.y + yDist, zero.z + zDist));
		// DrawLine(new Vector3(zero.x - xDist, zero.y - yDist, zero.z + zDist), new Vector3(zero.x - xDist, zero.y + yDist, zero.z + zDist));
		// DrawLine(new Vector3(zero.x - xDist, zero.y - yDist, zero.z + zDist), new Vector3(zero.x - xDist, zero.y + yDist, zero.z + zDist));

		// DrawLine(new Vector3(zero.x - xDist, zero.y - yDist, zero.z + zDist), new Vector3(zero.x - xDist, zero.y + yDist, zero.z + zDist));
		// DrawLine(new Vector3(zero.x - xDist, zero.y - yDist, zero.z + zDist), new Vector3(zero.x - xDist, zero.y + yDist, zero.z + zDist));
		// DrawLine(new Vector3(zero.x - xDist, zero.y - yDist, zero.z + zDist), new Vector3(zero.x - xDist, zero.y + yDist, zero.z + zDist));
		// DrawLine(new Vector3(zero.x - xDist, zero.y - yDist, zero.z + zDist), new Vector3(zero.x - xDist, zero.y + yDist, zero.z + zDist));
	} 


     void DrawLine(Vector3 start, Vector3 end)
	{
		GameObject myLine = new GameObject();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		//lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		// lr.SetColors(color, color);
		Color color = new Color(1f, 1f, 1f);
		lr.startColor = color;
		lr.endColor = color;
		// lr.SetWidth(0.1f, 0.1f);
		lr.startWidth = 1f;
		lr.endWidth = 1f;
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		//GameObject.Destroy(myLine, duration);
	}





    // Update is called once per frame
    void Update()
    {
        // resatrt simulation
		if (Input.GetKeyDown(KeyCode.R)) {
			StopCoroutine("GameStep");
			RandomiseState();
			StartCoroutine("GameStep");
		}


		// CAMERA
		Vector3 centre = new Vector3(0,0,0);
		
		// Zoom in and out
        if (Input.GetKey("up") && Vector3.Distance(Vector3.zero, Camera.main.transform.position) > 2) // don't allow to zoom to far in, toherwise can't zoom back out
        {
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, centre, 50f * Time.deltaTime);
        }
        if (Input.GetKey("down"))
        {
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, centre, -50f * Time.deltaTime);
        }

        // Rotate camera around on x plane (left to right)
        if (Input.GetKey(KeyCode.A))
        {
            Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.up, 50f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.up, -50f * Time.deltaTime);
        }

		// Rotate camera around on y plane (top to bottom)
        if (Input.GetKey(KeyCode.W))
        {
            Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.right, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.right, -1);
        }

		// INCREASE COMPLEXITY
		if (Input.GetKeyDown(KeyCode.P))
        {
            SetupGame(gridWidth+=5, gridHeight+=5, gridDepth+=5);
			Debug.Log("increase, s= " + gridWidth);
			RandomiseState();
        }
		// DECREASE COMPLEXITY
		if (Input.GetKeyDown(KeyCode.L))
        {
            SetupGame(gridWidth-=5, gridHeight-=5, gridDepth-=5);
			Debug.Log("decrease, s= " + gridWidth);
			RandomiseState();
        }

    }

	public void restartGame(int x, int y, int z, int starve, int overpop, int genLower, int genUpper){
		int vert, hor, zert;
		//vert = hor = zert = 5;

		vert = y;
		hor = x;
		zert = z;

		starvationValue = starve;
		overpopulationValue = overpop;
		generationLowerValue = genLower;
		generationUpperValue = genUpper;
		
		// if TryParse kicks up a fuss over any of the user's dimension inputs
		/*if (!(int.TryParse(x, out vert) && int.TryParse(y, out hor) && int.TryParse(z, out zert)))
			vert = -1; // atomatically fail the next if statement
*/
/*
		int vert = int.Parse(verticalField.text);
		int hor = int.Parse(horizontalField.text);
		int zert = int.Parse(zertizontalField.text);*/
		
		// if none of our three dimension fields are blank
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

		Debug.Log (	starvationValue + ", " + overpopulationValue +", " +  generationLowerValue +", " + 	generationUpperValue );
	}
}
