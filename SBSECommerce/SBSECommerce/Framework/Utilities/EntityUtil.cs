using SBSECommerce.Models;

namespace SBSECommerce.Framework.Utilities
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