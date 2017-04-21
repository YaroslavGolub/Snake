using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace SnakeGame
{

    public class SnakeManager : MonoBehaviour
    {
        private const int GRID_WIDTH = 20;
        private const int GRID_HEIGHT = 10;

        private int[,] _grid;

        [SerializeField]
        private int _initialSnakeLength = 3;

        [SerializeField]
        private int _initialFoodAmnt;

        private int _snakeLength;

        [SerializeField]
        [Range(0, 19)]
        private int _snakeX;
        [SerializeField]
        [Range(0, 9)]
        private int _snakeY;

        private Transform _snakeTransform;
        private float _lastMove;

        private Vector3 _direction;

        public Text scoreText;

        private bool _lost = true;
        private bool _speedEffect;

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

            transform.position = new Vector3(_snakeX, _snakeY);

            _moveRate = 1 / _movesPerSecond;

            _grid = new int[GRID_WIDTH, GRID_HEIGHT];

            _snakeTransform = this.transform;
            _direction = Vector3.right;
            _grid[_snakeX, _snakeY] = _snakeLength;

            UpdateText();

            for (int i = 0; i < _initialFoodAmnt; i++)
            {
                SpawnFood();
            }
        }

        private void Update()
        {
            if (_lost)
                return;

            Move();
            GetInput();
        }

        private void Move()
        {
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

                var go = _objPooler.GetPooledObject();
                go.SetActive(true);
                go.transform.position = new Vector3(_snakeX, _snakeY, 0);
                go.name = _snakeX.ToString() + _snakeY;

                SetDirection();
                VerifyNextMove();

                if (_grid[_snakeX, _snakeY] < 0)
                {
                    EatFood();
                }
                else if (_grid[_snakeX, _snakeY] > 0)
                {
                    GameOver();
                    _lost = true;
                    return;
                }

                _snakeTransform.position = new Vector3(_snakeX, _snakeY);
                _grid[_snakeX, _snakeY] = _snakeLength;
            }

        }

        private void SetDirection()
        {
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
        }

        private void EatFood()
        {
            var currentFood = _grid[_snakeX, _snakeY];
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
                case (int)Food.ReverseMotion:
                    ReverseSnake();
                    break;

                default:
                    break;
            }
            SpawnFood();
        }
        private void UpdateText()
        {
            scoreText.text = "Snake Length: " + _snakeLength;
        }
        /// <summary>
        /// Teleports to anouther side if snake is facing to the edge of the gameBoard
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
            StartCoroutine(SpecialEffect());
        }
        private void MakeSpeedUpMotion()
        {
            StartCoroutine(SpecialEffect(true));
        }
        private void ReverseSnake()
        {
            print("reflecting snake");
        }
        private void GameOver()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        private IEnumerator SpecialEffect(bool speedUp = false)
        {
            if (_speedEffect)
                yield break;

            _speedEffect = true;         // prevents double acceleration/deceleration
            float temp = _moveRate;
            _moveRate = (speedUp) ? _moveRate / 2.0f : _moveRate * 2.0f;
            yield return new WaitForSeconds(_specialEffectDuration);
            _moveRate = temp;
            _speedEffect = false;
        }
        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("food"))
            {
                other.gameObject.SetActive(false);
            }
        }
        public void StartGame()
        {
            _lost = false;
        }
    }
}
