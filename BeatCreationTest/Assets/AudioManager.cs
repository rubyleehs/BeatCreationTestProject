using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioClipData
{
    public KeyCode assignedKey;
    public Color color;
    public List<AudioClip> clips;
    public float baseDamage;
}

public class AudioManager : MonoBehaviour {

    public AudioClipData[] I_audioClipsInfo;
    public GameObject clipPlayer;
    public static AudioClipData[] audioClipsInfo;
    public static string[] clipsKeys;
    public AudioSource earlyFailAudio;
    public AudioSource lateFailAudio;

    public static int[] steps = { -3, -1, 0, 2, 4, 5, 7 };

    private void Awake()
    {
        audioClipsInfo = I_audioClipsInfo;
        clipsKeys = new string[audioClipsInfo.Length];
        for (int i = 0; i < audioClipsInfo.Length; i++)
        {
            clipsKeys[i] = audioClipsInfo[i].assignedKey.ToString();
        }
    }

    public static int FindKeyIndex(KeyCode _keyCode)
    {
        for (int i = 0; i < clipsKeys.Length; i++)
        {
            if(_keyCode == audioClipsInfo[i].assignedKey)
            {
                return i;
            }
        }
        return -1;
    }

    public IEnumerator PlayClip(int _index)
    {
        float _pitch = Mathf.Pow(1.05946f, steps[_index]);
        AudioSource _clipPLayer = Instantiate(clipPlayer).GetComponent<AudioSource>();
        _clipPLayer.clip = audioClipsInfo[_index].clips[0];
        _clipPLayer.pitch = _pitch;
        _clipPLayer.Play();
        yield return new WaitForSeconds(_clipPLayer.clip.length);
        Destroy(_clipPLayer.gameObject);
    }
    public void PlayFailAudio(bool _isLate)
    {
        if (_isLate) lateFailAudio.Play();
        else earlyFailAudio.Play();
    }
}
