using ARWNI2S.Caching;
using ARWNI2S.Engine.Caching;
using ARWNI2S.Engine.Data;
using ARWNI2S.Engine.Infrastructure;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace ARWNI2S.Framework.Configuration
{
    /// <summary>
    /// Setting manager
    /// </summary>
    public partial class SettingService : ISettingService
    {
        #region Fields

        protected readonly IRepository<Setting> _settingRepository;
        protected readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public SettingService(IRepository<Setting> settingRepository,
            IStaticCacheManager staticCacheManager)
        {
            _settingRepository = settingRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the settings
        /// </returns>
        protected virtual async Task<IDictionary<string, IList<Setting>>> GetAllSettingsDictionaryAsync()
        {
            return await _staticCacheManager.GetAsync(SettingsDefaults.SettingsAllAsDictionaryCacheKey, async () =>
            {
                var settings = await GetAllSettingsAsync();

                var dictionary = new Dictionary<string, IList<Setting>>();
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new Setting
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value,
                        NodeId = s.NodeId
                    };
                    if (!dictionary.TryGetValue(resourceName, out var value))
                        //first setting
                        dictionary.Add(resourceName, new List<Setting>
                        {
                            settingForCaching
                        });
                    else
                        //already added
                        //most probably it's the setting with the same name but for some certain node (nodeId > 0)
                        value.Add(settingForCaching);
                }

                return dictionary;
            });
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>
        /// Settings
        /// </returns>
        protected virtual IDictionary<string, IList<Setting>> GetAllSettingsDictionary()
        {
            return _staticCacheManager.Get(SettingsDefaults.SettingsAllAsDictionaryCacheKey, () =>
            {
                var settings = GetAllSettings();

                var dictionary = new Dictionary<string, IList<Setting>>();
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new Setting
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value,
                        NodeId = s.NodeId
                    };
                    if (!dictionary.TryGetValue(resourceName, out var value))
                        //first setting
                        dictionary.Add(resourceName, new List<Setting>
                        {
                            settingForCaching
                        });
                    else
                        //already added
                        //most probably it's the setting with the same name but for some certain node (nodeId > 0)
                        value.Add(settingForCaching);
                }

                return dictionary;
            });
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="nodeId">Node identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task SetSettingAsync(Type type, string key, object value, int nodeId = 0, bool clearCache = true)
        {
            ArgumentNullException.ThrowIfNull(key);
            key = key.Trim().ToLowerInvariant();
            var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);

            var allSettings = await GetAllSettingsDictionaryAsync();
            var settingForCaching = allSettings.TryGetValue(key, out var settings) ?
                settings.FirstOrDefault(x => x.NodeId == nodeId) : null;
            if (settingForCaching != null)
            {
                //update
                var setting = await GetSettingByIdAsync(settingForCaching.Id);
                setting.Value = valueStr;
                await UpdateSettingAsync(setting, clearCache);
            }
            else
            {
                //insert
                var setting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                    NodeId = nodeId
                };
                await InsertSettingAsync(setting, clearCache);
            }
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="nodeId">Node identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        protected virtual void SetSetting(Type type, string key, object value, int nodeId = 0, bool clearCache = true)
        {
            ArgumentNullException.ThrowIfNull(key);
            key = key.Trim().ToLowerInvariant();
            var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);

            var allSettings = GetAllSettingsDictionary();
            var settingForCaching = allSettings.TryGetValue(key, out var settings) ?
                settings.FirstOrDefault(x => x.NodeId == nodeId) : null;
            if (settingForCaching != null)
            {
                //update
                var setting = GetSettingById(settingForCaching.Id);
                setting.Value = valueStr;
                UpdateSetting(setting, clearCache);
            }
            else
            {
                //insert
                var setting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                    NodeId = nodeId
                };
                InsertSetting(setting, clearCache);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertSettingAsync(Setting setting, bool clearCache = true)
        {
            await _settingRepository.InsertAsync(setting);

            //cache
            if (clearCache)
                await ClearCacheAsync();
        }

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void InsertSetting(Setting setting, bool clearCache = true)
        {
            _settingRepository.Insert(setting);

            //cache
            if (clearCache)
                ClearCache();
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateSettingAsync(Setting setting, bool clearCache = true)
        {
            ArgumentNullException.ThrowIfNull(setting);

            await _settingRepository.UpdateAsync(setting);

            //cache
            if (clearCache)
                await ClearCacheAsync();
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void UpdateSetting(Setting setting, bool clearCache = true)
        {
            ArgumentNullException.ThrowIfNull(setting);

            _settingRepository.Update(setting);

            //cache
            if (clearCache)
                ClearCache();
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteSettingAsync(Setting setting)
        {
            await _settingRepository.DeleteAsync(setting);

            //cache
            await ClearCacheAsync();
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void DeleteSetting(Setting setting)
        {
            _settingRepository.Delete(setting);

            //cache
            ClearCache();
        }

        /// <summary>
        /// Deletes settings
        /// </summary>
        /// <param name="settings">Settings</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteSettingsAsync(IList<Setting> settings)
        {
            await _settingRepository.DeleteAsync(settings);

            //cache
            await ClearCacheAsync();
        }

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting
        /// </returns>
        public virtual async Task<Setting> GetSettingByIdAsync(int settingId)
        {
            return await _settingRepository.GetByIdAsync(settingId, cache => default);
        }

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>
        /// The setting
        /// </returns>
        public virtual Setting GetSettingById(int settingId)
        {
            return _settingRepository.GetById(settingId, cache => default);
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="nodeId">Node identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all nodes) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting
        /// </returns>
        public virtual async Task<Setting> GetSettingAsync(string key, int nodeId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var settings = await GetAllSettingsDictionaryAsync();
            key = key.Trim().ToLowerInvariant();
            if (!settings.TryGetValue(key, out var value))
                return null;

            var settingsByKey = value;
            var setting = settingsByKey.FirstOrDefault(x => x.NodeId == nodeId);

            //load shared value?
            if (setting == null && nodeId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.NodeId == 0);

            return setting != null ? await GetSettingByIdAsync(setting.Id) : null;
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="nodeId">Node identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all nodes) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>
        /// The setting
        /// </returns>
        public virtual Setting GetSetting(string key, int nodeId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var settings = GetAllSettingsDictionary();
            key = key.Trim().ToLowerInvariant();
            if (!settings.TryGetValue(key, out var value))
                return null;

            var settingsByKey = value;
            var setting = settingsByKey.FirstOrDefault(x => x.NodeId == nodeId);

            //load shared value?
            if (setting == null && nodeId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.NodeId == 0);

            return setting != null ? GetSettingById(setting.Id) : null;
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="nodeId">Node identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all nodes) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting value
        /// </returns>
        public virtual async Task<T> GetSettingByKeyAsync<T>(string key, T defaultValue = default,
            int nodeId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var settings = await GetAllSettingsDictionaryAsync();
            key = key.Trim().ToLowerInvariant();
            if (!settings.TryGetValue(key, out var value))
                return defaultValue;

            var settingsByKey = value;
            var setting = settingsByKey.FirstOrDefault(x => x.NodeId == nodeId);

            //load shared value?
            if (setting == null && nodeId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.NodeId == 0);

            return setting != null ? CommonHelper.To<T>(setting.Value) : defaultValue;
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="nodeId">Node identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all nodes) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>
        /// Setting value
        /// </returns>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default,
            int nodeId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettingsDictionary();
            key = key.Trim().ToLowerInvariant();
            if (!settings.TryGetValue(key, out var value))
                return defaultValue;

            var settingsByKey = value;
            var setting = settingsByKey.FirstOrDefault(x => x.NodeId == nodeId);

            //load shared value?
            if (setting == null && nodeId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.NodeId == 0);

            return setting != null ? CommonHelper.To<T>(setting.Value) : defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="nodeId">Node identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetSettingAsync<T>(string key, T value, int nodeId = 0, bool clearCache = true)
        {
            await SetSettingAsync(typeof(T), key, value, nodeId, clearCache);
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="nodeId">Node identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void SetSetting<T>(string key, T value, int nodeId = 0, bool clearCache = true)
        {
            SetSetting(typeof(T), key, value, nodeId, clearCache);
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the settings
        /// </returns>
        public virtual async Task<IList<Setting>> GetAllSettingsAsync()
        {
            var settings = await _settingRepository.GetAllAsync(query =>
            {
                return from s in query
                       orderby s.Name, s.NodeId
                       select s;
            }, cache => default);

            return settings;
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>
        /// Settings
        /// </returns>
        public virtual IList<Setting> GetAllSettings()
        {
            var settings = _settingRepository.GetAll(query => from s in query
                                                              orderby s.Name, s.NodeId
                                                              select s, cache => default);

            return settings;
        }

        /// <summary>
        /// Determines whether a setting exists
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="nodeId">Node identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the true -setting exists; false - does not exist
        /// </returns>
        public virtual async Task<bool> SettingExistsAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int nodeId = 0)
            where T : ISettings, new()
        {
            var key = GetSettingKey(settings, keySelector);

            var setting = await GetSettingByKeyAsync<string>(key, nodeId: nodeId);
            return setting != null;
        }

        /// <summary>
        /// Determines whether a setting exists
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="nodeId">Node identifier</param>
        /// <returns>
        /// The true -setting exists; false - does not exist
        /// </returns>
        public virtual bool SettingExists<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int nodeId = 0)
            where T : ISettings, new()
        {
            var key = GetSettingKey(settings, keySelector);

            var setting = GetSettingByKey<string>(key, nodeId: nodeId);
            return setting != null;
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="nodeId">Node identifier for which settings should be loaded</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<T> LoadSettingAsync<T>(int nodeId = 0) where T : ISettings, new()
        {
            return (T)await LoadSettingAsync(typeof(T), nodeId);
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="nodeId">Node identifier for which settings should be loaded</param>
        public virtual T LoadSetting<T>(int nodeId = 0) where T : ISettings, new()
        {
            return (T)LoadSetting(typeof(T), nodeId);
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="nodeId">Node identifier for which settings should be loaded</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<ISettings> LoadSettingAsync(Type type, int nodeId = 0)
        {
            var settings = Activator.CreateInstance(type);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return settings as ISettings;

            foreach (var prop in type.GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = type.Name + "." + prop.Name;
                //load by node
                var setting = await GetSettingByKeyAsync<string>(key, nodeId: nodeId, loadSharedValueIfNotFound: true);
                if (setting == null)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                    continue;

                var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings as ISettings;
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="nodeId">Node identifier for which settings should be loaded</param>
        /// <returns>Settings</returns>
        public virtual ISettings LoadSetting(Type type, int nodeId = 0)
        {
            var settings = Activator.CreateInstance(type);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return settings as ISettings;

            foreach (var prop in type.GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = type.Name + "." + prop.Name;
                //load by node
                var setting = GetSettingByKey<string>(key, nodeId: nodeId, loadSharedValueIfNotFound: true);
                if (setting == null)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                    continue;

                var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings as ISettings;
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="nodeId">Node identifier</param>
        /// <param name="settings">Setting instance</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SaveSettingAsync<T>(T settings, int nodeId = 0) where T : ISettings, new()
        {
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                var value = prop.GetValue(settings, null);
                if (value != null)
                    await SetSettingAsync(prop.PropertyType, key, value, nodeId, false);
                else
                    await SetSettingAsync(key, string.Empty, nodeId, false);
            }

            //and now clear cache
            await ClearCacheAsync();
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="nodeId">Node identifier</param>
        /// <param name="settings">Setting instance</param>
        public virtual void SaveSetting<T>(T settings, int nodeId = 0) where T : ISettings, new()
        {
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                var value = prop.GetValue(settings, null);
                if (value != null)
                    SetSetting(prop.PropertyType, key, value, nodeId, false);
                else
                    SetSetting(key, string.Empty, nodeId, false);
            }

            //and now clear cache
            ClearCache();
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="nodeId">Node ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SaveSettingAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            int nodeId = 0, bool clearCache = true) where T : ISettings, new()
        {
            if (keySelector.Body is not MemberExpression member)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo
                           ?? throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var key = GetSettingKey(settings, keySelector);
            var value = (TPropType)propInfo.GetValue(settings, null);
            if (value != null)
                await SetSettingAsync(key, value, nodeId, clearCache);
            else
                await SetSettingAsync(key, string.Empty, nodeId, clearCache);
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="nodeId">Node ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void SaveSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            int nodeId = 0, bool clearCache = true) where T : ISettings, new()
        {
            if (keySelector.Body is not MemberExpression member)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo
                           ?? throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var key = GetSettingKey(settings, keySelector);
            var value = (TPropType)propInfo.GetValue(settings, null);
            if (value != null)
                SetSetting(key, value, nodeId, clearCache);
            else
                SetSetting(key, string.Empty, nodeId, clearCache);
        }

        /// <summary>
        /// Save settings object (per node). If the setting is not overridden per node then it'll be delete
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="overrideForNode">A value indicating whether to setting is overridden in some node</param>
        /// <param name="nodeId">Node ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SaveSettingOverridablePerNodeAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            bool overrideForNode, int nodeId = 0, bool clearCache = true) where T : ISettings, new()
        {
            if (overrideForNode || nodeId == 0)
                await SaveSettingAsync(settings, keySelector, nodeId, clearCache);
            else if (nodeId > 0)
                await DeleteSettingAsync(settings, keySelector, nodeId);
        }

        /// <summary>
        /// Delete all settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteSettingAsync<T>() where T : ISettings, new()
        {
            var settingsToDelete = new List<Setting>();
            var allSettings = await GetAllSettingsAsync();
            foreach (var prop in typeof(T).GetProperties())
            {
                var key = typeof(T).Name + "." + prop.Name;
                settingsToDelete.AddRange(allSettings.Where(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
            }

            await DeleteSettingsAsync(settingsToDelete);
        }

        /// <summary>
        /// Delete settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="nodeId">Node ID</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteSettingAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int nodeId = 0) where T : ISettings, new()
        {
            var key = GetSettingKey(settings, keySelector);
            key = key.Trim().ToLowerInvariant();

            var allSettings = await GetAllSettingsDictionaryAsync();
            var settingForCaching = allSettings.TryGetValue(key, out var settings_) ?
                settings_.FirstOrDefault(x => x.NodeId == nodeId) : null;
            if (settingForCaching == null)
                return;

            //update
            var setting = await GetSettingByIdAsync(settingForCaching.Id);
            await DeleteSettingAsync(setting);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ClearCacheAsync()
        {
            await _staticCacheManager.RemoveByPrefixAsync(EntityCacheDefaults<Setting>.Prefix);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            _staticCacheManager.RemoveByPrefix(EntityCacheDefaults<Setting>.Prefix);
        }

        /// <summary>
        /// Get setting key (noded into database)
        /// </summary>
        /// <typeparam name="TSettings">Type of settings</typeparam>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <returns>Key</returns>
        public virtual string GetSettingKey<TSettings, T>(TSettings settings, Expression<Func<TSettings, T>> keySelector)
            where TSettings : ISettings, new()
        {
            if (keySelector.Body is not MemberExpression member)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            if (member.Member is not PropertyInfo propInfo)
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var key = $"{typeof(TSettings).Name}.{propInfo.Name}";

            return key;
        }

        #endregion

        async Task<ISetting> ISettingService.GetSettingByIdAsync(int settingId) => await GetSettingByIdAsync(settingId);

        ISetting ISettingService.GetSettingById(int settingId) => GetSettingById(settingId);

        async Task ISettingService.DeleteSettingAsync(ISetting setting) => await DeleteSettingAsync((Setting)setting);

        void ISettingService.DeleteSetting(ISetting setting) => DeleteSetting((Setting)setting);

        async Task ISettingService.DeleteSettingsAsync(IList<ISetting> settings) => await DeleteSettingsAsync((IList<Setting>)settings);

        async Task<ISetting> ISettingService.GetSettingAsync(string key, int nodeId, bool loadSharedValueIfNotFound) => await GetSettingAsync(key, nodeId, loadSharedValueIfNotFound);

        ISetting ISettingService.GetSetting(string key, int nodeId, bool loadSharedValueIfNotFound) => GetSetting(key, nodeId, loadSharedValueIfNotFound);

        async Task<IList<ISetting>> ISettingService.GetAllSettingsAsync() => (IList<ISetting>)await GetAllSettingsAsync();

        IList<ISetting> ISettingService.GetAllSettings() => (IList<ISetting>)GetAllSettingsAsync();

        async Task ISettingService.InsertSettingAsync(ISetting setting, bool clearCache) => await InsertSettingAsync((Setting)setting, clearCache);

        void ISettingService.InsertSetting(ISetting setting, bool clearCache) => InsertSetting((Setting)setting, clearCache);

        async Task ISettingService.UpdateSettingAsync(ISetting setting, bool clearCache) => await UpdateSettingAsync((Setting)setting, clearCache);

        void ISettingService.UpdateSetting(ISetting setting, bool clearCache) => UpdateSetting((Setting)setting, clearCache);

    }
}