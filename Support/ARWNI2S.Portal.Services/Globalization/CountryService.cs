using ARWNI2S.Node.Core;
using ARWNI2S.Node.Core.Caching;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Data.Extensions;
using ARWNI2S.Node.Services.Clustering;
using ARWNI2S.Node.Services.Localization;
using ARWNI2S.Portal.Services.Entities.Common;
using ARWNI2S.Portal.Services.Entities.Directory;

namespace ARWNI2S.Portal.Services.Globalization
{
    /// <summary>
    /// Country service
    /// </summary>
    public partial class CountryService : ICountryService
    {
        #region Fields

        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<Country> _countryRepository;
        private readonly INodeContext _nodeContext;
        private readonly INodeMappingService _nodeMappingService;

        #endregion

        #region Ctor

        public CountryService(
            IStaticCacheManager staticCacheManager,
            ILocalizationService localizationService,
            IRepository<Country> countryRepository,
            INodeContext nodeContext,
            INodeMappingService nodeMappingService)
        {
            _staticCacheManager = staticCacheManager;
            _localizationService = localizationService;
            _countryRepository = countryRepository;
            _nodeContext = nodeContext;
            _nodeMappingService = nodeMappingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a country
        /// </summary>
        /// <param name="country">Country</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCountryAsync(Country country)
        {
            await _countryRepository.DeleteAsync(country);
        }

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the countries
        /// </returns>
        public virtual async Task<IList<Country>> GetAllCountriesAsync(int languageId = 0, bool showHidden = false)
        {
            var node = await _nodeContext.GetCurrentNodeAsync();
            var key = _staticCacheManager.PrepareKeyForDefaultCache(GlobalizationServicesDefaults.CountriesAllCacheKey, languageId,
                showHidden, node);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var countries = await _countryRepository.GetAllAsync(async query =>
                {
                    if (!showHidden)
                    {
                        query = query.Where(c => c.Published);

                        //apply node mapping constraints
                        query = await _nodeMappingService.ApplyNodeMapping(query, node.Id);
                    }

                    return query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);
                });

                if (languageId > 0)
                {
                    //we should sort countries by localized names when they have the same display order
                    countries = await countries
                        .ToAsyncEnumerable()
                        .OrderBy(c => c.DisplayOrder)
                        .ThenByAwait(async c => await _localizationService.GetLocalizedAsync(c, x => x.Name, languageId))
                        .ToListAsync();
                }

                return countries;
            });
        }

        /// <summary>
        /// Gets all countries that allow billing
        /// </summary>
        /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the countries
        /// </returns>
        public virtual async Task<IList<Country>> GetAllCountriesForBillingAsync(int languageId = 0, bool showHidden = false)
        {
            return (await GetAllCountriesAsync(languageId, showHidden)).Where(c => c.AllowsBilling).ToList();
        }

        /// <summary>
        /// Gets all countries that allow shipping
        /// </summary>
        /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the countries
        /// </returns>
        public virtual async Task<IList<Country>> GetAllCountriesForShippingAsync(int languageId = 0, bool showHidden = false)
        {
            return (await GetAllCountriesAsync(languageId, showHidden)).Where(c => c.AllowsShipping).ToList();
        }

        /// <summary>
        /// Gets a country by address 
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        public virtual async Task<Country> GetCountryByAddressAsync(Address address)
        {
            return await GetCountryByIdAsync(address?.CountryId ?? 0);
        }

        /// <summary>
        /// Gets a country 
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        public virtual async Task<Country> GetCountryByIdAsync(int countryId)
        {
            return await _countryRepository.GetByIdAsync(countryId, cache => default);
        }

        /// <summary>
        /// Get countries by identifiers
        /// </summary>
        /// <param name="countryIds">Country identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the countries
        /// </returns>
        public virtual async Task<IList<Country>> GetCountriesByIdsAsync(int[] countryIds)
        {
            return await _countryRepository.GetByIdsAsync(countryIds);
        }

        /// <summary>
        /// Gets a country by two letter ISO code
        /// </summary>
        /// <param name="twoLetterIsoCode">Country two letter ISO code</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        public virtual async Task<Country> GetCountryByTwoLetterIsoCodeAsync(string twoLetterIsoCode)
        {
            if (string.IsNullOrEmpty(twoLetterIsoCode))
                return null;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(GlobalizationServicesDefaults.CountriesByTwoLetterCodeCacheKey, twoLetterIsoCode);

            var query = from c in _countryRepository.Table
                        where c.TwoLetterIsoCode == twoLetterIsoCode
                        select c;

            return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
        }

        /// <summary>
        /// Gets a country by three letter ISO code
        /// </summary>
        /// <param name="threeLetterIsoCode">Country three letter ISO code</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        public virtual async Task<Country> GetCountryByThreeLetterIsoCodeAsync(string threeLetterIsoCode)
        {
            if (string.IsNullOrEmpty(threeLetterIsoCode))
                return null;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(GlobalizationServicesDefaults.CountriesByThreeLetterCodeCacheKey, threeLetterIsoCode);

            var query = from c in _countryRepository.Table
                        where c.ThreeLetterIsoCode == threeLetterIsoCode
                        select c;

            return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
        }

        /// <summary>
        /// Inserts a country
        /// </summary>
        /// <param name="country">Country</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCountryAsync(Country country)
        {
            await _countryRepository.InsertAsync(country);
        }

        /// <summary>
        /// Updates the country
        /// </summary>
        /// <param name="country">Country</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCountryAsync(Country country)
        {
            await _countryRepository.UpdateAsync(country);
        }

        #endregion
    }
}