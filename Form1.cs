using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_Handler_Form
{
    public partial class Form1 : Form
    {

        string sourcePath;
        string destinationPath;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select the source folder";
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                sourcePath = fbd.SelectedPath;
                textBox1.Text = sourcePath;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select the destination folder";
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                destinationPath = fbd.SelectedPath;
                textBox2.Text = destinationPath;
            }
        }

        

        private const string File_Name = "FileHandlerLog.txt";

        public void FileHandler() {

            appendOutput("This app let you handle large amount of music files in bulk ordered by Artist's name..");                      
            //appendOutput("For greater efficiency of the program is recommended to have files");
            //appendOutput("named like: Artist - song name.mp3");
            appendOutput("FileHandler_Form version. 1.01 By Sam Ds \n");

            if (Directory.Exists(sourcePath))
            {
                appendOutput("Source path found.");

                if (Directory.Exists(destinationPath))
                {
                    appendOutput("Destination path found.");

                    appendOutput("Creating Log File at Destination.");
                    appendOutput("Wait we are making your life easy... \n");

                    string filePath = Path.Combine(destinationPath, File_Name);

                    if (File.Exists(File_Name))
                    {
                        appendOutput("Log found, deleting old one.");

                        File.Delete(filePath);

                    }
                    else
                    {

                        // Get source dir's files
                        string[] sourceFiles = Directory.GetFiles(sourcePath);

                        int percentageIndexer = 1;
                        int percentage = sourceFiles.Length;
                        foreach (string sourceFile in sourceFiles)
                        {

                            // Get destination dir's subdirs
                            string[] destinationSubDirs = Directory.GetDirectories(destinationPath);


                            ArrayList subDirList = DestinationSubDirNames(destinationSubDirs);
                            string fileToHandle = Path.GetFileName(sourceFile);
                            string fileArtistName = NameSplitter(sourceFile);

                            int folderIndexer = subDirList.IndexOf((object)fileArtistName);
                           
                            appendOutput("Currently handling file:");
                            appendOutput(fileToHandle);
                            

                            if (folderIndexer != -1)
                            {
                                //Get Artist folder's files
                                string dArtistPath = Path.Combine(destinationPath, fileArtistName);
                                int subFolderIndexer = Array.IndexOf(destinationSubDirs, dArtistPath);
                                string[] dArtistDirFiles = Directory.GetFiles(destinationSubDirs[subFolderIndexer]);
                                string dArtistFileFullPath = Path.Combine(destinationSubDirs[subFolderIndexer], fileToHandle);
                                int subDirFileChecker = Array.IndexOf(dArtistDirFiles, dArtistFileFullPath);

                                if (subDirFileChecker == -1)
                                {
                                    //appendOutput("File: {0} not found in subdir.", fileToHandle);
                                    CopyingFile(dArtistFileFullPath, sourceFile, fileToHandle);
                                    appendOutput("COPYING - File not found in subdir.");

                                }
                                else
                                {
                                    appendOutput("SKIPPING - File found in folder.");
                                }

                            }
                            else
                            {
                                //appendOutput("Folder not found, Artist: {0}.", fileArtistName);
                                string dirToCreate = Path.Combine(destinationPath, fileArtistName);

                                Directory.CreateDirectory(dirToCreate);
                                if (Directory.Exists(dirToCreate))
                                {/*
                                    appendOutput("Directory created:");
                                    appendOutput(fileArtistName);
                                    appendOutput(fileToHandle);
                                    appendOutput("File Copied");
                                    appendOutput("\n");
                                   */
                                    string newDirFileFullPath = Path.Combine(dirToCreate, fileToHandle);
                                    CopyingFile(newDirFileFullPath, sourceFile, fileToHandle);
                                }
                                else
                                {
                                    appendOutput("Error While creating folder:");
                                    appendOutput(fileArtistName);
                                }

                            }
                            //Percentage display
                            int percentageOfTask = (percentageIndexer * 100) / percentage;
                            /*
                            Console.SetCursorPosition(0, 20);
                            appendOutput("\r\n Completed: {0}%   ", percentageOfTask);
                            */
                            percentageIndexer++;
                        }
                    }
                }
                else
                {
                    appendOutput("Destination directory doesn't exists!");
                }

            }
            else
            {
                appendOutput("Source directory doesn't exists!");
            }

            Console.ReadLine();
        } //End of Main funtion (File Handler)




        private void appendOutput(String msg)
        {
            richTextBox1.AppendText(msg + "\n");
        }

        private void appendError(String msg, bool clearPrior)
        {
            if (clearPrior)
            {
                richTextBox1.Clear();
            }

            richTextBox1.SelectionColor = Color.Red;
            richTextBox1.SelectedText = msg + "\r\n";
        }


        // Make an array of destination subdirectories
        static ArrayList DestinationSubDirNames(string[] dPath)
        {

            ArrayList subDirList = new ArrayList();

            foreach (string subdir in dPath)
            {

                string subDirName = Path.GetFileName(subdir);
                subDirList.Add(subDirName);

            }

            return subDirList;
        }

        // Split source file name to find out the artist
        static string NameSplitter(string sourceFile)
        {
            string sourceFileName = Path.GetFileName(sourceFile);
            string sourceFileNameNoExtend = Path.GetFileNameWithoutExtension(sourceFile);

            string[] spliter1 = { "Feat.", "Feat", "feat.", "Feat.", "Ft.", " - ","-"," -","ft."};

            string[] splitedFileNames = sourceFileNameNoExtend.Split(spliter1, StringSplitOptions.None);

            string splitedFile = splitedFileNames[0];

            string[] spliter2 = { " vs ", "vs.", "Vs.", " Vs ", "&", "(", "," };
            string[] splitedFileArtists = splitedFile.Split(spliter2, StringSplitOptions.None);

            char[] charsToTrim = { ' ', '-', '.' };
            string fileArtist = splitedFileArtists[0].Trim(charsToTrim);

            return fileArtist;
        }

        static void CopyingFile(string dArtistFileFullPath, string sourceFile, string fileToHandle)
        {
            string dPathToHandle = dArtistFileFullPath;
            string sPathToHandle = sourceFile;

            try
            {

                // Copy the file.
                File.Copy(sPathToHandle, dPathToHandle);

                File.Copy(sPathToHandle, dPathToHandle, true);
                //appendOutput("Copy operation succeeded: {0}",fileToHandle);
            }

            catch
            {
               // appendOutput("Error while coping: {0}", fileToHandle);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            FileHandler();
        }
    }
}
