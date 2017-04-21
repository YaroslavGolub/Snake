using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Snake : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;

    private int[,] _grid;

    [SerializeField]
    private int _initialSnakeLength = 3;

    private int _snakeLength;

    [Header("Only whole numbers!")]
    [SerializeField]
    private Vector2 _snakeStartPosition;

    private int _snakeX;
    private int _snakeY;

    private Transform _snakeTransform;
    private float _lastMove;

    private Vector3 _direction;

    public GameObject foodPrefab;
    public Text scoreText;

    private bool lost;
    private bool speedEffect;

    [SerializeField]
    [Range(0.5f, 10.0f)]
    private float _movesPerSecond;
    private float _moveRate;

    [SerializeField]
    private float _specialEffectDuration;

    public GameObject objPooler;
    private IPooler _objPooler;

    public GameObject foodManager;
    private IFoodManager _foodManager;

    private void Start()
    {
        _objPooler = objPooler.GetComponent<IPooler>();
        _foodManager = foodManager.GetComponent<IFoodManager>();

        _snakeLength = _initialSnakeLength;
        transform.position = _snakeStartPosition;
        _snakeX = (int)_snakeStartPosition.x;
        _snakeY = (int)_snakeStartPosition.y;

        _moveRate = 1 / _movesPerSecond;

        _grid = new int[gridWidth, gridHeight];

        _snakeTransform = this.transform;
        _direction = Vector3.right;
        _grid[_snakeX, _snakeY] = _snakeLength;

        UpdateText();

        SpawnFood();
        SpawnFood();
    }

    private void Update()
    {
        if (lost)
            return;

        if (Time.time - _lastMove > _moveRate)
        {
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    if (_grid[i, j] > 0)
                    {
                        _grid[i, j]--;
                        if (_grid[i, j] == 0)
                        {
                            GameObject goToMove = GameObject.Find(i.ToString() + j.ToString());
                            if (goToMove != null)
                            {
                                goToMove.SetActive(false);
                            }
                        }
                    }
                }
            }

            _lastMove = Time.time;

            GameObject go = _objPooler.GetPooledObject();
            go.SetActive(true);
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

            VerifyNextMove();

            if (_grid[_snakeX, _snakeY] < 0) // -1 incremental food; -2 decremental food; // -3 slowMo food // -4 
            {
                print("increment || dicrement size here");// if length <2 after decrement - GameOver or will get a bug;

                int currentFood = _grid[_snakeX, _snakeY];
                switch (currentFood)
                {
                    case (int)Food.Common:
                        IncreaseSnakeSize();
                        break;
                    case (int)Food.Decrement:
                        DecreaseSnakeSize();
                        break;
                    case (int)Food.SlowMotion:
                        MakeSlowMotion();
                        break;
                    case (int)Food.SpeedUpMotion:
                        MakeSpeedUpMotion();
                        break;
                    default:
                        break;
                }

                UpdateText();
                //GameObject goRoDestroy = GameObject.Find("increment food");
                //if (goRoDestroy != null)
                //{
                //    Destroy(goRoDestroy);
                //}

                //for (int i = 0; i < _grid.GetLength(0); i++)
                //{
                //    for (int j = 0; j < _grid.GetLength(1); j++)
                //    {
                //        if (_grid[i, j] > 0)
                //            _grid[i, j]++;
                //    }
                //}

                // spawn Food  
                SpawnFood();
            }
            else if (_grid[_snakeX, _snakeY] != 0) // Change on > 0
            {
                Debug.LogError("PutGameOverConditionHere");
                GameOver();
                lost = true;
                return;
            }

            //_snakeTransform.position += _direction;
            _snakeTransform.position = new Vector3(_snakeX, _snakeY);
            _grid[_snakeX, _snakeY] = _snakeLength;

        }

        GetInput();
    }

    private void UpdateText()
    {
        scoreText.text = "Snake Length: " + _snakeLength;
    }

    /// <summary>
    /// Teleports to anouther side if we facing to the gameBoard
    /// </summary>
    private void VerifyNextMove()
    {
        if (_snakeX >= _grid.GetLength(0))
        {
            _snakeX = 0;
        }
        else if (_snakeX < 0)
        {
            _snakeX = _grid.GetLength(0) - 1;
        }

        if (_snakeY >= _grid.GetLength(1))
        {
            _snakeY = 0;
        }
        else if (_snakeY < 0)
        {
            _snakeY = _grid.GetLength(1) - 1;
        }
    }

    private void GetInput()
    {
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
    private void SpawnFood()
    {
        bool foodSpawned = false;
        while (!foodSpawned)
        {
            int x = Random.Range(0, _grid.GetLength(0));
            int y = Random.Range(0, _grid.GetLength(1));
            if (_grid[x, y] == 0)
            {
                _grid[x, y] = _foodManager.SpawnFood(x, y);
                foodSpawned = true;
            }
        }
    }
    private void IncreaseSnakeSize()
    {
        _snakeLength++;
        for (int i = 0; i < _grid.GetLength(0); i++)
        {
            for (int j = 0; j < _grid.GetLength(1); j++)
            {
                if (_grid[i, j] > 0)
                    _grid[i, j]++;
            }
        }
        print("increaseSize");
    }

    private void DecreaseSnakeSize()
    {
        print("decreaseSize");
        _snakeLength--;
        if (_snakeLength <= 2)
            GameOver();
    }

    private void MakeSlowMotion()
    {
        print("slowWWW");
        StartCoroutine(SpecialEffect());

    }
    private void MakeSpeedUpMotion()
    {
        print("speedUp");
        StartCoroutine(SpecialEffect(true));
    }

    private void ReflectSnake()
    {
        print("reflecting snake");
    }

    private void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private IEnumerator SpecialEffect(bool speedUp = false)
    {
        if (speedEffect)
            yield break;

        speedEffect = true;         // prevents double acceleration/deceleration
        float temp = _moveRate;
        _moveRate = (speedUp) ? _moveRate / 2.0f : _moveRate * 2.0f;
        yield return new WaitForSeconds(_specialEffectDuration);
        _moveRate = temp;
        speedEffect = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("food"))
        {
            print("fooooood");
            other.gameObject.SetActive(false);
        }
    }
}

public enum Food
{
    Common = -1,
    Decrement = -2,
    SlowMotion = -3,
    SpeedUpMotion = -4
    // -1 incremental food; -2 decremental food; // -3 slowMo food // -4 
}
