using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace _pikachu
{
    public class LineRenderPath : MonoBehaviour
    {
        [SerializeField] private LineMover _line1;
        [SerializeField] private LineMover _line2;
        [SerializeField] private LineRenderer _line3;
        [SerializeField] private List<Vector3> path1;
        [SerializeField] private List<Vector3> path2;
        [SerializeField] private List<Vector3> path3;

        private void Reset()
        {
            InitializeLineRenderer();
        }
        private void InitializeLineRenderer()
        {
            _line1 = this.transform.GetChild(0).GetComponent<LineMover>();
            _line2 = this.transform.GetChild(1).GetComponent<LineMover>();
            _line3 = this.transform.GetChild(2).GetComponent<LineRenderer>();
        }

        private int inDex = 0;
        public void DrawPathWithLineRenderer(List<Item> path)
        {
            inDex = 0;
            path1.Clear();
            path2.Clear();
            path3.Clear();
            if (GetChangeDirection(path) == 0)
            {
                path2 = Path(0, path);
                List<Vector3> path4 = new List<Vector3>();
                List<Vector3> path5 = new List<Vector3>();
                if (path2.Count == 2)
                {
                    path4.Add(path2[0]);
                    Vector3 middlePoint = new Vector3(
                        (path2[path2.Count - 1].x + path2[0].x) / 2f,
                        (path2[path2.Count - 1].y + path2[0].y) / 2f,
                        0f);
                    path4.Add(middlePoint);
                    path5.Add(path2[1]);
                    path5.Add(middlePoint);
                    _line1.SetPosition(path4[0]);
                    _line2.SetPosition(path5[0]);
                    _line1.gameObject.SetActive(true);
                    _line2.gameObject.SetActive(true);
                    _line1.speed1 = 5f;
                    _line2.speed1 = 5f;
                    _line1.StartCoroutine(_line1.MoveLine(path4));
                    _line2.StartCoroutine(_line2.MoveLine(path5));
                }
                else
                {
                    for (int i = 0; i <= path2.Count / 2; i++)
                    {
                        path4.Add(path2[i]);
                    }
                    for (int i = path2.Count - 1; i >= path2.Count / 2; i--)
                    {
                        path5.Add(path2[i]);
                    }
                    _line1.gameObject.SetActive(true);
                    _line2.gameObject.SetActive(true);
                    _line1.SetPosition(path4[0]);
                    _line2.SetPosition(path5[0]);
                    _line1.StartCoroutine(_line1.MoveLine(path4));
                    _line2.StartCoroutine(_line2.MoveLine(path5));
                }
            }
            else if (GetChangeDirection(path) == 1)
            {

                path1 = Path(inDex, path);
                path2 = Path(inDex - 1, path);
                path2.Reverse();
                _line1.SetPosition(path1[0]);
                _line2.SetPosition(path2[0]);
                _line1.gameObject.SetActive(true);
                _line2.gameObject.SetActive(true);
                SpeedMove1(path1, path2);
                _line1.StartCoroutine(_line1.MoveLine(path1));
                _line2.StartCoroutine(_line2.MoveLine(path2));

            }
            else
            {
                path1 = Path(inDex, path);
                path2 = Path(inDex - 1, path);
                path3 = Path(inDex - 2, path);
                path3.Reverse();
                List<Vector3> path4 = new List<Vector3>();
                List<Vector3> path5 = new List<Vector3>();
                for (int i = 0; i <= path2.Count / 2; i++)
                {
                    path4.Add(path2[i]);
                }
                for (int i = path2.Count - 1; i >= path2.Count / 2; i--)
                {
                    path5.Add(path2[i]);
                }
                _line1.SetPosition(path1[0]);
                _line2.SetPosition(path3[0]);
                _line1.gameObject.SetActive(true);
                _line2.gameObject.SetActive(true);
                _line3.gameObject.SetActive(true);
                _line3.SetPosition(0, path2[0]);
                _line3.SetPosition(1, path2[path2.Count - 1]);
                SpeedMove1(path1, path3);
                SpeedMove2(path4, path5);
                _line1.StartCoroutine(_line1.MoveLine(path1, path4, _line3.gameObject));
                _line2.StartCoroutine(_line2.MoveLine(path3, path5, _line3.gameObject));
            }
        }
        private float GetDistance(List<Vector3> path)
        {
            float distance = 0;
            for (int i = 1; i < path.Count; i++)
            {
                distance += Vector3.Distance(path[i - 1], path[i]);
            }
            return distance;
        }
        private void SpeedMove1(List<Vector3> path1, List<Vector3> path2)
        {
            float distance1 = GetDistance(path1);
            float distance2 = GetDistance(path2);

            float timeMove1 = distance1 / _line1.speed1;
            float timeMove2 = distance2 / _line2.speed1;

            if (timeMove1 > timeMove2)
            {
                _line2.speed1 = distance2 / timeMove1;
            }
            else
            {
                _line1.speed1 = distance1 / timeMove2;
            }
        }
        private void SpeedMove2(List<Vector3> path1, List<Vector3> path2)
        {
            float distance1 = GetDistance(path1);
            float distance2 = GetDistance(path2);

            float timeMove1 = distance1 / _line1.speed2;
            float timeMove2 = distance2 / _line2.speed2;

            if (timeMove1 > timeMove2)
            {
                _line2.speed2 = distance2 / timeMove1;
            }
            else
            {
                _line1.speed2 = distance1 / timeMove2;
            }
        }
        private List<Vector3> Path(int start, List<Item> listItem)
        {
            List<Vector3> path = new List<Vector3>();
            Vector3 previous = listItem[start].transform.position;
            for (int i = start; i < listItem.Count; i++)
            {
                Vector3 current = listItem[i].transform.position;

                float hasXChange = Mathf.Abs(current.x - previous.x);
                float hasYChange = Mathf.Abs(current.y - previous.y);
                if (hasYChange == 0 || hasXChange == 0)
                {
                    inDex++;
                    path.Add(current);
                }
                else
                {
                    break;
                }
            }

            return path;
        }
        private int GetChangeDirection(List<Item> listItem)
        {
            int numberChange = 0;
            Vector3 previous = listItem[0].transform.position;
            for (int i = 0; i < listItem.Count; i++)
            {
                Vector3 current = listItem[i].transform.position;

                float hasXChange = Mathf.Abs(current.x - previous.x);
                float hasYChange = Mathf.Abs(current.y - previous.y);
                if (hasYChange == 0 || hasXChange == 0)
                {
                    continue;
                }
                else
                {
                    previous = listItem[i - 1].transform.position;
                    numberChange++;
                }
            }
            return numberChange;
        }
        private void Update()
        {
            if (!_line1.gameObject.activeSelf && !_line2.gameObject.activeSelf)
            {
                GameManager.Instance.board._itemPool.ReturnItem(this);
            }
        }
    }
}