namespace Application.Wrappers;

public class ResponseWrapper:IResponseWrapper
{
    public List<string> Messages { get; set; } = [];
    public bool IsSuccessful { get; set; }
    
    public ResponseWrapper(){}

    
    // Flag - Fail
    public static IResponseWrapper Fail()
    {
        return new ResponseWrapper()
        {
            IsSuccessful = false
        };
    }

    public static IResponseWrapper Fail(string message)
    {
        return new ResponseWrapper()
        {
            IsSuccessful = false,
            Messages = [message]
        };
    }
    
    public static IResponseWrapper Fail(List<string> messages)
    {
        return new ResponseWrapper()
        {
            IsSuccessful = false,
            Messages = messages
        };
    }

    public static Task<IResponseWrapper> FailAsync()
    {
        return Task.FromResult(Fail());
    }
    
    public static Task<IResponseWrapper> FailAsync(string message)
    {
        return Task.FromResult(Fail(message));
    }
    
    public static Task<IResponseWrapper> FailAsync(List<string> messages)
    {
        return Task.FromResult(Fail(messages));
    }
    
    // Flag - Success
    public static IResponseWrapper Success()
    {
        return new ResponseWrapper()
        {
            IsSuccessful = true
        };
    }

    public static IResponseWrapper Success(string message)
    {
        return new ResponseWrapper()
        {
            IsSuccessful = true,
            Messages = [message]
        };
    }
    
    public static IResponseWrapper Success(List<string> messages)
    {
        return new ResponseWrapper()
        {
            IsSuccessful = true,
            Messages = messages
        };
    }

    public static Task<IResponseWrapper> SuccessAsync()
    {
        return Task.FromResult(Success());
    }
    
    public static Task<IResponseWrapper> SuccessAsync(string message)
    {
        return Task.FromResult(Success(message));
    }
    
    public static Task<IResponseWrapper> SuccessAsync(List<string> messages)
    {
        return Task.FromResult(Success(messages));
    }
}

public class ResponseWrapper<T> : ResponseWrapper, IResponseWrapper<T>
{
    public T Data { get; set; }

    public new static ResponseWrapper<T> Fail()
    {
        return new ResponseWrapper<T> { IsSuccessful = false };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public new static ResponseWrapper<T> Fail(string message)
    {
        return new ResponseWrapper<T> { IsSuccessful = false, Messages = [message] };
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public new static ResponseWrapper<T> Fail(List<string> message)
    {
        return new ResponseWrapper<T> { IsSuccessful = false, Messages = message };
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public new static Task<ResponseWrapper<T>> FailAsync(string message)
    {
        return Task.FromResult(Fail(message));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public new static ResponseWrapper<T> Success()
    {
        return new ResponseWrapper<T> { IsSuccessful = true };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public new static ResponseWrapper<T> Success(string message)
    {
        return new ResponseWrapper<T> { IsSuccessful = true, Messages = [message] };
    }
    
    public static ResponseWrapper<T> Success(T data)
    {
        return new ResponseWrapper<T> { IsSuccessful = true, Data = data };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ResponseWrapper<T> Success(T data, string message)
    {
        return new ResponseWrapper<T> { IsSuccessful = true,Data = data, Messages = [message] };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ResponseWrapper<T> Success(T data, List<string> messages)
    {
        return new ResponseWrapper<T> { IsSuccessful = true,Data = data, Messages = messages };
    }
    
    public new static Task<ResponseWrapper<T>> SuccessAsync()
    {
        return Task.FromResult(Success());
    }
    
    public new static Task<ResponseWrapper<T>> SuccessAsync(string message)
    {
        return Task.FromResult(Success(message));
    }
    
    public new static Task<ResponseWrapper<T>> SuccessAsync(T data)
    {
        return Task.FromResult(Success(data));
    }
    
    public new static Task<ResponseWrapper<T>> SuccessAsync(T data, string message)
    {
        return Task.FromResult(Success(data,message));
    }
    
    public new static Task<ResponseWrapper<T>> SuccessAsync(T data, List<string> message)
    {
        return Task.FromResult(Success(data, message));
    }
}