using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace _pikachu
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private CanvasGroup hoverAnimation;
        [SerializeField] private CanvasGroup hoverHintChange;
        [SerializeField] private CanvasGroup hoverText;
        [SerializeField] private Button btnHint;
        [SerializeField] private Button btnChange;
        [SerializeField] private Button btnBack;
        [SerializeField] private Button btnPause;
        [SerializeField] private TextMeshProUGUI txtNextLevel;
        [SerializeField] private Image dime;

        public TextMeshProUGUI txtCountCanUseHint;
        public TextMeshProUGUI txtCountCanUseChange;
        public TextMeshProUGUI txtTime;
        public Animator timeMinus;
        public GameObject panelPause;
        public bool IsClickChange { get; set; }

        public Transform panelLosseAndWin;
        [SerializeField] private Button btnReplay;

        private TextMeshProUGUI txtLevel;
        [System.Obsolete]
        private void Start()
        {
            btnBack.onClick.AddListener(() => { });
            btnPause.onClick.AddListener(() => {
        /*        if (GameManager.Instance.Over)
                {
                    return;
                }*/
                GameManager.Instance.Pause = true;
                panelPause.gameObject.SetActive(true);
            });
            btnHint.onClick.AddListener(() => {
                if (GameManager.Instance.Pause || GameManager.Instance.Over) return;
                GameManager.Instance.GetHint();
            });
            btnChange.onClick.AddListener(() => {
                if (GameManager.Instance.Pause || GameManager.Instance.Over) return;
                if (!IsClickChange)
                {
                    GameManager.Instance.GetChange();
                    IsClickChange = true;
                }
            });
            btnReplay.onClick.AddListener(() =>
            {
                ReplayGame();
            });
            DOVirtual.DelayedCall(0.3f, () => {
                if (txtLevel == null)
                {
                    txtLevel = transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>();
                }
                txtLevel.text = "Level " + GameManager.Instance.Level.ToString();
                txtCountCanUseChange.text = GameManager.Instance.NumberCanUseChange.ToString();
                txtCountCanUseHint.text = GameManager.Instance.NumberCanUseHint.ToString();
            });
            ActiveAnimationNextLevel();
        }
        private void Reset()
        {
            LoadCompoment();
        }
        private void LoadCompoment()
        {
            btnBack = transform.GetChild(0).GetChild(0).GetComponent<Button>();
            btnPause = transform.GetChild(0).GetChild(1).GetComponent<Button>();

            hoverAnimation = transform.GetChild(0).GetChild(4).GetComponent<CanvasGroup>();
            hoverHintChange = hoverAnimation.transform.GetChild(0).GetComponent<CanvasGroup>();
            hoverText = hoverAnimation.transform.GetChild(1).GetComponent<CanvasGroup>();
            btnHint = hoverHintChange.transform.GetChild(0).GetComponent<Button>();
            btnChange = hoverHintChange.transform.GetChild(1).GetComponent<Button>();
            txtNextLevel = hoverText.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            dime = hoverText.transform.GetChild(0).GetComponent<Image>();

            txtCountCanUseHint = btnHint.transform.GetComponentInChildren<TextMeshProUGUI>();
            txtCountCanUseChange = btnChange.transform.GetComponentInChildren<TextMeshProUGUI>();
            txtTime = transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
            timeMinus = txtTime.transform.GetChild(0).GetComponent<Animator>();

            panelLosseAndWin = transform.GetChild(0).GetChild(5);
            btnReplay = panelLosseAndWin.GetChild(0).GetChild(0).GetComponent<Button>();
            txtLevel = transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>();

        }

        [System.Obsolete]
        private void NextLevel()
        {
            GameManager.Instance.UpdateStatus();
            GameManager.Instance.SpawnLevel();
            GameManager.Instance.Finish = false;
            txtLevel.GetComponent<Animator>().SetTrigger("StartText");
            DOVirtual.DelayedCall(0.3f, () => { txtLevel.text = "Level " + GameManager.Instance.Level.ToString(); });
            foreach (var border in GameManager.Instance.board._listBorder)
            {
                GameManager.Instance.board._itemPool.ReturnItem(border);
            }
            GameManager.Instance.board._listBorder.Clear();
        }

        [System.Obsolete]
        public void ReplayGame()
        {
            foreach (var item in GameManager.Instance.board.items)
            {
                if ((item.value != -1 || item.gameObject.activeSelf) && !item.name.Contains("Border"))
                {
                    GameManager.Instance.board._itemPool.ReturnItem(item);
                }
            }
            GameManager.Instance.ResetItems();
            GameManager.Instance.Over = false;
            GameManager.Instance.Finish = false;
            panelLosseAndWin.gameObject.SetActive(false);
            foreach (var border in GameManager.Instance.board._listBorder)
            {
                GameManager.Instance.board._itemPool.ReturnItem(border);
            }
            GameManager.Instance.board._listBorder.Clear();
            GameManager.Instance.SpawnLevel();
        }

        [System.Obsolete]
        public void ActiveAnimationFinishLevel()
        {
            Debug.Log("WIN");
            hoverHintChange.DOFade(0f, 1f).OnComplete(() => hoverHintChange.gameObject.SetActive(false));
            btnHint.transform.DOScale(new Vector3(1.7f, 1.7f, 1f), 0.35f)
                .SetLoops(2, LoopType.Yoyo).OnComplete(() => {
                    btnHint.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                    FadeChildren(btnHint, 0f);
                });
            btnChange.transform.DOScale(new Vector3(1.7f, 1.7f, 1f), 0.35f)
               .SetLoops(2, LoopType.Yoyo).OnComplete(() => {
                   btnChange.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                   FadeChildren(btnChange, 0f);
               });
            DOVirtual.DelayedCall(0.1f, () =>
            {
                hoverText.gameObject.SetActive(true);
                dime.DOFade(0.5f, 1f);
                txtNextLevel.rectTransform.DOAnchorPos(new Vector2(0, 0), 1.5f).OnComplete(() => {
                    dime.DOFade(0, 1.5f);
                    txtNextLevel.rectTransform.DOAnchorPos(new Vector2(-770f, 0), 1.5f).OnComplete(() => {
                        txtNextLevel.rectTransform.anchoredPosition = new Vector2(770f, 0f);
                        hoverText.gameObject.SetActive(false);
                        ActiveAnimationNextLevel();
                        NextLevel();
                    });
                });
            });
        }
        public void ActiveAnimationNextLevel()
        {
            hoverHintChange.alpha = 1;
            hoverHintChange.gameObject.SetActive(true);
            btnHint.transform.DOScale(new Vector3(1f, 1f, 1f), 0.35f).OnComplete(() => { btnChange.transform.DOScale(new Vector3(1f, 1f, 1f), 0.35f); FadeChildren(btnChange, 1f); });
            FadeChildren(btnHint, 1f);
        }
        public void FadeChildren(Button btn, float targetAlpha)
        {
            foreach (Graphic childGraphic in btn.GetComponentsInChildren<Image>())
            {
                childGraphic.DOFade(targetAlpha, 0.7f);
            }
            btn.GetComponentInChildren<TextMeshProUGUI>().DOFade(targetAlpha, 0.7f);
        }
    }
}
