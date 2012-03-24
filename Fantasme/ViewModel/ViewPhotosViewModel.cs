﻿using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using NascondiChiappe.Messages;
using NascondiChiappe.Model;

namespace NascondiChiappe.ViewModel
{
    public class ViewPhotosViewModel : ViewModelBase
    {
        public string DirectoryName { get; private set; }
        public Photo SelectedPhoto { get; private set; }

        public ViewPhotosViewModel()
        {
            Messenger.Default.Register<ViewPhotoMessage>(this, m =>
            {
                SelectedPhoto = m.SelectedPhoto;
                DirectoryName = m.DirectoryName;
            });
        }

        public void CreateHtml()
        {
            var html = new XDocument(
                new XElement("html",
                    new XElement("head",
                        new XElement("meta",
                            new XAttribute("name", "viewport"),
                            new XAttribute("content", "width=480,height=800")),
                        new XElement("body",
                            new XAttribute("style", "background-color:black"),
                            new XElement("img",
                                new XAttribute("src", SelectedPhoto.Name),
                                new XAttribute("width", "480"),
                                new XAttribute("style", string.Format(
                                    "rotation:{0}deg;margin-top:auto; margin-bottom:auto;",
                                    SelectedPhoto.RotationAngle))
                                            )))));


            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            using (var isfs = isf.OpenFile(DirectoryName + "\\image.html", FileMode.Create))
            {
                using (var sw = new StreamWriter(isfs))
                {
                    sw.Write(html);
                    sw.Close();
                }
                isfs.Close();
            }
        }

    }
}