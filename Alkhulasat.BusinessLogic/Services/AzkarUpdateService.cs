using Alkhulasat.BusinessLogic.Messages;
using Alkhulasat.Domain.Interfaces;
using Alkhulasat.Domain.Models;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Text.Json;

namespace Alkhulasat.BusinessLogic.Services
{
    public class AzkarUpdateService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsService _settings;
        private readonly IAssetService _assets;
        private readonly IZekrRepository _repository;

        private const string VersionAppUrl = "https://gist.githubusercontent.com/AhmedELSayed86/f98e8b9a3ac0c68916cbc72e9a5dad9c/raw/versionApp.txt";
        private const string VersionDbUrl = "https://gist.githubusercontent.com/AhmedELSayed86/f98e8b9a3ac0c68916cbc72e9a5dad9c/raw/versionDb.txt";
        private const string JsonUrl = "https://gist.githubusercontent.com/AhmedELSayed86/f98e8b9a3ac0c68916cbc72e9a5dad9c/raw/azkar.json";

        public AzkarUpdateService(HttpClient httpClient, ISettingsService settings, IAssetService assets, IZekrRepository repository)
        {
            _httpClient = httpClient;
            _settings = settings;
            _assets = assets;
            _repository = repository;
        }

        public async Task SyncAzkarAsync()
        {
            // تأكد من تهيئة قاعدة البيانات أولاً قبل أي عملية
            await _repository.InitializeRepositoryAsync().ConfigureAwait(false);
            var localVersion = _settings.GetAzkarVersionDb() ?? "0.0";
            //System.Diagnostics.Debug.WriteLine($"localVersion: {localVersion}");

            if(localVersion == "0.0")
            {
                await LoadInitialDataFromAssets().ConfigureAwait(false);
               // localVersion = _settings.GetAzkarVersionDb() ?? "0.0";
            }

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var cloudVersion = await _httpClient.GetStringAsync(VersionDbUrl, cts.Token).ConfigureAwait(false);
                //System.Diagnostics.Debug.WriteLine($"localVersion: {localVersion} - cloudVersion: {cloudVersion}");

                cloudVersion = cloudVersion.Trim();
                if(cloudVersion != localVersion)
                {
                    var json = await _httpClient.GetStringAsync(JsonUrl, cts.Token).ConfigureAwait(false);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var newAzkar = JsonSerializer.Deserialize<List<ZekrModel>>(json, options);

                    if(newAzkar != null)
                    {
                        await _repository.SyncAzkarSmartlyAsync(newAzkar).ConfigureAwait(false);
                        _settings.SetAzkarVersionDb(cloudVersion);
                        // [إضافة سينيور]: إرسال رسالة لتحديث الواجهة فوراً
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            WeakReferenceMessenger.Default.Send(new SettingsChangedMessage("DbVersionUpdated"));
                        });
                    }
                }
            }
            catch(Exception ex)
            {
                // هذا السطر سيخبرك بالخطأ الحقيقي (ملف مفقود، أو جيسون غير صحيح)
                //System.Diagnostics.Debug.WriteLine($"CRITICAL ERROR: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if(Application.Current?.Windows.Count > 0)
                    {
                        var currentWindow = Application.Current.Windows[0];
                        if(currentWindow.Page != null)
                            await currentWindow.Page.DisplayAlertAsync("خطأ مزامنة - Release", ex.Message, "حسناً");
                    }
                });
            }
        }

        public async Task SyncVersionAppAsync()
        {
            var localVersion = _settings.GetAzkarVersionApp() ?? "0.0";

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var cloudVersion = await _httpClient.GetStringAsync(VersionAppUrl, cts.Token).ConfigureAwait(false);

                cloudVersion = cloudVersion.Trim();
                if(cloudVersion != localVersion)
                {
                    _settings.SetAzkarVersionApp(cloudVersion);
                }
            }
            catch(Exception ex)
            {
                // هذا السطر سيخبرك بالخطأ الحقيقي (ملف مفقود، أو جيسون غير صحيح)
                System.Diagnostics.Debug.WriteLine($"CRITICAL ERROR: {ex.Message}");
            }
        }

        private async Task LoadInitialDataFromAssets()
        {
            try
            {
                var json = await _assets.ReadRawFileAsync("azkar.json");
               // var version = await _assets.ReadRawFileAsync("versionDb.txt");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var initialAzkar = JsonSerializer.Deserialize<List<ZekrModel>>(json, options);

                if(initialAzkar != null)
                {
                    await _repository.SyncAzkarSmartlyAsync(initialAzkar);
                }
              //  _settings.SetAzkarVersionDb(version.Trim());
                // [إضافة سينيور]: إرسال رسالة لتحديث الواجهة فوراً
                //MainThread.BeginInvokeOnMainThread(() =>
                //{
                //    WeakReferenceMessenger.Default.Send(new SettingsChangedMessage("DbVersionUpdated"));
                //});
            }
            catch(Exception ex)
            {
                //Debug.WriteLine($"Error reading asset: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if(Application.Current?.Windows.Count > 0)
                    {
                        var currentWindow = Application.Current.Windows[0];
                        if(currentWindow.Page != null)
                            await currentWindow.Page.DisplayAlertAsync("خطأ مزامنة - Release", ex.Message, "حسناً");
                    }
                });
            }
        }
    }

}
