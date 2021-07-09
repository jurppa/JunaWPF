using IdänpikajunaUIWPF.Model;
using System;
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

namespace IdänpikajunaUIWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FromTo(object sender, RoutedEventArgs e)
        {
            Output.Text = "";
            string fromStation = FromStation.Text;
            string toStation = ToStation.Text;
            DateTime date = Convert.ToDateTime( Date.Text);
            var trainsFromTo = APIUtil.TrainFromTo(fromStation, toStation, date);

            for (int i = 0; i < trainsFromTo.Count; i++)
            {
                int lastIndex = trainsFromTo[i].timeTableRows.Count;
                //if (trainsFromTo[i].timeTableRows[i].type == "ARRIVAL") { continue; }

              
                Output.Text += trainsFromTo[i].trainType + " " + trainsFromTo[i].trainNumber + " | " + "LÄHTÖ" + " | " + trainsFromTo[i].timeTableRows[i].scheduledTime.ToLocalTime() + "\n";
                Output.Text += APIUtil.IsTrainLate(trainsFromTo[i].timeTableRows[i].liveEstimateTime, trainsFromTo[i].timeTableRows[i].scheduledTime.ToLocalTime()) + "\n";

                Output.Text += Convert.ToString( trainsFromTo[i].timeTableRows[lastIndex - 1].scheduledTime - trainsFromTo[i].timeTableRows[0].scheduledTime) + "\n";
                //Console.WriteLine(trainsFromTo[i].timeTableRows[lastIndex - 1].scheduledTime - trainsFromTo[i].timeTableRows[0].scheduledTime);

            }
        }

        private void GoingThruTrains(object sender, RoutedEventArgs e)
        {
            Output.Text = "";
            string[] stations = APIUtil.GetStationFullNames(GoingThru.Text);
            var demTrains = APIUtil.GoingThrough(stations[0]);

            for (int i = 0; i < demTrains.Count; i++)
            {
                int lastIndex = demTrains[i].timeTableRows.Count;

                Output.Text += demTrains[i].trainType + " " + demTrains[i].trainNumber + " Pääteasema: " + APIUtil.ShortNameToFullName(demTrains[i].timeTableRows[lastIndex - 1].stationShortCode) + "\n";
       
                Output.Text += APIUtil.IsTrainLate(demTrains[i].timeTableRows[i].actualTime, demTrains[i].timeTableRows[i].scheduledTime) + "\n";
            

            }
        }

        private void LiveTrackTrain(object sender, RoutedEventArgs e)
        {
            Output.Text = "";
            int trainNumber;

            bool success = Int32.TryParse(LiveTrain.Text, out trainNumber);

            if (success)
            {

                var live = APIUtil.TrackLiveTrainLocation(trainNumber);

                Output.Text += "Haetun junan viimeinen tieto: \n" + live.timestamp.ToLocalTime() + "\n";
                Output.Text += "Juna on tällä hetkellä asemalla: " + APIUtil.ShortNameToFullName(live.station) + "\n";

                if (live.nextStation != "END")
                {
                    Output.Text += "Seuraava asema: " + APIUtil.ShortNameToFullName(live.nextStation) + "\n";
                }
                else
                {
                    Output.Text += "Seuraava asema: Ei tiedossa vielä." + "\n";
                }
                if (live.previousStation != "END")
                {
                    Output.Text += "Edellinen asema: " + APIUtil.ShortNameToFullName(live.previousStation) + "\n";
                }
                else
                {
                    Output.Text += "Edellinen asema: Ei tiedossa.";
                }
            }
            else { MessageBox.Show("Virheellinen junanumero"); }
        }

        private void InfoAboutWagons(object sender, RoutedEventArgs e)
        {

            Output.Text = "";
            int trainNumber;
            bool success = Int32.TryParse(TrainServiceNumber.Text, out trainNumber);
            if (success)
            {
                DateTime date = Convert.ToDateTime(ServiceDate.Text);
                var wagon = APIUtil.GetWagonInfo(date, trainNumber);

                Output.Text += "Junan sisältämät palvelut: " + "\n";

                foreach (KeyValuePair<string, bool> item in wagon)
                {
                    Output.Text += item.Key + "\n";

                }
            } else { MessageBox.Show("Anna numero"); }
        }

        private void QuitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TrainServiceNumber_GotFocus(object sender, RoutedEventArgs e)
        {

            TextBox testi = (TextBox) sender;
            testi.Text = "";
       
        }
    }
}
