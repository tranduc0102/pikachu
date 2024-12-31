using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _pikachu
{
    public class GenerationLevel : MonoBehaviour
    {
        public Board board;
        public FitCamera cameraFit;
        private int countSprite = 0;

        private void Start()
        {
            cameraFit = FindAnyObjectByType<FitCamera>();
            board = GetComponent<Board>();
        }

        [Obsolete]
        public void EasyLevelBasic()
        {
            SwitchModeDir();
            board.TotalColumn = 10;
            board.TotalRow = 8;
            countSprite = Random.RandomRange(10, 13);
            int startSprite = Random.Range(0, 37 - countSprite);
            int endSprite = startSprite + countSprite;
            cameraFit.transform.position = Vector3.zero;
            board.SpawnShapeSquare(startSprite, endSprite);
            GameManager.Instance.totalItems = board.TotalItem;
            if (GameManager.Instance.Level == 1 && GameManager.Instance.IsFirstPlayGame_Pika)
            { // Tutorial
                if (board.items[2, 8].value != board.items[5, 8].value)
                {
                    foreach (var item in board.items)
                    {
                        if (item != board.items[2, 8] && item.value == board.items[2, 8].value)
                        {
                            board.SwapItems(item, board.items[5, 8]);
                            break;
                        }
                    }
                    board.items[2, 8].ActiveTut();
                    board.items[2, 8].IsSelectTut = true;
                    board.items[5, 8].ActiveTut();
                    board.items[5, 8].IsSelectTut = true;
                    Tutorial_Pikachu.Instance.ShowHint();
                }
            }
            else
            {
                while (!board.GetHint(true))
                {
                    board.Change();
                }
            }
            cameraFit.FitCameraToGrid(board.TotalRow, board.TotalColumn, board.cellXY);
        }

        [Obsolete]
        public void EasyLevel()
        {
            SwitchModeDir();
            board.TotalColumn = 10;
            board.TotalRow = 10;
            countSprite = Random.RandomRange(14, 16);
            int startSprite = Random.Range(0, 37 - countSprite);
            int endSprite = startSprite + countSprite;
            cameraFit.transform.position = Vector3.zero;
            board.SpawnShapeSquare(startSprite, endSprite);
            GameManager.Instance.totalItems = board.TotalItem;
            while (!board.GetHint(true))
            {
                board.Change();
            }
            cameraFit.FitCameraToGrid(board.TotalRow, board.TotalColumn, board.cellXY);
        }


        [Obsolete]
        public void NormalLevel()
        {
            SwitchModeDir();
            board.TotalColumn = Random.RandomRange(10, 13);
            board.TotalRow = 12;
            countSprite = Random.RandomRange(15, 21);
            int startSprite = Random.Range(0, 37 - countSprite);
            int endSprite = startSprite + countSprite;
            cameraFit.transform.position = Vector3.zero;
            if (board.TotalColumn == 12 && board.TotalRow == 10)
            {
                int choice = Random.RandomRange(0, 3);
                switch (choice)
                {
                    case 0:
                        board.SpawnShapePlus(startSprite, endSprite);
                        break;
                    case 1:
                        board.SpawnShapeDiamond(startSprite, endSprite);
                        break;
                    case 2:
                        board.SpawnShapeSquare(startSprite, endSprite);
                        break;
                }
            }
            else
            {
                board.SpawnShapeSquare(startSprite, endSprite);
            }
            GameManager.Instance.totalItems = board.TotalItem;
            while (!board.GetHint(true))
            {
                board.Change();
            }
            cameraFit.FitCameraToGrid(board.TotalRow, board.TotalColumn, board.cellXY);
        }

        [Obsolete]
        public void HardLevel()
        {
            SwitchModeDir();
            board.TotalColumn = Random.RandomRange(12, 15);
            board.TotalRow = 12;
            countSprite = Random.RandomRange(20, 37);
            int startSprite = Random.Range(0, 37 - countSprite);
            int endSprite = startSprite + countSprite;
            cameraFit.transform.position = Vector3.zero;
            if (board.TotalColumn == 12 && board.TotalRow == 10)
            {
                int choice = Random.RandomRange(0, 3);
                switch (choice)
                {
                    case 0:
                        board.SpawnShapePlus(startSprite, endSprite);
                        break;
                    case 1:
                        board.SpawnShapeDiamond(startSprite, endSprite);
                        break;
                    case 2:
                        board.SpawnShapeSquare(startSprite, endSprite);
                        break;
                }
            }
            else
            {
                board.SpawnShapeSquare(startSprite, endSprite);
            }
            GameManager.Instance.totalItems = board.TotalItem;
            while (!board.GetHint(true))
            {
                board.Change();
            }
            cameraFit.FitCameraToGrid(board.TotalRow, board.TotalColumn, board.cellXY);
        }

        [Obsolete]
        private void SwitchModeDir()
        {
            switch (GameManager.Instance.Level)
            {
                case 1:
                    GameManager.Instance.modeDir = ModeDirection.None;
                    break;
                case 2:
                    GameManager.Instance.modeDir = ModeDirection.Left;
                    break;
                case 3:
                    GameManager.Instance.modeDir = ModeDirection.Down;
                    break;
                case 4:
                    GameManager.Instance.modeDir = ModeDirection.Up;
                    break;
                case 5:
                    GameManager.Instance.modeDir = ModeDirection.Right;
                    break;
                default:
                    GameManager.Instance.modeDir = (ModeDirection)Random.RandomRange(0, 5);
                    break;
            }

        }
    }
}