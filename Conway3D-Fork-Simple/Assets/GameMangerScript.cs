using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMangerScript : MonoBehaviour
{
	private bool[,,,] state; // [x][y][z][LAYER] - layer: whether current render or next
	private float stepTime; // time between grid update
	private int cubeLen; // the length/breadth/depth of the rendered cubes
	private Vector3 gridAdjust; // used to offset grid so that the centre of the grid aligns with Vector3.zero
	private GameObject[,,] cubeArray; // a 3D array of to hold all our cube object

	// the 3 user set dimensions of the grid:
	private int gridWidth;
	private int gridHeight;
	private int gridDepth;

	// the rules for our game simulation:
	int starvationValue; // if number of neighbours is below this, the cell starves and dies
	int overpopulationValue; // if num of neighbours above this, there's overpopulation and the cell dies
	int generationLowerValue; // if neighbour count > than this AND -- 
	int generationUpperValue; // -- < than this, we've an ideal environment and a dead cell can spontaneously come to life


	// Start is called before the first frame update
	void Start()
	{
		stepTime = 1f; // update once a second
		cubeLen = 2; // cubes are 2 unity distance units in each dimension

		StartCoroutine("GameStep"); // start the game step timer going, ticking once every stepTime
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

	// runs through 3D state grid, set elements as either true or false at random
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

					state[x, y, z, 1] = false; // entire next state layer set to false right away (we don't know the future - yet)
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

	// Update is called once per frame
	void Update()
	{

	}
}
