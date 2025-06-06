using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace ShutdownScheduler
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ShutdownForm());
        }
    }

    public class ShutdownForm : Form
    {
        private TextBox hoursTextBox;
        private Button submitButton;
        private Button cancelButton;
        private Label messageLabel;

        public ShutdownForm()
        {
            InitializeComponents();
            this.Icon = SystemIcons.Question; // Set the form's icon to the standard Windows question icon
        }

        private void InitializeComponents()
        {
            Text = "ShutdownTimer";
            Width = 400;
            Height = 200;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            // Label for instructions
            Label instructionLabel = new Label
            {
                Text = "Enter hours until shutdown (0 to cancel):",
                Location = new Point(20, 20),
                Width = 300
            };

            // TextBox for hours input
            hoursTextBox = new TextBox
            {
                Location = new Point(20, 50),
                Width = 150
            };

            // Submit button
            submitButton = new Button
            {
                Text = "Schedule Shutdown",
                Location = new Point(20, 80),
                Width = 120
            };
            submitButton.Click += SubmitButton_Click;

            // Cancel button
            cancelButton = new Button
            {
                Text = "Exit",
                Location = new Point(150, 80),
                Width = 120
            };
            cancelButton.Click += CancelButton_Click;

            // Label for messages
            messageLabel = new Label
            {
                Text = "",
                Location = new Point(20, 110),
                Width = 350,
                Height = 40
            };

            // Add controls to form
            Controls.Add(instructionLabel);
            Controls.Add(hoursTextBox);
            Controls.Add(submitButton);
            Controls.Add(cancelButton);
            Controls.Add(messageLabel);
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            // Cancel any existing shutdown timers
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "shutdown",
                    Arguments = "/a",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                    if (process.ExitCode == 0)
                        messageLabel.Text = "Any existing shutdown timers have been canceled.";
                    // Else, no shutdown was scheduled, so ignore
                }
            }
            catch
            {
                // Ignore errors if no shutdown was scheduled
            }

            // Validate input
            if (double.TryParse(hoursTextBox.Text, out double hours))
            {
                if (hours == 0)
                {
                    messageLabel.Text = "No new shutdown scheduled.";
                    hoursTextBox.Clear();
                    return;
                }
                else if (hours > 0)
                {
                    // Convert hours to seconds
                    int seconds = (int)(hours * 3600);

                    try
                    {
                        // Schedule shutdown
                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            FileName = "shutdown",
                            Arguments = $"/s /t {seconds}",
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };
                        using (Process process = Process.Start(psi))
                        {
                            process.WaitForExit();
                            if (process.ExitCode != 0)
                                throw new Exception("Failed to schedule shutdown. Ensure you have administrative privileges.");
                        }
                        messageLabel.Text = $"System will shut down in {hours} hour(s).";

                        // Ask for confirmation with shutdown duration
                        DialogResult result = MessageBox.Show(
                            $"System will shut down in {hours} hour(s). Is this correct?",
                            "Confirm Shutdown",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (result == DialogResult.No)
                        {
                            try
                            {
                                psi = new ProcessStartInfo
                                {
                                    FileName = "shutdown",
                                    Arguments = "/a",
                                    CreateNoWindow = true,
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true
                                };
                                using (Process process = Process.Start(psi))
                                {
                                    process.WaitForExit();
                                    if (process.ExitCode == 0)
                                        messageLabel.Text = "Shutdown canceled.";
                                    else
                                        throw new Exception("Failed to cancel shutdown.");
                                }
                            }
                            catch (Exception ex)
                            {
                                messageLabel.Text = $"Failed to cancel shutdown: {ex.Message}";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        messageLabel.Text = $"Failed to schedule shutdown: {ex.Message}";
                    }
                }
                else
                {
                    messageLabel.Text = "Please enter a non-negative number.";
                }
            }
            else
            {
                messageLabel.Text = "Invalid input. Please enter a valid number.";
            }

            hoursTextBox.Clear();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}