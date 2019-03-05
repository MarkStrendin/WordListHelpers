using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace FindDuplicates
{
    class Program
    {
        static void Main(string[] args)
        {
            // Args:
            //  source filename
            string sourceFilename = string.Empty;

            try 
            {
                if (!string.IsNullOrEmpty(args[0])) {
                    sourceFilename = args[0].Trim();
                }
            } catch {
                SendSyntax();
                return;
            }

            if (string.IsNullOrEmpty(sourceFilename)) {
                SendSyntax();
                return;                
            }

            // Attempt to load the source file
            if (!File.Exists(sourceFilename)) {
                SendText("Source file does not exist!");
                return;
            }

            try {
                // Open the source file
                using (FileStream sourceFile = File.OpenRead(sourceFilename)) {
                    using (StreamReader sourceFileReader = new StreamReader(sourceFile)) {
                        Dictionary<string, List<int>> linesWithLocations = new Dictionary<string, List<int>>();

                        int sourceLineNumber = 0;
                        int emptyLines = 0;
                        string sourceLine;
                        while ((sourceLine = sourceFileReader.ReadLine()) != null) {
                            sourceLineNumber++;
                            
                            string parsedLine = sourceLine.Trim();

                            // Skip empty lines
                            if (string.IsNullOrEmpty(parsedLine)) {
                                emptyLines++;
                                continue;
                            }

                            // Catalog every line number
                            if (!linesWithLocations.ContainsKey(parsedLine)) {
                                linesWithLocations.Add(parsedLine, new List<int>());
                            }
                            linesWithLocations[parsedLine].Add(sourceLineNumber);

                        }                        

                        // Now go through the tree of a dictionary and find anything with more than one line mentioned
                        int foundDuplicates = linesWithLocations.Count(x => x.Value.Count > 1);                        
                        SendText("Found " + foundDuplicates + " duplicates");
                        if (emptyLines > 0) {
                            SendText("Skipped " + emptyLines + " empty lines.");
                        }
                        if (foundDuplicates > 0) {
                            foreach(KeyValuePair<string, List<int>> kvp in linesWithLocations.Where(x => x.Value.Count() > 1)) {                                
                                SendText("\"" + kvp.Key + "\" found " + kvp.Value.Count + " times, on lines: " + kvp.Value.ToCommaSeparatedString());
                            }
                        }
                    }                                    
                }                
            } catch(Exception ex) {
                SendText("ERROR: " + ex.Message);
            }
        }

        private static void SendSyntax() {
            Console.WriteLine("Syntax: <source filename>");
        }

        private static void SendText(string msg) {
            Console.WriteLine(msg);
        }
    }
}
