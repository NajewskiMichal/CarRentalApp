using System.Reflection;
using CarRental.Domain.Entities;

namespace CarRental.Infrastructure.Persistence.Repositories
{
    internal static class EntityIdSetter
    {
        private static readonly PropertyInfo? IdProperty =
            typeof(Entity).GetProperty(nameof(Entity.Id), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        public static void SetId(Entity entity, int id)
        {
            // Use reflection to set the protected Id property.
            IdProperty?.SetValue(entity, id);
        }
    }
}
