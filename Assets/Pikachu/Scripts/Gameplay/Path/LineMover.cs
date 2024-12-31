using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace _pikachu
{
    public class LineMover : MonoBehaviour
    {
        private LineRenderer lineRenderer;
        public float speed1 = 9.6f;
        public float speed2 = 9.6f;
        private Transform nodeObject;
        public void SetPosition(Vector3 pos)
        {
            if (nodeObject == null)
            {
                nodeObject = transform.GetChild(0);
            }
            nodeObject.position = pos;
            nodeObject.DOScale(new Vector3(0.25f, 0.25f, 1f), 0.05f)
                .SetLoops(4, LoopType.Yoyo)
                .OnComplete(() => {
                    lineRenderer.positionCount = 2;
                });
        }
        private void OnEnable()
        {
            speed1 = 9.6f;
            speed2 = 9.6f;
            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
                lineRenderer.positionCount = 2;
            }
            else
            {
                lineRenderer.positionCount = 2;
            }
            if (nodeObject == null)
            {
                nodeObject = transform.GetChild(0);
            }
            else
            {
                nodeObject.localScale = new Vector3(0.15f, 0.15f, 1f);
            }
        }
        public IEnumerator MoveLine(List<Vector3> positions1, List<Vector3> positions2 = null, GameObject gameobj = null)
        {
            lineRenderer.SetPosition(0, positions1[0]);
            Vector3 currentStartPos = positions1[0];
            Vector3 targetStartPos = positions1[^1];
            lineRenderer.SetPosition(1, positions1[^1]);

            while (Vector3.Distance(currentStartPos, targetStartPos) > 0.005f)
            {
                currentStartPos = Vector3.MoveTowards(currentStartPos, targetStartPos, speed1 * Time.deltaTime);
                lineRenderer.SetPosition(0, currentStartPos);
                nodeObject.position = currentStartPos;
                yield return null;
            }
            lineRenderer.SetPosition(0, positions1[^1]);

            // Di chuyển theo path 2 nếu có
            if (positions2 != null)
            {
                if (gameobj != null)
                {
                    gameobj.SetActive(false);
                }
                lineRenderer.SetPosition(0, positions2[0]);
                targetStartPos = positions2[^1];
                lineRenderer.SetPosition(1, positions2[^1]);

                while (Vector3.Distance(currentStartPos, targetStartPos) > 0.005f)
                {
                    currentStartPos = Vector3.MoveTowards(currentStartPos, targetStartPos, speed2 * Time.deltaTime);
                    lineRenderer.SetPosition(0, currentStartPos);
                    nodeObject.position = currentStartPos;
                    yield return null;
                }
                lineRenderer.SetPosition(0, positions2[^1]);
            }
            gameObject.SetActive(false);
        }

    }
}