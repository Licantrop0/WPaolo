﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace NascondiChiappe.Messages
{
    public class DeleteAlbumMessage
    {
        public int Id { get; set; }
        public DeleteAlbumMessage(int id)
        {
            Id = id;
        }
    }
}
