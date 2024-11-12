using ARWNI2S.Node.Services.Logging;
using ARWNI2S.Node.Services.ScheduleTasks;

namespace ARWNI2S.Portal.Services.Mailing
{
    /// <summary>
    /// Represents a task for sending queued message 
    /// </summary>
    public partial class QueuedMessagesSendTask : IScheduleTask
    {
        #region Fields

        private readonly IEmailAccountService _emailAccountService;
        private readonly IEmailSender _emailSender;
        private readonly ILogService _logger;
        private readonly IQueuedEmailService _queuedEmailService;
        private static readonly char[] separator = [';'];

        #endregion

        #region Ctor

        public QueuedMessagesSendTask(IEmailAccountService emailAccountService,
            IEmailSender emailSender,
            ILogService logger,
            IQueuedEmailService queuedEmailService)
        {
            _emailAccountService = emailAccountService;
            _emailSender = emailSender;
            _logger = logger;
            _queuedEmailService = queuedEmailService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual async Task ExecuteAsync()
        {
            var maxTries = 3;
            var queuedEmails = await _queuedEmailService.SearchEmailsAsync(null, null, null, null,
                true, true, maxTries, false, 0, 500);
            foreach (var queuedEmail in queuedEmails)
            {
                var bcc = string.IsNullOrWhiteSpace(queuedEmail.Bcc)
                            ? null
                            : queuedEmail.Bcc.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                var cc = string.IsNullOrWhiteSpace(queuedEmail.CC)
                            ? null
                            : queuedEmail.CC.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    await _emailSender.SendEmailAsync(await _emailAccountService.GetEmailAccountByIdAsync(queuedEmail.EmailAccountId),
                        queuedEmail.Subject,
                        queuedEmail.Body,
                       queuedEmail.From,
                       queuedEmail.FromName,
                       queuedEmail.To,
                       queuedEmail.ToName,
                       queuedEmail.ReplyTo,
                       queuedEmail.ReplyToName,
                       bcc,
                       cc,
                       queuedEmail.AttachmentFilePath,
                       queuedEmail.AttachmentFileName,
                       queuedEmail.AttachedDownloadId);

                    queuedEmail.SentOnUtc = DateTime.UtcNow;
                }
                catch (Exception exc)
                {
                    await _logger.ErrorAsync($"Error sending e-mail. {exc.Message}", exc);
                }
                finally
                {
                    queuedEmail.SentTries += 1;
                    await _queuedEmailService.UpdateQueuedEmailAsync(queuedEmail);
                }
            }
        }

        #endregion
    }
}