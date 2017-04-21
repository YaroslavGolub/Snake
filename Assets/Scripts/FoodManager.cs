using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    public class FoodManager : MonoBehaviour, IFoodManager
    {
        [SerializeField]
        private GameObject _foodPrefab;
        [SerializeField]
        private Color[] _foodColors;
        [SerializeField]
        private int _startAmn;
        private List<GameObject> _foodList = new List<GameObject>();

        private void Start()
        {
            for (int i = 0; i < _startAmn; i++)
            {
                AddNewObject();
            }
        }

        private GameObject GetPooledFood()
        {
            for (int i = 0; i < _foodList.Count; i++)
            {
                if (!_foodList[i].activeSelf)
                {
                    return _foodList[i];
                }
            }

            return AddNewObject();
        }

        private GameObject AddNewObject()
        {
            var go = Instantiate(_foodPrefab) as GameObject;
            go.SetActive(false);
            _foodList.Add(go);
            return go;
        }

        public int SpawnFood(int x, int y)
        {
            var foodType = UnityEngine.Random.Range((int)Food.Common, (int)Food.SpeedUpMotion - 1);
            var go = GetPooledFood();

            go.GetComponent<Renderer>().material.color = _foodColors[-(foodType + 1)];
            go.transform.position = new Vector3(x, y, 0.0f);
            go.SetActive(true);
            return foodType;
        }
    } 
}

