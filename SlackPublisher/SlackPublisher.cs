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
            string message = $"{ result.ProjectUrl}|{result.ProjectName} {result.Label} {result.Status}";
            message += result.Succeeded ? ":heavy_check_mark:" : ":interrobang:";
            if (!result.Succeeded)
            {
                message += GetBreakers(result);
                message += $"\nException:{result.ExceptionResult}";
            }

            return message;
        }

        private string GetBreakers(IIntegrationResult result)
        {
            if (result.FailureUsers == null || result.FailureUsers.Count == 0) return "";

            var uniqueFailingUsers = result.FailureUsers.Cast<string>().Distinct();
            return "\nBreakers:\n" + string.Join("\n", uniqueFailingUsers.Select(user => $"<@{user}>").ToArray());
        }
    }
}
