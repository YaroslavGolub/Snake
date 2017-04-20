using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{

    public int gridWidth = 10;
    public int gridHeight = 10;
    private int[,] _grid;

    private int _initialLenth = 5;

    private int _snakeLength;
    private int _snakeX = 0;
    private int _snakeY = 4;

    private Transform _snakeTransform;
    private float _lastMove;
    private float _timeBetweenMoves = 0.35f;
    private Vector3 _direction;

    public GameObject foodPrefab;
    public Text scoreText;

    private bool lost;
    [SerializeField]
    [Range(0.5f, 10.0f)]
    private float _movesPerSecond;
    private float _moveRate;

    private void Start()
    {
        _snakeLength = _initialLenth;
        _moveRate = 1 / _movesPerSecond;
        _grid = new int[gridWidth, gridHeight];

        _snakeTransform = this.transform;
        _direction = Vector3.right;
        _grid[_snakeX, _snakeY] = _snakeLength;
        scoreText.text = "Score: " + _snakeLength;
        SpawnFood(8, 4, -1);
    }

    private void Update()
    {

        if (lost)
            return;

        if (Time.time - _lastMove > _moveRate)
        {
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                // X value
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    // Y value
                    if (_grid[i, j] > 0)
                    {
                        _grid[i, j]--;
                        if (_grid[i, j] == 0)
                        {
                            //destroyed something
                            GameObject goToDestroy = GameObject.Find(i.ToString() + j.ToString());
                            if (goToDestroy != null)
                            {
                                Destroy(goToDestroy);
                            }
                        }
                    }
                }
            }

            _lastMove = Time.time;

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = new Vector3(_snakeX, _snakeY, 0);
            go.name = _snakeX.ToString() + _snakeY.ToString();

            // Generates next move direction
            if (_direction.x == 1)
            {
                _snakeX++;
            }

            if (_direction.x == -1)
            {
                _snakeX--;
            }

            if (_direction.y == 1)
            {
                _snakeY++;
            }

            if (_direction.y == -1)
            {
                _snakeY--;
            }

            // Check the edge of the gameField
            // if snakeX>=_grid.width snakeX=0 == if(snakeY>=gridHeight) snakeY = 0
            //CheckNextMove

            if (_snakeX >= gridWidth)
            {
                _snakeX = 0;
            }
            else if (_snakeX < 0)
            {
                _snakeX = gridWidth - 1;
            }

            if (_snakeY >= gridHeight)
            {
                _snakeY = 0;
            }
            else if (_snakeY < 0)
            {
                _snakeY = gridHeight - 1;
            }


            if (_grid[_snakeX, _snakeY] == -1) // -1 incremental food; -2 decremental food; // -3 slowMo food // -4 
            {
                print("increment || dicrement size here");// if length <2 after decrement - GameOver or will get a bug;
                _snakeLength++;
                scoreText.text = "Score: " + _snakeLength;
                print("SOME FOOD THERE");
                GameObject goRoDestroy = GameObject.Find("increment food");
                if (goRoDestroy != null)
                {
                    Destroy(goRoDestroy);
                }

                for (int i = 0; i < _grid.GetLength(0); i++)
                {
                    for (int j = 0; j < _grid.GetLength(1); j++)
                    {
                        if (_grid[i, j] > 0)
                            _grid[i, j]++;
                    }
                }

                // spawn Food  
                bool foodSpawned = false;
                while (!foodSpawned)
                {
                    int x = Random.Range(0, _grid.GetLength(0));
                    int y = Random.Range(0, _grid.GetLength(1));
                    if (_grid[x, y] == 0)
                    {
                        SpawnFood(x, y, -1);
                        foodSpawned = true;
                    }
                }
            }
            else if (_grid[_snakeX, _snakeY] != 0) // Change on > 0
            {
                Debug.LogError("PutGameOverConditionHere");
                lost = true;
                return;
            }

            //_snakeTransform.position += _direction;
            _snakeTransform.position = new Vector3(_snakeX, _snakeY);
            _grid[_snakeX, _snakeY] = _snakeLength;


        }


        if (Input.GetKeyDown(KeyCode.W))
        {
            _direction = Vector3.up;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _direction = Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _direction = Vector3.down;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _direction = Vector3.right;
        }
    }

    private void SpawnFood(int x, int y, int foodVal)
    {
        _grid[x, y] = foodVal;
        GameObject go = Instantiate(foodPrefab) as GameObject;
        go.transform.position = new Vector3(x, y, 0);
        go.name = "increment food";
    }
}
