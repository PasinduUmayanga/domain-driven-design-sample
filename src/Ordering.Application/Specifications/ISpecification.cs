namespace Ordering.Application.Specifications;

/// <summary>
/// Represents a named business rule that can be evaluated against a candidate object.
/// </summary>
/// <typeparam name="T">The type of object checked by the specification.</typeparam>
public interface ISpecification<in T>
{
    /// <summary>
    /// Determines whether the supplied candidate satisfies the business rule.
    /// </summary>
    /// <param name="candidate">The object to evaluate.</param>
    /// <returns><see langword="true"/> when the candidate satisfies the rule; otherwise, <see langword="false"/>.</returns>
    bool IsSatisfiedBy(T candidate);
}
