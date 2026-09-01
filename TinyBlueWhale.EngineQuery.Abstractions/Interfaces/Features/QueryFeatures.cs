
namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features
{

    /// <summary>
    /// Represents an internal operation applied by a provider feature to the current query composition.
    /// </summary>
    internal interface IQueryFeatureOperation;

    /// <summary>
    /// Identifies a database provider profile that supports common table expressions.
    /// </summary>
    public interface ICTEFeature;

    /// <summary>
    /// Identifies a database provider profile that supports recursive common table expressions.
    /// </summary>
    public interface IRecursiveCTEFeature : ICTEFeature;

    /// <summary>
    /// Identifies a database provider profile that supports SQL window functions.
    /// </summary>
    public interface IWindowFunctionFeature;

    /// <summary>
    /// Identifies a database provider profile that supports LATERAL joins or APPLY-equivalent joins.
    /// </summary>
    public interface ILateralJoinFeature;

    /// <summary>
    /// Identifies a database provider profile that supports INTERSECT set operations.
    /// </summary>
    public interface IIntersectFeature;

    /// <summary>
    /// Identifies a database provider profile that supports EXCEPT set operations.
    /// </summary>
    public interface IExceptFeature;

    /// <summary>
    /// Identifies a database provider profile that supports query pagination.
    /// </summary>
    public interface IPaginationFeature;

    /// <summary>
    /// Identifies a database provider profile that supports OFFSET/FETCH pagination syntax.
    /// </summary>
    public interface IOffsetFetchPaginationFeature : IPaginationFeature;

    /// <summary>
    /// Identifies a database provider profile that supports LIMIT/OFFSET pagination syntax.
    /// </summary>
    public interface ILimitOffsetPaginationFeature : IPaginationFeature;
}
