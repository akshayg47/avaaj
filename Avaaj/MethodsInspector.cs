using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Avaaj
{
    public class MethodsInspector
    {
        private static AssemblyDefinition _assembly = AssemblyDefinition.ReadAssembly(
        System.Reflection.Assembly.GetExecutingAssembly().Location);

        private string ContainingClassName;

        private string methodUnderTest;

        public MethodsInspector(string className, string methodTest)
        {
            ContainingClassName = className;
            methodUnderTest = methodTest;
        }

        public List<CandidatesModel> GetAllMethods()
        {
            var methods = GetMethodsCalled(GetMethod(methodUnderTest, ContainingClassName));
            var candidates = new List<CandidatesModel>();
            foreach (var method in methods)
            {
                if (method.IsPublic)
                {
                    var candidate = new CandidatesModel()
                    {
                        InterfaceName = method.DeclaringType.Name,
                        MethodName = method.Name,
                        IsSelected = false
                    };

                    candidates.Add(candidate);
                }
            }

            return candidates;
        }

        public ScaffoldingElements GetElementsForScaffolding(List<CandidatesModel> candidates)
        {
            var selectedMethods = candidates.Where(s => s.IsSelected.Equals(true)).ToList();
            var details = new ScaffoldingElements
            {
                ContainingClassName = ContainingClassName,
                MethodUnderTest = methodUnderTest
            };

            var namespaces = new HashSet<string>();
            var methods = GetMethodsCalled(GetMethod(methodUnderTest, ContainingClassName));
            methods = methods.Where(m => selectedMethods.Any(s => s.MethodName.ToLower().Equals(m.Name.ToLower())) && selectedMethods.Any(s => s.InterfaceName.ToLower().Equals(m.DeclaringType.Name.ToLower()))).ToList();
            var toBeArrangedMethods = new List<MethodEntityTobeArranged>();
            foreach (var method in methods)
            {
                if (method.IsPublic)
                {
                    var tobeArrangedEntity = new MethodEntityTobeArranged()
                    {
                        InterfaceName = method.DeclaringType.Name,
                        MethodName = method.Name,
                        ParameterTypes = new List<string>()
                    };

                    namespaces.Add(method.DeclaringType.Namespace);
                    foreach (var param in method.Parameters)
                    {
                        if (param.ParameterType.IsValueType)
                        {
                            tobeArrangedEntity.ParameterTypes.Add(param.ParameterType.Name);
                        }
                        else
                        {
                            namespaces.Add(param.ParameterType.Namespace);
                            tobeArrangedEntity.ParameterTypes.Add(EditParameterName(param.ParameterType.FullName, param.ParameterType.Namespace));
                        }
                    }

                    toBeArrangedMethods.Add(tobeArrangedEntity);
                }
            }

            details.NameSpacesToBeIncluded = namespaces.ToList();
            details.MethodsTobeArranged = toBeArrangedMethods;
            return details;
        }

        private string EditParameterName(string fullyQualifiedName, string correspondingNamespace)
        {
            var namespacePattern = @"\b" + correspondingNamespace + @"\.\b";
            foreach (Match match in Regex.Matches(fullyQualifiedName, namespacePattern, RegexOptions.IgnoreCase))
            {
                fullyQualifiedName = fullyQualifiedName.Replace(match.Value, string.Empty);
            }

            var escapePattern = @"\b\`\d+\b";

            foreach (Match match in Regex.Matches(fullyQualifiedName, escapePattern, RegexOptions.IgnoreCase))
            {
                fullyQualifiedName = fullyQualifiedName.Replace(match.Value, string.Empty);
            }

            return fullyQualifiedName;
        }

        private static IEnumerable<MethodDefinition> GetMethodsCalled(MethodDefinition caller)
        {
            return caller.Body.Instructions
                .Where(x => x.OpCode.Value.Equals(OpCodes.Call.Value) || x.OpCode.Value.Equals(OpCodes.Callvirt.Value))
                .Select(x => (MethodDefinition)x.Operand);
        }

        private static MethodDefinition GetMethod(string name, string documentName)
        {
            TypeDefinition programType = _assembly.MainModule.Types
                .FirstOrDefault(x => x.Name == documentName);
            return programType.Methods.First(x => x.Name == name);
        }
    }
}
