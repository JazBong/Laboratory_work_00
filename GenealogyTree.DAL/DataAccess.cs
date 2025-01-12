using BLL;
using System.Text.Json;

namespace DAL
{
    public class DataAccess
    {
        static string StartFilePath = "../../../../DAL/";
        static string EndFilePath = "family_tree.json";
        static string FullFilePath = "";

        public static void SaveTree(FamilyTree tree)
        {
            var json = JsonSerializer.Serialize(tree, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FullFilePath, json);
        }

        public static FamilyTree LoadTree()
        {
            if (!File.Exists(FullFilePath)) return new FamilyTree();
            var json = File.ReadAllText(FullFilePath);
            return JsonSerializer.Deserialize<FamilyTree>(json) ?? new FamilyTree();
        }
        public static void SetEndFilePath(string endPath)
        {
            EndFilePath = endPath + ".json";
            FullFilePath = StartFilePath + EndFilePath;
        }
        static DataAccess()
        {
            FullFilePath = StartFilePath + EndFilePath;
        }
    }
}
