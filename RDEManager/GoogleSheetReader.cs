using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

//from https://www.twilio.com/blog/2017/03/google-spreadsheets-and-net-core.html
namespace RDEManager
{
   
    class GoogleSheetReader
    {
        public GoogleSheetReader() { }

        public List<RDETrackingRecord> readGoogleSheet(string spreadsheetID, string sheet, string startColumn, string endColumn)
        {

            GoogleCredential credential;
            using (var stream = new FileStream("gs_client_secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            // Create Google Sheets API service.
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            var range = $"{sheet}!{startColumn}:{endColumn}";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetID, range);

            var response = request.Execute();
            IList<IList<object>> values = response.Values;

            List<RDETrackingRecord> results = new List<RDETrackingRecord>();
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {

                    results.Add(new RDETrackingRecord(row[0].ToString(), row[1].ToString(), row[2].ToString().ToLower(), row[3].ToString(), row[4].ToString()));
                }
            }

            return results;
        }

        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "RDE Manager";
        static SheetsService service;

    }
}
