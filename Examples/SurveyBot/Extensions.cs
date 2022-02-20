using Telegram.Bot.Types;

namespace SurveyBot
{
    internal static class Extensions
    {
        internal static HowLovelyWeAre ToHowLovelyWeAre(this CallbackQuery callbackQuery)
        {
            return callbackQuery switch
            {
                { Data: { } data } when data.StartsWith("HLWA_")
                    => (HowLovelyWeAre)int.Parse(data[5..]),
                _ => throw new InvalidDataException()
            };
        }

        internal static FoundFromWhere ToFoundFromWhere(this CallbackQuery callbackQuery)
        {
            return callbackQuery switch
            {
                { Data: { } data } when data.StartsWith("FFW_")
                    => (FoundFromWhere)int.Parse(data[4..]),
                _ => throw new InvalidDataException()
            };
        }
    }
}
