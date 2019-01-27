using UnityEngine;

public class TimeTracker : MonoBehaviour
{
    private const int SecondsPerMinute = 60;
    
    [SerializeField]
    private int _realDayDurationInMinutes = 3;
    [SerializeField]
    private int _dayDurationHours = 12;
    [SerializeField]
    private int _initialHour = 7;
    [SerializeField]
    private int _minutesFraction = 15;
    [SerializeField]
    private UnityEngine.UI.Text _clockText;

    public delegate void NewDayBegins(int dayNumber);
    public delegate void DayEnds();
    public event NewDayBegins onNewDayBegun;
    public event DayEnds onDayEnded;

    private float _timeForCurrentDayInSeconds;
    private float _secondsPerHour;
    private float _secondsPerDay;

    public int CurrentDay { get; set; } = 1;

    public void Start()
    {
        _timeForCurrentDayInSeconds = 0;
        _secondsPerHour = _realDayDurationInMinutes * SecondsPerMinute / _dayDurationHours;
        _secondsPerDay = _realDayDurationInMinutes * 60;
    }

    public void Update()
    {
        _timeForCurrentDayInSeconds = _timeForCurrentDayInSeconds += Time.deltaTime;

        if (_timeForCurrentDayInSeconds >= _secondsPerDay)
        {
            BeginNewDay();
        }

        var _currentTimeHour = (int)(_timeForCurrentDayInSeconds / _secondsPerHour);
        var _currentTimeRest = (_timeForCurrentDayInSeconds / _secondsPerHour) - _currentTimeHour;
        var _currentTimeMinute = 60 * _currentTimeRest;
        var _adjustedCurrentTimeHour = _currentTimeHour + _initialHour;

        _clockText.text = (_adjustedCurrentTimeHour % 24).ToString("00") + ":" + GetMinuteFraction(_currentTimeMinute).ToString("00");
    }

    private void BeginNewDay()
    {
        onDayEnded?.Invoke();
        CurrentDay++;
        onNewDayBegun?.Invoke(CurrentDay);
        _timeForCurrentDayInSeconds = 0;
    }

    private int GetMinuteFraction(float minute)
    {
        return ((int) (minute / _minutesFraction)) * _minutesFraction;
    }
}
