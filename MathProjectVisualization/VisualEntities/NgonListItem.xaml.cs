﻿using System;
using System.Collections.Generic;
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

namespace MathProjectVisualization.VisualEntities
{
    /// <summary>
    /// Логика взаимодействия для NgonListItem.xaml
    /// </summary>
    public partial class NgonListItem : UserControl
    {
        public NgonListItem()
        {
            InitializeComponent();
            
        }
        public void drawNgon(VisualNgon ngon)
        {
            ngon.draw(grid);
        }
    }
}
