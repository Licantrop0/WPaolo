using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Generic;

namespace NascondiChiappe.ViewModel
{
    public class AddEditAlbumViewModel
    {
        public AlbumViewModel CurrentAlbum { get; set; }

        /// <summary>Lista degli eventuali altri album su cui spostare le foto</summary>
        IEnumerable OtherAlbums
        {
            get
            {
                return null;
                //TODO: Come faccio a recuperare la lista di tutti gli album che sta nell'AlbumsViewModel da un altro ViewModel?
                //Settings.Albums.Where(a => a.DirectoryName != CurrentAlbum.DirectoryName);
            }
        }

        public IList<AlbumPhoto> SelectedPhotos { get; set; }

        public AddEditAlbumViewModel()
        {
            //TODO: se nuovo album, creare un nuovo guid per la directory:
            //CurrentAlbum = new AlbumViewModel(new Album(string.Empty, Guid.NewGuid().ToString()));
        }
    }
}
