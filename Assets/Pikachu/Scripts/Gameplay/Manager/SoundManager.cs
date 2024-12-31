using UnityEngine;

namespace _pikachu
{
    public class SoundManager : Singleton<SoundManager>
    {
        [SerializeField] private AudioClip msClick;
        [SerializeField] public AudioClip msLink;
        [SerializeField] public AudioClip msNoMove;
        [SerializeField] public AudioClip msWin;
        public AudioSource audioSourceClick;
        public AudioSource audioSourceLink;
        public AudioSource audioSourceNoMove;
        public AudioSource audioSourceWin;

        public void PlayClick()
        {
            audioSourceClick.clip = msClick;
            audioSourceClick.Play();
        }
        public void PlayLink()
        {
            audioSourceLink.clip = msLink;
            audioSourceLink.Play();
        }
        public void PlayNoMove()
        {
            audioSourceNoMove.clip = msNoMove;
            audioSourceNoMove.Play();
        }
        public void PlayWin()
        {
            audioSourceWin.clip = msWin;
            audioSourceWin.Play();
        }
    }
}