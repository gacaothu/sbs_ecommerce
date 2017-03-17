namespace SBSECommerge.Framework.Utilities
{
    public sealed class EntityUtil
    {
        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <returns></returns>
        public static SBS_DEVEntities GetEntity()
        {
            return new SBS_DEVEntities();
        }
    }
}