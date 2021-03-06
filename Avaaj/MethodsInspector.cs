﻿using Avaaj.Template;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Avaaj
{
    public class MethodsInspector
    {
        Assembly SampleAssembly;
        private static AssemblyDefinition _assembly;
        private string ContainingClassName;
        private string methodUnderTest;
        public string SolutionFilePath { get; set; }
        
        public MethodsInspector(string className, string methodTest, string dllPath)
        {
            ContainingClassName = className;
            methodUnderTest = methodTest;
            SampleAssembly = Assembly.LoadFrom(dllPath);
            _assembly = AssemblyDefinition.ReadAssembly(SampleAssembly.Location);
        }
        
        public List<CandidatesModel> GetAllMethods()
        {
            var methods = GetMethodsCalled(GetMethod(methodUnderTest, ContainingClassName));
            var candidates = new List<CandidatesModel>();

            var listOfInterfaces = GetConstructorInjections(ContainingClassName);

            foreach (var method in methods)
            {
                if (!method.IsDefinition)
                {
                    if (listOfInterfaces.Contains(method.DeclaringType.Name))
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
            }

            return candidates;
        }

        public ScaffoldingElements GetElementsForScaffolding(List<CandidatesModel> candidates)
        {
            var selectedMethods = candidates;
            var details = new ScaffoldingElements
            {
                ContainingClassName = GetClassName(ContainingClassName),
                MethodUnderTest = new MethodUnderTest { MethodName = methodUnderTest, ParameterTypes = new List<string>() }
            };

            var namespaces = new HashSet<string>();
            var methodDefinition = GetMethod(methodUnderTest, ContainingClassName);
            foreach (var param in methodDefinition.Parameters)
            {
                if (param.ParameterType.IsValueType)
                {
                    details.MethodUnderTest.ParameterTypes.Add(param.ParameterType.Name);
                }
                else
                {
                    namespaces.Add(param.ParameterType.Namespace);
                    details.MethodUnderTest.ParameterTypes.Add(EditParameterName(param.ParameterType.FullName, param.ParameterType.Namespace));
                }
            }

            var methods = GetMethodsCalled(methodDefinition);
            methods = methods.Where(m => selectedMethods.Any(s => s.MethodName.ToLower().Equals(m.Name.ToLower())) && selectedMethods.Any(s => s.InterfaceName.ToLower().Equals(m.DeclaringType.Name.ToLower()))).ToList();
            var toBeArrangedMethods = new List<MethodEntityToBeArranged>();
            foreach (var method in methods)
            {
                var tobeArrangedEntity = new MethodEntityToBeArranged()
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

            details.NameSpacesToBeIncluded = namespaces.ToList();
            details.MethodUnderTest.MethodsToBeArranged = toBeArrangedMethods;
            UnitTestTemplate unitTestTemplate = new UnitTestTemplate(details);
            unitTestTemplate.WriteFile(SolutionFilePath);
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

        private static IEnumerable<MethodReference> GetMethodsCalled(
    MethodDefinition caller)
        {
            var s = caller.Body.Instructions
                .Where(x => x.OpCode.Value.Equals(OpCodes.Call.Value) || x.OpCode.Value.Equals(OpCodes.Callvirt.Value)).ToList();
            //.Select(x => (MethodDefinition)x.Operand);
            return s.Select(x => (MethodReference)x.Operand);
        }

        private static MethodDefinition GetMethod(string name, string documentName)
        {
            TypeDefinition programType = _assembly.MainModule.Types
                .FirstOrDefault(x => x.Name.Equals(documentName, StringComparison.OrdinalIgnoreCase));
            return programType.Methods.First(x => x.Name == name);
        }

        private static List<string> GetConstructorInjections(string documentName)
        {
            try
            {
                TypeDefinition programType = _assembly.MainModule.Types
                .FirstOrDefault(x => x.Name.Equals(documentName, StringComparison.OrdinalIgnoreCase));
                return programType.Methods.Where(x => x.IsConstructor)
                    .OrderByDescending(x => x.Parameters.Count)
                    .First().Parameters.Select(x => x.ParameterType.Name).ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        private static string GetClassName(string documentName)
        {
            TypeDefinition programType = _assembly.MainModule.Types
                .FirstOrDefault(x => x.Name.Equals(documentName, StringComparison.OrdinalIgnoreCase));
            return programType?.Name.ToString();
        }
    }
}