﻿using ARWNI2S.Infrastructure;
using ARWNI2S.Infrastructure.Collections.Rendering;
using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Node.Core.Entities;
using ARWNI2S.Node.Data.Extensions;
using ARWNI2S.Node.Services.Localization;

namespace ARWNI2S.Portal.Services
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Convert to select list
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumObj">Enum</param>
        /// <param name="markCurrentAsSelected">Mark current value as selected</param>
        /// <param name="valuesToExclude">Values to exclude</param>
        /// <param name="useLocalization">Localize</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the selectList
        /// </returns>
        public static async Task<SelectList> ToSelectListAsync<TEnum>(this TEnum enumObj,
           bool markCurrentAsSelected = true, int[] valuesToExclude = null, bool useLocalization = true) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("An Enumeration type is required.", nameof(enumObj));

            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var values = await Enum.GetValues(typeof(TEnum)).OfType<TEnum>().Where(enumValue =>
                    valuesToExclude == null || !valuesToExclude.Contains(Convert.ToInt32(enumValue)))
                .SelectAwait(async enumValue => new
                {
                    ID = Convert.ToInt32(enumValue),
                    Name = useLocalization
                        ? await localizationService.GetLocalizedEnumAsync(enumValue)
                        : CommonHelper.ConvertEnum(enumValue.ToString())
                }).ToListAsync();

            object selectedValue = null;
            if (markCurrentAsSelected)
                selectedValue = Convert.ToInt32(enumObj);
            return new SelectList(values, "ID", "Name", selectedValue);
        }

        /// <summary>
        /// Convert to select list
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="objList">List of objects</param>
        /// <param name="selector">Selector for name</param>
        /// <returns>SelectList</returns>
        public static SelectList ToSelectList<T>(this T objList, Func<BaseEntity, string> selector) where T : IEnumerable<BaseEntity>
        {
            return new SelectList(objList.Select(p => new { ID = p.Id, Name = selector(p) }), "ID", "Name");
        }

    }
}
