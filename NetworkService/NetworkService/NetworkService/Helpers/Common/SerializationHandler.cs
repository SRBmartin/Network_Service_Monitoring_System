using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace NetworkService.Helpers.Common
{
    public class SerializationHandler
    {
        public static void SerializeEntitiesToFile(ObservableCollection<PowerConsumption> entities, string filePath = "../../Resource/json/entities.json")
        {
            var option = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            string jsonString = JsonSerializer.Serialize(entities, option);
            File.WriteAllText(filePath, jsonString);
        }
        public static ObservableCollection<PowerConsumption> DeserializeEntitiesFromFile(string filePath = "../../Resource/json/entities.json")
        {
            string jsonString = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(jsonString))
            {
                return new ObservableCollection<PowerConsumption>();
            }
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            var result = JsonSerializer.Deserialize<ObservableCollection<PowerConsumption>>(jsonString, options);
            return result ?? new ObservableCollection<PowerConsumption>();
        }
        public static void SerializeEntitiesToFile(ObservableCollection<LinesHolder> entities, string filePath = "../../Resource/json/entities.json")
        {
            var option = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            string jsonString = JsonSerializer.Serialize(entities, option);
            File.WriteAllText(filePath, jsonString);
        }
        public static ObservableCollection<LinesHolder> DeserializeEntitiesFromFileLines(string filePath = "../../Resource/json/linesConnections.json")
        {
            string jsonString = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(jsonString))
            {
                return new ObservableCollection<LinesHolder>();
            }
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            var result = JsonSerializer.Deserialize<ObservableCollection<LinesHolder>>(jsonString, options);
            return result ?? new ObservableCollection<LinesHolder>();
        }
    }
}
