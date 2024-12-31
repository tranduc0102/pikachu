using UnityEngine;

namespace _pikachu
{
    public class FitCamera : MonoBehaviour
    {
        public Camera _camera;
        public void FitCameraToGrid(int width, int height, float cellXY)
        {
            transform.position = Fit(width, height, cellXY);
        }
        private Vector3 Fit(int width, int height,float cellXY)
        {
            float x = 0;
            float y = 0;
            if (width < 10)
            {
                
                CheckSize(7.5f);
            }
            else
            {
                CheckSize(9.5f);
            }
            if (height >= 11 && height <= 14 && width > 10)
            { 
                CheckSize(11.5f);
            }
            x = ((width - 1) + ((width - 1) * cellXY))/2; 
            y = ((height - 1) + ((height - 1) * cellXY))/2; 
            return new Vector3(-x, 0.7f - y, 0f);
        }

        private void CheckSize(float size)
        {
            float screenRatio = (float)Screen.width / (float)Screen.height;
            float targetRatio = 10.8f / 19.2f;

            if (screenRatio >= targetRatio)
            {
                _camera.orthographicSize = size;
            }
            else
            {
                float changeSize = targetRatio / screenRatio;
                _camera.orthographicSize = size * changeSize;
            }
        }

    }
}