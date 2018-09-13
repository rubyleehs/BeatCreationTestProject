using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatData
{
    public int typeIndex;
    public float progressRatioTime;
}

public class TuneData
{
    public List<BeatData> tuneAllBeats;
    public List<List<BeatData>> tuneTypeBeats;
    public float tuneHalfDuration;
}

public class TuneManager : MonoBehaviour
{
    [Range(0,1)]
    public float scoreMinRatioSoftLowerLimit;
    public float scoreNoteTypeTurningCount;

    public EnemyManager enemyManager;
    public UIManager uiManager;
    public AudioManager audioManager;
    public TuneData[] tunes;//0 is cur tune and 1 is saved tune;

    public float currentTuneHalfDuration;
    public float captureRatioLeeway;
    public float maxMissLeeway;
    public List<BeatAnim> beatAnims = new List<BeatAnim>();

    private int numOfBeatTypes;
    private bool IsRecording = false;
    private bool IsMatching = false;
    public Transform recordingSprite;
    public Transform playingSprite;

    private void Start()
    {
        numOfBeatTypes = AudioManager.audioClipsInfo.Length;
        tunes = new TuneData[2];
        ClearTune(0,true);
        ClearTune(1,true);
    }

    private void Update()
    {
        if (IsRecording || IsMatching) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(tunes[0].tuneAllBeats == null || tunes[0].tuneAllBeats.Count == 0) StartCoroutine(StartNewRecording());
            else StartCoroutine(MatchTune());
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("Loading Saved Tune");
            CopyAndPasteTune(1, 0);
            UIManager.LoadTuneVisuals(tunes[0].tuneAllBeats);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CopyAndPasteTune(0, 1);
            Debug.Log("Saving Current Tune");
        }
        if (Input.GetKeyDown(KeyCode.L)) UIManager.ClearTuneVisuals();
    }

    public IEnumerator StartNewRecording()
    {
        Debug.Log("Recording Started!");
        if (IsRecording || IsMatching) yield break;

        IsRecording = true;
        recordingSprite.gameObject.SetActive(true); 
        ClearTune(0,true);
        //UIManager.ClearTuneVisuals();

        float _startTime = Time.time;
        float _progress = 0;

        while (_progress < 1)
        {
            _progress = (Time.time - _startTime) / (2 * currentTuneHalfDuration);
            HandleRecordingInput(_progress);
            UIManager.SetTimerLine(_progress);
            yield return null;
        }
        Debug.Log("Recording End!");
        TuneScorer.ScoreTune(this.GetComponent<TuneManager>(), scoreMinRatioSoftLowerLimit, scoreNoteTypeTurningCount);
        IsRecording = false;
        recordingSprite.gameObject.SetActive(false);
    }

    public IEnumerator MatchTune()
    {
        IsMatching = true;
        playingSprite.gameObject.SetActive(true);
        float _startTime = Time.time;
        float _progress = 0;
        int _matchIndex = 0;
        List<BeatData> _nextBeats = new List<BeatData>();
        while (_progress < 1)
        {
            _progress = (Time.time - _startTime) / (2 * currentTuneHalfDuration);
            UIManager.SetTimerLine(_progress);
            if (_matchIndex < tunes[0].tuneAllBeats.Count)
            {
                if (_nextBeats.Count == 0)
                {
                    _nextBeats = FindNextBeats(_matchIndex);
                }
                //
                if (_nextBeats[0].progressRatioTime + captureRatioLeeway < _progress)
                {
                    //beat missed
                    Debug.Log("Beat Missed - Too Late");
                    audioManager.PlayFailAudio(true);
                    while (_nextBeats.Count > 0)
                    {
                        InitiateBeatAnim(_matchIndex,false,0);
                        _matchIndex++;
                        _nextBeats.RemoveAt(0);
                    }
                }
                else
                {
                    for (int i = 0; i < _nextBeats.Count; i++)
                    {
                        if (Input.GetKeyDown(AudioManager.audioClipsInfo[_nextBeats[i].typeIndex].assignedKey))
                        {
                            if (_nextBeats[0].progressRatioTime - captureRatioLeeway > _progress && _nextBeats[0].progressRatioTime - maxMissLeeway < _progress)
                            {
                                Debug.Log("Beat Missed - Too Early");
                                audioManager.PlayFailAudio(false);
                                InitiateBeatAnim(_matchIndex, false,0);
                                _matchIndex ++;
                                _nextBeats.RemoveAt(i);
                                i--;
                            }
                            else if (_nextBeats[0].progressRatioTime + captureRatioLeeway > _progress && _nextBeats[0].progressRatioTime - captureRatioLeeway < _progress)//
                            {
                                float _acc = Mathf.SmoothStep(1.25f,0.5f,Mathf.Abs(_nextBeats[0].progressRatioTime - _progress) / captureRatioLeeway);
                                Debug.Log("Beat Captured");
                                StartCoroutine(audioManager.PlayClip(_nextBeats[i].typeIndex));
                                InitiateBeatAnim(_matchIndex, true,_acc);
                                _matchIndex++;
                                _nextBeats.RemoveAt(i);
                                i--;
                            }
                            //capture beat
                        }
                    }
                }
            }
            yield return null;      
        }
        while (_matchIndex < tunes[0].tuneAllBeats.Count)
        {
            Debug.Log("Beat Missed - Too Late");
            audioManager.PlayFailAudio(true);
            InitiateBeatAnim(_matchIndex, false,0);
            _matchIndex++;
        }
        yield return new WaitForSeconds(0.65f);
        IsMatching = false;
        playingSprite.gameObject.SetActive(false);
        ClearTune(0,false);
        enemyManager.ResetEnemy();
    }

    private List<BeatData> FindNextBeats(int _index)
    {
        List<BeatData> _beats = new List<BeatData>();
        if (tunes[0].tuneAllBeats.Count == 0) return _beats;

        _beats.Add(tunes[0].tuneAllBeats[_index]);
        int numBeatsAtNextTime = 1;
        while (_index + numBeatsAtNextTime < tunes[0].tuneAllBeats.Count)
        {
            if (tunes[0].tuneAllBeats[_index + numBeatsAtNextTime].progressRatioTime == _beats[0].progressRatioTime)
            {
                _beats.Add(tunes[0].tuneAllBeats[_index + numBeatsAtNextTime]);
                numBeatsAtNextTime++;
            }
            else break;
        }
        return _beats;
    }



    public static bool IsValidLetter(string _string)
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
                        AddBeat(_keyIndex, _progress, true);
                    }
                }
            }
        }
    }

    public void AddBeat(int _beatTypeIndex, float _progressRatioTime, bool _showVisual)
    {
        BeatData _curBeatData = new BeatData() { typeIndex = _beatTypeIndex, progressRatioTime = _progressRatioTime };
        tunes[0].tuneAllBeats.Add(_curBeatData);
        tunes[0].tuneTypeBeats[_beatTypeIndex].Add(_curBeatData);
        StartCoroutine(audioManager.PlayClip(_beatTypeIndex));
        if (_showVisual) beatAnims.Add(UIManager.CreateBeatTypeVisual(_beatTypeIndex, _progressRatioTime));
    }

    public void CopyAndPasteTune(int _fromIndex, int _toIndex)
    {
        ClearTune(_toIndex,true);
        tunes[_toIndex].tuneAllBeats.AddRange(tunes[_fromIndex].tuneAllBeats);
        for (int i = 0; i < tunes[_fromIndex].tuneTypeBeats.Count; i++)
        {
            tunes[_toIndex].tuneTypeBeats[i].AddRange(tunes[_fromIndex].tuneTypeBeats[i]);
        }
    }

    public void ClearTune(int _index, bool _clearToonVisuals)
    {
        Debug.Log("Tune Cleared!");
        if (_index == 0 && _clearToonVisuals) UIManager.ClearTuneVisuals();
        tunes[_index] = new TuneData
        {
            tuneAllBeats = new List<BeatData>(),
            tuneTypeBeats = new List<List<BeatData>>(),
            tuneHalfDuration = currentTuneHalfDuration,
        };
        for (int i = 0; i < numOfBeatTypes; i++)
        {
            tunes[_index].tuneTypeBeats.Add(new List<BeatData>());
        }
        beatAnims = new List<BeatAnim>();
    }

    public void InitiateBeatAnim(int _index, bool _beatCaptured, float _acc)
    {
        //CameraShake.Shake(0.15f, 0.28f);
        UIManager.lastEventTime = Time.time;
        uiManager.InitiateBarAnim(_beatCaptured);
        StartCoroutine(beatAnims[_index].EventAnim(_beatCaptured, _acc));
    }
}
