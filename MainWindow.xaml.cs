using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AffinityMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private List<AffinityPanel> _affinityPanels = new List<AffinityPanel>();        // List of active panels
        private ObjectPool<AffinityPanel> _panelPool = new ObjectPool<AffinityPanel>(); // Pool of inactive panels

        private void button_Search_Click(object sender, RoutedEventArgs e)
        {
            // Clear existing panels
            // Actually clear the panels
            // Object pool used because memory go up
            for (int i = 0; i < _affinityPanels.Count; i++)
            {
                var panel = _affinityPanels[i];
                stack_ProcessPanel.Children.Remove(panel.Border);
                _panelPool.Push(panel);
            }
            _affinityPanels.Clear();

            // Find processes by name
            string[] names = textBox_Search.Text.Split(Environment.NewLine);

            List<Process> processes = new List<Process>();
            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i];
                if (name == "")
                {
                    continue;
                }
                var procs = Process.GetProcessesByName(name);
                processes.AddRange(procs);
            }

            foreach (Process p in processes)
            {
                CreateProcessPanel(p);
            }
        }

        private void CreateProcessPanel(Process process)
        {
            AffinityPanel panel;

            if (_panelPool.Count > 0)
            {
                // Get panel from pool and update
                panel = _panelPool.Next;
                panel.SetProcess(process);
            }
            else
            {
                // If pool is empty, create new panel
                panel = new AffinityPanel(process);
            }

            if (panel != null)
            {
                if (stack_ProcessPanel.Children.Contains(panel.Border) == false)
                {
                    stack_ProcessPanel.Children.Add(panel.Border);
                }
                _affinityPanels.Add(panel);
            }


        }

        private void textBox_Search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if ((Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) > 0)
                {
                    e.Handled = true;
                }
            }
        }

        private void textBox_Search_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (e.IsRepeat)
                {
                    e.Handled = true;
                } else
                {
                    if ((Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) > 0)
                    {
                        button_Search_Click(sender, e);
                        e.Handled = true;
                    }
                }
            }
        }
    }
}
