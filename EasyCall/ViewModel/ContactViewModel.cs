using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Phone;
using EasyCall.ViewModel;

namespace EasyCall
{
	public class ContactViewModel : INotifyPropertyChanged, IGrouping<string, string>
	{
		#region INotifyPropertyChanged Implementation

		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		public string DisplayName { get; set; }
		public IEnumerable<string> NumberRepresentation { get; set; }
		public IEnumerable<string> Numbers { get; set; }
		private Stream _imageStream;
		private WriteableBitmap _bitmap;
		public WriteableBitmap Bitmap
		{
			get
			{
				if (_bitmap == null && _imageStream != null)
					//da rendere async?
					_bitmap = PictureDecoder.DecodeJpeg(_imageStream);

				return _bitmap;
			}
		}

		public ContactViewModel(string displayName, IEnumerable<string> numbers, Stream imageStream)
		{
			DisplayName = displayName;
			NumberRepresentation = TextToNum(displayName);
			Numbers = numbers.Select(n => Regex.Replace(n, @"[\s\-\(\)]", string.Empty)).ToArray();
			_imageStream = imageStream;
		}

		//Todo: quando il LongListSelector supporterà il binding sul SelectedItem...
		//public string SelectedNumber
		//{
		//    get { return null; }
		//    set
		//    {
		//        RaisePropertyChanged("SelectedNumber");
		//        CallHelper.Call(DisplayName, value);
		//    }
		//}

		private IEnumerable<string> TextToNum(string input)
		{
			if (string.IsNullOrEmpty(input))
				return new string[0];

			var output = input.ToLower().ToCharArray();
			for (int i = 0; i < output.Length; i++)
			{
				if (output[i] == 'a' || output[i] == 'b' || output[i] == 'c') output[i] = '2';
				else if (output[i] == 'd' || output[i] == 'e' || output[i] == 'f') output[i] = '3';
				else if (output[i] == 'g' || output[i] == 'h' || output[i] == 'i') output[i] = '4';
				else if (output[i] == 'j' || output[i] == 'k' || output[i] == 'l') output[i] = '5';
				else if (output[i] == 'm' || output[i] == 'n' || output[i] == 'o') output[i] = '6';
				else if (output[i] == 'p' || output[i] == 'q' || output[i] == 'r' || output[i] == 's') output[i] = '7';
				else if (output[i] == 't' || output[i] == 'u' || output[i] == 'v') output[i] = '8';
				else if (output[i] == 'w' || output[i] == 'x' || output[i] == 'y' || output[i] == 'z') output[i] = '9';
			}

			var words = new string(output).Split(' ');
			var searchableNumbers = new List<string>();

			for (int i = 0; i < words.Length; i++)
			{
				searchableNumbers.Add(string.Join(" ", words, i, words.Length - i));
			}

			return searchableNumbers;
		}

		#region IGrouping Implementation

		public string Key
		{
			get { return DisplayName; }
		}

		public IEnumerator<string> GetEnumerator()
		{
			foreach (string n in Numbers)
				yield return n;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.Numbers.GetEnumerator();
		}

		#endregion
	}
}