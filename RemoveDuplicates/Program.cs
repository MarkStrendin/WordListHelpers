using System;
using System.IO;
using System.Collections.Generic;

namespace RemoveDuplicates
{
    class Program
    {
        static void Main(string[] args)
        {
            // Args:
            //  source filename
            //  dest filename
            string sourceFilename = string.Empty;
            string destinationFilename = string.Empty;

            try 
            {
                if (!string.IsNullOrEmpty(args[0])) {
                    sourceFilename = args[0].Trim();
                }

                if (!string.IsNullOrEmpty(args[1])) {
                    destinationFilename = args[1].Trim();
                }
            } catch {
                SendSyntax();
                return;
            }

            if (
                (string.IsNullOrEmpty(sourceFilename)) || 
                (string.IsNullOrEmpty(destinationFilename))
            ) {
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
                        
                        // Iterate through the lines of the file
                        List<string> rawLines = new List<string>();
                        List<string> uniqueParsedLines = new List<string>();
                        List<int> dupeLineNumbers = new List<int>();

                        int sourceLineNumber = 0;
                        int emptylines = 0;
                        string sourceLine;
                        while ((sourceLine = sourceFileReader.ReadLine()) != null) {
                            sourceLineNumber++;

                            // Parse the line, to avoid issues caused by whitespace, etc...
                            string parsedLine = sourceLine.Trim();

                            // Skip empty lines
                            if (string.IsNullOrEmpty(parsedLine)) {
                                emptylines++;
                                continue;
                            }

                            if (uniqueParsedLines.Contains(parsedLine)) {
                                dupeLineNumbers.Add(sourceLineNumber);
                            } else {
                                uniqueParsedLines.Add(parsedLine);
                                rawLines.Add(sourceLine);
                            }
                        }

                        // If have have lines to write, start writing the destination file
                        if (rawLines.Count > 0) {
                            SendText("Parsed " + sourceLineNumber + " lines.");
                            SendText("Found and removed " + dupeLineNumbers.Count + " duplicates.");
                            if (emptylines > 0) {
                                SendText("Skipped " + emptylines + " empty lines");
                            }                                                        
                            SendText("Writing destination file...");

                            // Delete the destination file if it exists already
                            if (File.Exists(destinationFilename)) {
                                SendText("WARNING: Moving old destination file!");
                                File.Move(destinationFilename, destinationFilename + "." + DateTime.Now.ToString("yyyyMMddHHmmss"));
                            }

                            // Write to the destination file
                            using (FileStream destinationFile = File.Create(destinationFilename)) {
                                using (StreamWriter destinationFileWriter = new StreamWriter(destinationFile)) {
                                    int writtenLines = 0;
                                    foreach(string destLine in rawLines) {
                                        writtenLines++;
                                        if (writtenLines < rawLines.Count) {
                                            destinationFileWriter.WriteLine(destLine);
                                        } else {
                                            destinationFileWriter.Write(destLine);
                                        }
                                    }
                                    destinationFileWriter.Close();
                                    destinationFile.Close();
                                }
                            }    
                            SendText("Finished writing destination file: " + destinationFilename);
                        } else {
                            SendText("Source file (" + sourceFilename +  ") was empty or unparsable!");
                        }
                    }                                    
                }                
            } catch(Exception ex) {
                SendText("ERROR: " + ex.Message);
            }
        }

        private static void SendSyntax() {
            Console.WriteLine("Syntax: <source filename> <destination filename>");
        }

        private static void SendText(string msg) {
            Console.WriteLine(msg);
        }
    }
}
