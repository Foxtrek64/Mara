using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Remora.Discord.Caching.Services;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Commands.Services;
using Remora.Results;

namespace Mara.Plugins.Consent.Responders
{
    /// <summary>
    /// Handles checking for and prompting for consent.
    /// </summary>
    public class ConsentPreExecutionCommandEvent
    (
        IFeedbackService feedbackService,
        CacheService cacheService
    )
        : IPreExecutionEvent
    {
        public async Task<Result> BeforeExecutionAsync
        (
            ICommandContext context,
            IStringLocalizer<ConsentPreExecutionCommandEvent> localizer,
            CancellationToken ct = default
        )
        {
            if (context is InteractionContext interactionContext)
            {
                localizer["foo", ]
            }
            else
            {
                
            }
        }
    }
}
