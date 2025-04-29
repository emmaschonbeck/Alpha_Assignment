namespace Business.Models;

public class AuthResult<T> : ServiceResult
{
    public T? Result { get; set; }
}

public class AuthResult : ServiceResult
{

}
