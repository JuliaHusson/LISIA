using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Diagnostics;
using Windows.Storage;
using Windows.AI.MachineLearning;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using System.IO;
using Windows.Media.Core;
using Windows.Media.Playback;


// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace pretraitement
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Clavier : Page
    {
        private MediaPlayer mediaPlayer;

        public Clavier()
        {
            this.InitializeComponent();
            this.mediaPlayer = new MediaPlayer();
        }

        
        private void btn_retour_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            tb_txt.Text = "";
        }

        private async void btn_envoyer_Click(object sender, RoutedEventArgs e)
        {
            
            string monTexte = tb_txt.Text;
            
            var config = SpeechConfig.FromSubscription("2e2cba9872d3425a9a60b8c9505a9d98", "francecentral");

            try
            {
                using (SpeechSynthesizer synthesizer = new SpeechSynthesizer(config, null))
                {
                    using (var result = await synthesizer.SpeakTextAsync(monTexte).ConfigureAwait(false))
                    {
                        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                        {
                            Debug.WriteLine("Synthèse vocale réussie");
                            using (var audioStream = AudioDataStream.FromResult(result))
                            {
                                var filePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "outputaudio.wav");
                                await audioStream.SaveToWaveFileAsync(filePath);
                                mediaPlayer.Source = MediaSource.CreateFromStorageFile(await StorageFile.GetFileFromPathAsync(filePath));
                                mediaPlayer.Play();
                            }
                        }
                        else if (result.Reason == ResultReason.Canceled)
                        {
                            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                            Debug.WriteLine($"CANCELED: Reason= {cancellation.Reason}");
                            Debug.WriteLine($"CANCELED: ErrorCode= {cancellation.ErrorCode}");
                            Debug.WriteLine($"CANCELED: ErrorDetails=[ {cancellation.ErrorDetails} ]");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception : " + ex);
            }
        }
    }
}
