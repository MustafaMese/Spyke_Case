using UnityEngine.Events;

namespace SlotMachineCase.Model
{
    public class ObservableUnityEvent<T, U> : UnityEvent<T, U>
    {
        private Observable<T> _observable;

        public ObservableUnityEvent(Observable<T> observable) : base()
        {
            _observable = observable;
        }

        public new void AddListener(UnityAction<T, U> call)
        {
            base.AddListener(call);
        
            _observable.OnValueChangedRefresh();
        }
    }

    public class Observable<T>
    {
        public ObservableUnityEvent<T, T> OnValueChanged;

        public T Value
        {
            set
            {
                _currentValue = OnValueChanging(_currentValue, value);
                OnValueChanged.Invoke(_previousValue, _currentValue);
            }
            get
            {
                return _currentValue;
            }
        }

        public T PreviousValue => _previousValue;

        private T _previousValue;
        private T _currentValue;

        public Observable()
        {
            OnValueChanged = new ObservableUnityEvent<T, T>(this);
        }
    
        public void OnValueChangedRefresh()
        {
            OnValueChanged.Invoke(_previousValue, _currentValue);
        }
        
        private T OnValueChanging(T previousValue, T newValue)
        {
            _previousValue = previousValue;
            return newValue;
        }

    }
}