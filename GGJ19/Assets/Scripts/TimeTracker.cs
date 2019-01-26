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
    private UnityEngine.UI.Text _clockText;

    private float _timeForCurrentDayInSeconds;
    private float _secondsPerMinute;
    private float _secondsPerHour;
    private float _secondsPerDay;

    public int CurrentDay { get; set; } = 0;

    public void Start()
    {
        _timeForCurrentDayInSeconds = 0;
        _secondsPerHour = _realDayDurationInMinutes * SecondsPerMinute / _dayDurationHours;
        _secondsPerMinute = _secondsPerHour / 60;
        _secondsPerDay = _realDayDurationInMinutes * 60;
    }

    public void Update()
    {
        _timeForCurrentDayInSeconds = _timeForCurrentDayInSeconds += Time.deltaTime;

        if (_timeForCurrentDayInSeconds >= _secondsPerDay)
        {
            CurrentDay++;
            _timeForCurrentDayInSeconds = 0;
        }

        var _currentTimeHour = (int)(_timeForCurrentDayInSeconds / _secondsPerHour);
        var _currentTimeRest = (_timeForCurrentDayInSeconds / _secondsPerHour) - _currentTimeHour;
        var _currentTimeMinute = 60 * _currentTimeRest;
        var _adjustedCurrentTimeHour = _currentTimeHour + _initialHour;

        _clockText.text = (_adjustedCurrentTimeHour % 24).ToString("00") + ":" + GetMinuteFraction(_currentTimeMinute).ToString("00");
    }

    private int GetMinuteFraction(float minute)
    {
        if (minute < 15)
        {
            return 0;
        }

        if (minute < 30)
        {
            return 15;
        }

        if (minute < 45)
        {
            return 30;
        }

        return 45;
    }
}
