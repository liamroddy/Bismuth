using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMangerScript : MonoBehaviour
{
	private bool[,,,] state; // [x][y][z][LAYER] - layer: whether current render or next
	private int gridWidth;
	private int gridHeight;
	private int gridDepth;
	private float stepTime; // time between grid update
	private int cubeLen;
	private Vector3 gridAdjust; // used to offset grid so that the centre of the grid aligns with Vector3.zero
	private GameObject[,,] cubeArray;

	int starvationValue;
	int overpopulationValue;
	int generationLowerValue;
	int generationUpperValue;


	// Start is called before the first frame update
	void Start()
	{
		Camera.main.transform.position = new Vector3(50f, 50f, 50f);
		Camera.main.transform.LookAt(new Vector3(0, 0, 0));

		stepTime = 1f; // update once a second
		cubeLen = 2; // cubes are 2 unity distance units in each dimension

		StartCoroutine("GameStep");
	}

	// set-up the actual grid based on the user's specified dimensions
	private void SetupGame(int matrixW, int matrixH, int matrixD)
	{
		gridWidth = matrixW;
		gridHeight = matrixH;
		gridDepth = matrixD;

		gridAdjust = new Vector3(-(gridWidth / 2) * cubeLen, -(gridHeight / 2) * cubeLen, -(gridDepth / 2) * cubeLen);// offset grid so centre of the grid aligns with Vector3.zero
		state = new bool[gridWidth, gridHeight, gridDepth, 2];

		// if the cube array isn't empty/unitialised, clear it
		if (cubeArray != null)
		{
			foreach (GameObject cube in cubeArray)
				Object.Destroy(cube);
		}

		cubeArray = new GameObject[gridWidth, gridHeight, gridDepth];
	}

	// runs through £D state grid, set elements as either true or false at random
	private void RandomiseState()
	{
		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				for (int z = 0; z < gridDepth; z++)
				{
					if (Random.Range(0, 1f) <= 0.5)
						state[x, y, z, 0] = true;
					else
						state[x, y, z, 0] = false;

					state[x, y, z, 1] = false;
				}
			}
		}
	}

	private IEnumerator GameStep()
	{
		while (true)
		{
			// find out how many neighbours each cell has
			for (int x = 0; x < gridWidth; x++)
			{
				for (int y = 0; y < gridHeight; y++)
				{
					for (int z = 0; z < gridDepth; z++)
					{
						int cnt = countNeighbours(x, y, z);

						// apply life/death rules on NEXT LAYER
						if (cnt < starvationValue || cnt > overpopulationValue) // if starvation/ overpopulation values met
							state[x, y, z, 1] = false; // kill the cell
						if (cnt > generationLowerValue && cnt < generationUpperValue) // if the range of values for spontaneous gen met
							state[x, y, z, 1] = true; // IT'S ALIIIVVVEEEE!!!!

						if (state[x, y, z, 1])
						{
							if (cubeArray[x, y, z] == null)
							{  // if there's not already a cube in place from last cycle
								cubeArray[x, y, z] = GameObject.CreatePrimitive(PrimitiveType.Cube); ;
								cubeArray[x, y, z].transform.localScale = new Vector3(cubeLen, cubeLen, cubeLen);
								cubeArray[x, y, z].transform.position = new Vector3(gridAdjust.x + (x * cubeLen), gridAdjust.y + (y * cubeLen), gridAdjust.z + (z * cubeLen));


								float hue = ((float)y / (float)gridHeight); // entire rainbow should be run through from bottom floor to top
								cubeArray[x, y, z].GetComponent<Renderer>().material.color = Color.HSVToRGB(hue, 1f, 1f);
							}
						}
						else
						{ // if it's a dead cell this cycle
							if (cubeArray[x, y, z] != null) // if there's a cube here from last run
								Object.Destroy(cubeArray[x, y, z]);
						}
					}
				}
			}
			// make current layer become next
			for (int x = 0; x < gridWidth; x++)
			{
				for (int y = 0; y < gridHeight; y++)
				{
					for (int z = 0; z < gridDepth; z++)
					{
						state[x, y, z, 0] = state[x, y, z, 1];

					}
				}
			}
			yield return new WaitForSeconds(stepTime); // wait stepTime seconds before recalculating and re-rendering all over again
		}
	}

	// return number of neighbours for given cell
	private int countNeighbours(int x, int y, int z)
	{
		int cnt = 0;

		// find all 26 neigbours (includes self,see below)
		for (int xx = -1; xx < 2; xx++)
		{
			for (int yy = -1; yy < 2; yy++)
			{
				for (int zz = -1; zz < 2; zz++)
				{
					if (isAlive(x + xx, y + yy, z + zz))
						cnt++;
				}
			}
		}

		if (isAlive(x, y, z)) // if true then we counted the block itself above
			cnt--; // so uncount self

		return cnt;
	}

	// returns if given cell is alive (accounts for edge wrapping)
	private bool isAlive(int x, int y, int z)
	{
		// Wrap values if off grid
		if (x < 0)
			x = (gridWidth - 1);
		if (x >= gridWidth)
			x = 0;

		if (y < 0)
			y = (gridHeight - 1);
		if (y >= gridHeight)
			y = 0;

		if (z < 0)
			z = (gridDepth - 1);
		if (z >= gridDepth)
			z = 0;

		// if provided cell (wrapped) is alive, return true
		if (state[x, y, z, 0] == true)
			return true;

		return false;
	}

	// Update is called once per frame
	void Update()
	{
		// MOUSE CONTROLS		
		if (Input.GetMouseButton(0))
		{

			Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.up, 500f * Time.deltaTime * Input.GetAxis("Mouse X"));
			Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.right, -500f * Time.deltaTime * Input.GetAxis("Mouse Y"));
		}

		float scrollAmount = Input.GetAxis("Mouse ScrollWheel");


		if ((scrollAmount < 0 && Vector3.Distance(Vector3.zero, Camera.main.transform.position) > 25f) || scrollAmount > 0) // don't allow to zoom to far in, ooherwise can't zoom back out
			Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, Vector3.zero, (1 / scrollAmount) * -70f * Time.deltaTime);


		// KEYBOARD CONTROLS:
		Vector3 centre = Vector3.zero;

		// Zoom in and out
		if (Input.GetKey("up") && Vector3.Distance(Vector3.zero, Camera.main.transform.position) > 25f) // don't allow to zoom to far in, ooherwise can't zoom back out
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
			Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.right, 50f * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.S))
		{
			Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.right, -50f * Time.deltaTime);
		}

	}

	public void StartGame(int x, int y, int z, int starve, int overpop, int genLower, int genUpper)
	{
		// if our conditions field aren't blank or invalid
		if (starve > 0 && overpop > 0 && genLower > 0 && genUpper > 0)
		{
			starvationValue = starve;
			overpopulationValue = overpop;
			generationLowerValue = genLower;
			generationUpperValue = genUpper;

			// if none of our three dimension fields are blank
			if (x > 0 && y > 0 && z > 0)
			{
				gridDepth = z;
				gridHeight = y;
				gridWidth = x;

				// set up and start
				SetupGame(gridWidth, gridHeight, gridDepth);
				RandomiseState();
			}
			else
				Debug.Log("Dimension input invalid!");
		}
		else
			Debug.Log("Conditions input invalid!");
	}
}
