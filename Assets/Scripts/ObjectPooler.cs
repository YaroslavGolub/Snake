﻿using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    public class ObjectPooler : MonoBehaviour, IPooler
    {
        [SerializeField]
        private GameObject _prefab;
        [SerializeField]
        private int _startAmn;

        private List<GameObject> _objList = new List<GameObject>();

        private void Start()
        {
            for (int i = 0; i < _startAmn; i++)
            {
                AddNewObject();
            }
        }

        public GameObject GetPooledObject()
        {
            for (int i = 0; i < _objList.Count; i++)
            {
                if (!_objList[i].activeSelf)
                {
                    return _objList[i];
                }
            }

            return AddNewObject();
        }

        private GameObject AddNewObject()
        {
            var go = Instantiate(_prefab) as GameObject;
            go.transform.SetParent(this.transform);
            go.SetActive(false);
            _objList.Add(go);
            return go;
        }
    }


}