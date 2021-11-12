using System.Collections.Generic;
using UnityEngine;
namespace Audio {
    public class AudioManager : MonoBehaviour {
        public List<AudioClip> bgms = new List<AudioClip>();
        public AudioSource     audioSource;

        private int _bgmId = -1;
        
        public int BgmId {
            get => _bgmId;
            set {
                if (value != _bgmId) {
                    _bgmId = value;
                    Play(value);
                }
                
            }
        }

        public void Play (int id) {
            audioSource.Stop();
            audioSource.clip = bgms[id];
            audioSource.Play();
        }
    }
}
