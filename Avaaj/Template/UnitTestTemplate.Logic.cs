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
        private readonly ScaffoldingElements _classProperties;

        public string ContainingClassName { get; set; }
        public string TestClassName { get; set; }
        public string ContainingNamespace { get; set; }
        public string ClassUnderTest { get; set; }

        public UnitTestTemplate(ScaffoldingElements properties)
        {
            _classProperties = properties;
            ClassUnderTest = _classProperties.ContainingClassName;
            ContainingNamespace = $"{_classProperties.ContainingClassName}.Test";
            ContainingClassName = _classProperties.ContainingClassName;
            TestClassName = $"{_classProperties.ContainingClassName}Test";
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
            var sb = new StringBuilder();
            var method = _classProperties.MethodUnderTest;
            
            sb.AppendLine(WriteMethodSummary(method.MethodName));
            sb.AppendFormat("{0}[Fact]\n", tabs);
            sb.AppendFormat("{0}public void {1}_Success()\n", tabs, method.MethodName);
            sb.AppendFormat("{0}",tabs);
            sb.Append("{\n");
            sb.AppendFormat("{0}using (var container = CreateMockingContainer())\n", tabs);
            sb.AppendFormat("{0}", tabs);
            sb.Append("{\n");
            sb.AppendFormat("{0}\t//Arrange\n", tabs);
            sb.AppendLine(WriteArrangeStatements(method.MethodsToBeArranged));                

            sb.AppendFormat("{0}\t//Act\n", tabs);
            sb.AppendFormat("{0}\tcontainer.Instance.{1}(a);\n\n", tabs, method.MethodName);

            sb.AppendFormat("{0}\t//Assert\n", tabs);
            sb.AppendFormat("{0}\tcontainer.AssertAll();\n", tabs);
            sb.AppendFormat("{0}", tabs);
            sb.Append("}\n");
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

        public string WriteArrangeStatements(List<MethodEntityToBeArranged> calls)
        {
            var sb = new StringBuilder();
            foreach (var method in calls)
            {
                var variableName = $"{method.InterfaceName[1].ToString().ToLower()}{method.InterfaceName.Substring(2)}";
                var parameters = GetMethodParameters(method.ParameterTypes);
                sb.AppendFormat(
                    "\t\t\tcontainer.Arrange<{0}>({1} => {1}.{2}({3})).DoNothing();\n",
                    method.InterfaceName,
                    variableName,
                    method.MethodName,
                    parameters);
            }

            return sb.ToString();
        }

        public void WriteFile()
        {
            var pageContent = TransformText();
            string fileName = $"{ContainingClassName}Test.cs";
            string directory = Path.Combine(Directory.GetCurrentDirectory(), "Output");
            
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string filePath = Path.Combine(directory, fileName);
            if (File.Exists(filePath))
            {
                var searchKey = "/// <summary>";
                var textLines = File.ReadAllLines(filePath);
                textLines.LastOrDefault(line => line.StartsWith(searchKey));

            }
            else
            {
                File.WriteAllText(filePath, pageContent);
            }           
        }

        private string GetMethodParameters(List<string> parameterTypes)
        {
            var sb = new StringBuilder();
            var arguments = parameterTypes.Select(p => $"Arg.IsAny<{p}>()").ToList();
            sb.Append(string.Join(",", arguments));
            return sb.ToString();
        }

        private string AddTabs(int numOfTabs)
        {
            var sb = new StringBuilder();
            for(int i=0; i<numOfTabs; i++)
            {
                sb.Append("\t");
            }

            return sb.ToString();
        }
      
    }
}
