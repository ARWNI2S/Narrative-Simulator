using ARWNI2S.Infrastructure;
using ARWNI2S.Node.Core;
using ARWNI2S.Node.Core.Entities.Clustering;
using ARWNI2S.Node.Core.Entities.Users;
using ARWNI2S.Node.Core.Infrastructure;
using ARWNI2S.Node.Core.Services.Helpers;
using ARWNI2S.Node.Data;
using ARWNI2S.Node.Core.Entities;
using ARWNI2S.Node.Core.Entities.Common;
using ARWNI2S.Node.Core.Entities.Directory;
using ARWNI2S.Node.Core.Entities.Localization;
using ARWNI2S.Node.Core.Entities.Logging;
using ARWNI2S.Node.Core.Entities.ScheduleTasks;
using ARWNI2S.Node.Data.Extensions;
using ARWNI2S.Node.Services;
using ARWNI2S.Node.Services.Common;
using ARWNI2S.Node.Services.Configuration;
using ARWNI2S.Node.Services.Localization;
using ARWNI2S.Node.Services.Users;
using ARWNI2S.Portal.Services.Entities;
using ARWNI2S.Portal.Services.Entities.Blogs;
using ARWNI2S.Portal.Services.Entities.Cms;
using ARWNI2S.Portal.Services.Entities.Common;
using ARWNI2S.Portal.Services.Entities.Directory;
using ARWNI2S.Portal.Services.Entities.Gdpr;
using ARWNI2S.Portal.Services.Entities.Mailing;
using ARWNI2S.Portal.Services.Entities.Media;
using ARWNI2S.Portal.Services.Entities.News;
using ARWNI2S.Portal.Services.Entities.Security;
using ARWNI2S.Portal.Services.Entities.Seo;
using ARWNI2S.Portal.Services.Entities.Tax;
using ARWNI2S.Portal.Services.Entities.Topics;
using ARWNI2S.Portal.Services.Entities.Users;
using ARWNI2S.Portal.Services.ExportImport;
using ARWNI2S.Portal.Services.Localization;
using ARWNI2S.Portal.Services.Seo;
using System.Globalization;
using System.Text;
using ARWNI2S.Portal.Services.Clustering;
using ARWNI2S.Node.Services.Installation;
using ARWNI2S.Node.Core.Network;

namespace ARWNI2S.Portal.Services.Installation
{
    /// <summary>
    /// Installation service
    /// </summary>
    public partial class PortalInstallationService : IInstallationService
	{
		#region Fields

		private readonly INI2SDataProvider _dataProvider;
		private readonly IEngineFileProvider _fileProvider;
		private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
		private readonly IRepository<Address> _addressRepository;
		//private readonly IRepository<Blockchain> _blockchainRepository;
		private readonly IRepository<Country> _countryRepository;
		private readonly IRepository<Currency> _currencyRepository;
		private readonly IRepository<User> _userRepository;
		private readonly IRepository<UserRole> _userRoleRepository;
		private readonly IRepository<EmailAccount> _emailAccountRepository;
		private readonly IRepository<Language> _languageRepository;
		private readonly IRepository<MeasureDimension> _measureDimensionRepository;
		private readonly IRepository<MeasureWeight> _measureWeightRepository;
		private readonly IRepository<StateProvince> _stateProvinceRepository;
		private readonly IRepository<NI2SNode> _nodeRepository;
		//private readonly IRepository<TaxCategory> _taxCategoryRepository;
		//private readonly IRepository<Token> _tokenRepository;
		//private readonly IRepository<NonFungibleToken> _nftRepository;
		private readonly IRepository<Topic> _topicRepository;
		private readonly IRepository<TopicTemplate> _topicTemplateRepository;
		private readonly IRepository<UrlRecord> _urlRecordRepository;
		private readonly IWebHelper _webHelper;

		#endregion

		#region Ctor

		public PortalInstallationService(INI2SDataProvider dataProvider,
			IEngineFileProvider fileProvider,
			IRepository<ActivityLogType> activityLogTypeRepository,
			IRepository<Address> addressRepository,
			//IRepository<Blockchain> blockchainRepository,
			IRepository<Country> countryRepository,
			IRepository<Currency> currencyRepository,
			IRepository<User> userRepository,
			IRepository<UserRole> userRoleRepository,
			IRepository<EmailAccount> emailAccountRepository,
			IRepository<Language> languageRepository,
			IRepository<MeasureDimension> measureDimensionRepository,
			IRepository<MeasureWeight> measureWeightRepository,
			IRepository<StateProvince> stateProvinceRepository,
			IRepository<NI2SNode> nodeRepository,
			//IRepository<TaxCategory> taxCategoryRepository,
			//IRepository<Token> tokenRepository,
			//IRepository<NonFungibleToken> nftRepository,
			IRepository<Topic> topicRepository,
			IRepository<TopicTemplate> topicTemplateRepository,
			IRepository<UrlRecord> urlRecordRepository,
			IWebHelper webHelper)
		{
			_dataProvider = dataProvider;
			_fileProvider = fileProvider;
			_activityLogTypeRepository = activityLogTypeRepository;
			_addressRepository = addressRepository;
			//_blockchainRepository = blockchainRepository;
			_countryRepository = countryRepository;
			_currencyRepository = currencyRepository;
			_userRepository = userRepository;
			_userRoleRepository = userRoleRepository;
			_emailAccountRepository = emailAccountRepository;
			_languageRepository = languageRepository;
			_measureDimensionRepository = measureDimensionRepository;
			_measureWeightRepository = measureWeightRepository;
			_stateProvinceRepository = stateProvinceRepository;
			_nodeRepository = nodeRepository;
			//_taxCategoryRepository = taxCategoryRepository;
			//_tokenRepository = tokenRepository;
			//_nftRepository = nftRepository;
			_topicRepository = topicRepository;
			_topicTemplateRepository = topicTemplateRepository;
			_urlRecordRepository = urlRecordRepository;
			_webHelper = webHelper;
		}

		#endregion

