public class UnityTaskResult<T> {
    public bool HasResult { get; private set; }
    public T Result { get; private set; }

    public UnityTaskResult() {
        HasResult = false;
        Result = default(T);
    }

    public void SetResult(T value) {
        HasResult = true;
        Result = value;
    }
}