using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SheldonMix.Model;

namespace SheldonMix.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private SoundContainerMP3[] _soundResources;
        public SoundContainerMP3[] SoundResources
        {
            get
            {
                if (_soundResources == null)
                {
                    string[] files =
                    {
                        "CLAS_Bazinga1.mp3",
                        "CLAS_bazinga_one_of_my_jokes.mp3",
                        "CLAS_change_is_never_fine.mp3",
                        "CLAS_knock_knock_knock_Penny.mp3",
                        "CLAS_mua_ah_ah.mp3",
                        "CLAS_not_bad_it_s_horrible.mp3",
                        "CLAS_oouh.mp3",
                        "CLAS_Sheldon_laugh.mp3",
                        "CLAS_Sheldon_soft_kitty.mp3",
                        "CLAS_there_there.mp3",
                        "TBBT_theme_end.mp3",
                        "TBBT_theme_full.mp3",
                        "ZAZZ_brave_leonard_song.mp3",
                        "ZAZZ_Dr_Sheldon_Cooper_FTW.mp3",
                        "ZAZZ_Engineering_oompa_loompas_of_science.mp3",
                        "ZAZZ_hello_honeypuffs.mp3",
                        "ZAZZ_It_s_a_trap1.mp3",
                        "ZAZZ_i_possess_the_dna_of_leonard_nimoy.mp3",
                        "ZAZZ_peace_out.mp3",
                        "ZAZZ_Sheldon_drunk_sings.mp3",
                        "ZAZZ_To_Sheldon_live_long_and_prosper.mp3",
                        "ZAZZ_we_re_taking_the_train.mp3"
                    };

                    _soundResources =
                        files.OrderBy(f => f)
                        .Select(f => new SoundContainerMP3(f))
                        .ToArray();
                }
                return _soundResources;
            }
        }

        private IEnumerable<SoundContainerMP3> _suoniCLAS;
        public IEnumerable<SoundContainerMP3> SuoniCLAS
        {
            get
            {
                if (_suoniCLAS == null)
                    _suoniCLAS = SoundResources.Where(s => s.Category == SoundType.CLAS);
                return _suoniCLAS;
            }
        }

        private IEnumerable<SoundContainerMP3> _suoniTBBT;
        public IEnumerable<SoundContainerMP3> SuoniTBBT
        {
            get
            {
                if (_suoniTBBT == null)
                    _suoniTBBT = SoundResources.Where(s => s.Category == SoundType.TBBT);
                return _suoniTBBT;
            }
        }

        private IEnumerable<SoundContainerMP3> _suoniZAZZ;
        public IEnumerable<SoundContainerMP3> SuoniZAZZ
        {
            get
            {
                if (_suoniZAZZ == null)
                    _suoniZAZZ = SoundResources.Where(s => s.Category == SoundType.ZAZZ);
                return _suoniZAZZ;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}