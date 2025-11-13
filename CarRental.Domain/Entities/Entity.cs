namespace CarRental.Domain.Entities
{
    /// <summary>
    /// Base class for all entities with an integer ID.
    /// </summary>
    public abstract class Entity
    {
        public int Id { get; protected set; }

        protected Entity()
        {
        }

        protected Entity(int id)
        {
            Id = id;
        }
    }
}
