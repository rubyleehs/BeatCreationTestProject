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
    public AudioManager audioManager;
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

        while (_progress < 1)
        {
            _progress = (Time.time - _startTime) / (2 * tuneHalfDuration);
            HandleRecordingInput(_progress);
            UIManager.SetTimerLine(_progress);
            yield return null;
        }
        IsRecording = false;
    }

    public bool IsValidLetter(string _string)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(_string, "[A-Z]");
    }

    public void HandleRecordingInput(float _progress)
    {
        string _inputString = Input.inputString;
        if (_inputString == "") return;

        string _keys = _inputString.ToUpper();
        for (int i = 0; i < _keys.Length; i++)
        {
            string _key = _keys[i].ToString();
            if (IsValidLetter(_key))
            {
                KeyCode _keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), _key);

                if (Input.GetKeyDown(_keyCode))
                {
                    int _keyIndex = AudioManager.FindKeyIndex(_keyCode);
                    if (_keyIndex >= 0)
                    {
                        Debug.Log(_keyIndex);
                        AddBeat(_keyIndex, _progress, true);
                    }
                }
            }
        }
    }

    public void AddBeat(int _beatTypeIndex, float _progressRatioTime, bool _showVisual)
    {
        BeatData _curBeatData = new BeatData() { typeIndex = _beatTypeIndex, progressRatioTime = _progressRatioTime };
        tuneAllBeats.Add(_curBeatData);
        tuneTypeBeats[_beatTypeIndex].Add(_curBeatData);
        if (_showVisual) UIManager.CreateBeatTypeVisual(_beatTypeIndex, _progressRatioTime);
    }

}
