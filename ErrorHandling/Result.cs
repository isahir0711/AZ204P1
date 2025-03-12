namespace Project1.ErrorHandling
{
    public class Result<T>
    {
        public T Value { get; }
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }

        public Result(T value, bool issuccess, string errorMessage)
        {
            Value = value;
            IsSuccess = issuccess;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value, true, "");
        }

        public static Result<T> Failure(string errorMessage)
        {
            return new Result<T>(default, false, errorMessage);
        }
    }
}