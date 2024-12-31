using UnityEngine;
using DG.Tweening;
namespace _pikachu
{
    public class Item : MonoBehaviour
    {
        public SpriteRenderer spriteObject;
        public SpriteRenderer itemBackground;
        public int row, column;
        public int value = -1;
        private bool isSelectTut = false;
        private bool canClick = true;
        public bool IsSelectTut
        {
            set
            {
                isSelectTut = value;
            }
        }
        private void OnEnable()
        {
            transform.localScale = Vector3.one;
        }
        private void Reset()
        {
            LoadCompoment();
        }
        private void LoadCompoment()
        {
            spriteObject = GetComponent<SpriteRenderer>();
            if (transform.childCount > 0)
            {
                itemBackground = transform.GetChild(0).GetComponent<SpriteRenderer>();
            }
        }

        [System.Obsolete]
        private void OnMouseDown()
        {
            if (GameManager.Instance.Pause || GameManager.Instance.Finish || GameManager.Instance.Over)
            {
                return;
            }
            if (GameManager.Instance.IsFirstPlayGame_Pika && !isSelectTut)
            {
                return;
            }
            if (!canClick)
            {
                return;
            }
            SpriteRenderer itemBackgroundSpriteRender = itemBackground;

            if (GameManager.Instance.IsSelected())
            {
                if (GameManager.Instance.GetFirstItem() != this && GameManager.Instance.GetSecondItem() == null)
                {
                    canClick = false;
                    itemBackgroundSpriteRender.color = new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f);
                    GameManager.Instance.SelectSecondItem(this);
                    if (GameManager.Instance.IsFirstPlayGame_Pika)
                    {
                        Tutorial_Pikachu.Instance.CleanHint();
                    }
                    DOVirtual.DelayedCall(Time.deltaTime * 3, () => canClick = true);
                }
            }
            else
            {
                if (GameManager.Instance.GetFirstItem() == null)
                {

                    itemBackgroundSpriteRender.color = new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f);
                    GameManager.Instance.SelectFirstItem(this);
                    if (GameManager.Instance.IsFirstPlayGame_Pika)
                    {
                        Tutorial_Pikachu.Instance.handAnim.Play("Pressed_Item2");
                    }

                }
            }
        }
        public void AnimShuffe()
        {
            Sequence ItemSequence = DOTween.Sequence();
            ItemSequence
                .Append(transform.DOScale(new Vector3(0.1f, 0.1f, 1f), 0.15f))
                .Join(transform.DORotate(new Vector3(0, 0, 30f), 0.05f))
                .SetLoops(2, LoopType.Yoyo).OnKill(() => UIManager.Instance.IsClickChange = false);
        }
        public void ActiveTut()
        {
            spriteObject.sortingOrder = 20;
            itemBackground.sortingOrder = 15;
        }
        public void ClearTut()
        {
            spriteObject.sortingOrder = 5;
            itemBackground.sortingOrder = 1;
        }
    }
}
