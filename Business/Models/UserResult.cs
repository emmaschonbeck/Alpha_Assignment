namespace Business.Models;

public class UserResult : ServiceResult
{
    public IEnumerable<object>? Result { get; set; }
}
