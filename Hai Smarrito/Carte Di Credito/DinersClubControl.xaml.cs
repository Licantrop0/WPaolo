﻿using System.Windows.Controls;
using System.Windows;
using NientePanico.Helpers;

namespace NientePanico.Carte_Di_Credito
{
    public partial class DinersClubControl : UserControl
    {
        public DinersClubControl()
        {
            InitializeComponent();
        }

        private void Italia_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Diners Club Italia", "800393939");
        }

        private void Estero_Click(object sender, RoutedEventArgs e)
        {
            CallHelper.Call("Diners Club Estero", "00390232162656");
        }

    }
}