		#region Utilities

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task<T> InsertInstallationDataAsync<T>(T entity) where T : DataEntity
		{
			return await _dataProvider.InsertEntityAsync(entity);
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InsertInstallationDataAsync<T>(params T[] entities) where T : DataEntity
		{
			await _dataProvider.BulkInsertEntitiesAsync(entities);
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InsertInstallationDataAsync<T>(IList<T> entities) where T : DataEntity
		{
			if (!entities.Any())
				return;

			await InsertInstallationDataAsync(entities.ToArray());
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task UpdateInstallationDataAsync<T>(T entity) where T : DataEntity
		{
			await _dataProvider.UpdateEntityAsync(entity);
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task UpdateInstallationDataAsync<T>(IList<T> entities) where T : DataEntity
		{
			if (!entities.Any())
				return;

			foreach (var entity in entities)
				await _dataProvider.UpdateEntityAsync(entity);
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task<string> ValidateSeNameAsync<T>(T entity, string seName) where T : DataEntity
		{
			//duplicate of ValidateSeName method of \ARWNI2S.Node.Services\Seo\UrlRecordService.cs (we cannot inject it here)
			ArgumentNullException.ThrowIfNull(entity);

			//validation
			var okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
			seName = seName.Trim().ToLowerInvariant();

			var sb = new StringBuilder();
			foreach (var c in seName.ToCharArray())
			{
				var c2 = c.ToString();
				if (okChars.Contains(c2))
					sb.Append(c2);
			}

			seName = sb.ToString();
			seName = seName.Replace(" ", "-");
			while (seName.Contains("--"))
				seName = seName.Replace("--", "-");
			while (seName.Contains("__"))
				seName = seName.Replace("__", "_");

			//max length
			seName = CommonHelper.EnsureMaximumLength(seName, SeoServicesDefaults.SearchEngineNameLength);

			//ensure this seName is not reserved yet
			var i = 2;
			var tempSeName = seName;
			while (true)
			{
				//check whether such slug already exists (and that is not the current entity)

				var query = from ur in _urlRecordRepository.Table
							where tempSeName != null && ur.Slug == tempSeName
							select ur;
				var urlRecord = await query.FirstOrDefaultAsync();

				var entityName = entity.GetType().Name;
				var reserved = urlRecord != null && !(urlRecord.EntityId == entity.Id && urlRecord.EntityName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
				if (!reserved)
					break;

				tempSeName = $"{seName}-{i}";
				i++;
			}

			seName = tempSeName;

			return seName;
		}

		protected virtual string GetContentPath()
		{
			return _fileProvider.GetAbsolutePath(PortalInstallationDefaults.DefaultImagesPath);
		}

		#endregion

		#region Data Requeriments

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallNodesAsync()
		{
			var nodeUrl = _webHelper.GetNodeLocation();
			var nodes = new List<NI2SNode>
			{
				new() {
					//Name = nodeUrl.Contains("localhost") ? "Development Node" : "DragonCorp Metalink",
					//DefaultTitle = nodeUrl.Contains("localhost") ? "Development" : "Metalink",
					//DefaultMetaKeywords = string.Empty,
					//DefaultMetaDescription = string.Empty,
					//HomepageTitle = "Metalink",
					//HomepageDescription = "Metalink landing page",
					//Url = nodeUrl,
					//SslEnabled = _webHelper.IsCurrentConnectionSecured(),
					//Hosts = (nodeUrl.Contains("localhost") ? "localhost, " : "") + "metalink.dragoncorp.org, dragoncorp.azurewebsites.net",
					//DisplayOrder = 1,
					////should we set some default company info?
					//CompanyName = "Dragon Corp. Games, S.L.",
					//CompanyAddress = "Av. Catalana 178, Sant Adrià del Besos, Barcelona, Spain, zip, ",
					//CompanyPhoneNumber = "(123) 456-78901", //TODO: set real phone number
					//CompanyVat = null
				}
			};

			await InsertInstallationDataAsync(nodes);
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallMeasuresAsync(RegionInfo regionInfo)
		{
			var isMetric = regionInfo?.IsMetric ?? false;

			var measureDimensions = new List<MeasureDimension>
			{
				new() {
					Name = "inch(es)",
					SystemKeyword = SystemKeywords.Measures.Inches,
					Ratio = isMetric ? 39.3701M : 1M,
					DisplayOrder = isMetric ? 1 : 0
				},
				new() {
					Name = "feet",
					SystemKeyword = SystemKeywords.Measures.Feet,
					Ratio = isMetric ? 3.28084M : 0.08333333M,
					DisplayOrder = isMetric ? 1 : 0
				},
				new() {
					Name = "meter(s)",
					SystemKeyword = SystemKeywords.Measures.Meters,
					Ratio = isMetric ? 1M : 0.0254M,
					DisplayOrder = isMetric ? 0 : 1
				},
				new() {
					Name = "millimetre(s)",
					SystemKeyword = SystemKeywords.Measures.Millimetres,
					Ratio = isMetric ? 1000M : 25.4M,
					DisplayOrder = isMetric ? 0 : 1
				}
			};

			await InsertInstallationDataAsync(measureDimensions);

			var measureWeights = new List<MeasureWeight>
			{
				new() {
					Name = "ounce(s)",
					SystemKeyword = SystemKeywords.Measures.Ounces,
					Ratio = isMetric ? 35.274M : 16M,
					DisplayOrder = isMetric ? 1 : 0
				},
				new() {
					Name = "lb(s)",
					SystemKeyword = SystemKeywords.Measures.Pounds,
					Ratio = isMetric ? 2.20462M : 1M,
					DisplayOrder = isMetric ? 1 : 0
				},
				new() {
					Name = "kg(s)",
					SystemKeyword = SystemKeywords.Measures.Kilograms,
					Ratio = isMetric ? 1M : 0.45359237M,
					DisplayOrder = isMetric ? 0 : 1
				},
				new() {
					Name = "gram(s)",
					SystemKeyword = SystemKeywords.Measures.Grams,
					Ratio = isMetric ? 1000M : 453.59237M,
					DisplayOrder = isMetric ? 0 : 1
				}
			};

			await InsertInstallationDataAsync(measureWeights);
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallTaxCategoriesAsync()
		{
			var taxCategories = new List<TaxCategory>
			{
				new() {Name = "General", DisplayOrder = 1},
				new() {Name = "Reduced", DisplayOrder = 5},
				new() {Name = "Superreduced", DisplayOrder = 10},
			};

			await InsertInstallationDataAsync(taxCategories);
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallLanguagesAsync((string languagePackDownloadLink, int languagePackProgress) languagePackInfo, CultureInfo cultureInfo, RegionInfo regionInfo)
		{
			var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

			var defaultCulture = new CultureInfo(CommonServicesDefaults.DefaultLanguageCulture);
			var defaultLanguage = new Language
			{
				Name = defaultCulture.NativeName,
				LanguageCulture = defaultCulture.Name,
				UniqueSeoCode = defaultCulture.TwoLetterISOLanguageName,
				FlagImageFileName = $"{defaultCulture.Name.ToLowerInvariant()[^2..]}.png",
				Rtl = defaultCulture.TextInfo.IsRightToLeft,
				Published = true,
				DisplayOrder = 1
			};
			await InsertInstallationDataAsync(defaultLanguage);

			//Install locale resources for default culture
			var directoryPath = _fileProvider.MapPath(InstallationDefaults.LocalizationResourcesPath);
			var pattern = $"*.{InstallationDefaults.LocalizationResourcesFileExtension}";
			foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, pattern))
			{
				using var streamReader = new StreamReader(filePath);
				await localizationService.ImportResourcesFromXmlAsync(defaultLanguage, streamReader);
			}

			if (cultureInfo == null || regionInfo == null || cultureInfo.Name == CommonServicesDefaults.DefaultLanguageCulture)
				return;

			//Install resources for user culture
			var userLanguage = new Language
			{
				Name = cultureInfo.NativeName,
				LanguageCulture = cultureInfo.Name,
				UniqueSeoCode = cultureInfo.TwoLetterISOLanguageName,
				FlagImageFileName = $"{regionInfo.TwoLetterISORegionName.ToLowerInvariant()}.png",
				Rtl = cultureInfo.TextInfo.IsRightToLeft,
				Published = true,
				DisplayOrder = 2
			};
			await InsertInstallationDataAsync(userLanguage);

			if (!string.IsNullOrEmpty(languagePackInfo.languagePackDownloadLink))
			{
				//download and import language pack
				try
				{
					var httpClientFactory = EngineContext.Current.Resolve<IHttpClientFactory>();
					var httpClient = httpClientFactory.CreateClient(HttpDefaults.DefaultHttpClient);
					await using var stream = await httpClient.GetStreamAsync(languagePackInfo.languagePackDownloadLink);
					using var streamReader = new StreamReader(stream);
					await localizationService.ImportResourcesFromXmlAsync(userLanguage, streamReader);

					//set this language as default
					userLanguage.DisplayOrder = 0;
					await UpdateInstallationDataAsync(userLanguage);

					//save progress for showing in admin panel (only for first start)
					await InsertInstallationDataAsync(new GenericAttribute
					{
						EntityId = userLanguage.Id,
						Key = CommonServicesDefaults.LanguagePackProgressAttribute,
						KeyGroup = nameof(Language),
						Value = languagePackInfo.languagePackProgress.ToString(),
						//NodeId = 0,
						CreatedOrUpdatedDateUTC = DateTime.UtcNow
					});
				}
				catch { }
			}
			else
			{
				//Install locale resources for default culture
				var patternLocale = $"*.{InstallationDefaults.LocalizationResourcesFileExtension.Split('.')[0]}.{cultureInfo.Name}.{InstallationDefaults.LocalizationResourcesFileExtension.Split('.')[1]}";
				foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, patternLocale))
				{
					using var streamReader = new StreamReader(filePath);
					await localizationService.ImportResourcesFromXmlAsync(userLanguage, streamReader);
				}
			}

#if DEBUG
			//Install resources for user culture
			var debugLanguage = new Language
			{
				Name = "Debug",
				LanguageCulture = "en",
				UniqueSeoCode = "xx",
				FlagImageFileName = "debug.png",
				Rtl = false,
				Published = true,
				DisplayOrder = 10
			};
			await InsertInstallationDataAsync(debugLanguage);

#endif

		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallCurrenciesAsync(CultureInfo cultureInfo, RegionInfo regionInfo)
		{
			//set some currencies with a rate against the USD
			var defaultCurrencies = new List<string>() { "USD", "AUD", "GBP", "CAD", "CNY", "EUR", "HKD", "JPY", "RUB", "SEK", "INR" };
			var currencies = new List<Currency>
			{
				new() {
					Name = "US Dollar",
					CurrencyCode = "USD",
					Rate = 1,
					DisplayLocale = "en-US",
					CustomFormatting = string.Empty,
					Published = true,
					DisplayOrder = 1,
					CreatedOnUtc = DateTime.UtcNow,
					UpdatedOnUtc = DateTime.UtcNow,
					RoundingType = RoundingType.Rounding001
				},
				new() {
					Name = "Australian Dollar",
					CurrencyCode = "AUD",
					Rate = 1.34M,
					DisplayLocale = "en-AU",
					CustomFormatting = string.Empty,
					Published = false,
					DisplayOrder = 2,
					CreatedOnUtc = DateTime.UtcNow,
					UpdatedOnUtc = DateTime.UtcNow,
					RoundingType = RoundingType.Rounding001
				},
				new() {
					Name = "British Pound",
					CurrencyCode = "GBP",
					Rate = 0.75M,
					DisplayLocale = "en-GB",
					CustomFormatting = string.Empty,
					Published = false,
					DisplayOrder = 3,
					CreatedOnUtc = DateTime.UtcNow,
					UpdatedOnUtc = DateTime.UtcNow,
					RoundingType = RoundingType.Rounding001
				},
				new() {
					Name = "Canadian Dollar",
					CurrencyCode = "CAD",
					Rate = 1.32M,
					DisplayLocale = "en-CA",
					CustomFormatting = string.Empty,
					Published = false,
					DisplayOrder = 4,
					CreatedOnUtc = DateTime.UtcNow,
					UpdatedOnUtc = DateTime.UtcNow,
					RoundingType = RoundingType.Rounding001
				},
				new() {
					Name = "Chinese Yuan Renminbi",
					CurrencyCode = "CNY",
					Rate = 6.43M,
					DisplayLocale = "zh-CN",
					CustomFormatting = string.Empty,
					Published = false,
					DisplayOrder = 5,
					CreatedOnUtc = DateTime.UtcNow,
					UpdatedOnUtc = DateTime.UtcNow,
					RoundingType = RoundingType.Rounding001
				},
				new() {
					Name = "Euro",
					CurrencyCode = "EUR",
					Rate = 0.86M,
					DisplayLocale = string.Empty,
					CustomFormatting = $"{"\u20ac"}0.00", //euro symbol
					Published = false,
					DisplayOrder = 6,
					CreatedOnUtc = DateTime.UtcNow,
					UpdatedOnUtc = DateTime.UtcNow,
					RoundingType = RoundingType.Rounding001
				},
				new() {
					Name = "Hong Kong Dollar",
					CurrencyCode = "HKD",
					Rate = 7.84M,
					DisplayLocale = "zh-HK",
					CustomFormatting = string.Empty,
					Published = false,
					DisplayOrder = 7,
					CreatedOnUtc = DateTime.UtcNow,
					UpdatedOnUtc = DateTime.UtcNow,
					RoundingType = RoundingType.Rounding001
				},
				new() {
					Name = "Japanese Yen",
					CurrencyCode = "JPY",
					Rate = 110.45M,
					DisplayLocale = "ja-JP",
					CustomFormatting = string.Empty,
					Published = false,
					DisplayOrder = 8,
					CreatedOnUtc = DateTime.UtcNow,
					UpdatedOnUtc = DateTime.UtcNow,
					RoundingType = RoundingType.Rounding001
				},
				new() {
					Name = "Russian Rouble",
					CurrencyCode = "RUB",
					Rate = 63.25M,
					DisplayLocale = "ru-RU",
					CustomFormatting = string.Empty,
					Published = false,
					DisplayOrder = 9,
					CreatedOnUtc = DateTime.UtcNow,
					UpdatedOnUtc = DateTime.UtcNow,
					RoundingType = RoundingType.Rounding001
				},
				new() {
					Name = "Swedish Krona",
					CurrencyCode = "SEK",
					Rate = 8.80M,
					DisplayLocale = "sv-SE",
					CustomFormatting = string.Empty,
					Published = false,
					DisplayOrder = 10,
					CreatedOnUtc = DateTime.UtcNow,
					UpdatedOnUtc = DateTime.UtcNow,
					RoundingType = RoundingType.Rounding1
				},
				new() {
					Name = "Indian Rupee",
					CurrencyCode = "INR",
					Rate = 68.03M,
					DisplayLocale = "en-IN",
					CustomFormatting = string.Empty,
					Published = false,
					DisplayOrder = 12,
					CreatedOnUtc = DateTime.UtcNow,
					UpdatedOnUtc = DateTime.UtcNow,
					RoundingType = RoundingType.Rounding001
				}
			};

			//set additional currency
			if (cultureInfo != null && regionInfo != null)
			{
				if (!defaultCurrencies.Contains(regionInfo.ISOCurrencySymbol))
				{
					currencies.Add(new Currency
					{
						Name = regionInfo.CurrencyEnglishName,
						CurrencyCode = regionInfo.ISOCurrencySymbol,
						Rate = 1,
						DisplayLocale = cultureInfo.Name,
						CustomFormatting = string.Empty,
						Published = true,
						DisplayOrder = 0,
						CreatedOnUtc = DateTime.UtcNow,
						UpdatedOnUtc = DateTime.UtcNow,
						RoundingType = RoundingType.Rounding001
					});
				}

				foreach (var currency in currencies.Where(currency => currency.CurrencyCode == regionInfo.ISOCurrencySymbol))
				{
					currency.Published = true;
					currency.DisplayOrder = 0;
				}
			}

			await InsertInstallationDataAsync(currencies);
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallCountriesAndStatesAsync()
		{
			var countries = ISO3166.GetCollection().Select(country => new Country
			{
				Name = country.Name,
				AllowsBilling = true,
				AllowsShipping = true,
				TwoLetterIsoCode = country.Alpha2,
				ThreeLetterIsoCode = country.Alpha3,
				NumericIsoCode = country.NumericCode,
				SubjectToVat = country.SubjectToVat,
				DisplayOrder = country.NumericCode == 840 ? 1 : 100,
				Published = true
			}).ToList();

			await InsertInstallationDataAsync(countries.ToArray());

			//Import states for all countries
			var directoryPath = _fileProvider.MapPath(InstallationDefaults.LocalizationResourcesPath);
			var pattern = "*.txt";

			//we use different scope to prevent creating wrong settings in DI, because the settings data not exists yet
			var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
			using var scope = serviceScopeFactory.CreateScope();
			var importManager = EngineContext.Current.Resolve<IImportManager>(scope);
			foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, pattern))
			{
				await using var stream = new FileStream(filePath, FileMode.Open);
				await importManager.ImportStatesFromTxtAsync(stream, false);
			}
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallEmailAccountsAsync()
		{
			var emailAccounts = new List<EmailAccount>
			{
				new() {
					Email = "info@dragoncorp.org",
					DisplayName = "Metalink",
					Host = "mail.dragoncorp.org",
					Port = 465,
					Username = "info@dragoncorp.org",
					Password = "Dragoncorp01",
					EnableSsl = true,
					UseDefaultCredentials = false
				}
			};

			await InsertInstallationDataAsync(emailAccounts);
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallMessageTemplatesAsync()
		{
			var eaGeneral = _emailAccountRepository.Table.FirstOrDefault() ?? throw new NodeException("Default email account cannot be loaded");
			var messageTemplates = new List<MessageTemplate>
			{
				new() {
					Name = MessageTemplateSystemNames.BlogCommentNotification,
					Subject = "%Node.Name%. New blog comment.",
					Body = $"<p>{Environment.NewLine}<a href=\"%Node.URL%\">%Node.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new blog comment has been created for blog post \"%BlogComment.BlogPostTitle%\".{Environment.NewLine}</p>{Environment.NewLine}",
					IsActive = true,
					EmailAccountId = eaGeneral.Id
				},
				new() {
					Name = MessageTemplateSystemNames.UserEmailValidationMessage,
					Subject = "%Node.Name%. Email validation",
					Body = $"<a href=\"%Node.URL%\">%Node.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}To activate your account <a href=\"%User.AccountActivationURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Node.Name%{Environment.NewLine}",
					IsActive = true,
					EmailAccountId = eaGeneral.Id
				},
				new() {
					Name = MessageTemplateSystemNames.UserEmailRevalidationMessage,
					Subject = "%Node.Name%. Email validation",
					Body = $"<p>{Environment.NewLine}<a href=\"%Node.URL%\">%Node.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %User.FullName%!{Environment.NewLine}<br />{Environment.NewLine}To validate your new email address <a href=\"%User.EmailRevalidationURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Node.Name%{Environment.NewLine}</p>{Environment.NewLine}",
					IsActive = true,
					EmailAccountId = eaGeneral.Id
				},
				new() {
					Name = MessageTemplateSystemNames.UserPasswordRecoveryMessage,
					Subject = "%Node.Name%. Password recovery",
					Body = $"<a href=\"%Node.URL%\">%Node.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}To change your password <a href=\"%User.PasswordRecoveryURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Node.Name%{Environment.NewLine}",
					IsActive = true,
					EmailAccountId = eaGeneral.Id
				},
				new() {
					Name = MessageTemplateSystemNames.UserWelcomeMessage,
					Subject = "Welcome to %Node.Name%",
					Body = $"We welcome you to <a href=\"%Node.URL%\"> %Node.Name%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}You can now take part in the various services we have to offer you. Some of these services include:{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Permanent Cart - Any products added to your online cart remain there until you remove them, or check them out.{Environment.NewLine}<br />{Environment.NewLine}Address Book - We can now deliver your products to another address other than yours! This is perfect to send birthday gifts direct to the birthday-person themselves.{Environment.NewLine}<br />{Environment.NewLine}Order History - View your history of purchases that you have made with us.{Environment.NewLine}<br />{Environment.NewLine}Products Reviews - Share your opinions on products with our other users.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}For help with any of our online services, please email the node-owner: <a href=\"mailto:%Node.Email%\">%Node.Email%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Note: This email address was provided on our registration page. If you own the email and did not register on our site, please send an email to <a href=\"mailto:%Node.Email%\">%Node.Email%</a>.{Environment.NewLine}",
					IsActive = true,
					EmailAccountId = eaGeneral.Id
				},
				new() {
					Name = MessageTemplateSystemNames.UserRegisteredNotification,
					Subject = "%Node.Name%. New user registration",
					Body = $"<p>{Environment.NewLine}<a href=\"%Node.URL%\">%Node.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new user registered with your node. Below are the user's details:{Environment.NewLine}<br />{Environment.NewLine}Full name: %User.FullName%{Environment.NewLine}<br />{Environment.NewLine}Email: %User.Email%{Environment.NewLine}</p>{Environment.NewLine}",
					IsActive = true,
					EmailAccountId = eaGeneral.Id
				},
				new() {
					Name = MessageTemplateSystemNames.NewsCommentNotification,
					Subject = "%Node.Name%. New news comment.",
					Body = $"<p>{Environment.NewLine}<a href=\"%Node.URL%\">%Node.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new news comment has been created for news \"%NewsComment.NewsTitle%\".{Environment.NewLine}</p>{Environment.NewLine}",
					IsActive = true,
					EmailAccountId = eaGeneral.Id
				},
				new() {
					Name = MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage,
					Subject = "%Node.Name%. Subscription activation message.",
					Body = $"<p>{Environment.NewLine}<a href=\"%NewsLetterSubscription.ActivationUrl%\">Click here to confirm your subscription to our list.</a>{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}If you received this email by mistake, simply delete it.{Environment.NewLine}</p>{Environment.NewLine}",
					IsActive = true,
					EmailAccountId = eaGeneral.Id
				},
				new() {
					Name = MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage,
					Subject = "%Node.Name%. Subscription deactivation message.",
					Body = $"<p>{Environment.NewLine}<a href=\"%NewsLetterSubscription.DeactivationUrl%\">Click here to unsubscribe from our newsletter.</a>{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}If you received this email by mistake, simply delete it.{Environment.NewLine}</p>{Environment.NewLine}",
					IsActive = true,
					EmailAccountId = eaGeneral.Id
				},
				new() {
					Name = MessageTemplateSystemNames.NewVatSubmittedNotification,
					Subject = "%Node.Name%. New VAT number is submitted.",
					Body = $"<p>{Environment.NewLine}<a href=\"%Node.URL%\">%Node.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%User.FullName% (%User.Email%) has just submitted a new VAT number. Details are below:{Environment.NewLine}<br />{Environment.NewLine}VAT number: %User.VatNumber%{Environment.NewLine}<br />{Environment.NewLine}VAT number status: %User.VatNumberStatus%{Environment.NewLine}<br />{Environment.NewLine}Received name: %VatValidationResult.Name%{Environment.NewLine}<br />{Environment.NewLine}Received address: %VatValidationResult.Address%{Environment.NewLine}</p>{Environment.NewLine}",
					IsActive = true,
					EmailAccountId = eaGeneral.Id
				},
				new() {
					Name = MessageTemplateSystemNames.EmailAFriendMessage,
					Subject = "%Node.Name%. Referred Item",
					Body = $"<p>{Environment.NewLine}<a href=\"%Node.URL%\"> %Node.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%EmailAFriend.Email% was shopping on %Node.Name% and wanted to share the following item with you.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<b><a target=\"_blank\" href=\"%Product.ProductURLForUser%\">%Product.Name%</a></b>{Environment.NewLine}<br />{Environment.NewLine}%Product.ShortDescription%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}For more info click <a target=\"_blank\" href=\"%Product.ProductURLForUser%\">here</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%EmailAFriend.PersonalMessage%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Node.Name%{Environment.NewLine}</p>{Environment.NewLine}",
					IsActive = true,
					EmailAccountId = eaGeneral.Id
				},
				new() {
					Name = MessageTemplateSystemNames.NewPartnerAccountApplyNotification,
					Subject = "%Node.Name%. New partner account submitted.",
					Body = $"<p>{Environment.NewLine}<a href=\"%Node.URL%\">%Node.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%User.FullName% (%User.Email%) has just submitted for a partner account. Details are below:{Environment.NewLine}<br />{Environment.NewLine}Partner name: %Partner.Name%{Environment.NewLine}<br />{Environment.NewLine}Partner email: %Partner.Email%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}You can activate it in admin area.{Environment.NewLine}</p>{Environment.NewLine}",
					IsActive = true,
					EmailAccountId = eaGeneral.Id
				},
				new() {
					Name = MessageTemplateSystemNames.PartnerInformationChangeNotification,
					Subject = "%Node.Name%. Partner information change.",
					Body = $"<p>{Environment.NewLine}<a href=\"%Node.URL%\">%Node.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Partner %Partner.Name% (%Partner.Email%) has just changed information about itself.{Environment.NewLine}</p>{Environment.NewLine}",
					IsActive = true,
					EmailAccountId = eaGeneral.Id
				},
				new() {
					Name = MessageTemplateSystemNames.ContactUsMessage,
					Subject = "%Node.Name%. Contact us",
					Body = $"<p>{Environment.NewLine}%ContactUs.Body%{Environment.NewLine}</p>{Environment.NewLine}",
					IsActive = true,
					EmailAccountId = eaGeneral.Id
				},
				new() {
					Name = MessageTemplateSystemNames.ContactPartnerMessage,
					Subject = "%Node.Name%. Contact us",
					Body = $"<p>{Environment.NewLine}%ContactUs.Body%{Environment.NewLine}</p>{Environment.NewLine}",
					IsActive = true,
					EmailAccountId = eaGeneral.Id
				}
			};

			await InsertInstallationDataAsync(messageTemplates);
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallTopicTemplatesAsync()
		{
			var topicTemplates = new List<TopicTemplate>
			{
				new() {
					Name = "Default template",
					ViewPath = "TopicDetails",
					DisplayOrder = 1
				}
			};

			await InsertInstallationDataAsync(topicTemplates);
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallSettingsAsync(RegionInfo regionInfo)
		{
			var isMetric = regionInfo?.IsMetric ?? false;
			var country = regionInfo?.TwoLetterISORegionName ?? string.Empty;
			var isGermany = country == "DE";
			var isEurope = ISO3166.FromCountryCode(country)?.SubjectToVat ?? false;

			var settingService = EngineContext.Current.Resolve<ISettingService>();

			await settingService.SaveSettingAsync(new SitemapSettings
			{
				SitemapEnabled = true,
				SitemapPageSize = 200,
				SitemapIncludeBlogPosts = true,
				SitemapIncludeNews = true,
				SitemapIncludeTopics = true
			});

			await settingService.SaveSettingAsync(new SitemapXmlSettings
			{
				SitemapXmlEnabled = true,
				SitemapXmlIncludeBlogPosts = true,
				SitemapXmlIncludeNews = true,
				SitemapXmlIncludeCustomUrls = true,
				SitemapXmlIncludeTopics = true,
				RebuildSitemapXmlAfterHours = 2 * 24,
				SitemapBuildOperationDelay = 60
			});

			await settingService.SaveSettingAsync(new Node.Core.Entities.Common.CommonSettings
			{
				LogAllErrors = true,
				RestartTimeout = CommonServicesDefaults.RestartTimeout,
			});

			await settingService.SaveSettingAsync(new Entities.Common.CommonSettings
			{
				UseSystemEmailForContactUsForm = true,
				DisplayJavaScriptDisabledWarning = true,
				BreadcrumbDelimiter = "/",
				BbcodeEditorOpenLinksInNewWindow = false,
				PopupForTermsOfServiceLinks = true,
				JqueryMigrateScriptLoggingActive = false,
				UseResponseCompression = true,
				FaviconAndAppIconsHeadCode = "<link rel=\"apple-touch-icon\" sizes=\"180x180\" href=\"/icons/icons_0/apple-touch-icon.png\"><link rel=\"icon\" type=\"image/png\" sizes=\"32x32\" href=\"/icons/icons_0/favicon-32x32.png\"><link rel=\"icon\" type=\"image/png\" sizes=\"192x192\" href=\"/icons/icons_0/android-chrome-192x192.png\"><link rel=\"icon\" type=\"image/png\" sizes=\"16x16\" href=\"/icons/icons_0/favicon-16x16.png\"><link rel=\"manifest\" href=\"/icons/icons_0/site.webmanifest\"><link rel=\"mask-icon\" href=\"/icons/icons_0/safari-pinned-tab.svg\" color=\"#5bbad5\"><link rel=\"shortcut icon\" href=\"/icons/icons_0/favicon.ico\"><meta name=\"msapplication-TileColor\" content=\"#2d89ef\"><meta name=\"msapplication-TileImage\" content=\"/icons/icons_0/mstile-144x144.png\"><meta name=\"msapplication-config\" content=\"/icons/icons_0/browserconfig.xml\"><meta name=\"theme-color\" content=\"#ffffff\">",
				EnableHtmlMinification = true,
				HeaderCustomHtml = string.Empty,
				FooterCustomHtml = string.Empty
			});

			await settingService.SaveSettingAsync(new SeoSettings
			{
				PageTitleSeparator = ". ",
				PageTitleSeoAdjustment = PageTitleSeoAdjustment.PagenameAfterNodename,
				//GenerateProductMetaDescription = true,
				ConvertNonWesternChars = false,
				AllowUnicodeCharsInUrls = true,
				CanonicalUrlsEnabled = false,
				QueryStringInCanonicalUrlsEnabled = false,
				WwwRequirement = WwwRequirement.NoMatter,
				TwitterMetaTags = false,
				OpenGraphMetaTags = false,
				MicrodataEnabled = false,
				ReservedUrlRecordSlugs = SeoServicesDefaults.ReservedUrlRecordSlugs,
				CustomHeadTags = string.Empty
			});

			await settingService.SaveSettingAsync(new AdminAreaSettings
			{
				DefaultGridPageSize = 15,
				PopupGridPageSize = 7,
				GridPageSizes = "7, 15, 20, 50, 100",
				RichEditorAdditionalSettings = null,
				RichEditorAllowJavaScript = true,
				RichEditorAllowStyleTag = true,
				UseRichEditorForUserEmails = true,
				UseRichEditorInMessageTemplates = true,
				UseIsoDateFormatInJsonResult = true,
				ShowDocumentationReferenceLinks = false
			});

			await settingService.SaveSettingAsync(new GdprSettings
			{
				DeleteInactiveUsersAfterMonths = 36,
				GdprEnabled = false,
				LogPrivacyPolicyConsent = true,
				LogNewsletterConsent = true,
				LogUserProfileChanges = true
			});

			await settingService.SaveSettingAsync(new ClusteringSettings
			{
				IgnoreAcl = true,
				IgnoreNodeLimitations = true,
				//UNDONE MORE NODE SETTINGS
			});
			await settingService.SaveSettingAsync(new PortalSettings
			{
				DefaultViewMode = "grid",
				ExportImportAllowDownloadImages = true,
				ExportImportRelatedEntitiesByName = true,
				ExportImportUseDropdownlistsForAssociatedEntities = true,
				//UNDONE MORE NODE SETTINGS
			});
			
			await settingService.SaveSettingAsync(new PortalLocalizationSettings
			{
				DefaultAdminLanguageId = _languageRepository.Table.Single(l => l.LanguageCulture == CommonServicesDefaults.DefaultLanguageCulture).Id,
				UseImagesForLanguageSelection = false,
				SeoFriendlyUrlsForLanguagesEnabled = false,
				AutomaticallyDetectLanguage = false,
				LoadAllLocaleRecordsOnStartup = true,
				LoadAllLocalizedPropertiesOnStartup = true,
				LoadAllUrlRecordsOnStartup = false,
				IgnoreRtlPropertyForAdminArea = false
			});

			await settingService.SaveSettingAsync(new WebUserSettings
			{
				UsernamesEnabled = true,
				CheckUsernameAvailabilityEnabled = true,
				AllowUsersToChangeUsernames = true,
				DefaultPasswordFormat = PasswordFormat.Hashed,
				HashedPasswordFormat = UserServicesDefaults.DefaultHashedPasswordFormat,
				PasswordMinLength = 6,
				PasswordMaxLength = 64,
				PasswordRequireDigit = false,
				PasswordRequireLowercase = false,
				PasswordRequireNonAlphanumeric = false,
				PasswordRequireUppercase = false,
				UnduplicatedPasswordsNumber = 4,
				PasswordRecoveryLinkDaysValid = 7,
				PasswordLifetime = 90,
				FailedPasswordAllowedAttempts = 0,
				FailedPasswordLockoutMinutes = 30,
				UserRegistrationType = UserRegistrationType.Validation,
				AllowUsersToUploadAvatars = true,
				AvatarMaximumSizeBytes = 20000,
				DefaultAvatarEnabled = true,
				ShowUsersLocation = true,
				ShowUsersJoinDate = true,
				AllowViewingProfiles = true,
				NotifyNewUserRegistration = true,
				DownloadableProductsValidateUser = false,
				UserNameFormat = UserNameFormat.ShowUsernames,
				FirstNameEnabled = true,
				FirstNameRequired = true,
				LastNameEnabled = true,
				LastNameRequired = true,
				GenderEnabled = true,
				DateOfBirthEnabled = true,
				DateOfBirthRequired = true,
				DateOfBirthMinimumAge = 13,
				CompanyEnabled = false,
				StreetAddressEnabled = false,
				StreetAddress2Enabled = false,
				ZipPostalCodeEnabled = false,
				CityEnabled = false,
				CountyEnabled = false,
				CountyRequired = false,
				CountryEnabled = false,
				CountryRequired = false,
				StateProvinceEnabled = false,
				StateProvinceRequired = false,
				PhoneEnabled = false,
				FaxEnabled = false,
				AcceptPrivacyPolicyEnabled = false,
				NewsletterEnabled = true,
				NewsletterTickedByDefault = true,
				HideNewsletterBlock = false,
				NewsletterBlockAllowToUnsubscribe = true,
				OnlineUserMinutes = 20,
				StoreLastVisitedPage = true,
				StoreIpAddresses = true,
				LastActivityMinutes = 15,
				SuffixDeletedUsers = false,
				EnteringEmailTwice = false,
				RequireRegistrationForDownloadableProducts = true,
				DeleteGuestTaskOlderThanMinutes = 1440,
				PhoneNumberValidationEnabled = true,
				PhoneNumberValidationUseRegex = false,
				PhoneNumberValidationRule = "^[0-9]{1,14}?$"
			});

			await settingService.SaveSettingAsync(new MultiFactorAuthenticationSettings
			{
				ForceMultifactorAuthentication = false
			});

			await settingService.SaveSettingAsync(new AddressSettings
			{
				CompanyEnabled = true,
				CompanyRequired = false,
				StreetAddressEnabled = true,
				StreetAddressRequired = true,
				StreetAddress2Enabled = true,
				ZipPostalCodeEnabled = true,
				ZipPostalCodeRequired = true,
				CityEnabled = true,
				CityRequired = true,
				CountyEnabled = false,
				CountyRequired = false,
				CountryEnabled = true,
				StateProvinceEnabled = true,
				PhoneEnabled = true,
				PhoneRequired = true,
				FaxEnabled = true
			});

			await settingService.SaveSettingAsync(new MediaSettings
			{
				MaximumImageSize = 1980,
				DefaultUiIconPictureSize = 64,
				DefaultImageQuality = 80,
				MultipleThumbDirectories = false,
				DefaultPictureZoomEnabled = false,
				AllowSVGUploads = false,

				AvatarPictureSize = 415,
				AvatarThumbPictureSize = 120,
				ProfileAvatarPictureSize = 220,
				ProfileMedalBadgePictureSize = 78,

				QuestDetailsPictureSize = 415,
				QuestThumbPictureSize = 120,
				QuestThumbPictureSizeOnQuestDetailsPage = 220,
				AssociatedQuestPictureSize = 220,

				TournamentDetailsPictureSize = 415,
				TournamentThumbPictureSize = 120,
				TournamentThumbPictureSizeOnTournamentDetailsPage = 220,
				AssociatedTournamentPictureSize = 220,

				DefaultPagePictureSize = 415,
				DefaultThumbPictureSize = 120,
				MiniatureThumbPictureSize = 78,
				PartnerThumbPictureSize = 120,
				GameTitleThumbPictureSize = 120,

				AutoCompleteSearchThumbPictureSize = 20,
				ImageSquarePictureSize = 32,
				ImportGameplayImagesUsingHash = true,
				AzureCacheControlHeader = string.Empty,
				UseAbsoluteImagePath = true,
				VideoIframeAllow = "fullscreen",
				VideoIframeWidth = 300,
				VideoIframeHeight = 150,
			});

			await settingService.SaveSettingAsync(new PortalInfoSettings
			{
				PortalOffline = false,
				DisplayEuCookieLawWarning = isEurope,
				FacebookLink = "",
				DiscordInviteLink = "https://discord.com/invite/v6cRyscjAB",
				DiscordServerId = "888347396566814741",
				TwitterLink = "https://twitter.com/dragoncorp_org",
				YouTubeLink = "",
				InstagramLink = "https://www.instagram.com/dragoncorp_org/"
			});

			await settingService.SaveSettingAsync(new ExternalAuthenticationSettings
			{
				RequireEmailValidation = false,
				LogErrors = false,
				AllowUsersToRemoveAssociations = true
			});

			//await settingService.SaveSettingAsync(new WalletAuthenticationSettings
			//{
			//	RequireEmailValidation = false,
			//	LogErrors = false,
			//	AllowUsersToRemoveWallets = true
			//});

			var primaryCurrency = "EUR";
			await settingService.SaveSettingAsync(new CurrencySettings
			{
				DisplayCurrencyLabel = false,
				PrimaryNodeCurrencyId =
					_currencyRepository.Table.Single(c => c.CurrencyCode == primaryCurrency).Id,
				PrimaryExchangeRateCurrencyId =
					_currencyRepository.Table.Single(c => c.CurrencyCode == primaryCurrency).Id,
				ActiveExchangeRateProviderSystemName = "ExchangeRate.ECB",
				AutoUpdateEnabled = false
			});

			var baseDimension = isMetric ? SystemKeywords.Measures.Meters : SystemKeywords.Measures.Inches;
			var baseWeight = isMetric ? SystemKeywords.Measures.Kilograms : SystemKeywords.Measures.Pounds;

			await settingService.SaveSettingAsync(new MeasureSettings
			{
				BaseDimensionId =
					_measureDimensionRepository.Table.Single(m => m.SystemKeyword == baseDimension).Id,
				BaseWeightId = _measureWeightRepository.Table.Single(m => m.SystemKeyword == baseWeight).Id
			});

			//await settingService.SaveSettingAsync(new BlockchainSettings
			//{
			//	Enabled = true,

			//});

			//await settingService.SaveSettingAsync(new TokenSettings
			//{
			//	DisplayTokenLabel = true,
			//	ActiveExchangeRateProviderSystemName = "ExchangeRate.Binance",
			//	AutoUpdateEnabled = false
			//});

			//await settingService.SaveSettingAsync(new MetaverseSettings
			//{
			//	//TODO: METAVERSE SETTINGS :?
			//});


			await settingService.SaveSettingAsync(new MessageTemplatesSettings
			{
				CaseInvariantReplacement = false,
				Color1 = "#b9babe", //TODO: CHANGE COLORS
				Color2 = "#ebecee",
				Color3 = "#dde2e6"
			});

			await settingService.SaveSettingAsync(new PortalSecuritySettings
			{
				EncryptionKey = CommonHelper.GenerateRandomDigitCode(16),
				AdminAreaAllowedIpAddresses = null,
				HoneypotEnabled = true,
				HoneypotInputName = "hpinput",
				AllowNonAsciiCharactersInHeaders = true
			});

			await settingService.SaveSettingAsync(new TaxSettings
			{
				TaxBasedOn = TaxBasedOn.BillingAddress,
				TaxBasedOnPickupPointAddress = false,
				TaxDisplayType = TaxDisplayType.ExcludingTax,
				ActiveTaxProviderSystemName = "Tax.FixedOrByCountryStateZip",
				DefaultTaxAddressId = 0,
				DisplayTaxSuffix = false,
				DisplayTaxRates = false,
				PricesIncludeTax = false,
				AllowUsersToSelectTaxDisplayType = false,
				ForceTaxExclusionFromOrderSubtotal = false,
				DefaultTaxCategoryId = 0,
				HideZeroTax = false,
				HideTaxInOrderSummary = false,
				ShippingIsTaxable = false,
				ShippingPriceIncludesTax = false,
				ShippingTaxClassId = 0,
				PaymentMethodAdditionalFeeIsTaxable = false,
				PaymentMethodAdditionalFeeIncludesTax = false,
				PaymentMethodAdditionalFeeTaxClassId = 0,
				EuVatEnabled = false/*isEurope*/,
				EuVatEnabledForGuests = false,
				EuVatShopCountryId = isEurope ? _countryRepository.Table.FirstOrDefault(x => x.TwoLetterIsoCode == country)?.Id ?? 0 : 0,
				EuVatAllowVatExemption = true,
				EuVatUseWebService = false,
				EuVatAssumeValid = false,
				EuVatEmailAdminWhenNewVatSubmitted = false,
				LogErrors = false
			});

			await settingService.SaveSettingAsync(new DateTimeSettings
			{
				DefaultNodeTimeZoneId = string.Empty,
				AllowUsersToSetTimeZone = true
			});

			await settingService.SaveSettingAsync(new BlogSettings
			{
				Enabled = true,
				PostsPageSize = 10,
				AllowNotRegisteredUsersToLeaveComments = false,
				NotifyAboutNewBlogComments = true,
				NumberOfTags = 15,
				ShowHeaderRssUrl = true,
				BlogCommentsMustBeApproved = false,
				ShowBlogCommentsPerNode = false
			});
			await settingService.SaveSettingAsync(new NewsSettings
			{
				Enabled = true,
				AllowNotRegisteredUsersToLeaveComments = false,
				NotifyAboutNewNewsComments = true,
				ShowNewsOnMainPage = true,
				MainPageNewsCount = 3,
				NewsArchivePageSize = 10,
				ShowHeaderRssUrl = true,
				NewsCommentsMustBeApproved = false,
				ShowNewsCommentsPerNode = false
			});

			//await settingService.SaveSettingAsync(new PartnerSettings
			//{
			//	DefaultPartnerPageSizeOptions = "6, 3, 9",
			//	PartnersBlockItemsToDisplay = 0,
			//	ShowPartnerOnDetailsPage = true,
			//	AllowUsersToContactPartners = true,
			//	AllowUsersToApplyForPartnerAccount = true,
			//	TermsOfServiceEnabled = true,
			//	AllowPartnersToEditInfo = false,
			//	NotifyAboutPartnerInformationChange = true,
			//	MaximumQuestNumber = 100,
			//	AllowPartnersToCreateQuests = true
			//});

			var eaGeneral = _emailAccountRepository.Table.FirstOrDefault() ?? throw new NodeException("Default email account cannot be loaded");
			await settingService.SaveSettingAsync(new EmailAccountSettings { DefaultEmailAccountId = eaGeneral.Id });

			await settingService.SaveSettingAsync(new WidgetSettings
			{
				ActiveWidgetSystemNames = ["Widgets.NivoSlider"]
			});

			await settingService.SaveSettingAsync(new MenuDisplaySettings
			{
				DisplayHomepageMenuItem = true,
				DisplayNewContentMenuItem = true,
				DisplayContentSearchMenuItem = false,
				DisplayUserInfoMenuItem = false,
				DisplayGamesMenuItem = true,
				DisplayQuestsMenuItem = true,
				DisplayInventoryMenuItem = true,
				DisplayTournamentsMenuItem = true,
				DisplayBlogMenuItem = false,
				DisplayNewsMenuItem = true,
				DisplayContactUsMenuItem = false
			});

			await settingService.SaveSettingAsync(new FooterDisplaySettings
			{
				DisplaySitemapFooterItem = false,
				DisplayContactUsFooterItem = true,
				DisplaySearchFooterItem = false,
				DisplayNewsFooterItem = true,
				DisplayBlogFooterItem = true,
				DisplayNewContentFooterItem = false,
				DisplayUserInfoFooterItem = false,
				DisplayApplyPartnerAccountFooterItem = true
			});

			await settingService.SaveSettingAsync(new CaptchaSettings
			{
				ReCaptchaApiUrl = "https://www.google.com/recaptcha/",
				ReCaptchaDefaultLanguage = string.Empty,
				ReCaptchaPrivateKey = "6Lftd9omAAAAAPTiu0_GDZHvjGZEDtyIsw4YX-IF",
				ReCaptchaPublicKey = "6Lftd9omAAAAADXFOX39Is5W18akuMHKO_6tbyss",
				ReCaptchaRequestTimeout = 20,
				ReCaptchaTheme = string.Empty,
				AutomaticallyChooseLanguage = true,
				Enabled = true,
				CaptchaType = CaptchaType.ReCaptchaV3,
				ReCaptchaV3ScoreThreshold = 0.5M,
				ShowOnApplyPartnerPage = true,
				ShowOnBlogCommentPage = true,
				ShowOnContactUsPage = true,
				ShowOnForgotPasswordPage = true,
				ShowOnLoginPage = true,
				ShowOnNewsCommentPage = true,
				ShowOnRegistrationPage = true,
			});

			await settingService.SaveSettingAsync(new MessagesSettings { UsePopupNotifications = false });

			await settingService.SaveSettingAsync(new ProxySettings
			{
				Enabled = false,
				Address = string.Empty,
				Port = string.Empty,
				Username = string.Empty,
				Password = string.Empty,
				BypassOnLocal = true,
				PreAuthenticate = true
			});

			await settingService.SaveSettingAsync(new CookieSettings
			{
				CompareProductsCookieExpires = 24 * 10,
				RecentlyViewedProductsCookieExpires = 24 * 10,
				UserCookieExpires = 24 * 365
			});

			await settingService.SaveSettingAsync(new RobotsTxtSettings
			{
				DisallowPaths =
				[
					"/admin",
					"/bin/",
					"/files/",
					"/files/exportimport/",
					"/country/getstatesbycountryid",
					"/install",
					"/setproductreviewhelpfulness",
					"/*?*returnUrl="
				],
				LocalizableDisallowPaths =
				[
					"/changecurrency",
					"/changetoken",
					"/changelanguage",
					"/changetaxtype",
					"/user/avatar",
					"/user/activation",
					"/user/addresses",
					"/user/changepassword",
					"/user/checkusernameavailability",
					"/user/downloadableproducts",
					"/user/info",
					"/user/productreviews",
					"/eucookielawaccept",
					"/inboxupdate",
					"/newsletter/subscriptionactivation",
					"/passwordrecovery/confirm",
					"/poll/vote",
					"/search?",
					"/sentupdate",
					"/nodeoffline",
					"/subscribenewsletter",
					"/topic/authenticate",
				]
			});
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallUsersAndRolesAsync(string defaultUserEmail, string defaultUserPassword)
		{
			var urAdministrators = new UserRole
			{
				Name = "Administrators",
				Active = true,
				IsSystemRole = true,
				SystemName = UserDefaults.AdministratorsRoleName
			};
			var urModerators = new UserRole
			{
				Name = "Moderators",
				Active = true,
				IsSystemRole = true,
				SystemName = UserDefaults.ModeratorsRoleName
			};
			var urRegistered = new UserRole
			{
				Name = "Registered",
				Active = true,
				IsSystemRole = true,
				SystemName = UserDefaults.RegisteredRoleName
			};
			var urGuests = new UserRole
			{
				Name = "Guests",
				Active = true,
				IsSystemRole = true,
				SystemName = UserDefaults.GuestsRoleName
			};
			var urPartners = new UserRole
			{
				Name = "Partners",
				Active = true,
				IsSystemRole = true,
				SystemName = UserDefaults.PartnersRoleName
			};
			var urPlayers = new UserRole
			{
				Name = "Players",
				Active = true,
				IsSystemRole = true,
				SystemName = UserDefaults.PlayersRoleName
			};
			var userRoles = new List<UserRole>
			{
				urAdministrators,
				urModerators,
				urRegistered,
				urGuests,
				urPartners,
				urPlayers
			};

			await InsertInstallationDataAsync(userRoles);

			//default node 
			var defaultNode = await _nodeRepository.Table.FirstOrDefaultAsync() ?? throw new NodeException("No default node could be loaded");
			var nodeId = defaultNode.Id;

			//admin user
			var adminUser = new User
			{
				UserGuid = Guid.NewGuid(),
				Email = defaultUserEmail,
				Username = defaultUserEmail,
				Active = true,
				CreatedOnUtc = DateTime.UtcNow,
				LastActivityDateUtc = DateTime.UtcNow,
				RegisteredInNodeId = nodeId
			};

			var defaultAdminUserAddress = await InsertInstallationDataAsync(
				new Address
				{
					FirstName = "Administrador",
					LastName = string.Empty,
					PhoneNumber = string.Empty,
					Email = defaultUserEmail,
					FaxNumber = string.Empty,
					Company = "Dragon Corp. Games S.L.",
					Address1 = string.Empty,
					Address2 = string.Empty,
					City = "Barcelona",
					StateProvinceId = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "Barcelona")?.Id,
					CountryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "ESP")?.Id,
					ZipPostalCode = "08080",
					CreatedOnUtc = DateTime.UtcNow
				});

			adminUser.BillingAddressId = defaultAdminUserAddress.Id;
			adminUser.ShippingAddressId = defaultAdminUserAddress.Id;
			adminUser.FirstName = defaultAdminUserAddress.FirstName;
			adminUser.LastName = defaultAdminUserAddress.LastName;

			await InsertInstallationDataAsync(adminUser);

			await InsertInstallationDataAsync(new UserAddressMapping { UserId = adminUser.Id, AddressId = defaultAdminUserAddress.Id });

			await InsertInstallationDataAsync(
				new UserUserRoleMapping { UserId = adminUser.Id, UserRoleId = urAdministrators.Id },
				new UserUserRoleMapping { UserId = adminUser.Id, UserRoleId = urModerators.Id },
				new UserUserRoleMapping { UserId = adminUser.Id, UserRoleId = urRegistered.Id });

			//set hashed admin password
			var userRegistrationService = EngineContext.Current.Resolve<IUserRegistrationService>();
			await userRegistrationService.ChangePasswordAsync(new ChangePasswordRequest(defaultUserEmail, false,
				 PasswordFormat.Hashed, defaultUserPassword, null, UserServicesDefaults.DefaultHashedPasswordFormat));

			//search engine (crawler) built-in user
			var searchEngineUser = new User
			{
				Email = "builtin@search_engine.com",
				UserGuid = Guid.NewGuid(),
				AdminComment = "Built-in system guest record used for requests from search engines.",
				Active = true,
				IsSystemAccount = true,
				SystemName = UserDefaults.SearchEngineUserName,
				CreatedOnUtc = DateTime.UtcNow,
				LastActivityDateUtc = DateTime.UtcNow,
				RegisteredInNodeId = nodeId
			};

			await InsertInstallationDataAsync(searchEngineUser);

			await InsertInstallationDataAsync(new UserUserRoleMapping { UserRoleId = urGuests.Id, UserId = searchEngineUser.Id });

			//built-in user for background tasks
			var backgroundTaskUser = new User
			{
				Email = "builtin@background-task.com",
				UserGuid = Guid.NewGuid(),
				AdminComment = "Built-in system record used for background tasks.",
				Active = true,
				IsSystemAccount = true,
				SystemName = UserDefaults.BackgroundTaskUserName,
				CreatedOnUtc = DateTime.UtcNow,
				LastActivityDateUtc = DateTime.UtcNow,
				RegisteredInNodeId = nodeId
			};

			await InsertInstallationDataAsync(backgroundTaskUser);

			await InsertInstallationDataAsync(new UserUserRoleMapping { UserId = backgroundTaskUser.Id, UserRoleId = urGuests.Id });
		}

		///// <returns>A task that represents the asynchronous operation</returns>
		//protected virtual async Task InstallGamesAsync()
		//{
		//	var pictureService = EngineContext.Current.Resolve<IPictureService>();
		//	var gameImagesPath = GetContentPath();

		//	var genres = new List<Genre>
		//	{
		//		new() {
		//			Name = "Action",
		//			DisplayOrder = 1,
		//			Description = "Action games emphasize physical challenges that require eye-hand coordination and motor skill to overcome. They center around the player, who is in control of most of the action.",
		//			IconFileName = "action.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-action.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-action"))).Id,
		//			Published = true
		//		},
		//		new() {
		//			Name = "Adventure",
		//			DisplayOrder = 2,
		//			Description = "Adventure games emphasize experiencing a story through dialogue and puzzle solving. Gameplay mechanics emphasize decision over action.",
		//			IconFileName = "adventure.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-adventure.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-adventure"))).Id,
		//			Published = true
		//		},
		//		new() {
		//			Name = "Role-Playing",
		//			DisplayOrder = 3,
		//			Description = "Role-playing games (RPGs) allow players to assume the role of a character(s) and through decision making determine the outcome of the game. Action RPGs (ARPGs) emphasize combat and leveling up, while Japanese RPGs (JRPGs) emphasize storytelling and turn-based combat.",
		//			IconFileName = "rpg.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-rpg.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-rpg"))).Id,
		//			Published = true
		//		},
		//		new() {
		//			Name = "Simulation",
		//			DisplayOrder = 4,
		//			Description = "Simulation games attempt to accurately replicate real-world activities and environments.",
		//			IconFileName = "simulation.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-simulation.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-simulation"))).Id,
		//			Published = true
		//		},
		//		new() {
		//			Name = "Strategy",
		//			DisplayOrder = 5,
		//			Description = "Strategy games emphasize planning and skillful thinking in order to achieve victory. They can be either real-time in which the play clock ticks continuously or turn-based where each player has a dedicated turn to finish their action. It is not uncommon to include exploration and resource management.",
		//			IconFileName = "strategy.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-strategy.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-strategy"))).Id,
		//			Published = true
		//		},
		//		new() {
		//			Name = "Sports",
		//			DisplayOrder = 6,
		//			Description = "Sports games simulate a variety of sports including football, baseball, basketball, track and field, and combat sports. They tend to be very competitive. Some are realistically modeled after their real world counterparts while others are satirical with a focus on fun and wacky gameplay.",
		//			IconFileName = "sports.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-sports.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-sports"))).Id,
		//			Published = true
		//		},
		//		new() {
		//			Name = "MMO",
		//			DisplayOrder = 7,
		//			Description = "Massively Multiplayer Online (MMO) games are played online by many players at once. They focus on the co-operative aspects of the game and often require players to form alliances in order to achieve their goals.",
		//			IconFileName = "mmo.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-mmo.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-mmo"))).Id,
		//			Published = true
		//		},
		//		new() {
		//			Name = "Puzzle",
		//			DisplayOrder = 8,
		//			Description = "Puzzle games require players to solve a variety of puzzles ranging from logic to patterns to words and numbers. They are sometimes timed and sometimes do not have a time limit. Some puzzle games have a level progression built in to them and others are in a constant state of puzzle.",
		//			IconFileName = "puzzle.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-puzzle.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-puzzle"))).Id,
		//			Published = true
		//		},
		//		new() {
		//			Name = "Idle",
		//			DisplayOrder = 9,
		//			Description = "Idle games are a type of incremental games where the player will perform a simple action repeatedly to gain a currency. This currency can then be used to purchase upgrades to make the game more efficient. Idle games are sometimes called clicker games due to the fact that they require very little input from the player.",
		//			IconFileName = "idle.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-idle.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-idle"))).Id,
		//			Published = true
		//		},
		//		new() {
		//			Name = "Racing",
		//			DisplayOrder = 10,
		//			Description = "Racing games simulate racing competitions, and often track racing with various vehicles. The genre also includes stunt driving games.",
		//			IconFileName = "racing.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-racing.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-racing"))).Id,
		//			Published = true
		//		},
		//		new() {
		//			Name = "Educational",
		//			DisplayOrder = 11,
		//			Description = "Educational games are games explicitly designed with educational purposes, or which have incidental or secondary educational value. All types of games may be used in an educational environment.",
		//			IconFileName = "educational.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-educational.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-educational"))).Id,
		//			Published = true
		//		},
		//		new() {
		//			Name = "Board",
		//			DisplayOrder = 12,
		//			Description = "Board games are tabletop games that typically use pieces moved or placed on a pre-marked board (playing surface) and often include elements of table, card, role-playing, and miniatures games as well.",
		//			IconFileName = "board.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-board.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-board"))).Id,
		//			Published = true
		//		},
		//		new() {
		//			Name = "Cards",
		//			DisplayOrder = 13,
		//			Description = "Card games are games played with a deck of playing cards. They are typically trick-taking games, where players attempt to win rounds by taking the most tricks.",
		//			IconFileName = "cards.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-cards.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-cards"))).Id,
		//			Published = true
		//		},
		//		new() {
		//			Name = "Multiplayer",
		//			DisplayOrder = 14,
		//			Description = "Multiplayer games are games that are designed to be played by multiple people at once. They can be played by people in the same room or online from anywhere in the world.",
		//			IconFileName = "multiplayer.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-multiplayer.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-multiplayer"))).Id,
		//			Published = true
		//		},
		//		new() {
		//			Name = "Arcade",
		//			DisplayOrder = 15,
		//			Description = "Arcade games are coin-operated video games, usually found in public places such as video arcades, restaurants, bars, and theaters. Many arcade games are redemption games, where the player wins tickets by playing the game and can trade them in for prizes.",
		//			IconFileName = "arcade.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-arcade.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-arcade"))).Id,
		//			Published = true
		//		},
		//		new() {
		//			Name = "Arena",
		//			DisplayOrder = 16,
		//			Description = "Arena games are games that are played in a competitive setting. They are typically played in a multiplayer setting where players can compete against each other.",
		//			IconFileName = "arena.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "genre-arena.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("genre-arena"))).Id,
		//			Published = true
		//		}
		//	};

		//	await InsertInstallationDataAsync(genres);

		//	var platforms = new List<Platform>
		//	{
		//		new() {
		//			Name = "Windows",
		//			DisplayOrder = 1,
		//			Description = "Windows x86/x64 operative system.",
		//			IconFileName = "windows.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "platform-win.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("platform-win"))).Id,
		//		},
		//		new() {
		//			Name = "Linux",
		//			DisplayOrder = 2,
		//			Description = "Linux operative system.",
		//			IconFileName = "linux.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "platform-linux.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("platform-linux"))).Id,
		//		},
		//		new() {
		//			Name = "MacOS",
		//			DisplayOrder = 3,
		//			Description = "MacOS operative system.",
		//			IconFileName = "macos.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "platform-macos.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("platform-macos"))).Id,
		//		},
		//		new() {
		//			Name = "Android",
		//			DisplayOrder = 4,
		//			Description = "Android mobile platform.",
		//			IconFileName = "android.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "platform-android.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("platform-android"))).Id,
		//		},
		//		new() {
		//			Name = "iOS",
		//			DisplayOrder = 5,
		//			Description = "iPad/iPhone mobile platform.",
		//			IconFileName = "ios.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "platform-ios.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("platform-ios"))).Id,
		//		},
		//		new() {
		//			Name = "Web",
		//			DisplayOrder = 6,
		//			Description = "Web browser platform.",
		//			IconFileName = "web.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "platform-web.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("platform-web"))).Id,
		//		},
		//		new() {
		//			Name = "dApp",
		//			DisplayOrder = 7,
		//			Description = "Decentralized application.",
		//			IconFileName = "ethereum.png",
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "platform-dapp.jpg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("platform-dapp"))).Id,
		//		}
		//	};

		//	await InsertInstallationDataAsync(platforms);

		//}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallTopicsAsync()
		{
			var defaultTopicTemplate =
				_topicTemplateRepository.Table.FirstOrDefault(tt => tt.Name == "Default template") ?? throw new NodeException("Topic template cannot be loaded");
			var topics = new List<Topic>
			{
				new() {
					SystemName = "AboutUs",
					IncludeInSitemap = false,
					IsPasswordProtected = false,
					IncludeInFooterColumn1 = true,
					DisplayOrder = 20,
					Published = true,
					Title = "About Us",
					Body = "<p>At Dragon Corp, our mission is to drive innovation and growth in the video game industry by creating synergies between investors, gamers, and businesses. We embarked on our exciting journey in the year 2021 when a group of passionate gamers and visionary investors came together with a common goal: to revolutionize how the world of gaming connects and thrives.</p>" +
						"<h5>Our Mission</h5>" +
						"<p>Our mission at Dragon Corp. is clear: to strengthen the gaming community by facilitating strategic and financial collaborations among all involved parties. We believe that every player and company in this vibrant ecosystem has an important role to play, and by working together, we can achieve astounding results.</p>" +
						"<h5>What We Do</h5>" +
						"<p>At Dragon Corp, we create opportunities for investors and companies to discover the potential of the gaming industry and for talented gamers to find the support they need to take their skills to the next level. We facilitate investment in promising projects, foster strategic partnerships, and provide a platform where talents and brilliant ideas can flourish.</p>" +
						"<h5>Our Commitment</h5>" +
						"<p>We are committed to excellence, transparency, and integrity in everything we do. We value the diversity of talents and creativity in the gaming community and work tirelessly to foster an inclusive environment where everyone can thrive.</p>" + "<p>At Dragon Corp, we are excited about the future of gaming and are determined to play a pivotal role in its evolution. Join us on this exciting journey as we create a bright future for the gaming industry, driven by collaboration and passion.</p>" +
						"<p><strong>WE GO WHERE OTHERS DON'T WANT TO GO AND DO WHAT OTHERS CAN'T. WE ARE DRAGON CORP</strong></p>",
					TopicTemplateId = defaultTopicTemplate.Id
				},
				new() {
					SystemName = "TermsOfUse",
					IncludeInSitemap = false,
					IsPasswordProtected = false,
					IncludeInFooterColumn1 = true,
					DisplayOrder = 15,
					Published = true,
					Title = "Terms of Use",
					Body = "<p>We are Dragon Corp. Games S.L., owners and operators of the metalink.dragoncorp.org web2/web3 node. By accessing and using this website, you agree to comply with the following terms and conditions. If you do not agree to these terms, we recommend that you do not use the site.</p>" +
						   "<h5>Acceptable Use:</h5>" +
						   "<p>You are authorized to access and use the Website for lawful purposes and in accordance with these Terms of Use.You may not use the Site in a manner that causes damage or interferes with its normal operation.This includes not attempting to access restricted areas without authorization and not engaging in activities that may harm the security of the site.</p>" +
						   "<h5>Intellectual property:</h5>" +
						   "<p>All content on the website, including texts, images, graphics, logos and software, is protected by intellectual property rights and is the property of Dragon Corp. Games S.L. or third parties, used under license.You are not permitted to reproduce, distribute, modify, or create derivative works based on this content without authorization.</p>" +
						   "<h5>Links to Third Parties:</h5>" +
						   "<p>The Website may contain links to third-party websites.We are not responsible for the content or privacy practices of these sites.Links do not imply that we endorse or have affiliation with these sites.</p>" +
						   "<h5>Limitation of Liability:</h5>" +
						   "<p>The Website is provided \"as is\" and we do not warrant that it will be error-free or uninterrupted. We are not liable for any direct, indirect, incidental, special or consequential damages that may result from the use or inability to use the website.</p>" +
						   "<h5>Changes to Terms:</h5>" +
						   "<p>We reserve the right to modify these Terms of Use at any time. We recommend that you review these terms periodically to stay informed of any changes.</p>" +
						   "<h5>Applicable Law:</h5>" +
						   "<p>These Terms of Use are governed by the laws of Spain.Any dispute arising in connection with the Website shall be subject to the exclusive jurisdiction of the Spanish courts.</p>",
					TopicTemplateId = defaultTopicTemplate.Id
				},
				new() {
					SystemName = "ContactUs",
					IncludeInSitemap = false,
					IsPasswordProtected = false,
					DisplayOrder = 1,
					Published = true,
					Title = string.Empty,
					Body = "<p>Have a question, suggestion, or just want to say hello? We'd love to hear from you! Feel free to reach out through our contact form, and we'll get back to you as soon as possible. Your feedback is important to us, and we appreciate your interest in connecting with our team.</p>",
					TopicTemplateId = defaultTopicTemplate.Id
				},
				new() {
					SystemName = "HomepageText",
					IncludeInSitemap = false,
					IsPasswordProtected = false,
					DisplayOrder = 1,
					Published = true,
					Title = "UNAUTHORIZED ACCESS",
					Body = "<p>Metalink is the Dragon Corp's connection and navigation system for metaverse interactivity. Only authorized members have access to explore Metalink's data, activity and contents.</p>" +
						   "<p>Please <a href=\"/login?returnUrl=%2F\">login</a> if you have an existing account or <a href=\"register?returnUrl=%2F\">register</a> using your preferred method.</p>" +
						   "<div class=\"buttons\"><a href=\"/register?returnUrl=%2F\" class=\"button button-1\">Register</a><a href=\"/login?returnUrl=%2F\" class=\"button button-1 dc-notch\">Login</a></div>",
					TopicTemplateId = defaultTopicTemplate.Id
				},
				new() {
					SystemName = "LoginRegistrationInfo",
					IncludeInSitemap = false,
					IsPasswordProtected = false,
					DisplayOrder = 1,
					Published = true,
					Title = "About user login/registration",
					Body = "<p>Welcome to Metalink, your portal to the endless metaverse! Proudly brought to you by Dragon Corp. Whether you're a seasoned gamer or embarking on your gaming journey, Metalink is your key to a world of immersive experiences. Unlock the power of play by registering or logging in below. Join the Dragon Corp. Games community and dive into a realm where gaming dreams come to life. Let the adventure begin!</p>",
					TopicTemplateId = defaultTopicTemplate.Id
				},
				new() {
					SystemName = "PrivacyInfo",
					IncludeInSitemap = false,
					IsPasswordProtected = false,
					IncludeInFooterColumn1 = true,
					DisplayOrder = 10,
					Published = true,
					Title = "Privacy notice",
					Body = "<p>Last Updated: 09/29/2023</p>" +
						   "<p>Thank you for choosing Dragon Corp.Games' Metalink. We are committed to protecting your privacy and providing a secure online experience. This Privacy Notice explains how we collect, use, disclose, and safeguard your personal information. By accessing or using our website, you consent to the terms outlined in this Privacy Notice.</p>" +
						   "<h5>1. Information We Collect</h5>" +
						   "<ul><li><p><strong>Personal Information:</strong> When you register, create an account, or use our services, we may collect personal information such as your name, email address, contact details, and other relevant details.</p></li>" +
						   "<li><p><strong>Usage Information:</strong> We automatically collect information about how you interact with our website, including your IP address, browser type, operating system, and browsing behavior.</p></li>" +
						   "<li><p><strong>Cookies and Similar Technologies:</strong> We use cookies and similar technologies to enhance your experience, customize content, and analyze usage patterns.</p></li></ul>" +
						   "<h5>2. How We Use Your Information</h5>" +
						   "<p>We use the information we collect for various purposes, including:</p>" +
						   "<ul><li>Providing and improving our services.</li>" +
						   "<li>Personalizing your experience.</li>" +
						   "<li>Communicating with you.</li>" +
						   "<li>Analyzing usage patterns.</li>" +
						   "<li>Enhancing security.</li></ul>" +
						   "<h5>3. Information Sharing and Disclosure</h5>" +
						   "<p>We do not sell, trade, or rent your personal information to third parties. However, we may share information with:</p>" +
						   "<ul><li>Service providers: Third - party service providers who assist us in delivering our services.</li>" +
						   "<li>Legal purposes: To comply with legal requirements, respond to legal requests, or protect our rights.</li></ul>" +
						   "<h5>4. Your Choices and Rights</h5>" +
						   "<p>You have the right to:</p>" +
						   "<ul><li>Access, correct, or delete your personal information.</li>" +
						   "<li>Opt -out of certain data processing activities.</li>" +
						   "<li>Withdraw consent.</li></ul>" +
						   "<h5>5. Security</h5>" +
						   "<p>We take reasonable measures to protect your personal information from unauthorized access, disclosure, alteration, and destruction.</p>" +
						   "<h5>6. Children's Privacy</h5>" +
						   "<p>Our services are not directed to individuals under the age of 13. We do not knowingly collect personal information from children.</p>" +
						   "<h5>7. Changes to this Privacy Notice</h5>" +
						   "<p>We may update this Privacy Notice periodically. We will notify you of any changes by posting the updated notice on our website.</p>" +
						   "<h5>8. Contact Us</h5>" +
						   "<p>If you have any questions or concerns about our Privacy Notice, please feel free to < a title = \"contact us\" href = \"contactus\">contact us </a >.</p>",
					TopicTemplateId = defaultTopicTemplate.Id
				},
				new() {
					SystemName = "PageNotFound",
					IncludeInSitemap = false,
					IsPasswordProtected = false,
					DisplayOrder = 1,
					Published = true,
					Title = string.Empty,
					Body = "<p><strong>Oh no! The page you were looking for seems to have taken a detour.</strong></p>" +
					"<ul><li>If you typed the URL directly, double-check the spelling —it's a tricky business.</li>" +
					"<li>Unfortunately, it looks like the page has bid farewell. Our sincerest apologies for any inconvenience caused.</li></ul>" +
					"<p>While you're here, how about a shortcut back to the <a href=\"/\">homepage</a>? It's where you can begin to explore metalink's contents.</p>" +
					"<div class=\"buttons\">" +
					"<a class=\"button button-1 dc-notch\" href=\"/\">Back</a>" +
					"</div>",
					TopicTemplateId = defaultTopicTemplate.Id
				},
				new() {
					SystemName = "ApplyPartner",
					IncludeInSitemap = false,
					IsPasswordProtected = false,
					DisplayOrder = 1,
					Published = true,
					Title = string.Empty,
					Body = "<p><strong>Ready to level up your gaming journey?</strong> Apply for a partner account with Dragon Corp. Games on Metalink!</p>" +
						   "<p>Unleash the power of collaboration and be a part of the Dragon Corp. Games community. As a partner, you'll dive into exclusive opportunities, unlock new realms, and join us in shaping the future of gaming.</p>" +
						   "<p><strong>Calling all gaming guilds, streamers, and content creators!</strong> Elevate your gaming experience by partnering with Dragon Corp. Games on Metalink. Join our dynamic community and unlock exclusive perks designed for guilds, streamers, and content creators. Dive into a world of collaborative gaming, exclusive content, and exciting opportunities.</p>" +
						   "<p>Ready to amplify your presence in the gaming universe? Game developers and publishers, seize the opportunity to collaborate with Dragon Corp. Games on Metalink! Elevate your games to new heights by becoming a valued partner. Access a platform tailored for developers, connect with a vibrant gaming community, and showcase your creations to a broader audience.</p>" +
						   "<p>Ready to bring your games to the next level? Apply for a partner account below and embark on a journey of innovation and success in the gaming industry!</p>" +
						   "<p><strong>Complete the form and let the gaming synergy begin!</strong></p>",
					//Body = "<p>Power up your gaming ventures with Dragon Corp. Games! Apply for a partner account today and level up your opportunities. As a Dragon Corp. Games partner, you'll gain access to exclusive perks, collaborative projects, and a gaming community like no other. Join us in shaping the future of gaming. Apply now and let the adventures begin with Dragon Corp. Games!</p>",
					TopicTemplateId = defaultTopicTemplate.Id
				},
				new() {
					SystemName = "PartnerTermsOfService",
					IncludeInSitemap = false,
					IsPasswordProtected = false,
					IncludeInFooterColumn1 = false,
					DisplayOrder = 1,
					Published = true,
					Title = "Terms of services for partners",
					Body = "<h5>1. Acceptance of Terms</h5>" +
						   "<p>By applying for and accepting a partner account with Dragon Corp. Games on Metalink, you agree to be bound by these Terms of Service. If you do not agree with any part of these terms, you may not proceed with the partner application.</p>" +
						   "<h5>2. Partner Eligibility</h5>" +
						   "<p>To be eligible for a partner account, you must comply with all applicable laws and regulations. Dragon Corp. Games reserves the right to refuse or terminate partner accounts at its discretion.</p>" +
						   "<h5>3. Partner Responsibilities</h5>" +
						   "<p>As a partner, you agree to:</p>" +
						   "<ul><li>Comply with all Metalink policies, guidelines, and community standards.</li>" +
						   "<li>Provide accurate and up-to-date information during the application process.</li>" +
						   "<li>Ensure the content you publish, including game titles and quests, adheres to our content guidelines and does not violate any laws or third-party rights.</li></ul>" +
						   "<h5>4. Intellectual Property</h5>" +
						   "<p>Partners retain ownership of their intellectual property. By publishing content on Metalink, partners grant Dragon Corp. Games a non-exclusive, worldwide, royalty-free license to use, display, and promote the content on Metalink.</p>" +
						   "<h5>5. Game Titles and Quests</h5>" +
						   "<p>Partners may publish their own game titles and quests on Metalink. Dragon Corp. Games reserves the right to review and approve submitted content. Content that violates our guidelines may be rejected or removed at our discretion.</p>" +
						   "<h5>6. Revenue Sharing</h5>" +
						   "<p>Partners may be eligible for revenue sharing based on agreed-upon terms outlined in separate agreements. Revenue sharing is subject to change, and partners will be notified of any modifications.</p>" +
						   "<h5>7. Termination of Partnership</h5>" +
						   "<p>Dragon Corp. Games reserves the right to terminate partner accounts for violations of these terms, breach of agreements, or any other reasons deemed necessary. Partners may also terminate their partnership by providing written notice.</p>" +
						   "<h5>8. Dispute Resolution</h5>" +
						   "<p>Any disputes arising from or related to these Terms of Service will be resolved through negotiation, mediation, or arbitration in accordance with the laws of <strong>Spain</strong> and the <strong>European Union (EU)</strong>..</p>" +
						   "<h5>9. Changes to Terms</h5>" +
						   "<p>Dragon Corp. Games may update these Terms of Service. Partners will be notified of changes, and continued use of the partner account constitutes acceptance of the modified terms.</p>" +
						   "<h5>10. Governing Law</h5>" +
						   "<p>These Terms of Service are governed by the laws of <strong>Spain</strong> and the <strong>European Union (EU)</strong>.. Any legal actions or proceedings related to these terms shall be brought exclusively in the courts of <strong>Spain</strong> and the <strong>European Union (EU)</strong>..</p>",
					TopicTemplateId = defaultTopicTemplate.Id
				}
			};

			await InsertInstallationDataAsync(topics);

			//search engine names
			foreach (var topic in topics)
			{
				await InsertInstallationDataAsync(new UrlRecord
				{
					EntityId = topic.Id,
					EntityName = nameof(Topic),
					LanguageId = 0,
					IsActive = true,
					Slug = await ValidateSeNameAsync(topic, !string.IsNullOrEmpty(topic.Title) ? topic.Title : topic.SystemName)
				});
			}
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallActivityLogTypesAsync()
		{
			var activityLogTypes = new List<ActivityLogType>
			{
				//admin area activities
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewAddressAttribute,
					Enabled = true,
					Name = "Add a new address attribute"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewAddressAttributeValue,
					Enabled = true,
					Name = "Add a new address attribute value"
				},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.AddNewAffiliate,
					Enabled = true,
					Name = "Add a new affiliate"
				},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.AddNewBlogPost,
					Enabled = true,
					Name = "Add a new blog post"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewCampaign,
					Enabled = true,
					Name = "Add a new campaign"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewCountry,
					Enabled = true,
					Name = "Add a new country"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewCurrency,
					Enabled = true,
					Name = "Add a new currency"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewUser,
					Enabled = true,
					Name = "Add a new user"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewUserAttribute,
					Enabled = true,
					Name = "Add a new user attribute"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewUserAttributeValue,
					Enabled = true,
					Name = "Add a new user attribute value"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewUserRole,
					Enabled = true,
					Name = "Add a new user role"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewEmailAccount,
					Enabled = true,
					Name = "Add a new email account"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewLanguage,
					Enabled = true,
					Name = "Add a new language"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewMeasureDimension,
					Enabled = true,
					Name = "Add a new measure dimension"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewMeasureWeight,
					Enabled = true,
					Name = "Add a new measure weight"
				},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.AddNewNews,
					Enabled = true,
					Name = "Add a new news"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewSetting,
					Enabled = true,
					Name = "Add a new setting"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewStateProvince,
					Enabled = true,
					Name = "Add a new state or province"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.AddNewNode,
					Enabled = true,
					Name = "Add a new node"
				},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.AddNewTopic,
					Enabled = true,
					Name = "Add a new topic"
				},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewPartner,
				//	Enabled = true,
				//	Name = "Add a new partner"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewPartnerAttribute,
				//	Enabled = true,
				//	Name = "Add a new partner attribute"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewPartnerAttributeValue,
				//	Enabled = true,
				//	Name = "Add a new partner attribute value"
				//},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.AddNewWidget,
					Enabled = true,
					Name = "Add a new widget"
				},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewToken,
				//	Enabled = true,
				//	Name = "Add a new token"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewPlayer,
				//	Enabled = true,
				//	Name = "Add a new player"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewPlayerAttribute,
				//	Enabled = true,
				//	Name = "Add a new player attribute"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewPlayerAttributeValue,
				//	Enabled = true,
				//	Name = "Add a new player attribute value"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewBlockchain,
				//	Enabled = true,
				//	Name = "Add a new blockchain"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewGenre,
				//	Enabled = true,
				//	Name = "Add a new game genre"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewPlatform,
				//	Enabled = true,
				//	Name = "Add a new game platform"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewPublisher,
				//	Enabled = true,
				//	Name = "Add a new game publisher"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewTitle,
				//	Enabled = true,
				//	Name = "Add a new game title"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewRank,
				//	Enabled = true,
				//	Name = "Add a new rank"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewInventoryItem,
				//	Enabled = true,
				//	Name = "Add a new inventory item"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewNonFungibleToken,
				//	Enabled = true,
				//	Name = "Add a new non fungible token"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewQuest,
				//	Enabled = true,
				//	Name = "Add a new quest"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewQuestGoal,
				//	Enabled = true,
				//	Name = "Add a new quest goal"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewQuestReward,
				//	Enabled = true,
				//	Name = "Add a new quest reward"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewSmartContract,
				//	Enabled = true,
				//	Name = "Add a new smart contract"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewTournament,
				//	Enabled = true,
				//	Name = "Add a new tournament"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewGoalAttribute,
				//	Enabled = true,
				//	Name = "Add a new goal attribute"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.AddNewGoalAttributeValue,
				//	Enabled = true,
				//	Name = "Add a new goal attribute value"
				//},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteActivityLog,
					Enabled = true,
					Name = "Delete activity log"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteAddressAttribute,
					Enabled = true,
					Name = "Delete an address attribute"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteAddressAttributeValue,
					Enabled = true,
					Name = "Delete an address attribute value"
				},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeleteAffiliate,
				//	Enabled = true,
				//	Name = "Delete an affiliate"
				//},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.DeleteBlogPost,
					Enabled = true,
					Name = "Delete a blog post"
				},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.DeleteBlogPostComment,
					Enabled = true,
					Name = "Delete a blog post comment"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteCampaign,
					Enabled = true,
					Name = "Delete a campaign"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteCountry,
					Enabled = true,
					Name = "Delete a country"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteCurrency,
					Enabled = true,
					Name = "Delete a currency"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteUser,
					Enabled = true,
					Name = "Delete a user"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteUserAttribute,
					Enabled = true,
					Name = "Delete a user attribute"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteUserAttributeValue,
					Enabled = true,
					Name = "Delete a user attribute value"
				},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeleteGoalAttribute,
				//	Enabled = true,
				//	Name = "Delete a goal attribute"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeleteGoalAttributeValue,
				//	Enabled = true,
				//	Name = "Delete a goal attribute value"
				//},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteUserRole,
					Enabled = true,
					Name = "Delete a user role"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteEmailAccount,
					Enabled = true,
					Name = "Delete an email account"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteLanguage,
					Enabled = true,
					Name = "Delete a language"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteMeasureDimension,
					Enabled = true,
					Name = "Delete a measure dimension"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteMeasureWeight,
					Enabled = true,
					Name = "Delete a measure weight"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteMessageTemplate,
					Enabled = true,
					Name = "Delete a message template"
				},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.DeleteNews,
					Enabled = true,
					Name = "Delete a news"
				},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.DeleteNewsComment,
					Enabled = true,
					Name = "Delete a news comment"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteModule,
					Enabled = true,
					Name = "Delete a module"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteSetting,
					Enabled = true,
					Name = "Delete a setting"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteStateProvince,
					Enabled = true,
					Name = "Delete a state or province"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteNode,
					Enabled = true,
					Name = "Delete a node"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.DeleteSystemLog,
					Enabled = true,
					Name = "Delete system log"
				},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.DeleteTopic,
					Enabled = true,
					Name = "Delete a topic"
				},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeletePartner,
				//	Enabled = true,
				//	Name = "Delete a partner"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeletePartnerAttribute,
				//	Enabled = true,
				//	Name = "Delete a partner attribute"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeletePartnerAttributeValue,
				//	Enabled = true,
				//	Name = "Delete a partner attribute value"
				//},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.DeleteWidget,
					Enabled = true,
					Name = "Delete a widget"
				},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeleteToken,
				//	Enabled = true,
				//	Name = "Delete a token"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeletePlayer,
				//	Enabled = true,
				//	Name = "Delete a player"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeletePlayerAttribute,
				//	Enabled = true,
				//	Name = "Delete a player attribute"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeletePlayerAttributeValue,
				//	Enabled = true,
				//	Name = "Delete a player attribute value"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeleteBlockchain,
				//	Enabled = true,
				//	Name = "Delete a blockchain"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeleteGenre,
				//	Enabled = true,
				//	Name = "Delete a game genre"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeletePlatform,
				//	Enabled = true,
				//	Name = "Delete a game platform"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeletePublisher,
				//	Enabled = true,
				//	Name = "Delete a game publisher"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeleteTitle,
				//	Enabled = true,
				//	Name = "Delete a game title"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeleteRank,
				//	Enabled = true,
				//	Name = "Delete a rank"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeleteNonFungibleToken,
				//	Enabled = true,
				//	Name = "Delete a non fungible token"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeleteQuest,
				//	Enabled = true,
				//	Name = "Delete a quest"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeleteQuestGoal,
				//	Enabled = true,
				//	Name = "Delete a quest goal"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeleteQuestReward,
				//	Enabled = true,
				//	Name = "Delete a quest reward"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeleteSmartContract,
				//	Enabled = true,
				//	Name = "Delete a smart contract"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeleteTournament,
				//	Enabled = true,
				//	Name = "Delete a tournament"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.DeleteInventoryItem,
				//	Enabled = true,
				//	Name = "Delete a inventory item"
				//},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditActivityLogTypes,
					Enabled = true,
					Name = "Edit activity log types"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditAddressAttribute,
					Enabled = true,
					Name = "Edit an address attribute"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditAddressAttributeValue,
					Enabled = true,
					Name = "Edit an address attribute value"
				},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditAffiliate,
				//	Enabled = true,
				//	Name = "Edit an affiliate"
				//},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.EditBlogPost,
					Enabled = true,
					Name = "Edit a blog post"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditCampaign,
					Enabled = true,
					Name = "Edit a campaign"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditCountry,
					Enabled = true,
					Name = "Edit a country"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditCurrency,
					Enabled = true,
					Name = "Edit a currency"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditUser,
					Enabled = true,
					Name = "Edit a user"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditUserAttribute,
					Enabled = true,
					Name = "Edit a user attribute"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditUserAttributeValue,
					Enabled = true,
					Name = "Edit a user attribute value"
				},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditGoalAttribute,
				//	Enabled = true,
				//	Name = "Edit a goal attribute"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditGoalAttributeValue,
				//	Enabled = true,
				//	Name = "Edit a goal attribute value"
				//},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditUserRole,
					Enabled = true,
					Name = "Edit a user role"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditEmailAccount,
					Enabled = true,
					Name = "Edit an email account"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditLanguage,
					Enabled = true,
					Name = "Edit a language"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditMeasureDimension,
					Enabled = true,
					Name = "Edit a measure dimension"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditMeasureWeight,
					Enabled = true,
					Name = "Edit a measure weight"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditMessageTemplate,
					Enabled = true,
					Name = "Edit a message template"
				},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.EditNews,
					Enabled = true,
					Name = "Edit a news"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditModule,
					Enabled = true,
					Name = "Edit a module"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditSettings,
					Enabled = true,
					Name = "Edit setting(s)"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditStateProvince,
					Enabled = true,
					Name = "Edit a state or province"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditNode,
					Enabled = true,
					Name = "Edit a node"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.EditTask,
					Enabled = true,
					Name = "Edit a task"
				},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditPartner,
				//	Enabled = true,
				//	Name = "Edit a partner"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditPartnerAttribute,
				//	Enabled = true,
				//	Name = "Edit a partner attribute"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditPartnerAttributeValue,
				//	Enabled = true,
				//	Name = "Edit a partner attribute value"
				//},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.EditTopic,
					Enabled = true,
					Name = "Edit a topic"
				},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.EditWidget,
					Enabled = true,
					Name = "Edit a widget"
				},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditToken,
				//	Enabled = true,
				//	Name = "Edit a token"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditPlayer,
				//	Enabled = true,
				//	Name = "Edit a player"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditPlayerAttribute,
				//	Enabled = true,
				//	Name = "Edit a player attribute"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditPlayerAttributeValue,
				//	Enabled = true,
				//	Name = "Edit a player attribute value"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditBlockchain,
				//	Enabled = true,
				//	Name = "Edit a blockchain"
				//},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.EditBlogComment,
					Enabled = true,
					Name = "Edit a blog comment"
				},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditGenre,
				//	Enabled = true,
				//	Name = "Edit a game genre"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditPlatform,
				//	Enabled = true,
				//	Name = "Edit a game platform"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditPublisher,
				//	Enabled = true,
				//	Name = "Edit a game publisher"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditTitle,
				//	Enabled = true,
				//	Name = "Edit a game title"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditGoalTypes,
				//	Enabled = true,
				//	Name = "Edit goal types"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditRank,
				//	Enabled = true,
				//	Name = "Edit a rank"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditInventoryItem,
				//	Enabled = true,
				//	Name = "Edit a inventory item"
				//},
				new() {
					SystemKeyword = PortalKeywords.AdminArea.EditNewsComment,
					Enabled = true,
					Name = "Edit a news item comment"
				},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditNonFungibleToken,
				//	Enabled = true,
				//	Name = "Edit a non fungible token"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditQuest,
				//	Enabled = true,
				//	Name = "Edit a quest"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditQuestGoal,
				//	Enabled = true,
				//	Name = "Edit a quest goal"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditQuestReward,
				//	Enabled = true,
				//	Name = "Edit a quest reward"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditSmartContract,
				//	Enabled = true,
				//	Name = "Edit a smart contract"
				//},
				//new() {
				//	SystemKeyword = SystemKeywords.AdminArea.EditTournament,
				//	Enabled = true,
				//	Name = "Edit a tournament"
				//},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.ImpersonationStarted,
					Enabled = true,
					Name = "User impersonation session. Started"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.ImpersonationFinished,
					Enabled = true,
					Name = "User impersonation session. Finished"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.ImportNewsLetterSubscriptions,
					Enabled = true,
					Name = "Newsletter subscriptions were imported"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.ImportStates,
					Enabled = true,
					Name = "States were imported"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.ExportUsers,
					Enabled = true,
					Name = "Users were exported"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.ExportStates,
					Enabled = true,
					Name = "States were exported"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.ExportNewsLetterSubscriptions,
					Enabled = true,
					Name = "Newsletter subscriptions were exported"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.InstallNewModule,
					Enabled = true,
					Name = "Install a new module"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.UninstallModule,
					Enabled = true,
					Name = "Uninstall a module"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.UploadNewModule,
					Enabled = true,
					Name = "Upload a module"
				},
				new() {
					SystemKeyword = SystemKeywords.AdminArea.UploadIcons,
					Enabled = true,
					Name = "Upload a favicon and app icons"
				},
				//public node activities
				new() {
					SystemKeyword = SystemKeywords.PublicServer.ContactUs,
					Enabled = false,
					Name = "Public node. Use contact us form"
				},
				new() {
					SystemKeyword = SystemKeywords.PublicServer.Login,
					Enabled = false,
					Name = "Public node. Login"
				},
				new() {
					SystemKeyword = SystemKeywords.PublicServer.Logout,
					Enabled = false,
					Name = "Public node. Logout"
				},
				new() {
					SystemKeyword = PortalKeywords.PublicServer.AddNewsComment,
					Enabled = false,
					Name = "Public node. Add news comment"
				},
				new() {
					SystemKeyword = PortalKeywords.PublicServer.AddBlogComment,
					Enabled = false,
					Name = "Public node. Add blog comment"
				},
			};

			await InsertInstallationDataAsync(activityLogTypes);
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallScheduleTasksAsync()
		{
			var lastEnabledUtc = DateTime.UtcNow;
			var tasks = new List<ScheduleTask>
			{
				new() {
					Name = "Send emails",
					//1 minute
					Seconds = 60,
					Type = "ARWNI2S.Node.Services.Messages.QueuedMessagesSendTask, ARWNI2S.Node.Services",
					Enabled = true,
					LastEnabledUtc = lastEnabledUtc,
					StopOnError = false
				},
				new() {
					Name = "Keep alive",
					//5 minutes
					Seconds = 300,
					Type = "ARWNI2S.Node.Services.Common.KeepAliveTask, ARWNI2S.Node.Services",
					Enabled = true,
					LastEnabledUtc = lastEnabledUtc,
					StopOnError = false
				},
				new() {
					Name = "Delete guests",
					//10 minutes
					Seconds = 600,
					Type = "ARWNI2S.Node.Services.Users.DeleteGuestsTask, ARWNI2S.Node.Services",
					Enabled = true,
					LastEnabledUtc = lastEnabledUtc,
					StopOnError = false
				},
				new() {
					Name = "Clear cache",
					//10 minutes
					Seconds = 600,
					Type = "ARWNI2S.Node.Services.Caching.ClearCacheTask, ARWNI2S.Node.Services",
					Enabled = false,
					StopOnError = false
				},
				new() {
					Name = "Clear log",
					//60 minutes
					Seconds = 3600,
					Type = "ARWNI2S.Node.Services.Logging.ClearLogTask, ARWNI2S.Node.Services",
					Enabled = false,
					StopOnError = false
				},
				new() {
					Name = "Update currency exchange rates",
					//60 minutes
					Seconds = 3600,
					Type = "ARWNI2S.Node.Services.Directory.UpdateExchangeRateTask, ARWNI2S.Node.Services",
					Enabled = true,
					LastEnabledUtc = lastEnabledUtc,
					StopOnError = false
				},
				new() {
					Name = "Delete inactive users (GDPR)",
					//24 hours
					Seconds = 86400,
					Type = "ARWNI2S.Node.Services.Gdpr.DeleteInactiveUsersTask, ARWNI2S.Node.Services",
					Enabled = false,
					StopOnError = false
				},
				new() {
					Name = "Update blocks (Web3)",
					//10 minutes
					Seconds = 600,
					Type = "ARWNI2S.Node.Services.Metalink.UpdateBlockchainBlocksTask, ARWNI2S.Node.Services",
					Enabled = false,
					StopOnError = false
				}
			};

			await InsertInstallationDataAsync(tasks);
		}

		#endregion

		//#region Gameplay Methods

		///// <returns>A task that represents the asynchronous operation</returns>
		//protected virtual async Task IntallGoalsAndRewardsAsync()
		//{
		//	var goalTypes = new List<GoalType>() {
		//		new() {
		//			SystemKeyword = SystemKeywords.PublicServer.Login,
		//			Name = "Login on site",
		//			Enabled = true
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.PlayerDailyLogin,
		//			Name = "Daily login as player",
		//			Enabled = true
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.PublicServer.AddNewsComment,
		//			Name = "Comment on a news item",
		//			Enabled = true
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.PublicServer.AddBlogComment,
		//			Name = "Comment on a blog post",
		//			Enabled = true
		//		}
		//	};

		//	await InsertInstallationDataAsync(goalTypes);

		//	var attributes = new List<GoalAttribute>() {
		//		new() {
		//			AttributeControlTypeId = (int)AttributeControlType.TextBox,
		//			DisplayOrder = 1,
		//			GoalTypeId = 1,
		//			Name = "Total logins",
		//			IsRequired = false
		//		},
		//		new() {
		//			AttributeControlTypeId = (int)AttributeControlType.TextBox,
		//			DisplayOrder = 1,
		//			GoalTypeId = 2,
		//			Name = "Total logins",
		//			IsRequired = true
		//		},
		//		new() {
		//			AttributeControlTypeId = (int)AttributeControlType.Checkboxes,
		//			DisplayOrder = 2,
		//			GoalTypeId = 2,
		//			Name = "Options",
		//			IsRequired = true
		//		},
		//		new() {
		//			AttributeControlTypeId = (int)AttributeControlType.TextBox,
		//			DisplayOrder = 1,
		//			GoalTypeId = 3,
		//			Name = "News item ID",
		//			IsRequired = true
		//		},
		//		new() {
		//			AttributeControlTypeId = (int)AttributeControlType.MultilineTextbox,
		//			DisplayOrder = 2,
		//			GoalTypeId = 3,
		//			Name = "What should they write a comment about? (optional)",
		//			IsRequired = false
		//		},
		//		new() {
		//			AttributeControlTypeId = (int)AttributeControlType.TextBox,
		//			DisplayOrder = 1,
		//			GoalTypeId = 4,
		//			Name = "Blog post ID",
		//			IsRequired = true
		//		},
		//		new() {
		//			AttributeControlTypeId = (int)AttributeControlType.MultilineTextbox,
		//			DisplayOrder = 2,
		//			GoalTypeId = 4,
		//			Name = "What should they write a comment about? (optional)",
		//			IsRequired = false
		//		}
		//	};

		//	await InsertInstallationDataAsync(attributes);

		//	var attributeValues = new List<GoalAttributeValue>() {
		//		new() {
		//			GoalAttributeId = 3,
		//			Name = "In a row",
		//			DisplayOrder = 1
		//		}
		//	};

		//	await InsertInstallationDataAsync(attributeValues);
		//}
		///// <returns>A task that represents the asynchronous operation</returns>
		//protected virtual async Task IntallLevelsAsync()
		//{
		//	var experienceLevels = new List<ExperienceLevel>()
		//	{
		//		new()
		//		{
		//			Name = string.Empty,
		//			Number = 1,
		//			Experience = 0,
		//			IsActive = true,
		//			SystemName = "level 1"
		//		},
		//		new()
		//		{
		//			Name = string.Empty,
		//			Number = 2,
		//			Experience = 1000,
		//			IsActive = true,
		//			SystemName = "level 2"
		//		},
		//		new()
		//		{
		//			Name = string.Empty,
		//			Number = 3,
		//			Experience = 2000,
		//			IsActive = true,
		//			SystemName = "level 3"
		//		},
		//		new()
		//		{
		//			Name = string.Empty,
		//			Number = 4,
		//			Experience = 3500,
		//			IsActive = true,
		//			SystemName = "level 4"
		//		},
		//		new()
		//		{
		//			Name = string.Empty,
		//			Number = 5,
		//			Experience = 5000,
		//			IsActive = true,
		//			SystemName = "level 5"
		//		},
		//		new()
		//		{
		//			Name = string.Empty,
		//			Number = 6,
		//			Experience = 7000,
		//			IsActive = true,
		//			SystemName = "level 6"
		//		},
		//		new()
		//		{
		//			Name = string.Empty,
		//			Number = 7,
		//			Experience = 9000,
		//			IsActive = true,
		//			SystemName = "level 7"
		//		},
		//		new()
		//		{
		//			Name = string.Empty,
		//			Number = 8,
		//			Experience = 11500,
		//			IsActive = true,
		//			SystemName = "level 8"
		//		},
		//		new()
		//		{
		//			Name = string.Empty,
		//			Number = 9,
		//			Experience = 13000,
		//			IsActive = true,
		//			SystemName = "level 9"
		//		},
		//		new()
		//		{
		//			Name = string.Empty,
		//			Number = 10,
		//			Experience = 16000,
		//			IsActive = true,
		//			SystemName = "level 10"
		//		}
		//	};

		//	await InsertInstallationDataAsync(experienceLevels);
		//}

		///// <returns>A task that represents the asynchronous operation</returns>
		//protected virtual async Task IntallRanksAsync()
		//{
		//	//pictures
		//	var pictureService = EngineContext.Current.Resolve<IPictureService>();
		//	var gameImagesPath = GetContentPath();

		//	// TODO LOCALIZED RANKS
		//	var ranks = new List<Rank>() {
		//		new() {
		//			Name = "Enlisted",
		//			Description = string.Empty,
		//			PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(gameImagesPath, "badge_enlisted.png")), MimeTypes.ImagePng, await pictureService.GetPictureSeNameAsync("test_dummy_badge"))).Id,
		//			IsPublished = true,
		//			PreviousRankId = 0
		//		}
		//	};

		//	await InsertInstallationDataAsync(ranks);
		//}

		///// <returns>A task that represents the asynchronous operation</returns>
		//protected virtual async Task IntallInventoryAsync()
		//{
		//	//pictures
		//	//var pictureService = EngineContext.Current.Resolve<IPictureService>();
		//	//var gameImagesPath = GetSamplesPath();

		//	// TODO LOCALIZED INVENTORY ITEMS
		//	var inventoryItems = new List<InventoryItem>() {
		//		new() {
		//			Name = "First-In",
		//			Description = "First in, first contact with meta_EARTH# dataverse.",
		//			InventoryItemTypeId = (int)InventoryItemType.Badge,
		//			NonFungibleTokenId = 1,
		//			Published = true
		//		}
		//	};

		//	await InsertInstallationDataAsync(inventoryItems);

		//}

		///// <returns>A task that represents the asynchronous operation</returns>
		//protected virtual async Task InstallGameplayActivityLogTypesAsync()
		//{
		//	var gameplayActivityTypes = new List<ActivityLogType>
		//	{
		//		//achievement activities
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.AchievementProgress,
		//			Enabled = false,
		//			Name = "Achievement. Progress"
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.AchievementUnlocked,
		//			Enabled = false,
		//			Name = "Achievement. Unlock"
		//		},
		//		//governance activities
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.GovernanceEnlist,
		//			Enabled = false,
		//			Name = "Governance. Enlist."
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.GovernancePromote,
		//			Enabled = false,
		//			Name = "Governance. Promote"
		//		},
		//		//player activities
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.PlayerDailyLogin,
		//			Enabled = false,
		//			Name = "Player. Daily login"
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.PlayerGainExperience,
		//			Enabled = false,
		//			Name = "Player. Gain experience"
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.PlayerLevelReached,
		//			Enabled = false,
		//			Name = "Player. Reach level"
		//		},
		//		//quests activities
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.QuestPlayerEnrolled,
		//			Enabled = false,
		//			Name = "Quests. Enroll"
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.QuestCompleted,
		//			Enabled = false,
		//			Name = "Quests. Complete"
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.QuestGoalAdvanced,
		//			Enabled = false,
		//			Name = "Quests. Advance goal"
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.QuestGoalCompleted,
		//			Enabled = false,
		//			Name = "Quests. Complete goal"
		//		},
		//		//tournaments activities
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.TournamentPlayerEnrolled,
		//			Enabled = false,
		//			Name = "Tournaments. Enroll"
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.TournamentBegin,
		//			Enabled = false,
		//			Name = "Tournaments. Begin"
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.TournamentStageProgress,
		//			Enabled = false,
		//			Name = "Tournaments. Stage progress"
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.TournamentMatchStart,
		//			Enabled = false,
		//			Name = "Tournaments. Match start"
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.TournamentMatchEnd,
		//			Enabled = false,
		//			Name = "Tournaments. Match end"
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.TournamentMatchWin,
		//			Enabled = false,
		//			Name = "Tournaments. Match winner"
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.TournamentMatchLose,
		//			Enabled = false,
		//			Name = "Tournaments. Match loser"
		//		},
		//		new() {
		//			SystemKeyword = SystemKeywords.Gameplay.TournamentEnd,
		//			Enabled = false,
		//			Name = "Tournaments. End"
		//		}
		//	};

		//	await InsertInstallationDataAsync(gameplayActivityTypes);
		//}

		///// <returns>A task that represents the asynchronous operation</returns>
		//protected virtual async Task InstallGameplaySettingsAsync()
		//{
		//	var settingService = EngineContext.Current.Resolve<ISettingService>();

		//	await settingService.SaveSettingAsync(new AchievementSettings
		//	{
		//		//TODO AchievementSettings
		//	});

		//	await settingService.SaveSettingAsync(new ExperienceSettings
		//	{
		//		UseLevelNames = false,
		//		AutoCalculateLevelExperience = false,
		//		LevelExperienceRange = 0,
		//		LevelExperienceRangeAdjustment = 0,
		//		LevelExperienceCalculationTypeId = (int)LevelExperienceCalculationType.None,
		//		UseExperincePointsHistory = true,
		//		ShowNextLevelTotalExperience = true,
		//		ShowRemainingLevelExperience = true,
		//		MinimumExperiecePointsRewarded = 0,
		//		MaximumExperiecePointsRewarded = 0,
		//		HistoryPageSize = 15
		//	});

		//	await settingService.SaveSettingAsync(new GovernanceSettings
		//	{
		//		RankBadgeThumbnailSize = 78,
		//	});

		//	await settingService.SaveSettingAsync(new PlayerSettings
		//	{
		//		ShowPlayerStatsOnHomePage = true,
		//		AllowUsersToCreatePlayerAccount = true,
		//		CreateByUserRegistration = true,
		//		CreateByUserActivation = true,
		//		RequireUsername = true,
		//		RequireEmail = true,
		//		SkipUserValidation = false,
		//		RequireWallet = true,
		//		RequireAccessNft = false,
		//		AllowPlayersToImportInventory = true,
		//		ActivateByWalletAutoRegister = true,
		//		RequireAdminApproval = true,
		//		NotifyAboutPlayerInformationChange = true,
		//		NewPlayerExperienceLevelId = 0,
		//		NewPlayerRankId = 0,
		//	});

		//	await settingService.SaveSettingAsync(new RewardSettings
		//	{
		//		//TODO RewardSettings
		//	});

		//	await settingService.SaveSettingAsync(new QuestSettings
		//	{
		//		Enabled = true,
		//		PanelPageSize = 6,
		//	});

		//	await settingService.SaveSettingAsync(new TournamentSettings
		//	{
		//		Enabled = true,
		//		ExternalTournamentsEnabled = true,
		//		PanelPageSize = 6,
		//	});

		//	await settingService.SaveSettingAsync(new GameplaySettings
		//	{
		//		SystemMessagesPageSize = 10,
		//		DisplayAllPicturesOnGameplayPages = false,
		//		ShowEntityQuestNumber = true,
		//		ShowEntityQuestNumberIncludingSubentities = false,
		//	});

		//	await settingService.SaveSettingAsync(new MetaverseSettings
		//	{
		//		//TODO: METAVERSE FULL SETTINGS :P ?
		//	});
		//}

		//#endregion

