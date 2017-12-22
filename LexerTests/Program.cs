﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LexerTests
{
    class Program
    {
        static int Main(string[] args)
        {

            //--------------------------------------------------------------
            // Add initialization of any class from each one of projects 
            // containing test classes and test methods you wish to execute.
            Lexer.Implementation.Tests.RegexLexerTests regexLexerTests = new Lexer.Implementation.Tests.RegexLexerTests();

            //--------------------------------------------------------------

            #region Execute tests
            TestReflectionHelper trh = new TestReflectionHelper();
            bool isOk = trh.RunAllTests();
            if (isOk)
                return 0; // ok
            else
                return 1; // tests failed
            #endregion Execute tests
        }

        /// <summary>
        /// Runs all the test methods in all test classes in loaded assemblies
        /// </summary>
        public class TestReflectionHelper
        {
            /// <summary>
            /// Run all the test methods in all test classes in loaded assemblies
            /// </summary>
            /// <returns>true if all tests succeed, otherwise false</returns>
            public bool RunAllTests()
            {
                int testProjectsCount = 0;
                int testClassesCount = 0;
                int testMethodsCount = 0;
                int testMethodsCountPassed = 0;
                // Get all loaded assemblies
                foreach (Assembly assm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    bool isAssemblyNamePrinted = false;

                    // Get all types inside the assembly
                    Type[] types = assm.GetTypes();
                    foreach (Type tp in types)
                    {
                        // check that the type is decorated with "TestClass" attribute
                        bool isTestClass = false;
                        object[] attributes = tp.GetCustomAttributes(false);
                        foreach (object attr in attributes)
                        {
                            if (attr.GetType().Name == "TestClassAttribute")
                            {
                                isTestClass = true;
                                break;
                            }
                        }

                        // Process the test class
                        if (isTestClass)
                        {
                            if (!isAssemblyNamePrinted)
                            {
                                Console.WriteLine("Assembly " + assm.GetName().Name);
                                testProjectsCount++;
                                isAssemblyNamePrinted = true;
                            }
                            testClassesCount++;
                            Console.WriteLine("  Test Class " + tp.Name);

                            // instantiate the class
                            object testClassInstance = null;
                            try
                            {
                                testClassInstance = Activator.CreateInstance(tp);
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Failed to create instance of test class {0}: {1}", tp.Name, ex.ToString());
                                Console.ForegroundColor = ConsoleColor.Gray;
                                continue;
                            }

                            // Get all methods
                            MethodInfo[] methods = tp.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                            foreach (MethodInfo mi in methods)
                            {
                                // check method attributes - is it "TestMethod"
                                bool isTestMethod = false;
                                object[] mattributes = mi.GetCustomAttributes(false);
                                foreach (object attr in mattributes)
                                {
                                    if (attr.GetType().Name == "TestMethodAttribute")
                                    {
                                        isTestMethod = true;
                                        break;
                                    }
                                }

                                // Run test method
                                if (isTestMethod)
                                {
                                    Console.Write("    " + mi.Name);
                                    testMethodsCount++;
                                    try
                                    {
                                        // Invoke
                                        mi.Invoke(testClassInstance, null);

                                        // Success
                                        testMethodsCountPassed++;
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine(" Ok");
                                        Console.ForegroundColor = ConsoleColor.Gray;
                                    }
                                    catch (Exception ex)
                                    {
                                        // Failure
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine(" Failed");
                                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                        Console.WriteLine("{0}", ex.InnerException.Message);
                                        Console.ForegroundColor = ConsoleColor.Gray;
                                    }
                                }

                            }
                        }
                    }
                }

                // Summary
                Console.WriteLine();
                Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
                Console.WriteLine("{0} test projects, {1} classes processed.", testProjectsCount, testClassesCount);
                if (testMethodsCount != testMethodsCountPassed)
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0} tests executed, {1} passed, {2} failed.", testMethodsCount, testMethodsCountPassed, testMethodsCount - testMethodsCountPassed);
                Console.ForegroundColor = ConsoleColor.Gray;

                return (testMethodsCount == testMethodsCountPassed);
            }
        }
    }
}
