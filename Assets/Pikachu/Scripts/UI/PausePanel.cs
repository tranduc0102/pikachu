using UnityEngine;
using UnityEngine.UI;

namespace _pikachu
{
    public class PausePanel : MonoBehaviour
    {
        public Button replay;
        public Button resume;

        [System.Obsolete]
        void Start()
        {
            replay.onClick.AddListener(() => {
                UIManager.Instance.ReplayGame();
                GameManager.Instance.Pause = false;
                this.gameObject.SetActive(false);
            }) ;
            resume.onClick.AddListener(() => {
                GameManager.Instance.Pause = false;
                this.gameObject.SetActive(false);
            });
        }
    }
}
