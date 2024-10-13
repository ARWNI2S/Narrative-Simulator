namespace ARWNI2S.Portal.Services.Entities.Common
{
    /// <summary>
    /// Represents the entity sorting
    /// </summary>
    public enum SortingEnum
    {
        /// <summary>
        /// Position (display order)
        /// </summary>
        Position = 0,

        /// <summary>
        /// Name: A to Z
        /// </summary>
        NameAsc = 5,

        /// <summary>
        /// Name: Z to A
        /// </summary>
        NameDesc = 6,

        /// <summary>
        /// Value: Low to High
        /// </summary>
        ValueAsc = 10,

        /// <summary>
        /// Value: High to Low
        /// </summary>
        ValueDesc = 11,

        /// <summary>
        /// Product creation date
        /// </summary>
        CreatedOn = 15,
    }
}