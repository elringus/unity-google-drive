#if ENABLE_VSTU

using SyntaxTree.VisualStudio.Unity.Bridge;
using System.IO;
using System.Text;
using System.Xml.Linq;
using UnityEditor;
using System.Linq;

[InitializeOnLoad]
public class ProjectFileHook
{
    private static readonly XNamespace defaultNamespace = @"http://schemas.microsoft.com/developer/msbuild/2003";

    private class Utf8StringWriter : StringWriter
    {
        // Necessary for XLinq to save the xml project file in UTF-8 encoding.
        public override Encoding Encoding => Encoding.UTF8;
    }

    static ProjectFileHook ()
    {
        ProjectFilesGenerator.ProjectFileGeneration += (string name, string content) => {
            var document = XDocument.Parse(content);
            
            var propGroupElement = new XElement(defaultNamespace + "PropertyGroup");
            var docElement = new XElement(defaultNamespace + "DocumentationFile", "Assembly-CSharp.xml");
            propGroupElement.Add(docElement);
            document.Root.AddFirst(propGroupElement);

            var noWarnElements = document.Root.Elements().SelectMany(e => e.Elements().Where(ep => ep.Name.LocalName == "NoWarn"));
            foreach (var element in noWarnElements)
                element.SetValue(element.Value.ToString() + ";1591");

            var str = new Utf8StringWriter();
            document.Save(str);

            return str.ToString();
        };
    }
}

#endif
