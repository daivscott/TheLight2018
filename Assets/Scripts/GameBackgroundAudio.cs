using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBackgroundAudio : MonoBehaviour {

    [SerializeField] AudioSource backingTonesAudio;

    void Start()
    {
        //StartCoroutine(PlayBackingAudio());
        backingTonesAudio.loop = true;
        backingTonesAudio.Play();
    }

    //IEnumerator PlayBackingAudio()
    //{
    //    yield return new WaitForSeconds(1);
    //    backingTonesAudio.loop = true;
    //    backingTonesAudio.Play();
    //}
}
