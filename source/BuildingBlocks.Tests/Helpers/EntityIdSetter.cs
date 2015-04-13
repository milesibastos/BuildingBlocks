namespace BuildingBlocks.Tests.Helpers
{
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using BuildingBlocks.Domain;

    /// <summary>
    ///     For better data integrity, it is imperitive that the <see cref = "Entity.Id" />
    ///     property is read-only and set only by the ORM.  With that said, some unit tests need 
    ///     Id set to a particular value; therefore, this utility enables that ability.  This class should 
    ///     never be used outside of the testing project; instead, implement <see cref = "IHasAssignedId{IdT}" /> to 
    ///     expose a public setter.
    /// </summary>
    public static class EntityIdSetter
    {
        /// <summary>
        ///     Uses reflection to set the Id of a <see cref = "Entity{IdT}" />.
        /// </summary>
        public static void SetIdOf<TId>(IEntity<TId> entity, TId id)
        {
            // Set the data property reflectively
            var idProperty = entity.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);

            Contract.Ensures(idProperty != null, "idProperty could not be found");

            idProperty.SetValue(entity, id, null);
        }

        /// <summary>
        ///     Uses reflection to set the Id of a <see cref = "Entity{IdT}" />.
        /// </summary>
        public static IEntity<TId> SetIdTo<TId>(this IEntity<TId> entity, TId id)
        {
            SetIdOf(entity, id);
            return entity;
        }
    }
}