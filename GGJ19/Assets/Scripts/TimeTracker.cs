using Assets.Scripts.Audio;
using UnityEngine;
using UnityEngine.UI;

public class TimeTracker : MonoBehaviour
{
    private const int SecondsPerMinute = 60;
    
    [SerializeField]
    private float _realDayDurationInMinutes = 3;
    [SerializeField]
    private int _dayDurationHours = 12;
    [SerializeField]
    private int _initialHour = 7;
    [SerializeField]
    private int _minutesFraction = 15;
    [SerializeField]
    private Text _clockText;
    [SerializeField]
    private Text _dayText;

    public delegate void NewDayBegins(int dayNumber);
    public delegate void DayEnds(int dayNumber);
    public event NewDayBegins onNewDayBegun;
    public event DayEnds onDayEnded;

    private float _timeForCurrentDayInSeconds;
    private float _secondsPerHour;
    private float _secondsPerDay;
    private bool _stopped;
    private SoundPlayer _soundPlayer;
    private float _timeToStartEndOfDaySoundInSeconds;
    private bool _endOfDaySoundPlaying = false;

    public int CurrentDay { get; set; } = 1;

    public void Awake()
    {
        _soundPlayer = FindObjectOfType<SoundPlayer>();
    }

    public void Start()
    {
        _timeForCurrentDayInSeconds = 0;
        _secondsPerHour = _realDayDurationInMinutes * SecondsPerMinute / _dayDurationHours;
        _secondsPerDay = _realDayDurationInMinutes * 60;
        _dayText.text = "1";
        Debug.Log("Day 1 starts.");

        const float EndOfDaySoundDuration = 10;
        _timeToStartEndOfDaySoundInSeconds = _secondsPerDay - EndOfDaySoundDuration;
    }

    public void Update()
    {
        if (_stopped)
        {
            return;
        }

        _timeForCurrentDayInSeconds = _timeForCurrentDayInSeconds += Time.deltaTime;

        if (_timeForCurrentDayInSeconds >= _secondsPerDay)
        {
            EndDay();
        }
        else if (MustPlayEndOfDaySound())
        {
            PlayEndOfDaySound();
        }

        var _currentTimeHour = (int)(_timeForCurrentDayInSeconds / _secondsPerHour);
        var _currentTimeRest = (_timeForCurrentDayInSeconds / _secondsPerHour) - _currentTimeHour;
        var _currentTimeMinute = 60 * _currentTimeRest;
        var _adjustedCurrentTimeHour = _currentTimeHour + _initialHour;

        _clockText.text = (_adjustedCurrentTimeHour % 24).ToString("00") + ":" + GetMinuteFraction(_currentTimeMinute).ToString("00");
    }

    public void BeginNewDay()
    {
        _stopped = false;
        CurrentDay++;
        Debug.Log($"Day {CurrentDay} starts.");
        onNewDayBegun?.Invoke(CurrentDay);
        _timeForCurrentDayInSeconds = 0;
        _dayText.text = CurrentDay.ToString();
    }

    public void EndDay()
    {
        _stopped = true;
        Debug.Log($"Day {CurrentDay} ends.");

        StopEndOfDaySound();

        onDayEnded?.Invoke(CurrentDay);
    }

    public void PauseTimer()
    {
        _stopped = true;
    }

    public void StartTimer()
    {
        _stopped = false;
    }

    private int GetMinuteFraction(float minute)
    {
        return ((int) (minute / _minutesFraction)) * _minutesFraction;
    }

    private bool MustPlayEndOfDaySound()
    {
        return !_endOfDaySoundPlaying && _timeForCurrentDayInSeconds >= _timeToStartEndOfDaySoundInSeconds;
    }

    private void PlayEndOfDaySound()
    {
        _endOfDaySoundPlaying = true;
        _soundPlayer.Play(Sound.EndOfDay);
    }

    private void StopEndOfDaySound()
    {
        _soundPlayer.Stop(Sound.EndOfDay);
        _endOfDaySoundPlaying = false;
    }
}
