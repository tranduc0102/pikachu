using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;
namespace _pikachu
{
    public enum ModeDirection
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }
    public class GameManager : Singleton<GameManager>
    {
        [Header("Setting Game")]
        private bool isFinishLevel = false;
        private bool isPause;
        private bool overGame = false;
        [System.Obsolete]
        public bool Finish
        {
            get { return isFinishLevel; }
            set { isFinishLevel = value; CheckWin(); }
        }

        public bool Pause
        {
            get { return isPause; }
            set { isPause = value; }
        }
        public bool Over
        {
            get { return overGame; }
            set { overGame = value; OverGame(); }
        }

        [Header("Level")]
        #region Setting level
        private int level;
        public int Level
        {
            get { return level; }
            set { level = value; }
        }
        private int timeSecond = 0;
        public GenerationLevel genLevel;

        private int difficultyLevel;
        private bool isIncreasingDifficulty;
        private int DifficultyLevel
        {
            get { return PlayerPrefs.GetInt(StringUse.DifficultyLevel, 0); }
            set
            {
                PlayerPrefs.SetInt(StringUse.DifficultyLevel, value);
            }
        }
        private bool IsIncreasingDifficulty
        {
            get { return PlayerPrefs.GetInt(StringUse.IsIncreasingDifficulty, 1) != 0; }
            set
            {
                PlayerPrefs.SetInt(StringUse.IsIncreasingDifficulty, value ? 1 : 0);
            }
        }
        public bool IsFirstPlayGame_Pika
        {
            get { return PlayerPrefs.GetInt(StringUse.IsFirstPlayGame, 1) != 0; }
            set
            {
                PlayerPrefs.SetInt(StringUse.IsFirstPlayGame, value ? 1 : 0);
            }
        }
        public int TimeSecond
        {
            get { return timeSecond; }
            set
            {
                timeSecond = value;
                if (timeSecond <= 0)
                {
                    UIManager.Instance.txtTime.text = $"{0} : 00";
                    Over = true;
                    return;
                }
                int minute = timeSecond / 60;
                int second = timeSecond - minute * 60;
                if (second < 10)
                {
                    UIManager.Instance.txtTime.text = $"{minute} : 0{second}";
                }
                else
                {
                    UIManager.Instance.txtTime.text = $"{minute} : {second}";
                }
            }
        }
        public ModeDirection modeDir;
        #endregion

        #region Setting Tool
        public int NumberCanUseHint
        {
            get { return PlayerPrefs.GetInt(StringUse.NumberCanUseHint, 3); }
            set
            {
                PlayerPrefs.SetInt(StringUse.NumberCanUseHint, value);
                PlayerPrefs.Save();
                UIManager.Instance.txtCountCanUseHint.text = value.ToString();
            }
        }

        public int NumberCanUseChange
        {
            get { return PlayerPrefs.GetInt(StringUse.NumberCanUseChange, 3); }
            set
            {
                PlayerPrefs.SetInt(StringUse.NumberCanUseChange, value);
                PlayerPrefs.Save();
                UIManager.Instance.txtCountCanUseChange.text = value.ToString();
            }
        }
        #endregion

        #region Setting select item
        private bool isSelected;
        public Item firstItem;
        public Item secondItem;

        public int totalItems;
        public int clearItems = 0;
        public Board board;
        public bool IsSelected()
        {
            return isSelected;
        }

        public Item GetFirstItem()
        {
            return firstItem;
        }
        public Item GetSecondItem()
        {
            return secondItem;
        }
        public void SelectFirstItem(Item item)
        {
            if (!isFinishLevel)
            {
                firstItem = item;
                isSelected = true;
                SoundManager.Instance.PlayClick();
            }
        }

        [System.Obsolete]
        public void SelectSecondItem(Item item)
        {
            secondItem = item;
            SoundManager.Instance.PlayClick();
            CompareTwoItems();
        }
        #endregion
        //Time
        float time = 0;
        float oneSecond = 1f;
        [System.Obsolete]
        private void Start()
        {
            PlayerPrefs.DeleteAll();
            isFinishLevel = false;
            isPause = false;
            overGame = false;
            clearItems = 0;
            DOTween.SetTweensCapacity(500, 200);
            level = PlayerPrefs.GetInt(StringUse.CurrentLevel, 1);
            NumberCanUseChange = 300;
            NumberCanUseHint = 300;
            difficultyLevel = DifficultyLevel;
            isIncreasingDifficulty = IsIncreasingDifficulty;
            SpawnLevel();
        }
        private void Reset()
        {
            LoadCompoments();
        }
        private void LoadCompoments()
        {
            board = GetComponent<Board>();
            genLevel = GetComponent<GenerationLevel>();
        }

        [System.Obsolete]
        private void Update()
        {
            if (Pause || IsFirstPlayGame_Pika) return;
            time += Time.deltaTime;
            if (time >= oneSecond && !isFinishLevel)
            {
                TimeSecond -= 1;
                time = 0;
            }
        }

        [System.Obsolete]
        private void CompareTwoItems()
        {
            if (firstItem != null && secondItem != null)
            {
                Item firstPosition = firstItem;
                Item secondPosition = secondItem;

                bool isConnection = board.CheckConnectItem(firstPosition, secondPosition);

                if (firstPosition.value == secondPosition.value && isConnection)
                {

                    firstItem.value = -1;
                    secondItem.value = -1;
                    firstPosition.transform.DOScale(new Vector3(0.35f, 0.35f, 1f), 0.25f)
                     .OnComplete(() => { board._itemPool.ReturnItem(firstPosition); });
                    secondPosition.transform.DOScale(new Vector3(0.35f, 0.35f, 1f), 0.25f)
                     .OnComplete(() => { board._itemPool.ReturnItem(secondPosition); });
                    SoundManager.Instance.PlayLink();
                    if (modeDir == ModeDirection.Right)
                    {
                        DOVirtual.DelayedCall(0.3f, () => { board.UpdateBoardLevelRight(firstPosition, secondPosition); }).OnKill(() => ResetItems());

                    }
                    else if (modeDir == ModeDirection.Left)
                    {
                        DOVirtual.DelayedCall(0.3f, () => { board.UpdateBoardLevelLeft(firstPosition, secondPosition); }).OnKill(() => ResetItems());

                    }
                    else if (modeDir == ModeDirection.Down)
                    {
                        DOVirtual.DelayedCall(0.3f, () => { board.UpdateBoardLevelDown(firstPosition, secondPosition); }).OnKill(() => ResetItems());

                    }
                    else if (modeDir == ModeDirection.Up)
                    {
                        DOVirtual.DelayedCall(0.3f, () => { board.UpdateBoardLevelUp(firstPosition, secondPosition); }).OnKill(() => ResetItems());

                    }
                    else
                    {
                        ResetItems();
                    }
                    clearItems += 2;
                    if (clearItems >= totalItems)
                    {
                        Finish = true;
                    }
                }
                else
                {
                    Sequence firstItemSequence = DOTween.Sequence();
                    firstItemSequence
                        .Append(firstItem.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.15f))
                        .Join(firstItem.transform.DORotate(new Vector3(0, 0, 10f), 0.05f))
                        .SetLoops(2, LoopType.Yoyo);

                    Sequence secondItemSequence = DOTween.Sequence();
                    secondItemSequence
                        .Append(secondItem.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.15f))
                        .Join(secondItem.transform.DORotate(new Vector3(0, 0, 10f), 0.05f))
                        .SetLoops(2, LoopType.Yoyo)
                        .OnComplete(() =>
                        {
                            ResetItems();
                        });
                    SoundManager.Instance.PlayNoMove();
                    TimeSecond -= 20;
                    UIManager.Instance.timeMinus.SetTrigger("timeMinus");
                }
                DOVirtual.DelayedCall(0.35f, () =>
                {
                    if (clearItems >= totalItems)
                    {
                        return;
                    }
                    StartCoroutine(AutoSwapItem());
                });
            }
        }
        private IEnumerator AutoSwapItem()
        {
            while (!board.GetHint(true))
            {
                board.Change(true);
                yield return new WaitForSeconds(1f);
            }
            yield return null;
        }

        public void ResetItems()
        {
            if (firstItem == null) return;
            if (secondItem == null) return;
            firstItem.itemBackground.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            secondItem.itemBackground.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            firstItem = null;
            secondItem = null;
            isSelected = false;
        }

        [System.Obsolete]
        private void CheckWin()
        {
            if (isFinishLevel)
            {
                PlayerPrefs.SetInt(StringUse.CurrentLevel, PlayerPrefs.GetInt(StringUse.CurrentLevel, 1) + 1);
                if (IsIncreasingDifficulty)
                {
                    if (level <= 12)
                    {
                        if (DifficultyLevel == 0)
                        {
                            DifficultyLevel += 1;
                        }
                        else
                        {
                            DifficultyLevel = 0;
                            IsIncreasingDifficulty = false;
                        }

                    }
                    else
                    {
                        if (DifficultyLevel < 2)
                        {
                            DifficultyLevel += 1;
                        }
                        else
                        {
                            DifficultyLevel = 1;
                            IsIncreasingDifficulty = false;
                        }
                    }
                }
                else
                {
                    if (level <= 12)
                    {
                        DifficultyLevel = 1;
                        IsIncreasingDifficulty = true;
                    }
                    else
                    {
                        if (DifficultyLevel > 0)
                        {
                            DifficultyLevel -= 1;
                        }
                        else
                        {
                            DifficultyLevel = 1;
                            IsIncreasingDifficulty = true;
                        }
                    }
                }
                PlayerPrefs.Save();
                DOVirtual.DelayedCall(0.4f, () =>
                {
                    SoundManager.Instance.PlayWin();
                    UIManager.Instance.ActiveAnimationFinishLevel();
                });
            }
        }
        private void OverGame()
        {
            if (overGame)
            {
                UIManager.Instance.panelLosseAndWin.gameObject.SetActive(true);
            }
        }

        public void GetHint()
        {
            if (NumberCanUseHint > 0)
            {
                if (board.GetHint())
                {
                    NumberCanUseHint -= 1;
                }
            }
        }
        public void GetChange()
        {
            if (NumberCanUseChange > 0)
            {
                board.Change(true);
                StartCoroutine(AutoSwapItem());
                NumberCanUseChange -= 1;
            }
        }

        [System.Obsolete]
        public void SpawnLevel()
        {
            if (level <= 12)
            {
                if (difficultyLevel == 0)
                {
                    genLevel.EasyLevelBasic();
                }
                else
                {
                    genLevel.EasyLevel();
                }

            }
            else if (level >= 13 && level < 35)
            {
                if (difficultyLevel == 0)
                {
                    genLevel.EasyLevelBasic();
                }
                else if (difficultyLevel == 1)
                {
                    genLevel.EasyLevel();
                }
                else
                {
                    genLevel.NormalLevel();
                }

            }
            else
            {
                if (difficultyLevel == 0)
                {
                    int choiceRandom = Random.RandomRange(0, 2);
                    if (choiceRandom == 0)
                    {
                        genLevel.EasyLevel();
                    }
                    else
                    {
                        genLevel.EasyLevelBasic();
                    }
                }
                else if (difficultyLevel == 1)
                {
                    genLevel.NormalLevel();
                }
                else
                {
                    genLevel.HardLevel();
                }
            }
            clearItems = 0;
            TimeSecond = 300;
        }
        public void UpdateStatus()
        {
            level = PlayerPrefs.GetInt(StringUse.CurrentLevel, 1);
            difficultyLevel = DifficultyLevel;
            isIncreasingDifficulty = IsIncreasingDifficulty;
        }
    }
}
