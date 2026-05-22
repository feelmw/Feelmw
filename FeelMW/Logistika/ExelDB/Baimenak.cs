using FeelmwLogistika.Logistika.DatuModeloak;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace FeelmwLogistika.Logistika.ExelDB
{
    public static class Baimenak
    {
        public static SheetsService service;
        public static GoogleCredential credential;// 👈 IMPORTANTE

        public static void Autentikazioa()
        {
            if (service != null)
                return;

            string[] scopes =
            {
            SheetsService.Scope.Spreadsheets,
            DriveService.Scope.Drive
            };

            string credentialPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "aplikaziotik-datuak-irakurri-280b708488bf.json");
            if (!File.Exists(credentialPath))
            {
                credentialPath = "aplikaziotik-datuak-irakurri-280b708488bf.json";
            }

            if (!File.Exists(credentialPath))
            {
                throw new FileNotFoundException("Ez da aurkitu Google Sheets kredentzialen fitxategia.", credentialPath);
            }

            using (var stream = new FileStream(
                credentialPath,
                FileMode.Open,
                FileAccess.Read))
            {
                credential = GoogleCredential
                    .FromStream(stream)
                    .CreateScoped(scopes);
            }

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "MiAppSheets"
            });
        }
    }
}
