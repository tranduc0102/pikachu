using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace _pikachu
{
    public class Tutorial_Pikachu : Singleton<Tutorial_Pikachu>
    {
        [SerializeField] private GameObject hand;
        [SerializeField] private CanvasGroup dime;
        public Animator handAnim;
        public bool HintShowing { get; private set; }
        private void Reset()
        {
            dime = transform.GetChild(1).GetComponent<CanvasGroup>();
            hand = dime.transform.GetChild(1).GetChild(0).gameObject;
            handAnim = hand.GetComponent<Animator>();
        }
        public void ShowHint()
        {
            if (HintShowing) return;
            GameManager.Instance.board.items[2, 8].ActiveTut();
            GameManager.Instance.board.items[2, 8].IsSelectTut = true;
            GameManager.Instance.board.items[5, 8].ActiveTut();
            GameManager.Instance.board.items[5, 8].IsSelectTut = true;
            DOVirtual.DelayedCall(0.1f, () =>
            {
                HintShowing = true;
                dime.gameObject.SetActive(true);
                dime.DOFade(1f, 0.5f);
                hand.SetActive(true);
                handAnim.Play("Pressed_Item1");
            });
        }

        public void CleanHint()
        {
            HintShowing = false;
            hand.SetActive(false);
            dime.DOFade(0, 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                dime.gameObject.SetActive(false);
            });
            foreach(var item in GameManager.Instance.board.items)
            {
                item.ClearTut();
            }
            GameManager.Instance.IsFirstPlayGame_Pika = false;
        }
    }
}
