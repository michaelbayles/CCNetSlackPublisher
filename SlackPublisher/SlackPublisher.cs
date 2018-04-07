using Exortech.NetReflector;
using SlackPublisher;
using System.Linq;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    [ReflectorType("slackPublisher")]
    public class SlackPublisher : ITask
    {
        [ReflectorProperty("webhookUrl")]
        public string WebhookUrl { get; set; }

        public void Run(IIntegrationResult result)
        {
            if (string.IsNullOrEmpty(WebhookUrl))
                return;

            var payload = new Payload(FormatText(result));

            HttpPostHelper.HttpPost(WebhookUrl, payload.ToJson());
        }

        private string FormatText(IIntegrationResult result)
        {
            string message = $"<{result.ProjectUrl}|{result.ProjectName}> {result.Label} {result.Status} ";
            message += result.Succeeded ? ":heavy_check_mark:" : ":interrobang:";
            if (!result.Succeeded)
            {
                message += GetBreakers(result.Modifications);
            }

            return message;
        }

        private string GetBreakers(Modification[] modifications)
        {
            if (modifications == null || modifications.Length == 0) return "";

            return "\nBreaking Modifications:\n" + string.Join("\n", modifications.Select(m => $"<@{m.UserName}> - {m.Comment}").ToArray());
        }
    }
}
