using System.Collections.Generic;
using bookstore_Management.Core.Constants;

namespace bookstore_Management.Core.Results
{
    /// <summary>
    /// Class kiểm tra và trả về lỗi + thông báo lỗi
    /// Bao gồm : class thông thường và class generic
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; } // trạng thái thành công
        public bool IsFailure => !IsSuccess; // trạng thái thất bại 
        public string ErrorMessage { get; } // tin nhắn báo lỗi
        public List<string> ValidationErrors { get; } // danh sách lỗi

        protected Result(bool isSuccess, string errorMessage = null, List<string> validationErrors = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ValidationErrors = validationErrors ?? new List<string>(); // nếu validationErrors null thì lấy vế phải và ngược lại
        }

        // Factory methods - Success
        public static Result Success() => new Result(true); 
        public static Result Success(string message) => new Result(true, message);
        
        
        // Factory methods - Failure
        public static Result Fail(string errorMessage) => new Result(false, errorMessage);
        public static Result ValidationFail(List<string> validationErrors) 
            => new Result(false, MessageConstants.InvalidData , validationErrors);
        
        // Helper method
        public static Result Combine(params Result[] results)
        {
            foreach (var result in results)
            {
                if (result.IsFailure)
                    return result;
            }
            return Success();
        }
    }
    
    public class Result<T> : Result // kế thừa từ result
    {
        public T Data { get; }

        private Result(bool isSuccess, T data, string errorMessage = null, List<string> validationErrors = null)
            : base(isSuccess, errorMessage, validationErrors)
        {
            Data = data;
        }

        // Factory method - sucess
        public static Result<T> Success(T data) => new Result<T>(true, data);
        public static Result<T> Success(T data, string message) => new Result<T>(true, data, message);
        
        // Factory method - failure
        public new static Result<T> Fail(string errorMessage) => new Result<T>(false, default, errorMessage);
        public new static Result<T> ValidationFail(List<string> validationErrors) 
            => new Result<T>(false, default, MessageConstants.InvalidData, validationErrors);
        
        // Implicit conversion
        public static implicit operator Result<T>(T value) => Success(value);
    }

}