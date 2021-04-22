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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace pretraitement
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private chatbot_model_TESTModel modelGen;
        private chatbot_model_TESTInput modelInput = new chatbot_model_TESTInput();
        private chatbot_model_TESTOutput modelOutput;

        private MediaPlayer mediaPlayer;

        List<List<string>> lists = new List<List<string>> { };
        List<Button> list_btn = new List<Button>();

        List<string> salut_intro = new List<string> { "Bonjour, comment vas-tu ?", "Salut", "ça va ?", "Content de faire ta connaissance" };
        List<string> salut_fin = new List<string> { "A plus", "Au revoir", "Rentre bien", "Bisous", "Bonne soirée", "A bientôt" };
        List<string> remerciements = new List<string> { "De rien", "Tout le plaisir était pour moi", "Avec plaisir", "Pas de problème", "Pas de soucis", "Il n'y a pas de quoi", "Je t'en prie" };
        List<string> sentiments = new List<string> { "Bien", "Mal", "Super", "Bof", "Très bien", "Pas terrible", "Fatigué", "Epuisé" };
        List<string> actions = new List<string> { "Pas grand chose", "Je lis", "J'écris", "Je mange", "Je joue", "Je prend l'air", "Je travaille", "J'écoute de la musique" };
        List<string> ressentis = new List<string> { "Toujours mal au même endroit", "Non ça va merci", "J'ai super mal", "Je me sens pas bien", "J'ai une douleur dans le bas du corps", "J'ai une douleur dans le haut du corps", "J'ai mal à la tête" };
        List<string> actualités = new List<string> { "J'ai vu, c'est incroyable", "Non, explique-moi", "Non je n'ai pas eu le temps", "Oui, c'est triste", "Oui c'est super !", "Oui, tu en penses quoi ?" };
        List<string> journée = new List<string> { "Sortir dehors, il fait beau", "Rester dedans, il fait pas beau", "Rien de spécial", "J'ai un rendez-vous médical", "Je dois travailler", "J'ai des visites", "Je ne sais pas encore", "Pas encore décidé pour le moment", "Pas grand chose" };
        List<string> retours = new List<string> { "Pareil que toi", "Je suis totalement d'accord avec toi", "A l'inverse de toi", "Je ne suis pas du tout d'accord", "Je ne sais pas", "Je n'ai pas d'avis" };
        List<string> rendez_vous = new List<string> { "Médecin", "Pharmacie", "Hôpital", "Ergothérapeute", "Kiné", "Psychologue", "Centre", "Chez mes parents", "Chez mes enfants" };
        List<string> incompréhension = new List<string> { "L'ordinateur n'a pas compris", "Peux-tu reformuler s'il-te-plaît ?", "Peux-tu répéter s'il-te-plaît ?" };
        List<string> meteo = new List<string> { "Il fait beau", "Il fait mauvais", "Il y a du vent", "J'aime le temps qu'il fait" };
        List<string> beau_temps = new List<string> { "Oui, il fait très beau", "J'aimerai aller prendre l'air", "Il fait trop chaud", "J'aime le temps qu'il fait" };
        List<string> mauvais_temps = new List<string> { "J'aime bien la pluie", "C'est un temps à rester chez soi", "Je n'aime pas ce temps", "Le temps est désagrable en ce moment" };
        List<string> temps_chaud = new List<string> { "Peux-tu ouvrir la fenêtre", "Il faut faire attention aux coups de soleil", "La température est agréable" };
        List<string> temps_froid = new List<string> { "Il fait froid", "Peux tu allumer le radiateur ?" };
        List<string> tempete = new List<string> { "Il va y avoir une tempête", "Il fait très mauvais temps", "Il vaut mieux ne pas sortir aujourd'hui" };
        List<string> temperature = new List<string> { "Non je n'ai pas chaud", "Oui j'ai chaud", "Non je n'ai pas froid", "Oui j'ai froid", "La température est parfaite" };
        List<string> sante = new List<string> { "Ca va super merci", "Je ne suis pas au top en ce moment", "Rien à signaler", "Je ne me sens pas bien", "Je ne suis pas bien" };
        List<string> fievre = new List<string> { "J'ai de la fièvre", "Je n'ai pas de fièvre", "J'ai mal à la tête", "Je n'ai pas mal à la tête" };

        List<string> words = new List<string>
        { "a", "agréable", "annoncent", "as-tu", "au", "aujourd'hui", "beau", "beaucoup", "bisous", "bon", "bonjour", "bonne", "c'est", "ce",
        "chaud", "ciao", "comment", "coucou", "de", "dehors", "dois-tu", "en", "enchanté", "encore", "est", "et", "faire", "fais", "fait",
        "fait-il", "fièvre", "fois", "fort", "froid", "gentil", "hello", "hey", "hiver", "hola", "il", "je", "l'actualité", "l'ombre", "l'orage",
        "la", "le", "mal", "mauvais", "merci", "météo", "n'as", "ne", "neuf", "nous", "où", "pa", "part", "passé", "pen", "pleut", "plus", "pour",
        "prochaine", "programme", "prévu", "qu'as-tu", "qu'il", "que", "quel", "quelque", "qui", "quoi", "remercie", "remercier", "rendez-vous",
        "revoir", "s'est", "sale", "salut", "santé", "sen", "soirée", "soleil", "somme", "super", "te", "temp", "tiens", "toi", "ton", "trouves",
        "très", "tu", "télé", "tête", "un", "une", "va", "vent", "voir", "vu", "yo", "à", "ça"};


        public MainPage()
        {
            this.InitializeComponent();
            Task.Run(async () => await LoadModelAsync());
            this.mediaPlayer = new MediaPlayer();
            
            lists.Add(actions);
            lists.Add(actualités);
            lists.Add(beau_temps);
            lists.Add(fievre);
            lists.Add(incompréhension);
            lists.Add(journée);
            lists.Add(mauvais_temps);
            lists.Add(meteo);
            lists.Add(remerciements);
            lists.Add(rendez_vous);
            lists.Add(ressentis);
            lists.Add(retours);
            lists.Add(salut_fin);
            lists.Add(salut_intro);
            lists.Add(sante);
            lists.Add(sentiments);
            lists.Add(temperature);
            lists.Add(tempete);
            lists.Add(temps_chaud);
            lists.Add(temps_froid);

            list_btn.Add(btnNuage1);
            list_btn.Add(btnNuage2);
            list_btn.Add(btnNuage3);
            list_btn.Add(btnNuage4);
            list_btn.Add(btnNuage5);
            list_btn.Add(btnNuage6);
            list_btn.Add(btnNuage7);
            list_btn.Add(btnNuage8);
            list_btn.Add(btnNuage9);
        }


        private async void textToSpeech(string str)
        {
            var config = SpeechConfig.FromSubscription("2e2cba9872d3425a9a60b8c9505a9d98", "francecentral");

            try
            {
                using (SpeechSynthesizer synthesizer = new SpeechSynthesizer(config, null))
                {
                    using (var result = await synthesizer.SpeakTextAsync(str).ConfigureAwait(false))
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

        //Accèder aux réglages via le bouton "Réglages"
        private void btn_reglages_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Reglages));
        }

        //Accèder au clavier via le bouton "Clavier"
        private void btn_clavier_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Clavier));
        }

        private void btn_urgence1_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem monButtonUrgence= sender as MenuFlyoutItem;
            string monUrgence = monButtonUrgence.Text.ToString();

            textToSpeech(monUrgence);
        }

        private void btn_urgence2_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem monButtonUrgence = sender as MenuFlyoutItem;
            string monUrgence = monButtonUrgence.Text.ToString();

            textToSpeech(monUrgence);
        }

        private void btn_urgence3_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem monButtonUrgence = sender as MenuFlyoutItem;
            string monUrgence = monButtonUrgence.Text.ToString();

            textToSpeech(monUrgence);
        }



        //Reconnaissance vocale via le bouton "Parler"
        private async void btn_parler_Click(object sender, RoutedEventArgs e)
        {
            STT.Items.Clear();
            btnNuage1.Content = "";
            btnNuage2.Content = "";
            btnNuage3.Content = "";
            btnNuage4.Content = "";
            btnNuage5.Content = "";
            btnNuage6.Content = "";
            btnNuage7.Content = "";
            btnNuage8.Content = "";
            btnNuage9.Content = "";
            var speechConfig = SpeechConfig.FromSubscription("2e2cba9872d3425a9a60b8c9505a9d98", "francecentral");
            string sentence;

            //Reconnaissance vocale
            try
            {
                using (var recognizer = new SpeechRecognizer(speechConfig, "fr-FR"))
                {
                    var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);
                    if (result.Reason != ResultReason.RecognizedSpeech)
                    {
                        NotifyUserButton("ERREUR ! Vérifiez votre compte AZURE ou votre connexion internet et rééssayez");
                    }
                    else
                    {
                        sentence = result.Text;
                        char[] separators = { ',', '.', '!', '?', ';', ':' };
                        string[] words = sentence.Split(separators);
                        foreach (string word in words)
                        {
                            NotifyUserButton(word);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyUserButton("ERREUR ! Vérifiez votre micro et rééssayez");
            }
        }



        //Appel de la fonction permettant de faire afficher le résultats de la reconnaissance vocale
        private void NotifyUserButton(string strMessage)
        {
            if (Dispatcher.HasThreadAccess)
            {
                create_text(strMessage);
            }
            else
            {
                var task = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => create_text(strMessage));
            }
        }



        //Création des Textblock comportant la reconnaissance vocale
        void create_text(string text)
        {
            if (text.Length != 0)
            {
                TextBlock txt = new TextBlock();
                txt.Text = text;
                txt.FontSize = 20;
                txt.Margin = new Thickness(0, 0, 0, 0);
                STT.Items.Add(txt);

                if (text == "ERREUR ! Vérifiez votre compte AZURE ou votre connexion internet et rééssayez")
                {
                    txt.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                }
                else if (text == "ERREUR ! Vérifiez votre micro et rééssayez")
                {
                    txt.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                }
                else
                {
                    txt.Foreground = new SolidColorBrush(Windows.UI.Colors.CornflowerBlue);
                }
            }
        }



        //Récupération du texte sélectionné, passage en entrée du modèle, Prediction et affichage du résultat
        private async void btn_liste_Click(object sender, ItemClickEventArgs e)
        {
            TextBlock txt = (TextBlock)e.ClickedItem;
            string monInput = txt.Text;

            Debug.WriteLine("Entrée : " + monInput);

            List<int> myContent = Traitement(monInput);
            float[] tabTest = new float[104];

            for (int i = 0; i < myContent.Count; i++)
            {
                tabTest[i] = myContent[i];
            }

            TensorFloat testTensor = TensorFloat.CreateFromArray(new long[] { 1, 104 }, tabTest);

            modelInput.dense_input = testTensor;
            modelOutput = await modelGen.EvaluateAsync(modelInput);

            Debug.WriteLine("Traitement en cours");
            
            IReadOnlyList<float> vectorText = modelOutput.dense_output.GetAsVectorView();
            IList<float> textList = vectorText.ToList();

            Debug.WriteLine("vectorText :");
            for (int i = 0; i < vectorText.Count; i++)
            {
                Debug.WriteLine(i);
                Debug.WriteLine(vectorText[i]);
            }
           
            int maxIndex = textList.IndexOf(textList.Max());
            int maxIndexExact = maxIndex - 5;

            if(maxIndexExact == -1)
            {
                maxIndexExact = 19;
            }
            if (maxIndexExact == -2)
            {
                maxIndexExact = 18;
            }
            if (maxIndexExact == -3)
            {
                maxIndexExact = 17;
            }
            if (maxIndexExact == -4)
            {
                maxIndexExact = 16;
            }
            if (maxIndexExact == -5)
            {
                maxIndexExact = 15;
            }

            Debug.WriteLine("Proba : " + textList.Max());

            int m = 0;

            foreach (string element in lists[maxIndexExact])
            {
                list_btn[m].Content = element;
                m = m + 1;
            }
        }


        void btn_nuage1_Click(object sender, RoutedEventArgs e)
        {
            Button monBoutonSelection = sender as Button;
            string monOutputSelection = monBoutonSelection.Content.ToString();
            Debug.WriteLine("Ma sortie sélectionnée : " + monOutputSelection);
            textToSpeech(monOutputSelection);
        }
        void btn_nuage2_Click(object sender, RoutedEventArgs e)
        {
            Button monBoutonSelection = sender as Button;
            string monOutputSelection = monBoutonSelection.Content.ToString();
            Debug.WriteLine("Ma sortie sélectionnée : " + monOutputSelection);
            textToSpeech(monOutputSelection);
        }
        void btn_nuage3_Click(object sender, RoutedEventArgs e)
        {
            Button monBoutonSelection = sender as Button;
            string monOutputSelection = monBoutonSelection.Content.ToString();
            Debug.WriteLine("Ma sortie sélectionnée : " + monOutputSelection);
            textToSpeech(monOutputSelection);
        }
        void btn_nuage4_Click(object sender, RoutedEventArgs e)
        {
            Button monBoutonSelection = sender as Button;
            string monOutputSelection = monBoutonSelection.Content.ToString();
            Debug.WriteLine("Ma sortie sélectionnée : " + monOutputSelection);
            textToSpeech(monOutputSelection);
        }
        void btn_nuage5_Click(object sender, RoutedEventArgs e)
        {
            Button monBoutonSelection = sender as Button;
            string monOutputSelection = monBoutonSelection.Content.ToString();
            Debug.WriteLine("Ma sortie sélectionnée : " + monOutputSelection);
            textToSpeech(monOutputSelection);
        }
        void btn_nuage6_Click(object sender, RoutedEventArgs e)
        {
            Button monBoutonSelection = sender as Button;
            string monOutputSelection = monBoutonSelection.Content.ToString();
            Debug.WriteLine("Ma sortie sélectionnée : " + monOutputSelection);
            textToSpeech(monOutputSelection);
        }
        void btn_nuage7_Click(object sender, RoutedEventArgs e)
        {
            Button monBoutonSelection = sender as Button;
            string monOutputSelection = monBoutonSelection.Content.ToString();
            Debug.WriteLine("Ma sortie sélectionnée : " + monOutputSelection);
            textToSpeech(monOutputSelection);
        }
        void btn_nuage8_Click(object sender, RoutedEventArgs e)
        {
            Button monBoutonSelection = sender as Button;
            string monOutputSelection = monBoutonSelection.Content.ToString();
            Debug.WriteLine("Ma sortie sélectionnée : " + monOutputSelection);
            textToSpeech(monOutputSelection);
        }
        void btn_nuage9_Click(object sender, RoutedEventArgs e)
        {
            Button monBoutonSelection = sender as Button;
            string monOutputSelection = monBoutonSelection.Content.ToString();
            Debug.WriteLine("Ma sortie sélectionnée : " + monOutputSelection);
            textToSpeech(monOutputSelection);
        }
        void btn_nuage10_Click(object sender, RoutedEventArgs e)
        {
            Button monBoutonSelection = sender as Button;
            string monOutputSelection = monBoutonSelection.Content.ToString();
            Debug.WriteLine("Ma sortie sélectionnée : " + monOutputSelection);
            textToSpeech(monOutputSelection);
        }
        void btn_nuage11_Click(object sender, RoutedEventArgs e)
        {
            Button monBoutonSelection = sender as Button;
            string monOutputSelection = monBoutonSelection.Content.ToString();
            Debug.WriteLine("Ma sortie sélectionnée : " + monOutputSelection);
            textToSpeech(monOutputSelection);
        }
        void btn_nuage12_Click(object sender, RoutedEventArgs e)
        {
            Button monBoutonSelection = sender as Button;
            string monOutputSelection = monBoutonSelection.Content.ToString();
            Debug.WriteLine("Ma sortie sélectionnée : " + monOutputSelection);
            textToSpeech(monOutputSelection);
        }


        //Chargement du modèle
        private async Task LoadModelAsync()
        {
            //Load a machine learning model
            StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/chatbot_model_TEST.onnx"));
            modelGen = await chatbot_model_TESTModel.CreateFromStreamAsync(modelFile as IRandomAccessStreamReference);
        }

        

        //Prétraitement de l'entrée du modèle
        List<int> Traitement(string input)
        {
            List<int> bagMatrixFinal = new List<int>();
            char space = (' ');
            string inputWord = "";
            List<string> inputClean = new List<string>();
            //On prend la chaîne de caractère input et on met chaque mot dans tabInput
            string[] tabInput = input.Split(space);
            //Pour chaque string contenu dans le tableau tabInput
            foreach (string s in tabInput)
            {
                //On met tous les mots en minuscule
                inputWord = s.ToLower();
                //On supprime les accents
                inputWord = inputWord.Replace('à', 'a');
                inputWord = inputWord.Replace('ä', 'a');
                inputWord = inputWord.Replace('â', 'a');
                inputWord = inputWord.Replace('ã', 'a');
                inputWord = inputWord.Replace('å', 'a');
                inputWord = inputWord.Replace('é', 'e');
                inputWord = inputWord.Replace('è', 'e');
                inputWord = inputWord.Replace('ê', 'e');
                inputWord = inputWord.Replace('ë', 'e');
                inputWord = inputWord.Replace('ì', 'i');
                inputWord = inputWord.Replace('ì', 'i');
                inputWord = inputWord.Replace('í', 'i');
                inputWord = inputWord.Replace('ï', 'i');
                inputWord = inputWord.Replace('î', 'i');
                inputWord = inputWord.Replace('ò', 'o');
                inputWord = inputWord.Replace('ó', 'o');
                inputWord = inputWord.Replace('ô', 'o');
                inputWord = inputWord.Replace('ö', 'o');
                inputWord = inputWord.Replace('û', 'u');
                inputWord = inputWord.Replace('ü', 'u');
                inputWord = inputWord.Replace('ù', 'u');
                inputWord = inputWord.Replace('ú', 'u');
                inputWord = inputWord.Replace('ý', 'y');
                inputWord = inputWord.Replace('ÿ', 'y');
                inputWord = inputWord.Replace('ç', 'c');
                inputWord = inputWord.Replace('ñ', 'n');
                //Debug.WriteLine(inputClean);
                //Ajout des mots dans la liste inputClean
                inputClean.Add(inputWord);
            }

            //Matrice [1,103]
            int[] bagMatrix = new int[618];

            //On remplit la matrice avec des 0
            for (int i = 0; i < 103; i++)
            {
                bagMatrix[i] = 0;
                //Debug.WriteLine(bagMatrix[i]);
                //Debug.WriteLine(i);
            }

            int j = 0;
            //pour chaque mot du tableau inputClean
            foreach (string s in inputClean)
            {
                //Pour chaque mot de la liste words initialialement déclarée (contient les mots du dataset)
                foreach (string ss in words)
                {
                    //Si le mot du tableau correspond à un mot contenu dans la liste
                    if (ss == s)
                    {
                        //On met un 1 dans la matrice de taille 103
                        bagMatrix[j] = 1;
                    }

                    //On récupère seulement la dernière liste qui contient tous les 1
                    if (s == inputClean[inputClean.Count - 1])
                    {
                        //Debug.WriteLine(bagMatrix[j]);
                        bagMatrixFinal.Add(bagMatrix[j]);
                    }
                    j = j + 1;
                }
                j = 0;
            }
            return bagMatrixFinal;
        }
    }
}

