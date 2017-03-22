using SBS_Ecommerce.Models;

namespace SBS_Ecommerce.Framework.Utilities
{
    public sealed class EntityUtil
    {
        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <returns></returns>
        public static SBS_DevEntities GetEntity()
        {
            return new SBS_DevEntities();
        }
    }
}