using System.Net.Http;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace VRChat.Analytics
{
    [InitializeOnLoad]
    public class RuntimeCrashHandlerAnalytics : Editor
    {
        private const string PROJECT_ID = "Ctc001_08d9e320-8946-443b-8a6c-9d75baf24292";
        private const string ANALYTICS_CONTENT_TYPE = "application/json";

        static readonly HttpClient _client;
        static RuntimeCrashHandlerAnalytics()
        {
            CrashHandler crashHandler = new CrashHandler();
            using (_client = new HttpClient())
            using (HttpRequestMessage analyticsRequest = new HttpRequestMessage(HttpMethod.Post,
                    crashHandler.GetAnalyticsURL(true) + "&projectid=" + PROJECT_ID))
            {
                var logs = crashHandler.GetCrashErrorLog();
                if (logs != null)
                {
                    string payload = crashHandler.ToJSON(logs);
                    analyticsRequest.Content = new StringContent(payload, Encoding.UTF8, ANALYTICS_CONTENT_TYPE);

                    RuntimeCrashHandlerAnalytics.SendAnalytics(analyticsRequest);
                }
            }
        }

        static void SendAnalytics(HttpRequestMessage request)
        {
            try
            {
                _client.SendAsync(request).GetAwaiter().GetResult();
            } catch { }
        }
    }
}