#if DEBUG
		#region Content
		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallDebugNewsAsync()
		{
			var defaultLanguageId = _languageRepository.Table.Single(l => l.LanguageCulture == CommonServicesDefaults.DefaultLanguageCulture).Id;

			var news = new List<NewsItem>
			{
				new() {
					LanguageId = defaultLanguageId,
					Title = "News item 1",
					Short = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris.",
					Full = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris nis duis autem vel eum irure dolor in reprehenderit i, dolore eu fugiat nulla pariatur. At vero eos et accusa praesant luptatum delenit aigue duos dolor et mole provident, simil tempor sunt in culpa qui officia de fuga. Et harumd dereud facilis est er expedit disti eligend optio congue nihil impedit doming id quod assumenda est, omnis dolor repellend. Temporibud.",
					Published = true,
					AllowComments = true
				},
				new() {
					LanguageId = defaultLanguageId,
					Title = "News item 2",
					Short = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris.",
					Full = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris nis duis autem vel eum irure dolor in reprehenderit i, dolore eu fugiat nulla pariatur. At vero eos et accusa praesant luptatum delenit aigue duos dolor et mole provident, simil tempor sunt in culpa qui officia de fuga. Et harumd dereud facilis est er expedit disti eligend optio congue nihil impedit doming id quod assumenda est, omnis dolor repellend. Temporibud.",
					Published = true,
					AllowComments = true
				},
				new() {
					LanguageId = defaultLanguageId,
					Title = "News item 3",
					Short = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris.",
					Full = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris nis duis autem vel eum irure dolor in reprehenderit i, dolore eu fugiat nulla pariatur. At vero eos et accusa praesant luptatum delenit aigue duos dolor et mole provident, simil tempor sunt in culpa qui officia de fuga. Et harumd dereud facilis est er expedit disti eligend optio congue nihil impedit doming id quod assumenda est, omnis dolor repellend. Temporibud.",
					Published = false,
					AllowComments = true
				},
				new() {
					LanguageId = defaultLanguageId,
					Title = "News item 4",
					Short = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris.",
					Full = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris nis duis autem vel eum irure dolor in reprehenderit i, dolore eu fugiat nulla pariatur. At vero eos et accusa praesant luptatum delenit aigue duos dolor et mole provident, simil tempor sunt in culpa qui officia de fuga. Et harumd dereud facilis est er expedit disti eligend optio congue nihil impedit doming id quod assumenda est, omnis dolor repellend. Temporibud.",
					Published = true,
					AllowComments = true
				},
				new() {
					LanguageId = defaultLanguageId,
					Title = "News item 5",
					Short = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris.",
					Full = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris nis duis autem vel eum irure dolor in reprehenderit i, dolore eu fugiat nulla pariatur. At vero eos et accusa praesant luptatum delenit aigue duos dolor et mole provident, simil tempor sunt in culpa qui officia de fuga. Et harumd dereud facilis est er expedit disti eligend optio congue nihil impedit doming id quod assumenda est, omnis dolor repellend. Temporibud.",
					Published = true,
					AllowComments = true
				},
			};

			await InsertInstallationDataAsync(news);

			//search engine names
			foreach (var item in news)
			{
				await InsertInstallationDataAsync(new UrlRecord
				{
					EntityId = item.Id,
					EntityName = nameof(NewsItem),
					LanguageId = item.LanguageId,
					IsActive = true,
					Slug = await ValidateSeNameAsync(item, item.Title)
				});
			}
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		protected virtual async Task InstallDebugBlogsAsync()
		{
			var defaultLanguageId = _languageRepository.Table.Single(l => l.LanguageCulture == CommonServicesDefaults.DefaultLanguageCulture).Id;

			var blogPosts = new List<BlogPost>
			{
				new() {
					LanguageId = defaultLanguageId,
					IncludeInSitemap = true,
					Title = "Blog post 1",
					Body = "<p>Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris nis duis autem vel eum irure dolor in reprehenderit i, dolore eu fugiat nulla pariatur. At vero eos et accusa praesant luptatum delenit aigue duos dolor et mole provident, simil tempor sunt in culpa qui officia de fuga. Et harumd dereud facilis est er expedit disti eligend optio congue nihil impedit doming id quod assumenda est, omnis dolor repellend. Temporibud.</p>",
					BodyOverview = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris.",
					AllowComments = true,
				},
				new() {
					LanguageId = defaultLanguageId,
					IncludeInSitemap = true,
					Title = "Blog post 2",
					Body = "<p>Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris nis duis autem vel eum irure dolor in reprehenderit i, dolore eu fugiat nulla pariatur. At vero eos et accusa praesant luptatum delenit aigue duos dolor et mole provident, simil tempor sunt in culpa qui officia de fuga. Et harumd dereud facilis est er expedit disti eligend optio congue nihil impedit doming id quod assumenda est, omnis dolor repellend. Temporibud.</p>",
					BodyOverview = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris.",
					AllowComments = true,
				},
				new() {
					LanguageId = defaultLanguageId,
					IncludeInSitemap = true,
					Title = "Blog post 3",
					Body = "<p>Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris nis duis autem vel eum irure dolor in reprehenderit i, dolore eu fugiat nulla pariatur. At vero eos et accusa praesant luptatum delenit aigue duos dolor et mole provident, simil tempor sunt in culpa qui officia de fuga. Et harumd dereud facilis est er expedit disti eligend optio congue nihil impedit doming id quod assumenda est, omnis dolor repellend. Temporibud.</p>",
					BodyOverview = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris.",
					AllowComments = true,
				},
				new() {
					LanguageId = defaultLanguageId,
					IncludeInSitemap = false,
					Title = "Blog post 4",
					Body = "<p>Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris nis duis autem vel eum irure dolor in reprehenderit i, dolore eu fugiat nulla pariatur. At vero eos et accusa praesant luptatum delenit aigue duos dolor et mole provident, simil tempor sunt in culpa qui officia de fuga. Et harumd dereud facilis est er expedit disti eligend optio congue nihil impedit doming id quod assumenda est, omnis dolor repellend. Temporibud.</p>",
					BodyOverview = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris.",
					AllowComments = true,
				},
				new() {
					LanguageId = defaultLanguageId,
					IncludeInSitemap = true,
					Title = "Blog post 5",
					Body = "<p>Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris nis duis autem vel eum irure dolor in reprehenderit i, dolore eu fugiat nulla pariatur. At vero eos et accusa praesant luptatum delenit aigue duos dolor et mole provident, simil tempor sunt in culpa qui officia de fuga. Et harumd dereud facilis est er expedit disti eligend optio congue nihil impedit doming id quod assumenda est, omnis dolor repellend. Temporibud.</p>",
					BodyOverview = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris.",
					AllowComments = true,
				},
				new() {
					LanguageId = defaultLanguageId,
					IncludeInSitemap = true,
					Title = "Blog post 6",
					Body = "<p>Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris nis duis autem vel eum irure dolor in reprehenderit i, dolore eu fugiat nulla pariatur. At vero eos et accusa praesant luptatum delenit aigue duos dolor et mole provident, simil tempor sunt in culpa qui officia de fuga. Et harumd dereud facilis est er expedit disti eligend optio congue nihil impedit doming id quod assumenda est, omnis dolor repellend. Temporibud.</p>",
					BodyOverview = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris.",
					AllowComments = true,
				},
				new() {
					LanguageId = defaultLanguageId,
					IncludeInSitemap = true,
					Title = "Blog post 7",
					Body = "<p>Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris nis duis autem vel eum irure dolor in reprehenderit i, dolore eu fugiat nulla pariatur. At vero eos et accusa praesant luptatum delenit aigue duos dolor et mole provident, simil tempor sunt in culpa qui officia de fuga. Et harumd dereud facilis est er expedit disti eligend optio congue nihil impedit doming id quod assumenda est, omnis dolor repellend. Temporibud.</p>",
					BodyOverview = "Lorem ipsum dolor si amet, consectetur adipiscing incidunt ut labore et dolore magna aliquam erat nostrud exercitation ullamcorper suscipit laboris.",
					AllowComments = true,
				},
			};

			await InsertInstallationDataAsync(blogPosts);

			//search engine names
			foreach (var blogPost in blogPosts)
			{
				await InsertInstallationDataAsync(new UrlRecord
				{
					EntityId = blogPost.Id,
					EntityName = nameof(BlogPost),
					LanguageId = blogPost.LanguageId,
					IsActive = true,
					Slug = await ValidateSeNameAsync(blogPost, blogPost.Title)
				});
			}
		}

		/// <returns>A task that represents the asynchronous operation</returns>
		//protected virtual async Task InstallDebugQuestsAsync()
		//{
		//	var quests = new List<Quest>
		//	{
		//		new()
		//		{
		//			AllowEnrollment = true,
		//			Body = "",
		//			CanFail = false,
		//			CanRetry = false,
		//			CreatedOnUtc = DateTime.UtcNow,
		//			Deleted = false,
		//			DisplayOrder = 1,
		//			IncludeInSitemap = true,
		//			IsRepeatable = false,
		//			Published = true,
		//			ShowInTopMenu = false,
		//			SystemName = "Demo.Quest.Generic",
		//			Overview = "A simple, generic, demo quest",
		//			Title = "Demo Quest",
		//		},
		//		new()
		//		{
		//			AllowEnrollment = true,
		//			AvailableEndDateTimeUtc = DateTime.UtcNow+TimeSpan.FromDays(7),
		//			AvailableStartDateTimeUtc = DateTime.UtcNow,
		//			Body = "",
		//			CanFail = false,
		//			CanRetry = false,
		//			CreatedOnUtc = DateTime.UtcNow,
		//			Deleted = false,
		//			DisplayOrder = 2,
		//			EndDateUtc = DateTime.UtcNow+TimeSpan.FromDays(15),
		//			IncludeInSitemap = true,
		//			IsRepeatable = false,
		//			Published = true,
		//			ShowInTopMenu = false,
		//			StartDateUtc = DateTime.UtcNow,
		//			SystemName = "Demo.Quest.Timed",
		//			Overview = "A demo quest using availability and start-end dates.",
		//			Title = "Demo Quest, Timed",
		//		},
		//		new()
		//		{
		//			AllowEnrollment = true,
		//			Body = "",
		//			CanFail = true,
		//			CanRetry = true,
		//			CreatedOnUtc = DateTime.UtcNow,
		//			Deleted = false,
		//			DisplayOrder = 3,
		//			IncludeInSitemap = false,
		//			IsRepeatable = true,
		//			Published = true,
		//			MaxRetries = 10,
		//			ShowInTopMenu = false,
		//			SystemName = "Demo.Quest.Repeat",
		//			Overview = "A demo quest using failure, reapeatable and 10 retries.",
		//			Title = "Demo Quest, Repeatable",
		//		}
		//	};

		//	await InsertInstallationDataAsync(quests);

		//	//search engine names
		//	foreach (var quest in quests)
		//	{
		//		await InsertInstallationDataAsync(new UrlRecord
		//		{
		//			EntityId = quest.Id,
		//			EntityName = nameof(Quest),
		//			IsActive = true,
		//			Slug = await ValidateSeNameAsync(quest, quest.SystemName)
		//		});
		//	}
		//}

		#endregion
#endif

		#region Localized content methods

		protected virtual async Task IntallLocalizedMessageTemplatesAsync(ILocalizedEntityService localizedEntityService)
		{
			await Task.Delay(100);
		}

		protected virtual async Task IntallLocalizedBlogsAsync(ILocalizedEntityService localizedEntityService)
		{
			await Task.Delay(100);
		}

		protected virtual async Task IntallLocalizedNewsAsync(ILocalizedEntityService localizedEntityService)
		{
			await Task.Delay(100);
		}

		protected virtual async Task IntallLocalizedTopicsAsync(ILocalizedEntityService localizedEntityService)
		{
			var originalVersionTopics = await _topicRepository.Table.ToListAsync();
			//var urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();

			var translatedTopics = new List<Topic>
			{
				new() {
					Id = 1,
					Title = "Acerca de",
					Body = "<p>En Dragon Corp, nuestra misión es impulsar la innovación y el crecimiento en la industria de los videojuegos creando sinergias entre inversores, jugadores y empresas. Iniciamos nuestra emocionante travesía en el año 2021 cuando un grupo de apasionados jugadores e inversores visionarios se unieron con un objetivo común: revolucionar la forma en que se conecta y prospera el mundo de los videojuegos.</p>" +
						   "<h5>Nuestra Misión</h5>" +
						   "<p>Nuestra misión en Dragon Corp. es clara: fortalecer la comunidad de jugadores facilitando colaboraciones estratégicas y financieras entre todas las partes involucradas. Creemos que cada jugador y empresa en este vibrante ecosistema tiene un papel importante que desempeñar, y trabajando juntos, podemos lograr resultados asombrosos.</p>" +
						   "<h5>Lo Que Hacemos</h5>" +
						   "<p>En Dragon Corp, creamos oportunidades para que inversores y empresas descubran el potencial de la industria de los videojuegos y para que los talentosos jugadores encuentren el apoyo necesario para llevar sus habilidades al siguiente nivel. Facilitamos la inversión en proyectos prometedores, fomentamos asociaciones estratégicas y proporcionamos una plataforma donde los talentos y las brillantes ideas pueden florecer.</p>" +
						   "<h5>Nuestro Compromiso</h5>" +
						   "<p>Estamos comprometidos con la excelencia, la transparencia y la integridad en todo lo que hacemos. Valoramos la diversidad de talentos y creatividad en la comunidad de jugadores y trabajamos incansablemente para fomentar un entorno inclusivo donde todos puedan prosperar.</p>" +
						   "<p>En Dragon Corp, estamos emocionados por el futuro de los videojuegos y estamos decididos a desempeñar un papel fundamental en su evolución. Únete a nosotros en este emocionante viaje mientras creamos un futuro brillante para la industria de los videojuegos, impulsado por la colaboración y la pasión.</p>" +
						   "<p><strong>VAMOS DONDE OTROS NO QUIEREN IR Y HACEMOS LO QUE OTROS NO PUEDEN. SOMOS DRAGON CORP</strong></p>"
				},
				new() {
					Id = 2,
					Title = "Términos de Uso",
					Body = "<p>Somos Dragon Corp. Games S.L., propietarios y operadores del nodo web2/web3 metalink.dragoncorp.org. Al acceder y utilizar este sitio web, aceptas cumplir con los siguientes términos y condiciones. Si no estás de acuerdo con estos términos, te recomendamos que no utilices el sitio.</p>" +
						   "<h5>Uso Aceptable:</h5>" +
						   "<p>Estás autorizado para acceder y utilizar el sitio web con fines lícitos y de acuerdo con estos Términos de Uso. No puedes utilizar el sitio de una manera que cause daño o interfiera con su funcionamiento normal. Esto incluye no intentar acceder a áreas restringidas sin autorización y no participar en actividades que puedan dañar la seguridad del sitio.</p>" +
						   "<h5>Propiedad Intelectual:</h5>" +
						   "<p>Todo el contenido del sitio web, incluyendo textos, imágenes, gráficos, logotipos y software, está protegido por derechos de propiedad intelectual y es propiedad de Dragon Corp. Games S.L. o terceros, utilizados bajo licencia. No se te permite reproducir, distribuir, modificar o crear obras derivadas basadas en este contenido sin autorización.</p>" +
						   "<h5>Enlaces a Terceros:</h5>" +
						   "<p>El sitio web puede contener enlaces a sitios web de terceros. No somos responsables del contenido ni de las prácticas de privacidad de estos sitios. Los enlaces no implican que respaldemos o tengamos afiliación con estos sitios.</p>" +
						   "<h5>Limitación de Responsabilidad:</h5>" +
						   "<p>El sitio web se proporciona \"tal cual\" y no garantizamos que sea libre de errores o ininterrumpido. No somos responsables de ningún daño directo, indirecto, incidental, especial o consecuente que pueda resultar del uso o la incapacidad de usar el sitio web.</p>" +
						   "<h5>Cambios en los Términos:</h5>" +
						   "<p>Nos reservamos el derecho de modificar estos Términos de Uso en cualquier momento. Recomendamos que revises estos términos periódicamente para estar informado de cualquier cambio.</p>" +
						   "<h5>Ley Aplicable:</h5>" +
						   "<p>Estos Términos de Uso están regidos por las leyes de España. Cualquier disputa relacionada con el sitio web estará sujeta a la jurisdicción exclusiva de los tribunales españoles.</p>"
				},
				new() {
					Id = 3,
					Title = string.Empty,
					Body = "<p>¿Tienes alguna pregunta, sugerencia o simplemente quieres saludar? ¡Nos encantaría saber de ti! Siéntete libre de ponerte en contacto a través de nuestro formulario de contacto y te responderemos tan pronto como sea posible. Tu opinión es importante para nosotros y agradecemos tu interés en conectarte con nuestro equipo.</p>"
				},
				new() {
					Id = 4,
					Title = "ACCESO NO AUTORIZADO",
					Body = "<p>Metalink es el sistema de conexión y navegación de Dragon Corp para la interactividad en el metaverso. Solo los miembros autorizados tienen acceso para explorar los datos, actividades y contenidos de Metalink.</p>" +
						   "<p>Por favor, <a href=\"/login?returnUrl=%2F\">inicia sesión</a> si ya tienes una cuenta o <a href=\"register?returnUrl=%2F\">regístrate</a> utilizando tu método preferido.</p>" +
						   "<div class=\"buttons\"><a href=\"/register?returnUrl=%2F\" class=\"button button-1\">Registrarse</a><a href=\"/login?returnUrl=%2F\" class=\"button button-1 dc-notch\">Iniciar sesión</a></div>"
				},
				new() {
					Id = 5,
					Title = "Información de inicio de sesión/registro de usuario",
					Body = "<p>Bienvenido a Metalink, tu portal al interminable metaverso. Con orgullo presentado por Dragon Corp. Ya seas un jugador experimentado o estés comenzando tu viaje en el mundo de los videojuegos, Metalink es tu llave hacia un mundo de experiencias inmersivas. Desbloquea el poder del juego registrándote o iniciando sesión a continuación. Únete a la comunidad de Dragon Corp. Games y sumérgete en un reino donde los sueños de los videojuegos cobran vida. ¡Que comience la aventura!</p>"
				},
				new() {
					Id = 6,
					Title = "Aviso de privacidad",
					Body = "<p>Última actualización: 29/09/2023</p>" +
						   "<p>Gracias por elegir Metalink de Dragon Corp. Games. Estamos comprometidos a proteger tu privacidad y proporcionar una experiencia en línea segura. Este Aviso de Privacidad explica cómo recopilamos, utilizamos, divulgamos y protegemos tu información personal. Al acceder o utilizar nuestro sitio web, aceptas los términos descritos en este Aviso de Privacidad.</p>" +
						   "<h5>1. Información que recopilamos</h5>" +
						   "<ul><li><p><strong>Información personal:</strong> Cuando te registras, creas una cuenta o utilizas nuestros servicios, podemos recopilar información personal como tu nombre, dirección de correo electrónico, detalles de contacto y otros detalles relevantes.</p></li>" +
						   "<li><p><strong>Información de uso:</strong> Recopilamos automáticamente información sobre cómo interactúas con nuestro sitio web, incluida tu dirección IP, tipo de navegador, sistema operativo y comportamiento de navegación.</p></li>" +
						   "<li><p><strong>Cookies y tecnologías similares:</strong> Utilizamos cookies y tecnologías similares para mejorar tu experiencia, personalizar contenido y analizar patrones de uso.</p></li></ul>" +
						   "<h5>2. Cómo utilizamos tu información</h5>" +
						   "<p>Utilizamos la información que recopilamos para diversos fines, incluyendo:</p>" +
						   "<ul><li>Proporcionar y mejorar nuestros servicios.</li>" +
						   "<li>Personalizar tu experiencia.</li>" +
						   "<li>Comunicarnos contigo.</li>" +
						   "<li>Analizar patrones de uso.</li>" +
						   "<li>Mejorar la seguridad.</li></ul>" +
						   "<h5>3. Compartir y divulgar información</h5>" +
						   "<p>No vendemos, intercambiamos ni alquilamos tu información personal a terceros. Sin embargo, podemos compartir información con:</p>" +
						   "<ul><li>Proveedores de servicios: Terceros que nos ayudan a brindar nuestros servicios.</li>" +
						   "<li>Fines legales: Para cumplir con requisitos legales, responder a solicitudes legales o proteger nuestros derechos.</li></ul>" +
						   "<h5>4. Tus opciones y derechos</h5>" +
						   "<p>Tienes el derecho de:</p>" +
						   "<ul><li>Acceder, corregir o eliminar tu información personal.</li>" +
						   "<li>Optar por no participar en ciertas actividades de procesamiento de datos.</li>" +
						   "<li>Retirar el consentimiento.</li></ul>" +
						   "<h5>5. Seguridad</h5>" +
						   "<p>Tomamos medidas razonables para proteger tu información personal contra acceso no autorizado, divulgación, alteración y destrucción.</p>" +
						   "<h5>6. Privacidad de los niños</h5>" +
						   "<p>Nuestros servicios no están dirigidos a personas menores de 13 años. No recopilamos conscientemente información personal de niños.</p>" +
						   "<h5>7. Cambios en este Aviso de Privacidad</h5>" +
						   "<p>Podemos actualizar este Aviso de Privacidad periódicamente. Te notificaremos de cualquier cambio publicando el aviso actualizado en nuestro sitio web.</p>" +
						   "<h5>8. Contáctanos</h5>" +
						   "<p>Si tienes alguna pregunta o inquietud acerca de nuestro Aviso de Privacidad, no dudes en <a title=\"contactarnos\" href=\"contactus\">contactarnos</a>.</p>"
				},
				new() {
					Id = 7,
					Title = string.Empty,
					Body = "<p><strong>¡Ay, no! Parece que la página que buscabas ha tomado un desvío.</strong></p>" +
						   "<ul><li>Si escribiste la URL directamente, verifica la ortografía, es un asunto complicado.</li>" +
						   "<li>Desafortunadamente, parece que la página se ha despedido. Nuestras más sinceras disculpas por cualquier inconveniente causado.</li></ul>" +
						   "<p>Mientras estás aquí, ¿qué te parece un atajo de vuelta a la <a href=\"/\">página de inicio</a>? Es donde puedes comenzar a explorar los contenidos de Metalink.</p>" +
						   "<div class=\"buttons\">" +
						   "<a class=\"button button-1 dc-notch\" href=\"/\">Volver</a>" +
						   "</div>"
				},
				new() {
					Id = 8,
					Title = string.Empty,
					Body = "<p><strong>¿Listo para llevar tu experiencia de juego al siguiente nivel?</strong> ¡Solicita una cuenta de socio con Dragon Corp. Games en Metalink!</p>" +
						   "<p>Libera el poder de la colaboración y sé parte de la comunidad de Dragon Corp. Games. Como socio, te sumergirás en oportunidades exclusivas, desbloquearás nuevos reinos y nos acompañarás en dar forma al futuro de los videojuegos.</p>" +
						   "<p><strong>¡Llamando a todos los gremios de juegos, streamers y creadores de contenido!</strong> Eleva tu experiencia de juego asociándote con Dragon Corp. Games en Metalink. Únete a nuestra comunidad dinámica y desbloquea beneficios exclusivos diseñados para gremios, streamers y creadores de contenido. Sumérgete en un mundo de juego colaborativo, contenido exclusivo y emocionantes oportunidades.</p>" +
						   "<p>¿Listo para amplificar tu presencia en el universo de los videojuegos? Desarrolladores y editores de juegos, ¡aprovechen la oportunidad de colaborar con Dragon Corp. Games en Metalink! Eleva tus juegos a nuevas alturas al convertirte en un socio valioso. Accede a una plataforma diseñada para desarrolladores, conecta con una vibrante comunidad de jugadores y muestra tus creaciones a un público más amplio.</p>" +
						   "<p>¿Preparado para llevar tus juegos al siguiente nivel? ¡Solicita una cuenta de socio a continuación y embarcate en un viaje de innovación y éxito en la industria de los videojuegos!</p>" +
						   "<p><strong>¡Completa el formulario y que comience la sinergia de juegos!</strong></p>"
				},
				new() {
					Id = 9,
					Title = "Términos de servicio para socios",
					Body = "<h5>1. Aceptación de los términos</h5>" +
						   "<p>Al solicitar y aceptar una cuenta de socio con Dragon Corp. Games en Metalink, aceptas quedar sujeto a estos Términos de Servicio. Si no estás de acuerdo con alguna parte de estos términos, no podrás continuar con la solicitud de socio.</p>" +
						   "<h5>2. Elegibilidad del socio</h5>" +
						   "<p>Para ser elegible para una cuenta de socio, debes cumplir con todas las leyes y regulaciones aplicables. Dragon Corp. Games se reserva el derecho de rechazar o terminar cuentas de socios a su discreción.</p>" +
						   "<h5>3. Responsabilidades del socio</h5>" +
						   "<p>Como socio, aceptas:</p>" +
						   "<ul><li>Cumplir con todas las políticas, directrices y normas comunitarias de Metalink.</li>" +
						   "<li>Proporcionar información precisa y actualizada durante el proceso de solicitud.</li>" +
						   "<li>Asegurarte de que el contenido que publicas, incluidos títulos de juegos y misiones, cumpla con nuestras directrices de contenido y no infrinja ninguna ley o derechos de terceros.</li></ul>" +
						   "<h5>4. Propiedad intelectual</h5>" +
						   "<p>Los socios retienen la propiedad de su propiedad intelectual. Al publicar contenido en Metalink, los socios otorgan a Dragon Corp. Games una licencia no exclusiva, mundial y libre de regalías para usar, mostrar y promocionar el contenido en Metalink.</p>" +
						   "<h5>5. Títulos de juegos y misiones</h5>" +
						   "<p>Los socios pueden publicar sus propios títulos de juegos y misiones en Metalink. Dragon Corp. Games se reserva el derecho de revisar y aprobar el contenido enviado. El contenido que infringe nuestras directrices puede ser rechazado o eliminado a nuestra discreción.</p>" +
						   "<h5>6. Participación en ingresos</h5>" +
						   "<p>Los socios pueden ser elegibles para la participación en ingresos según los términos acordados en acuerdos separados. La participación en ingresos está sujeta a cambios, y los socios serán notificados de cualquier modificación.</p>" +
						   "<h5>7. Terminación de la asociación</h5>" +
						   "<p>Dragon Corp. Games se reserva el derecho de terminar cuentas de socios por violaciones de estos términos, incumplimiento de acuerdos u otras razones que se consideren necesarias. Los socios también pueden terminar su asociación proporcionando un aviso por escrito.</p>" +
						   "<h5>8. Resolución de disputas</h5>" +
						   "<p>Cualquier disputa derivada o relacionada con estos Términos de Servicio se resolverá mediante negociación, mediación o arbitraje de acuerdo con las leyes de <strong>España</strong> y la <strong>Unión Europea (UE)</strong>.</p>" +
						   "<h5>9. Cambios en los términos</h5>" +
						   "<p>Dragon Corp. Games puede actualizar estos Términos de Servicio. Los socios serán notificados de los cambios, y el uso continuado de la cuenta de socio constituye la aceptación de los términos modificados.</p>" +
						   "<h5>10. Ley aplicable</h5>" +
						   "<p>Estos Términos de Servicio están regidos por las leyes de <strong>España</strong> y la <strong>Unión Europea (UE)</strong>. Cualquier acción legal o procedimiento relacionado con estos términos se llevará exclusivamente ante los tribunales de <strong>España</strong> y la <strong>Unión Europea (UE)</strong>.</p>"
				}
			};

			foreach (var topic in originalVersionTopics)
			{
				await localizedEntityService.SaveLocalizedValueAsync(topic,
					x => x.Title,
					topic.Title,
					1);

				await localizedEntityService.SaveLocalizedValueAsync(topic,
					x => x.Body,
					topic.Body,
					1);

				////search engine name
				//var seName = await urlRecordService.ValidateSeNameAsync(topic, localized.SeName, localized.Title, false);
				//await urlRecordService.SaveSlugAsync(topic, seName, localized.LanguageId);

			}

			foreach (var topic in translatedTopics)
			{
				await localizedEntityService.SaveLocalizedValueAsync(topic,
					x => x.Title,
					topic.Title,
					2);

				await localizedEntityService.SaveLocalizedValueAsync(topic,
					x => x.Body,
					topic.Body,
					2);

			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Install required data
		/// </summary>
		/// <param name="defaultUserEmail">Default user email</param>
		/// <param name="defaultUserPassword">Default user password</param>
		/// <param name="languagePackInfo">Language pack info</param>
		/// <param name="regionInfo">RegionInfo</param>
		/// <param name="cultureInfo">CultureInfo</param>
		/// <returns>A task that represents the asynchronous operation</returns>
		public virtual async Task InstallRequiredDataAsync(string defaultUserEmail, string defaultUserPassword,
			(string languagePackDownloadLink, int languagePackProgress) languagePackInfo, RegionInfo regionInfo, CultureInfo cultureInfo)
		{
			await InstallNodesAsync();
			await InstallMeasuresAsync(regionInfo);
			await InstallTaxCategoriesAsync();
			await InstallLanguagesAsync(languagePackInfo, cultureInfo, regionInfo);
			await InstallCurrenciesAsync(cultureInfo, regionInfo);
			await InstallCountriesAndStatesAsync();
			await InstallEmailAccountsAsync();
			await InstallMessageTemplatesAsync();
			//await InstallGamesAsync();
			await InstallTopicTemplatesAsync();
			await InstallSettingsAsync(regionInfo);
			await InstallUsersAndRolesAsync(defaultUserEmail, defaultUserPassword);
			await InstallTopicsAsync();
			await InstallActivityLogTypesAsync();
			await InstallScheduleTasksAsync();

			//await InstallRequiredGameplayDataAsync();

			await InstallContentDataAsync();
		}

		//protected virtual async Task InstallRequiredGameplayDataAsync()
		//{
		//	await IntallGoalsAndRewardsAsync();
		//	await IntallLevelsAsync();
		//	await IntallRanksAsync();
		//	await IntallInventoryAsync();
		//	await InstallGameplayActivityLogTypesAsync();

		//	await InstallGameplaySettingsAsync();
		//}

		protected virtual async Task InstallContentDataAsync()
		{
#if DEBUG
			await InstallDebugNewsAsync();
			await InstallDebugBlogsAsync();
			//await InstallDebugQuestsAsync();
#endif

			var localizedEntityService = EngineContext.Current.Resolve<ILocalizedEntityService>();

			await IntallLocalizedTopicsAsync(localizedEntityService);
			await IntallLocalizedNewsAsync(localizedEntityService);
			await IntallLocalizedBlogsAsync(localizedEntityService);
			await IntallLocalizedMessageTemplatesAsync(localizedEntityService);
		}

        public Task InstallNodeAsync(string adminUserEmail, string passwordUserPassword)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
