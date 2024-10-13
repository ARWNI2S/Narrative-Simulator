using ARWNI2S.Infrastructure.Collections.Generic;
using ARWNI2S.Portal.Services.Entities.Mailing;

namespace ARWNI2S.Portal.Services.Mailing
{
    public interface ISystemMessageService
    {
        #region System Messages

        /// <summary>
        /// Deletes a system message
        /// </summary>
        /// <param name="systemMessage">System message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteSystemMessageAsync(SystemMessage systemMessage);

        /// <summary>
        /// Gets a system message
        /// </summary>
        /// <param name="systemMessageId">The system message identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the system message
        /// </returns>
        Task<SystemMessage> GetSystemMessageByIdAsync(int systemMessageId);

        /// <summary>
        /// Gets private messages
        /// </summary>
        /// <param name="nodeId">The node identifier; pass 0 to load all messages</param>
        /// <param name="toPlayerId">The user identifier who should receive the message</param>
        /// <param name="isRead">A value indicating whether loaded messages are read. false - to load not read messages only, 1 to load read messages only, null to load all messages</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the private messages
        /// </returns>
        Task<IPagedList<SystemMessage>> GetAllSystemMessagesAsync(int nodeId,
            int toPlayerId, bool? isRead,
            string keywords, int pageIndex = 0, int pageSize = int.MaxValue);

        ///// <summary>
        ///// Inserts a system message
        ///// </summary>
        ///// <param name="systemMessage">System message</param>
        ///// <param name="attachments">Attachment reward list</param>
        ///// <returns>A task that represents the asynchronous operation</returns>
        //Task InsertSystemMessageAsync(SystemMessage systemMessage, IList<Reward> attachments = null);

        /// <summary>
        /// Updates the system message
        /// </summary>
        /// <param name="systemMessage">System message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateSystemMessageAsync(SystemMessage systemMessage);

        /// <summary>
        /// Formats the system message text
        /// </summary>
        /// <param name="systemMessage">System message</param>
        /// <returns>Formatted text</returns>
        string FormatSystemMessageText(SystemMessage systemMessage);

        /// <summary>
        /// Deletes a system message
        /// </summary>
        /// <param name="systemMessage">System message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<bool> TryDeleteSystemMessageAsync(SystemMessage systemMessage);

        #endregion

        #region Message attachments

        /// <summary>
        /// Deletes a message attachment
        /// </summary>
        /// <param name="messageAttachment">Message attachment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteMessageAttachmentAsync(MessageAttachment messageAttachment);

        /// <summary>
        /// Deletes all system message attachments
        /// </summary>
        /// <param name="systemMessage">System message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteMessageAttachmentsAsync(SystemMessage systemMessage);

        /// <summary>
        /// Inserts a system message
        /// </summary>
        /// <param name="messageAttachment">Message attachment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertMessageAttachmentAsync(MessageAttachment messageAttachment);

        /// <summary>
        /// Gets a system message
        /// </summary>
        /// <param name="id">The message attachment identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the system message
        /// </returns>
        Task<MessageAttachment> GetMessageAttachmentByIdAsync(int id);

        /// <summary>
        /// Gets a system message
        /// </summary>
        /// <param name="systemMessageId">The system message identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the system message
        /// </returns>
        Task<IList<MessageAttachment>> GetMessageAttachmentsBySystemMessageIdAsync(int systemMessageId);

        #endregion
    }
}
