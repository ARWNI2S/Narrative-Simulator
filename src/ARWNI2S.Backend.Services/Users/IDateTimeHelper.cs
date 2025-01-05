
namespace ARWNI2S.Backend.Services.Users
{
    public interface IDateTimeHelper
    {
        Task<DateTime> ConvertToUserTimeAsync(DateTime now);
    }
}