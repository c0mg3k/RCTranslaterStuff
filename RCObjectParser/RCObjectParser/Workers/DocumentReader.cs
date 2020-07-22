using RCObjectParser.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RCObjectParser.Workers
{
    public static class DocumentReader
    {
        public static void WriteFiles(string fileLocation)
        {
            //C:\programming\sandbox\LPP\RapidConnect\raw\gmfschema.txt
            //C:\programming\sandbox\LPP\RapidConnect\raw\rchttpservice.txt

            foreach(string result in  Directory.GetFiles(fileLocation))
            {
                generateBlocks(result, "[System.CodeDom.Compiler.GeneratedCodeAttribute(\"xsd\", \"4.8.3928.0\")]", "using System.Xml.Serialization;");
            }
        }

        private static List<string> generateBlocks(string filename, string indicator, string usings)
        {
            List<string> results = new List<string>();
            string text = File.ReadAllText(filename);
            string[] tmpresults = text.Split(indicator);
            foreach(string result in tmpresults)
            {
                results.Add(indicator + result);
                FileConfig fc = getFileInfo(indicator + result);
                if (!string.IsNullOrWhiteSpace(fc.FileName))
                {
                    string baseFilePath = fc.Type == "enum" ? "C:\\programming\\sandbox\\LPP\\RapidConnect\\results\\enums\\" : "C:\\programming\\sandbox\\LPP\\RapidConnect\\results\\entities\\";
                    string ns = fc.Type == "enum" ? "LPPRapidConnect.Enums" : "LPPRapidConnect.Entities";
                    writeToFile(indicator + result, fc.FileName, ns, baseFilePath);
                }
            }
            //string[] results = text.Split("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"xsd\", \"4.8.3928.0\")]");
            
            return new List<string>();
        }
        private static void writeToFile(string txt, string fileName, string ns, string baseFilePath)
        {
            string wrapper = @"using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace [NAMESPACE]
{
    [CONTENT HERE]
}";
            wrapper = wrapper.Replace("[NAMESPACE]", ns);
            wrapper = wrapper.Replace("[CONTENT HERE]", txt);
            File.WriteAllText(baseFilePath + fileName + ".cs", wrapper);
        }
        private static FileConfig getFileInfo(string txt)
        {
            FileConfig fc = new FileConfig();
            string[] words = txt.Split(" ");
            for(var i = 0; i< words.Length; i++)
            {
                if(words[i] == "class" || words[i] == "enum")
                {
                    fc.Type = words[i];
                    fc.FileName = words[i + 1];
                }
            }
            return fc;
        }
    }
}
