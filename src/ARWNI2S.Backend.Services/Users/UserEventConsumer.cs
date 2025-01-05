using ARWNI2S.Backend.Services.Users.Entities;
using ARWNI2S.Events;

namespace ARWNI2S.Backend.Services.Users
{
    /// <summary>
    /// Represents a user event consumer
    /// </summary>
    public class UserEventConsumer : IEventConsumer<UserChangeWorkingLanguageEvent>
    {
        #region Fields

        protected readonly IUserService _userService;
        //protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        //protected readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public UserEventConsumer(IUserService userService)//,
                                                          //INewsLetterSubscriptionService newsLetterSubscriptionService,
                                                          //IStoreContext storeContext)
        {
            _userService = userService;
            //_newsLetterSubscriptionService = newsLetterSubscriptionService;
            //_storeContext = storeContext;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Handle working language changed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(UserChangeWorkingLanguageEvent eventMessage)
        {
            if (eventMessage.User is not User user)
                return;

            if (await _userService.IsGuestAsync(user))
                return;

            //var store = await _storeContext.GetCurrentStoreAsync();
            //var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(user.Email, store.Id);
            //if (subscription != null && subscription.LanguageId != user.LanguageId)
            //{
            //    subscription.LanguageId = user.LanguageId ?? 0;
            //    await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
            //}
        }

        #endregion
    }
}
