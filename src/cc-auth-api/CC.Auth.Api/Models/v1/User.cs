namespace CC.Auth.Api.Models.v1;

public class User : BaseEntity<UserId, string>
{
    public const string FixedPartitionKey = "CoreCoaching";
    public const string ContainerId = "users";

    public string PartitionKey { get; } = FixedPartitionKey;

    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    public User(string email, string firstName, string lastName) : base(UserId.Create())
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }
}

public class UserId : Id<string>
{
    private UserId(string value) : base(value) { }

    public static UserId Create() => new(Guid.CreateVersion7().ToString());
}

public abstract class BaseEntity<TId, TIdType>(TId id)
    where TId : Id<TIdType>
    where TIdType : notnull
{
    public TId Id { get; init; } = id;
}

public abstract class Id<TValue> : ValueObject where TValue : notnull
{
    public TValue Value { get; init; }

    protected Id(TValue value)
    {
        Value = value;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

public abstract class ValueObject : IEquatable<ValueObject>
{
    public abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var valueObject = (ValueObject)obj;

        return GetEqualityComponents()
            .SequenceEqual(valueObject.GetEqualityComponents());
    }

    public bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }

    public static bool operator ==(ValueObject left, ValueObject right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ValueObject left, ValueObject right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x.GetHashCode())
            .Aggregate((x, y) => x ^ y);
    }
}