namespace EchoRoom.Shared.Constants.Core;

public class ApiResult
{
    [JsonIgnore]
    public bool IsSucceed { get; set; }

    [JsonIgnore]
    public int StatusCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ErrorMessage { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ErrorCode { get; set; }


    protected ApiResult() 
    { 
    }

    public static ApiResult Success(int statusCode)
        => new()
        { 
            IsSucceed = true,
            StatusCode = statusCode 
        };

    public static ApiResult Fail(int statusCode, string errorMessage, ErrorStatusCode errorCode)
        => new()
        { 
            IsSucceed = false, 
            ErrorMessage = errorMessage, 
            ErrorCode = errorCode.ToString(), 
            StatusCode = statusCode 
        };
}

public class ApiResult<T> : ApiResult
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T? Data { get; private set; }

    private ApiResult() 
    { 
    }

    public static ApiResult<T> Success(int statusCode, T data)
        => new()
        { 
            IsSucceed = true,
            StatusCode = statusCode,
            Data = data, 
        };

    public new static ApiResult<T> Fail(int statusCode, string errorMessage, ErrorStatusCode errorCode)
        => new()
        { 
            IsSucceed = false, 
            ErrorMessage = errorMessage, 
            ErrorCode = errorCode.ToString(), 
            StatusCode = statusCode 
        };
}