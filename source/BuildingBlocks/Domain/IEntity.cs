namespace BuildingBlocks.Domain
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    ///     This serves as a base interface for <see cref="Entity{TKey}" /> and 
    ///     <see cref = "Entity" />. Also provides a simple means to develop your own base entity.
    /// </summary>
    public interface IEntity<TKey>
    {
        TKey Id { get; }

        IEnumerable<PropertyInfo> GetSignatureProperties();

        bool IsTransient();
    }
}