using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace bchunkGUIWin;

public partial class Form1 : Form
{
    private Label binLabel;
    private Label cueLabel;
    private Label progressLabel;
    private string binPath = "";
    private string cuePath = "";

    public Form1()
    {
        InitializeComponent();

        this.Text = "bChunkGUIWin v1.0 (x86)";

        Button openBinButton = new Button();
        openBinButton.Text = "Open \".bin\" file";
        openBinButton.Location = new Point(50, 50);
        openBinButton.AutoSize = true;
        openBinButton.Click += ClickOpenBinButton;
        this.Controls.Add(openBinButton);

        binLabel = new Label();
        binLabel.Text = "bin: [Not Selected]";
        binLabel.Location = new Point(50, 200);
        binLabel.Font = new Font("Calibri", 16, FontStyle.Bold);
        binLabel.AutoSize = true;
        this.Controls.Add(binLabel);

        Button openCueButton = new Button();
        openCueButton.Text = "Open \".cue\" file";
        openCueButton.Location = new Point(500, 50);
        openCueButton.AutoSize = true;
        openCueButton.Click += ClickOpenCueButton;
        this.Controls.Add(openCueButton);

        cueLabel = new Label();
        cueLabel.Text = "cue: [Not Selected]";
        cueLabel.Location = new Point(50, 250);
        cueLabel.Font = new Font("Calibri", 16, FontStyle.Bold);
        cueLabel.AutoSize = true;
        this.Controls.Add(cueLabel);

        Button convertButton = new Button();
        convertButton.Text = "Convert to \".iso\" file";
        convertButton.Location = new Point(250, 300);
        convertButton.AutoSize = true;
        convertButton.Click += ClickConvertButton;
        this.Controls.Add(convertButton);

        progressLabel = new Label();
        progressLabel.Text = "";
        progressLabel.Location = new Point(250, 350);
        progressLabel.Font = new Font("Calibri", 16, FontStyle.Bold);
        progressLabel.AutoSize = true;
        this.Controls.Add(progressLabel);
    }

    private void ClickOpenBinButton(object? sender, EventArgs e)
    {
        OpenDialog("bin");
    }

    private void ClickOpenCueButton(object? sender, EventArgs e)
    {
        OpenDialog("cue");
    }

    private void OpenDialog(string ext)
    {
        using (OpenFileDialog fileDialog = new OpenFileDialog())
        {
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            
            if (ext == "bin")
            {
                fileDialog.Filter = "Bin files (*.bin)|*.bin";
            }
            else if (ext == "cue")
            {
                fileDialog.Filter = "Cue files (*.cue)|*.cue";
            }

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = "";
                if (ext == "bin")
                {
                    binPath = fileDialog.FileName;
                    filename = Path.GetFileName(binPath);
                    binLabel.Text = $"bin: {filename}";
                }
                else if (ext == "cue")
                {
                    cuePath = fileDialog.FileName;
                    filename = Path.GetFileName(cuePath);
                    cueLabel.Text = $"cue: {filename}";
                }
                
                MessageBox.Show($"You selected: {filename}", "File Loaded");
            }
        }
    }

    private void ClickConvertButton(object? sender, EventArgs e)
    {
        progressLabel.Text = "Converting. . .";
        progressLabel.Refresh();

        if (binPath == "" || cuePath == "")
        {
            MessageBox.Show("Unable to convert since at least one file was not provided!");
            return;
        }

        RunBchunk();
    }

    private void RunBchunk()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();

        startInfo.FileName = "bchunk.exe";
        startInfo.Arguments = $"\"{binPath}\" \"{cuePath}\" \"{binPath[..^4]}\"";

        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        try
        {
            using (Process? process = Process.Start(startInfo))
            {
                if (process != null)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        progressLabel.Text = "Success!";
                        MessageBox.Show("Conversion success!", "Success!");
                    }
                    else
                    {
                        progressLabel.Text = "Failure!";
                        MessageBox.Show("Failure with exit code {process.ExitCode} and Error: {error}", "Error");
                    }
                }
            }
        }
        catch (Exception e)
        {
            progressLabel.Text = "Failure!";
            MessageBox.Show($"Sorry the conversion failed hard because: {e.Message}", "Execution Error");
        }
    }
}

