using System;
using UnityEngine;

namespace GridSystem
{
    public class Grid<T>
    {
        public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
        public class OnGridValueChangedEventArgs : EventArgs
        {
            public int x;
            public int y;
        }

        private int _width;
        private int _height;
        private float _cellSize;
        private Vector3 _originPosition;
        private T[,] _gridArray;
        //private TextMeshPro[,] _debugTextArray;

        public Grid(int width, int height, float cellSize, Vector3 originPosition, Transform parent,
            Func<Grid<T>, int, int, T> createGridObj) // passing the grid object, x and y
        {
            //var showDebug = true;

            _width = width;
            _height = height;
            _cellSize = cellSize;
            _originPosition = originPosition;

            _gridArray = new T[width, height];

            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {
                    _gridArray[x, y] = createGridObj(this, x, y);
                }
            }

            //if (showDebug)
            //{
            //    _debugTextArray = new TextMeshPro[width, height];

            //    var offset = new Vector3(cellSize, cellSize) * 0.45f;
            //    var fontSize = (int)_cellSize * 5;

            //    for (int x = 0; x < _gridArray.GetLength(0); x++)
            //    {
            //        for (int y = 0; y < _gridArray.GetLength(1); y++)
            //        {
            //            _debugTextArray[x, y] = CreateWorldText(parent, _gridArray[x, y]?.ToString(), GetWorldPosition(x, y) + offset, fontSize,
            //                Color.white, TextAlignmentOptions.Center, 0);

            //            Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
            //            Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            //        }
            //    }
            //    Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            //    Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            //    OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
            //    {
            //        _debugTextArray[eventArgs.x, eventArgs.y].text = _gridArray[eventArgs.x, eventArgs.y]?.ToString();
            //    };
            //}
        }

        public int GetWolrdIndex(int x, int y)
        {
            return x + y * _width;
        }

        public float GetCellSize()
        {
            return _cellSize;
        }

        public int GetGridWidth()
        {
            return _width;
        }

        public int GetGridHeight()
        {
            return _height;
        }

        public T[,] GetGridArray()
        {
            return _gridArray;
        }


        public void SetGridObject(int x, int y, T value)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                _gridArray[x, y] = value;

                if (OnGridValueChanged != null)
                {
                    OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
                }
            }
        }

        public void TriggerOnGridObjectChanged(int x, int y)
        {
            OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, y = y });
        }

        public void GetXandY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
            y = Mathf.FloorToInt((worldPosition - _originPosition).y / _cellSize);
        }

        public void SetGridObject(Vector3 worldPosition, T value)
        {
            int x, y;
            GetXandY(worldPosition, out x, out y);
            SetGridObject(x, y, value);
        }

        public T GetGridObject(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                return _gridArray[x, y];
            }
            else
            {
                return default(T);
            }
        }

        public T GetGridObject(Vector3 worldPosition)
        {
            int x, y;
            GetXandY(worldPosition, out x, out y);
            return GetGridObject(x, y);
        }

        //public TextMeshPro CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color,
        //            TextAlignmentOptions textAlignment, int sortingOrder)
        //{
        //    var gameObject = new GameObject("World_Text", typeof(TextMeshPro));
        //    var transform = gameObject.transform;
        //    transform.SetParent(parent, false);
        //    transform.localPosition = localPosition;

        //    var tmp = gameObject.GetComponent<TextMeshPro>();
        //    // rect transform width and hight
        //    tmp.rectTransform.sizeDelta = new Vector2(_cellSize, _cellSize);

        //    tmp.alignment = textAlignment;
        //    tmp.text = text;
        //    tmp.fontSize = fontSize;
        //    tmp.color = color;
        //    tmp.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        //    return tmp;
        //}

    }

}