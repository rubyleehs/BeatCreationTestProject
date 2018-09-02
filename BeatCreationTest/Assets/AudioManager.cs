using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioClipData
{
    public KeyCode assignedKey;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour {

    public AudioClipData[] I_audioClipsInfo;

    public static AudioClipData[] audioClipsInfo;
    public static string[] clipsKeys;

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
}
