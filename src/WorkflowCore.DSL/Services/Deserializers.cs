using Newtonsoft.Json;
using SharpYaml;
using System;
using WorkflowCore.Models.DefinitionStorage.v1;

namespace WorkflowCore.Services.DefinitionStorage
{
    public static class Deserializers
    {
        public static Func<string, DefinitionSourceV1> Json = (source) => JsonConvert.DeserializeObject<DefinitionSourceV1>(source);

        public static Func<string, DefinitionSourceV1> Yaml = (source) => YamlSerializer.Deserialize<DefinitionSourceV1>(source);
    }
}
