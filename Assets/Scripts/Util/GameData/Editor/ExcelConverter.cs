using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Text;
using System.Diagnostics;

using Debug = UnityEngine.Debug;


namespace QT.Core
{
    public class ExcelConverter
    {
        private static string GameDataPath = $"{Directory.GetCurrentDirectory()}/_GameData";
        private static string JsonDataPath = $"{Directory.GetCurrentDirectory()}/Assets/Resources/GameData";


        [MenuItem("File/Convert Excel Sheets")]
        public static void ConvertExcelToJson()
        {
            try
            {
                var stopWatch = new Stopwatch();

                string[] files = Directory.GetFiles(GameDataPath, $"*.xlsx");
                foreach (string file in files)
                {
                    var filename = file.Split('\\', '.', '/');
                    var name = filename[^2];

                    if(name.StartsWith("~$"))
                    {
                        continue;
                    }

                    stopWatch.Start();
                    Debug.Log($"Converting {name}...");

                    var data = ExcelDataToJson(file);

                    if (data == null)
                    {
                        continue;
                    }

                    StreamWriter sw = new StreamWriter($"{JsonDataPath}/{name}.json", false, Encoding.UTF8);
                    sw.WriteLine(data);
                    sw.Close();


                    stopWatch.Stop();
                    Debug.Log($"{name} Finished (time : {stopWatch.ElapsedMilliseconds}ms)");
                    stopWatch.Reset();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            AssetDatabase.Refresh();
        }



        private static string ExcelDataToJson(string excelFilePath)
        {
            if (!File.Exists(excelFilePath))
            {
                return null;
            }

            var result = new StringBuilder();

            var columns = new List<string>();

            using (var stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var xssWorkbook = new XSSFWorkbook(stream);
                ISheet sheet = xssWorkbook.GetSheetAt(0);


                // 1 번째 행은 어떤 데이터가 있는지 확인하는 용도
                result.Append("[\n\t[\n");

                IRow headerRow = sheet.GetRow(1);
                for (int cellNum = 0; cellNum < headerRow.LastCellNum; cellNum++)
                {
                    ICell cell = headerRow.GetCell(cellNum);
                    var name = cell?.ToString().Trim() ?? null;

                    if (string.IsNullOrWhiteSpace(name) || name[0] == '#')
                    {
                        name = null;
                    }

                    columns.Add(name);

                    if (name != null)
                    {
                        result.Append($"\t\t\"{name}\",\n");
                    }
                }

                result.Append("\t],\n\t[\n");

                // 나머지 행들을 돌아가며 데이터 가공

                for (int rowNum = 2; rowNum <= sheet.LastRowNum; rowNum++)
                {
                    var datas = new Dictionary<string, string>();
                    IRow row = sheet.GetRow(rowNum);

                    for (int cellNum = 0; cellNum < columns.Count; cellNum++)
                    {
                        if (string.IsNullOrEmpty(columns[cellNum]))
                        {
                            continue;
                        }
                        ICell cell = row.GetCell(cellNum);

                        var data = cell?.ToString() ?? null;

                        if (!string.IsNullOrWhiteSpace(data))
                        {
                            datas.Add(columns[cellNum], data);
                        }

                    }

                    if (datas.Count != 0)
                    {
                        result.Append("\t\t{\n");
                        foreach (var data in datas)
                        {
                            result.Append($"\t\t\t\"{data.Key}\":\"{data.Value}\",\n");
                        }
                        result.Append("\t\t},\n");
                    }
                }

                result.Append("\t]\n]");
            }

            return result.ToString();
        }
    }
}
