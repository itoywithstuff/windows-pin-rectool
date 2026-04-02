using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class PinGeneratorForm : Form
{
    private TextBox txtOutput;
    private NumericUpDown numLength;
    private NumericUpDown numInterval;
    private CheckBox chkSaveToFile;
    private Button btnToggle;
    private Timer generationTimer;
    private Random randomSource;
    private bool isRunning;
    private HashSet<string> generatedHistory;

    public PinGeneratorForm()
    {
        randomSource = new Random();
        isRunning = false;
        generatedHistory = new HashSet<string>();

        this.Text = "PIN Recovery Tool";
        this.Size = new Size(300, 350);

        Label lblOutput = new Label();
        lblOutput.Text = "Generated PIN:";
        lblOutput.Location = new Point(20, 20);
        this.Controls.Add(lblOutput);

        txtOutput = new TextBox();
        txtOutput.Location = new Point(20, 40);
        txtOutput.Width = 240;
        txtOutput.ReadOnly = true;
        this.Controls.Add(txtOutput);

        Label lblLength = new Label();
        lblLength.Text = "Character Length:";
        lblLength.Location = new Point(20, 80);
        this.Controls.Add(lblLength);

        numLength = new NumericUpDown();
        numLength.Location = new Point(20, 100);
        numLength.Minimum = 1;
        numLength.Maximum = 9;
        numLength.Value = 4;
        this.Controls.Add(numLength);

        Label lblTimer = new Label();
        lblTimer.Text = "Interval (ms):";
        lblTimer.Location = new Point(20, 140);
        this.Controls.Add(lblTimer);

        numInterval = new NumericUpDown();
        numInterval.Location = new Point(20, 160);
        numInterval.Minimum = 1;
        numInterval.Maximum = 10000;
        numInterval.Value = 1000;
        this.Controls.Add(numInterval);

        chkSaveToFile = new CheckBox();
        chkSaveToFile.Text = "Output to file (log.txt)";
        chkSaveToFile.Location = new Point(20, 200);
        chkSaveToFile.Width = 200;
        this.Controls.Add(chkSaveToFile);

        btnToggle = new Button();
        btnToggle.Text = "Start";
        btnToggle.Location = new Point(20, 240);
        btnToggle.Width = 240;
        btnToggle.Height = 40;
        btnToggle.Click += OnToggleButtonClick;
        this.Controls.Add(btnToggle);

        generationTimer = new Timer();
        generationTimer.Tick += OnTimerTick;
    }

    private void OnToggleButtonClick(object sender, EventArgs e)
    {
        if (isRunning)
        {
            generationTimer.Stop();
            btnToggle.Text = "Start";
            isRunning = false;
        }
        else
        {
            generationTimer.Interval = (int)numInterval.Value;
            generationTimer.Start();
            btnToggle.Text = "Stop";
            isRunning = true;
        }
    }

    private void OnTimerTick(object sender, EventArgs e)
    {
        int length = (int)numLength.Value;
        double maxCombinations = Math.Pow(10, length);

        if (generatedHistory.Count >= maxCombinations)
        {
            generationTimer.Stop();
            isRunning = false;
            btnToggle.Text = "Complete";
            MessageBox.Show("All possible combinations have been generated.");
            return;
        }

        string result = string.Empty;
        bool uniqueFound = false;

        while (!uniqueFound)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(randomSource.Next(0, 10).ToString());
            }
            string candidate = sb.ToString();
            if (!generatedHistory.Contains(candidate))
            {
                result = candidate;
                generatedHistory.Add(candidate);
                uniqueFound = true;
            }
        }

        txtOutput.Text = result;

        if (chkSaveToFile.Checked)
        {
            try
            {
                File.AppendAllText("log.txt", string.Format("{0}{1}", result, Environment.NewLine));
            }
            catch
            {
            }
        }
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new PinGeneratorForm());
    }
}