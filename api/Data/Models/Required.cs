namespace JwtAuthentication.Data.Models;

public class Required<TValue, TError>
{
    private readonly LinkedList<TError> _errors;
    public TValue? Value { get; }

    public IReadOnlyCollection<TError> Errors => _errors;

    public bool HasValue => Value is not null;

    public bool HasErrors => _errors.Count > 0;

    public Required(TValue value)
    {
        _errors = new LinkedList<TError>();

        ArgumentNullException.ThrowIfNull(value);

        Value = value;

    }

    public void AddError(TError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        _errors.AddLast(error);
    }

    public void AddErrors(IEnumerable<TError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        foreach (var error in errors)
        {
            _errors.AddLast(error);
        }
    }
}
