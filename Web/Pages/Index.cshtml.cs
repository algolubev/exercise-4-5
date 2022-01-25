using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Pages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        IMessageSession messageSession;

        public string ResponseText { get; set; }

        public IndexModel(IMessageSession messageSession)
        {
            this.messageSession = messageSession;
        }

        public async Task<IActionResult> OnPostAsync(string textField)
        {
            if (string.IsNullOrWhiteSpace(textField))
            {
                return Page();
            }

            #region ActionHandling

            if (!int.TryParse(textField, out var number))
            {
                return Page();
            }
            var command = new Command
            {
                Id = number,
                OrderId = Guid.NewGuid().ToString()
            };

            var sendOptions = new SendOptions();
            sendOptions.SetDestination("WorkerQueue");
            var code = await messageSession.Request<ResponceMessage>(command, sendOptions);
            //ResponseText = $"Responce: { command.Id }";
            ResponseText = $"Responce: { code.NumberType }";

            return Page();
            #endregion
        }
    }
}
