using System.Windows.Controls;
using MixUpload.Net;

namespace Flow.Launcher.Plugin.MixUpload
{
    public class Main : IAsyncPlugin, IPluginI18n, ISettingProvider, IDisposable
    {

        private PluginInitContext _context;
        private MixUploadClient _client;
        //private readonly Settings _settings;

        public Task InitAsync(PluginInitContext context)
        {
            _context = context;
            _client = new MixUploadClient();
            return Task.CompletedTask;
        }

        public async Task<List<Result>> QueryAsync(Query query, CancellationToken token)
        {
            List<Result> results = new List<Result>();
            token.ThrowIfCancellationRequested();

            if (query.FirstSearch.Length > 0)
            {
                var response = await _client.SearchAsync(query.Search);
                if (response.Any())
                {

                    foreach (var mixUpladResponse in response)
                    {
                        var result = new Result
                        {
                            Title = mixUpladResponse.title,
                            SubTitle = mixUpladResponse.artist,
                            IcoPath = $"https://static.mixupload.com/n{mixUpladResponse.srv}/{mixUpladResponse.picture}",
                            Action = e =>
                            {

                                bool ret;
                                try
                                {
                                    _context.API.OpenUrl($"https://mixupload.com/track/{mixUpladResponse.url}");
                                    ret = true;
                                }
                                catch (Exception)
                                {
                                    _context.API.ShowMsg(string.Format(_context.API.GetTranslation("flow_plugin_url_canot_open_url"), query.Search));
                                    ret = false;
                                }
                                return ret;
                            }
                        };
                        results.Add(result);
                    }
                }
                else
                {
                    results.Add(new Result { Title = "Error", SubTitle = "", IcoPath = "Images\\mixupload.png" });
                }
            }
            return results;
        }

        public string GetTranslatedPluginTitle()
        {
            return _context.API.GetTranslation("flow_plugin_mixupload_plugin_name");
        }

        public string GetTranslatedPluginDescription()
        {
            return _context.API.GetTranslation("flow_plugin_mixupload_plugin_description");
        }

        public Control CreateSettingPanel()
        {
            return new MixUploadSettings();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
