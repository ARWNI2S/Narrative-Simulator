﻿using ARWNI2S.Node.Services.Logging;
using Newtonsoft.Json;

namespace ARWNI2S.Portal.Services.Mailing
{
    /// <summary>
    /// Notification service
    /// </summary>
    public partial class NotificationService : INotificationService
    {
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogService _logger;
        private readonly PortalWorkContext _workContext;

        #endregion

        #region Ctor

        public NotificationService(IHttpContextAccessor httpContextAccessor,
            ILogService logger,
            //ITempDataDictionaryFactory tempDataDictionaryFactory,
            PortalWorkContext workContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Save message into TempData
        /// </summary>
        /// <param name="type">Notification type</param>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        protected virtual void PrepareTempData(NotifyType type, string message, bool encode = true)
        {
            var context = _httpContextAccessor.HttpContext;

            //Messages have stored in a serialized list
            var messages = context.Items.TryGetValue(MessageServicesDefaults.NotificationListKey, out var value)
                ? JsonConvert.DeserializeObject<IList<NotifyData>>(value.ToString())
                : [];

            messages.Add(new NotifyData
            {
                Message = message,
                Type = type,
                Encode = encode
            });

            context.Items[MessageServicesDefaults.NotificationListKey] = JsonConvert.SerializeObject(messages);
        }

        /// <summary>
        /// Log exception
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task LogExceptionAsync(Exception exception)
        {
            if (exception == null)
                return;
            var user = await _workContext.GetCurrentUserAsync();
            await _logger.ErrorAsync(exception.Message, exception, user);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Display notification
        /// </summary>
        /// <param name="type">Notification type</param>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        public virtual void Notification(NotifyType type, string message, bool encode = true)
        {
            PrepareTempData(type, message, encode);
        }

        /// <summary>
        /// Display success notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        public virtual void SuccessNotification(string message, bool encode = true)
        {
            PrepareTempData(NotifyType.Success, message, encode);
        }

        /// <summary>
        /// Display warning notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        public virtual void WarningNotification(string message, bool encode = true)
        {
            PrepareTempData(NotifyType.Warning, message, encode);
        }

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        public virtual void ErrorNotification(string message, bool encode = true)
        {
            PrepareTempData(NotifyType.Error, message, encode);
        }

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="logException">A value indicating whether exception should be logged</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ErrorNotificationAsync(Exception exception, bool logException = true)
        {
            if (exception == null)
                return;

            if (logException)
                await LogExceptionAsync(exception);

            ErrorNotification(exception.Message);
        }

        #endregion
    }
}
