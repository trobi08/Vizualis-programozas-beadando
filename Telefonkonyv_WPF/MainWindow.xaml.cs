using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Telefonkonyv_WPF
{
    public partial class MainWindow : Window
    {
        TelefonkonyvEntities db = new TelefonkonyvEntities();
        Telefonszamok aktualisSzam = null;

        public MainWindow()
        {
            InitializeComponent();
            KontaktokListaz();
        }

        private void KontaktokListaz()
        {
            kontaktLista.ItemsSource = null;
            kontaktLista.ItemsSource = db.Kontaktoks.ToList();
        }

        private void KontaktModosit(int id, string ujNev, string ujEmail)
        {
            var kontakt = db.Kontaktoks.Find(id);
            if (kontakt != null)
            {
                kontakt.Nev = ujNev;
                kontakt.Email = ujEmail;
                db.SaveChanges();
            }
        }

        private void KontaktTorol(int id)
        {
            var kontakt = db.Kontaktoks.Find(id);
            if (kontakt != null)
            {
                db.Kontaktoks.Remove(kontakt);
                db.SaveChanges();
            }
        }

        private void Listaz_Click(object sender, RoutedEventArgs e)
        {
            db = new TelefonkonyvEntities();
            KontaktokListaz();
        }

        private void KapcsolatHozzaad_Click(object sender, RoutedEventArgs e)
        {
            string nev = nevInput.Text.Trim();
            string email = emailInput.Text.Trim();

            if (!string.IsNullOrWhiteSpace(nev) && !string.IsNullOrWhiteSpace(email))
            {
                var ujKontakt = new Kontaktok { Nev = nev, Email = email };
                db.Kontaktoks.Add(ujKontakt);
                db.SaveChanges();

                nevInput.Clear();
                emailInput.Clear();
                KontaktokListaz();
            }
        }

        private void Modosit_Click(object sender, RoutedEventArgs e)
        {
            if (kontaktLista.SelectedItem is Kontaktok k)
            {
                KontaktModosit(k.ID, "Módosított Név", "modositott@pelda.hu");
                KontaktokListaz();
            }
        }

        private void Torol_Click(object sender, RoutedEventArgs e)
        {
            if (kontaktLista.SelectedItem is Kontaktok k)
            {
                KontaktTorol(k.ID);
                KontaktokListaz();
                telefonszamLista.ItemsSource = null;
                ClearTelefonszamInputs();
            }
        }

        private void TelefonszamHozzaad_Click(object sender, RoutedEventArgs e)
        {
            if (kontaktLista.SelectedItem is Kontaktok k)
            {
                string tipus = (tipusCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();
                string szam = telefonszamInput.Text?.Trim();

                if (!string.IsNullOrWhiteSpace(szam) && !string.IsNullOrWhiteSpace(tipus))
                {
                    var ujSzam = new Telefonszamok
                    {
                        Telefonszam = szam,
                        Tipus = tipus,
                        KontaktID = k.ID
                    };

                    db.Telefonszamoks.Add(ujSzam);
                    db.SaveChanges();

                    TelefonszamokListaz(k.ID);
                    ClearTelefonszamInputs();
                }
            }
        }

        private void TelefonszamModosit_Click(object sender, RoutedEventArgs e)
        {
            if (aktualisSzam != null)
            {
                string ujSzam = telefonszamInput.Text?.Trim();
                string ujTipus = (tipusCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();

                if (!string.IsNullOrWhiteSpace(ujSzam) && !string.IsNullOrWhiteSpace(ujTipus))
                {
                    var szam = db.Telefonszamoks.Find(aktualisSzam.ID);
                    if (szam != null)
                    {
                        szam.Telefonszam = ujSzam;
                        szam.Tipus = ujTipus;
                        db.SaveChanges();

                        TelefonszamokListaz(szam.KontaktID);
                        ClearTelefonszamInputs();
                    }
                }
            }
        }

        private void TelefonszamTorol_Click(object sender, RoutedEventArgs e)
        {
            if (aktualisSzam != null)
            {
                var torlendo = db.Telefonszamoks.Find(aktualisSzam.ID);
                if (torlendo != null)
                {
                    db.Telefonszamoks.Remove(torlendo);
                    db.SaveChanges();

                    TelefonszamokListaz(torlendo.KontaktID);
                    ClearTelefonszamInputs();
                }
            }
        }

        private void TelefonszamokListaz(int kontaktId)
        {
            aktualisSzam = null;
            telefonszamLista.ItemsSource = null;
            telefonszamLista.ItemsSource = db.Telefonszamoks
                .Where(t => t.KontaktID == kontaktId)
                .ToList();
        }

        private void kontaktLista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (kontaktLista.SelectedItem is Kontaktok k)
            {
                TelefonszamokListaz(k.ID);
            }
        }

        private void telefonszamLista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

            if (telefonszamLista.SelectedItem is Telefonszamok szam)
            {
                aktualisSzam = szam;
                telefonszamInput.Text = szam.Telefonszam;

                foreach (ComboBoxItem item in tipusCombo.Items)
                {
                    if (item.Content?.ToString() == szam.Tipus)
                    {
                        tipusCombo.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void ClearTelefonszamInputs()
        {
            telefonszamInput.Clear();
            tipusCombo.SelectedIndex = -1;
            aktualisSzam = null;
        }
    }
}
