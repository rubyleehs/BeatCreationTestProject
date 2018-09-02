using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatData
{
    public int typeIndex;
    public float progressRatioTime;
}

public class TuneData : MonoBehaviour
{
    public List<BeatData> tuneAllBeats;

    public float tuneHalfDuration;

    public List<List<BeatData>> tuneTypeBeats;

    private int numOfBeatTypes;
    private bool IsRecording = false;

    private void Start()
    {
        numOfBeatTypes = AudioManager.audioClipsInfo.Length;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) StartCoroutine(StartNewRecording());
    }

    public IEnumerator StartNewRecording()
    {
        if (IsRecording) yield break;
        IsRecording = true;
        tuneAllBeats = new List<BeatData>();
        tuneTypeBeats = new List<List<BeatData>>();
        for (int i = 0; i < numOfBeatTypes; i++)
        {
            tuneTypeBeats.Add(new List<BeatData>());
        }

        float _startTime = Time.time;
        float _progress = 0;
        Debug.Log("test");

        while (_progress < 1)
        {
            _progress = (Time.time - _startTime) / (2 * tuneHalfDuration);
            string _inputString = Input.inputString;
            if (_inputString != "")
            {
                string _keyName = _inputString.ToUpper()[0].ToString();
                if (IsValidLetter(_keyName))
                {

                    KeyCode _keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), _keyName);

                    if (Input.GetKeyDown(_keyCode))
                    {
                        int _keyIndex = AudioManager.FindKeyIndex(_keyCode);
                        if (_keyIndex >= 0)
                        {
                            Debug.Log(_keyIndex);
                            BeatData _curBeatData = new BeatData() { typeIndex = _keyIndex, progressRatioTime = _progress };
                            tuneAllBeats.Add(_curBeatData);
                            tuneTypeBeats[_keyIndex].Add(_curBeatData);
                        }
                    }
                }
            }
            yield return null;
        }
        IsRecording = false;
    }

    public bool IsValidLetter(string _string)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(_string, "[A-Z]");
    }
}
