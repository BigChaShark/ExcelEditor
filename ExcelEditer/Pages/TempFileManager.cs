using Newtonsoft.Json;
using static IndexModel;

namespace ExcelEditor.Pages
{
    public class TempFileManager
    {
        public static string SaveUsersToTempFile(List<UserModel> users)
        {
            var tempDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            var fileName = $"users_{Guid.NewGuid()}.json";
            var filePath = Path.Combine(tempDir, fileName);
            var json = JsonConvert.SerializeObject(users);
            System.IO.File.WriteAllText(filePath, json);
            return fileName;
        }
        public static string SaveSummaryUsersToTempFile(List<UserSummaryModel> users)
        {
            var tempDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            var fileName = $"users_{Guid.NewGuid()}.json";
            var filePath = Path.Combine(tempDir, fileName);
            var json = JsonConvert.SerializeObject(users);
            System.IO.File.WriteAllText(filePath, json);
            return fileName;
        }

        public static List<UserModel> LoadUsersFromTempFile(string fileName)
        {
            var tempPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp", fileName);
            if (!System.IO.File.Exists(tempPath)) return new List<UserModel>();

            var json = System.IO.File.ReadAllText(tempPath);
            return JsonConvert.DeserializeObject<List<UserModel>>(json);
        }
        public static List<UserSummaryModel> LoadUsersFromSummaryTemp(string fileName)
        {
            var tempPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp", fileName);
            if (!System.IO.File.Exists(tempPath)) return new List<UserSummaryModel>();

            var json = System.IO.File.ReadAllText(tempPath);
            return JsonConvert.DeserializeObject<List<UserSummaryModel>>(json);
        }
    }
}
