using System;

namespace GroupDocs.Viewer.UI.Cloud.Api.Common
{
    internal class Result
    {
        public bool IsSuccess { get; }
       
        public string Message { get; }
     
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, string message)
        {
            if (isSuccess && message != string.Empty)
                throw new InvalidOperationException();
            if (!isSuccess && message == string.Empty)
                throw new InvalidOperationException();

            IsSuccess = isSuccess;
            Message = message;
        }

        public static Result<T> Fail<T>(string message)
        {
            return new Result<T>(default(T), false, message);
        }

        public static Result<T> Fail<T>(Result result)
        {
            return new Result<T>(default(T), false, result.Message);
        }

        public static Result Fail(string message)
        {
            return new Result(false, message);
        }

        public static Result Ok()
        {
            return new Result(true, string.Empty);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, string.Empty);
        }
    }

    internal class Result<T> : Result
    {
        private readonly T _value;

        public T Value
        {
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException();

                return _value;
            }
        }

        protected internal Result(T value, bool isSuccess, string message)
            : base(isSuccess, message)
        {
            _value = value;
        }
    }
}