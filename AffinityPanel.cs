﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AffinityMaster
{
    public class AffinityPanel
    { 
        private List<CheckBox> _checkBoxes;
        private List<ComboBox> _comboBoxes;

        public Border Border { get; private set; }

        private Process? _process;
        private Label _label_processInfo;

        public void SetProcess(Process process)
        {
            this._process = process;
            this._label_processInfo.Content = $"{_process.Id} | {_process.ProcessName}";
        }
        public void Disable()
        {
            this._process = null;
            this._label_processInfo.Content = "No process";
        }

        static int debug_Total = 0;
        public AffinityPanel(Process process)
        {
            debug_Total++;
            System.Diagnostics.Debug.WriteLine($"Creating new AffinityPanel, total: {debug_Total}");
            this._process = process;
            this._checkBoxes = new List<CheckBox>();
            this._comboBoxes = new List<ComboBox>();

            // Main border for this process
            Border = new Border();
            Border.BorderThickness = new Thickness(2);
            Border.BorderBrush = System.Windows.Media.Brushes.Orange;
            Border.Margin = new Thickness(2);

            var stack_content = new StackPanel();
            stack_content.Orientation = Orientation.Horizontal;

            _label_processInfo = new Label() { Content = $"{_process.Id} | {_process.ProcessName}" };
            var border_panel = CreateCpuSelectionPanel();

            stack_content.Children.Add(_label_processInfo);
            stack_content.Children.Add(border_panel);
            Border.Child = stack_content;
        }

        private Border CreateCpuSelectionPanel()
        {
            // Cpu info and grid sizing
            var cpuCount = Environment.ProcessorCount;
            int rows = cpuCount / 8;
            int cols = cpuCount / rows;

            // Stack panel for buttons
            StackPanel stack_cpuButtons = new StackPanel();
            stack_cpuButtons.Orientation = Orientation.Horizontal;

            // Grid for checkboxes
            Grid grid_checkbox = new Grid();

            for (int y = 0; y < rows; y++)
            {
                grid_checkbox.RowDefinitions.Add(new RowDefinition());
            }
            for (int x = 0; x < cols; x++)
            {
                grid_checkbox.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Create checkboxes
            for (int i = 0; i < cpuCount; i++)
            {
                // Create and store checkbox
                var checkbox = CreateCpuCheckbox(i);
                grid_checkbox.Children.Add(checkbox);
                _checkBoxes.Add(checkbox);

                // Set row and column
                Grid.SetRow(checkbox, i / cols);
                Grid.SetColumn(checkbox, i % cols);
            };

            // Create CPU buttons
            Dictionary<string, int[]> comboToCpuMap = new Dictionary<string, int[]>();  // Cpu map for comboboxes and checkboxes

            // Create selection comboboxes for divisors of cpuCount
            for (int i = 2; i <= 8; i++)
            {
                // If divisible by i, add a combobox
                if (cpuCount % i == 0)
                {

                    StackPanel stack_divisorCombo;
                    ComboBox comboBox = CreateCpuComboBox_Stack(i, out stack_divisorCombo);
                    _comboBoxes.Add(comboBox);

                    int div = cpuCount / i;
                    int[][] cpuMap = new int[div][];
                    for (int j = 0; j < div; j++)
                    {
                        string s = $"CPU {j * i} - {j * i + i - 1}";
                        comboBox.Items.Add(s);
                        cpuMap[j] = new int[2]
                        {
                            j * i,
                            j * i + i - 1
                        };
                        comboToCpuMap.Add(s, cpuMap[j]);
                        comboBox.SelectionChanged += (s, e) =>
                        {
                            var combo = (ComboBox)s;        // The combobox itself
                            var item = combo.SelectedItem;  // The selected item 
                            if (item == null)
                            {
                                return;       // If no item is selected, return
                            }

                            int[] map;
                            if (comboToCpuMap.TryGetValue(item.ToString(), out map) == false)
                            {
                                return;
                            }


                            for (int k = 0; k < _checkBoxes.Count; k++)
                            {
                                _checkBoxes[k].IsChecked = k >= map[0] && k <= map[1];
                            }
                        };
                        comboBox.DropDownOpened += (s, e) =>
                        {
                            //ResetComboSelection();
                        };
                    }
                    stack_cpuButtons.Children.Add(stack_divisorCombo);
                }
            }

            Border border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = System.Windows.Media.Brushes.Black;
            border.Margin = new Thickness(1);

            DockPanel dockPanel = new DockPanel();
            border.Child = dockPanel;
            dockPanel.Children.Add(grid_checkbox);
            dockPanel.Children.Add(stack_cpuButtons);



            return border;

        }

        private ComboBox CreateCpuComboBox_Stack(int div, out StackPanel stack)
        {
            stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            stack.Margin = new Thickness(0, 0, 12, 0);

            var label = new Label();
            label.Content = $"x{div}";
            var comboBox = new ComboBox();

            stack.Children.Add(label);
            stack.Children.Add(comboBox);

            return comboBox;
        }
        private CheckBox CreateCpuCheckbox(int i)
        {
            var checkBox = new CheckBox();
            checkBox.Content = "CPU " + i;
            checkBox.Name = "cpu" + i;
            checkBox.Margin = new Thickness(4);
            return checkBox;
        }
    }
}
