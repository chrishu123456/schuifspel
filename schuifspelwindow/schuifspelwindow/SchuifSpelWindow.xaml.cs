using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SchuifSpel
{
    /// <summary>
    /// Interaction logic for SchuifSpelWindow.xaml
    /// </summary>
    public partial class SchuifSpelWindow : Window
    {
        private int rijvanstukdatwewillenmoven, kolomvanstukdatwewillenmoven, kolomwaarwenaarverslepen, rijwaarwenaarverslepen;
        public SchuifSpelWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Shuffle();
        }


        private void Check()
        {
            int irij, ikolom, grij, gkolom;
            int aantalfout = 0;
            foreach (Image stukje in puzzelGrid.Children)
            {
                irij = Convert.ToInt16(stukje.Name.Substring(4, 1));
                ikolom = Convert.ToInt16(stukje.Name.Substring(5, 1));
                grij = Grid.GetRow(stukje);
                gkolom = Grid.GetColumn(stukje);
                if ((irij != grij) || (ikolom != gkolom))
                {
                    aantalfout++;
                }
            }
            if (aantalfout == 0)
                Oplossing();
        }


        private void OplossingButton_Click(object sender, RoutedEventArgs e)
        {
            Oplossing();
        }


        private void Oplossing()
        {
            puzzelGrid.Children.Clear();
            for (int r = 0; r <= 3; r++)
            {
                for (int k = 0; k <= 3; k++)
                {
                    Image stuk = new Image();
                    BitmapImage bi = new BitmapImage(new Uri(@"images/vdab" + r + k + ".jpg", UriKind.Relative));
                    stuk.Name = "stuk" + r + k;
                    stuk.Source = bi;
                    zetImage(r, k, stuk);
                }
            }
        }

        private void zetImage(int rij, int kolom, Image zetstuk)
        {
            Image stuk = new Image();
            stuk = zetstuk;
            Grid.SetColumn(stuk, kolom);
            Grid.SetRow(stuk, rij);
            if (stuk.Name == "stuk33")
            {
                stuk.Drop += puzzelGrid_Drop;
                stuk.AllowDrop = true;
            }
            else
            {
                stuk.MouseMove += stuk_MouseMove;
                stuk.AllowDrop = false;
            }
            puzzelGrid.Children.Add(stuk);
        }



        private void Shuffle()
        {
            puzzelGrid.Children.Clear();
            int[,] checken = new int[4, 4];
            for (int r = 0; r <= 3; r++)
            {
                for (int k = 0; k <= 3; k++)
                {
                    checken[r, k] = 0;
                }
            }
            checken[3, 3] = 1;

            Random rnd = new Random();
            int rij, kolom;
            for (int r = 0; r <= 3; r++)
            {
                for (int k = 0; k <= 3; k++)
                {
                    if (k < 3 || r < 3)
                    {
                        do
                        {
                            rij = rnd.Next(0, 4);
                            kolom = rnd.Next(0, 4);
                        } while (checken[rij, kolom] == 1);

                        checken[rij, kolom] = 1;
                        Image stuk = new Image();
                        BitmapImage bi = new BitmapImage(new Uri(@"images/vdab" + r + k + ".jpg", UriKind.Relative));
                        stuk.Name = "stuk" + r + k;
                        stuk.Source = bi;
                        zetImage(rij, kolom, stuk);
                    }
                }
            }

            Image leegstuk = new Image();
            BitmapImage bl = new BitmapImage(new Uri(@"images/leeg33.jpg", UriKind.Relative));
            leegstuk.Name = "stuk33";
            leegstuk.Source = bl;
            zetImage(3, 3, leegstuk);
            rijwaarwenaarverslepen = 3;
            kolomwaarwenaarverslepen = 3;
        }



        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            Shuffle();
        }

        private void stuk_MouseMove(object sender, MouseEventArgs e)
        {
            // wat is een stuk ?
            // en hoe geraak je bij stuk_mousemove ? via ZetImage().
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // MessageBox.Show(sender.ToString());
                // tis een image dat zit in de sender
                Image stuk = (Image)sender;
           //     MessageBox.Show(stuk.Name.ToString());
                // name geeft weer waar in de oplossing de image zich bevindt.
                // weten we de locatie van het stuk die we moven ?
                // gewoon proberen te moven.. nog geen zin of te controlleren of het wel kan dat we moven..
                // toch eventjes zien of we kunnen de rij en kolom uitmaken ?
                rijvanstukdatwewillenmoven = Grid.GetRow(stuk);
           //     MessageBox.Show(rijvanstukdatwewillenmoven.ToString());
                kolomvanstukdatwewillenmoven = Grid.GetColumn(stuk);
             //   MessageBox.Show(kolomvanstukdatwewillenmoven.ToString());

                DataObject stukopnieuwelocatie = new DataObject("mijnstuk",stuk);
                DragDrop.DoDragDrop(stuk, stukopnieuwelocatie, DragDropEffects.Move);

            }
        }
        
        private void puzzelGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("mijnstuk"))
            {
                if (geldig())
              {
                    Image stuk = (Image)e.Data.GetData("mijnstuk");
                if (sender is Image)
                {
                    Image stukwaarwenaarverslepen = (Image)sender;

                    rijwaarwenaarverslepen = Grid.GetRow(stukwaarwenaarverslepen);
                    kolomwaarwenaarverslepen = Grid.GetColumn(stukwaarwenaarverslepen);
                    //    MessageBox.Show(rijwaarwenaarverslepen.ToString());
                    //    MessageBox.Show(kolomwaarwenaarverslepen.ToString());
                    puzzelGrid.Children.Remove(stuk);
                    puzzelGrid.Children.Remove(stukwaarwenaarverslepen);
                    zetImage(rijwaarwenaarverslepen, kolomwaarwenaarverslepen, stuk);
                    zetImage(rijvanstukdatwewillenmoven, kolomvanstukdatwewillenmoven, stukwaarwenaarverslepen);
                    rijwaarwenaarverslepen = rijvanstukdatwewillenmoven;
                    kolomwaarwenaarverslepen = kolomvanstukdatwewillenmoven;
                    Check();
                }
               }
              
            }
        }

        private Boolean geldig()
        {
            if (((rijvanstukdatwewillenmoven + 1 == rijwaarwenaarverslepen) || (rijvanstukdatwewillenmoven - 1 == rijwaarwenaarverslepen)) &&
(kolomvanstukdatwewillenmoven == kolomwaarwenaarverslepen))
            { return true; }
            else
            {
                if (((kolomvanstukdatwewillenmoven + 1 == kolomwaarwenaarverslepen) || (kolomvanstukdatwewillenmoven - 1 == kolomwaarwenaarverslepen)) &&
   (rijvanstukdatwewillenmoven == rijwaarwenaarverslepen))
                { return true; }
            }
            return false;
        }

    }
}
