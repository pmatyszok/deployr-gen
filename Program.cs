using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace deployRGen
{
    class Program
    {
        private static string Namespace;

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Usage();
                return;
            }

            Namespace = args[0];
            var fileToProcess = args.Where(s => s.EndsWith(".R")).ToList();

            foreach (var fileName in fileToProcess)
            {
                ProcessFile(fileName);
            }
        }

        private static void ProcessFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                System.Console.WriteLine($"File {fileName} does not exists");
                return;
            }

            var content = File.ReadAllText(fileName, Encoding.UTF8);

            var inputDef = Patterns.InputDefinition.Matches(content);

            var generatedTypes = new List<GeneratedTypeDescription>();

            foreach (Match match in inputDef)
            {
                var newType = new GeneratedTypeDescription();
                var json = match.Groups["json"].Value;

                var nameMatch = Patterns.NameDefinition.Match(json);
                if (!nameMatch.Success)
                {
                    System.Console.WriteLine($"Name of input not defined in file {fileName} ");
                    continue;
                }
                newType.Name = nameMatch.Groups["name"].Value.Trim();

                var typeMatch = Patterns.TypeDefinition.Match(json);
                if (!typeMatch.Success)
                {
                    Console.WriteLine($"Type (property 'render') of {newType.Name} input not defined in file {fileName} ");
                    continue;
                }
                newType.Type = typeMatch.Groups["type"].Value.Trim();

                var defaultMatch = Patterns.DefaultDefinition.Match(json);
                if (!defaultMatch.Success)
                {
                    Console.WriteLine($"Default value for input {newType.Name} not defined in file {fileName} ");
                    continue;
                }
                newType.DefaultValueText = defaultMatch.Groups["value"].Value.Trim();

                if (newType.IsValid())
                    generatedTypes.Add(newType);
            }

            if (generatedTypes.Count > 0)
            {
                var builder = new StringWriter();

                WriteUsings(builder);

                builder.WriteLine($"namespace {Namespace}");
                builder.WriteLine("{");

                string className = Path.GetFileNameWithoutExtension(fileName);

                builder.WriteLine($"\tclass {className}");
                builder.WriteLine("\t{");
                

                builder.Write(GenerateFields(generatedTypes));

                //class
                builder.Write("\t}");

                //namespace
                builder.Write("}");
            
                builder.WriteLine();

                File.WriteAllText(Path.ChangeExtension(fileName, "cs"), builder.ToString());
            }
            else
            {
                System.Console.WriteLine("Couldn't generate C# class for {fileName} file. Refer to previous errors.");
            }
        }

        private static void WriteUsings(StringWriter sw)
        {
            sw.WriteLine("using System;");
        }

        private static string GenerateFields(List<GeneratedTypeDescription> generatedTypes)
        {
            var sw = new StringWriter();

            foreach (var generatedTypeDescription in generatedTypes)
            {
                sw.WriteLine($"\t\tpublic {generatedTypeDescription.CsType} {generatedTypeDescription.Name} {{ get; set; }}");
            }

            return sw.ToString();
        }

        private static void Usage()
        {
            System.Console.WriteLine("Usage: deployr-gen default-namespace filename.R");
        }
    }
}
