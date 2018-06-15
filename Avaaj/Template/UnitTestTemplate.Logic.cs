using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Avaaj.Template
{
    public partial class UnitTestTemplate
    {
        private readonly string _projectName;
        private readonly string _projectFolderName;

        private readonly ScaffoldingElements _classProperties;

        private List<string> _methodParameters;

        private static List<string> valueTypes = new List<string> { "bool",
                                                                    "byte",
                                                                    "char",
                                                                    "decimal",
                                                                    "double",
                                                                    "enum",
                                                                    "float",
                                                                    "int",
                                                                    "long",
                                                                    "sbyte",
                                                                    "short",
                                                                    "struct",
                                                                    "uint",
                                                                    "ulong",
                                                                    "ushort",
                                                                    "string"
                                                                  };

        public string TestClassName { get; set; }
        public string ContainingNamespace { get; set; }
        public string ClassUnderTest { get; set; }

        public UnitTestTemplate(ScaffoldingElements properties)
        {
            _projectName = "WhiteNoise.BusinessImpl.Test";
            _projectFolderName= "WhiteNoise.Business.Test";
            _methodParameters = new List<string>();
            _classProperties = properties;
            ClassUnderTest = _classProperties.ContainingClassName;
            ContainingNamespace = _projectName;
            TestClassName = $"{ClassUnderTest}Test";
        }

        public string WriteUsingStatements()
        {
            var sb = new StringBuilder();
            foreach (var usingNamespace in _classProperties.NameSpacesToBeIncluded)
            {
                sb.AppendFormat("\tusing {0};\n", usingNamespace);
            }
            return sb.ToString();
        }

        public string WriteMethodTest()
        {
            var tabs = AddTabs(2);
            var tabs3 = AddTabs(3);
            var tabs4 = AddTabs(4);

            var sb = new StringBuilder();
            var method = _classProperties.MethodUnderTest;

            sb.AppendLine(WriteMethodSummary(method.MethodName));
            sb.AppendFormat("{0}[Fact]\n", tabs);
            sb.AppendFormat("{0}public void {1}_Success()\n", tabs, method.MethodName);
            sb.AppendFormat("{0}", tabs);
            sb.Append("{\n");
            sb.AppendFormat("{0}using (var container = CreateMockingContainer())\n", tabs3);
            sb.AppendFormat("{0}", tabs3);
            sb.Append("{\n");
            sb.AppendFormat("{0}//Arrange\n", tabs4);
            sb.AppendLine(WriteTypeDeclarations(method.ParameterTypes));
            sb.AppendLine(WriteArrangeStatements(method.MethodsToBeArranged));

            sb.AppendFormat("{0}//Act\n", tabs4);
            sb.AppendLine(WriteTestMethodCall(method.MethodName));

            sb.AppendFormat("{0}//Assert\n", tabs4);
            sb.AppendFormat("{0}container.AssertAll();\n", tabs4);
            sb.AppendFormat("{0}", tabs3);
            sb.Append("}\n");
            sb.AppendFormat("{0}", tabs);
            sb.Append("}\n");


            return sb.ToString();
        }

        public string WriteMethodSummary(string methodName)
        {
            var tabs = AddTabs(2);
            var sb = new StringBuilder();
            sb.AppendFormat("{0}/// <summary>\n", tabs);
            sb.AppendFormat("{0}/// Test for {1} Success\n", tabs, methodName);
            sb.AppendFormat("{0}/// </summary>", tabs);
            return sb.ToString();
        }

        public string WriteTypeDeclarations(List<string> parameterTypes)
        {
            int count = 1;
            var tabs = AddTabs(4);
            var sb = new StringBuilder();
            foreach (var type in parameterTypes)
            {
                var isValueType = valueTypes.Contains(type);
                var variableName = $"v{type}{count++}";
                if (isValueType)
                {
                    sb.AppendFormat("{0}{1} {2};\n", tabs, type, variableName);
                }
                else
                {
                    sb.AppendFormat("{0}{1} {2} = new {3}();\n", tabs, "var", variableName, type);
                }

                _methodParameters.Add(variableName);
            }

            return sb.ToString();
        }

        public string WriteArrangeStatements(List<MethodEntityToBeArranged> calls)
        {
            var tabs = AddTabs(4);
            var sb = new StringBuilder();
            foreach (var method in calls)
            {
                var variableName = $"{method.InterfaceName[1].ToString().ToLower()}{method.InterfaceName.Substring(2)}";
                var parameters = GetMethodParameters(method.ParameterTypes);
                sb.AppendFormat(
                    "{0}container.Arrange<{1}>({2} => {2}.{3}({4})).DoNothing();\n",
                    tabs,
                    method.InterfaceName,
                    variableName,
                    method.MethodName,
                    parameters);
            }

            return sb.ToString();
        }

        public void WriteFile(string solutionFilePath)
        {
            var projectFileLocation = $"{solutionFilePath}\\{_projectFolderName}";
            string fileName = $"{ClassUnderTest}Test.cs";
            var p = new Microsoft.Build.Evaluation.Project($"{projectFileLocation}\\{_projectName}.csproj");


            p.AddItem("Compile", $"{projectFileLocation}\\{fileName}");
            p.Save();

            string filePath = Path.Combine(projectFileLocation, fileName);
            if (File.Exists(filePath))
            {
                var sb = AppendMethodToFile(filePath);
                File.WriteAllText(filePath, sb.ToString());
            }
            else
            {
                var pageContent = TransformText();
                File.WriteAllText(filePath, pageContent);
            }
        }

        private string WriteTestMethodCall(string methodName)
        {
            var tabs = AddTabs(4);
            var sb = new StringBuilder();
            sb.AppendFormat("{0}container.Instance.{1}({2});\n", tabs, methodName, string.Join(", ", _methodParameters));
            return sb.ToString();
        }

        private StringBuilder AppendMethodToFile(string filePath)
        {
            var searchKey = "/// <summary>";
            var textLines = File.ReadAllLines(filePath).ToList();
            var insertIndex = textLines.FindLastIndex(line => line.Contains(searchKey));
            var sb = new StringBuilder();
            for (int l = 0; l < insertIndex; l++)
            {
                sb.AppendFormat("{0}\n", textLines[l]);
            }

            sb.AppendFormat("{0}\n", WriteMethodTest());

            for (int l = insertIndex; l < textLines.Count; l++)
            {
                sb.AppendFormat("{0}\n", textLines[l]);
            }

            return sb;
        }

        private string GetMethodParameters(List<string> parameterTypes)
        {
            var sb = new StringBuilder();
            var arguments = parameterTypes.Select(p => $"Arg.IsAny<{p}>()").ToList();
            sb.Append(string.Join(", ", arguments));
            return sb.ToString();
        }

        private string AddTabs(int numOfTabs)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < numOfTabs; i++)
            {
                sb.Append("\t");
            }

            return sb.ToString();
        }

    }
}