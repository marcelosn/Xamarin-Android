using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;
using BR.Com.Gertec.Gedi.Exceptions;
using Plugin.DeviceInfo;
using System;

namespace ExemploImpressao
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        string modelo;

        Context context;

        TextView txtModelo;

        EditText txtMensagem;

        RadioButton rbEsquerda, rbCentralizado, rbDireta;

        ToggleButton btnNegrito, btnItalico, btnSublinhado;

        Spinner spFonte, spTamanho;

        Spinner spCodeHeight, spCodeWidth, spTipoCode;

        Button btnStatusImpressora, btnImprimirMensagem, btnImprimirImagem, btnImprimirBarCode;

        GertecPrinter printer;
        ConfigPrint configPrint;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            modelo = CrossDeviceInfo.Current.Model;

            // Ini Textview
            iniTextview();

            // Ini EditText
            iniEditText();

            // Ini Radio Button
            iniRadioButton();

            // Ini ToggleButton
            iniToggleButton();

            // Ini Spinner
            iniSpinner();

            // Ini Buttons
            iniButtons();

            // Ini funções bottuns
            iniFuncoesButtons();

            // Carrega todos os Spinner
            iniLoadSpinner();

            // Mostra modelo
            iniLoadModelo();

            // Ini context
            context = ApplicationContext;
            printer = new GertecPrinter(this, context);
            configPrint = new ConfigPrint();
            printer.setConfigImpressao(configPrint);

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void iniTextview()
        {
            txtModelo = FindViewById<TextView>(Resource.Id.txtModelo);
        }

        private void iniEditText()
        {
            txtMensagem = FindViewById<EditText>(Resource.Id.txtMensagem);
        }

        private void iniRadioButton()
        {
            rbEsquerda = FindViewById<RadioButton>(Resource.Id.rbEsquerta);
            rbCentralizado = FindViewById<RadioButton>(Resource.Id.rbCentralizado);
            rbDireta = FindViewById<RadioButton>(Resource.Id.rbDireta);
        }

        private void iniToggleButton()
        {
            btnNegrito = FindViewById<ToggleButton>(Resource.Id.btnNegrito);
            btnItalico = FindViewById<ToggleButton>(Resource.Id.btnItalico);
            btnSublinhado = FindViewById<ToggleButton>(Resource.Id.btnSublinhado);
        }

        private void iniSpinner()
        {
            spFonte = FindViewById<Spinner>(Resource.Id.spFonte);
            spTamanho = FindViewById<Spinner>(Resource.Id.spTamanho);
            spCodeHeight = FindViewById<Spinner>(Resource.Id.spCodeHeight);
            spCodeWidth = FindViewById<Spinner>(Resource.Id.spCodeWidth);
            spTipoCode = FindViewById<Spinner>(Resource.Id.spTipoCode);
        }

        private void iniButtons()
        {
            btnStatusImpressora = FindViewById<Button>(Resource.Id.btnStatusImpressora);
            btnImprimirMensagem = FindViewById<Button>(Resource.Id.btnImprimirMensagem);
            btnImprimirImagem = FindViewById<Button>(Resource.Id.btnImprimirImagem);
            btnImprimirBarCode = FindViewById<Button>(Resource.Id.btnImprimirBarCode);
        }

        private void iniLoadModelo()
        {
            if (modelo.Equals("Smart G800"))
            {
                txtModelo.Text = "Xamarin Impressão TSG800";
            }
            else
            {
                txtModelo.Text = "Xamarin Impressão GPOS700";
            }
        }

        private void iniLoadSpinner()
        {
            ArrayAdapter adapter;

            // Irá mostrar todas as Fontes
            adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.Fonts, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spFonte.Adapter = adapter;

            // Irá mostrar todos os possíveis tamanhos
            adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.Tamanhos, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spTamanho.Adapter = adapter;

            // Irá mostrar todos os possíveis tamanhos
            adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.Widths, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spCodeHeight.Adapter = adapter;

            // Irá mostrar todos os possíveis tamanhos
            adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.Widths, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spCodeWidth.Adapter = adapter;

            // Irá mostrar todos os possíveis tamanhos
            adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.BarCodes, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spTipoCode.Adapter = adapter;
        }

        private void iniFuncoesButtons()
        {
            btnStatusImpressora.Click += BtnStatusImpressora_Click;
            btnImprimirMensagem.Click += BtnImprimirMensagem_Click;
            btnImprimirImagem.Click += BtnImprimirImagem_Click;
            btnImprimirBarCode.Click += BtnImprimirBarCode_Click;
        }


        protected void BtnStatusImpressora_Click(object sender, EventArgs args)
        {
            string statusImpressora;

            try
            {
                statusImpressora = printer.getStatusImpressora();
                Toast.MakeText(context, statusImpressora, ToastLength.Long).Show();
            }
            catch (Exception e)
            {
                Toast.MakeText(context, e.Message, ToastLength.Long).Show();
            }

        }

        protected void BtnImprimirMensagem_Click(object sender, EventArgs args)
        {
            string mensagem;

            try
            {
                mensagem = txtMensagem.Text;
                if (rbEsquerda.Checked)
                {
                    configPrint.Alinhamento = "LEFT";
                }
                else if (rbCentralizado.Checked)
                {
                    configPrint.Alinhamento = "CENTER";
                }
                else if (rbDireta.Checked)
                {
                    configPrint.Alinhamento = "RIGHT";
                }

                configPrint.Negrito = btnNegrito.Checked;
                configPrint.Italico = btnItalico.Checked;
                configPrint.SubLinhado = btnSublinhado.Checked;

                configPrint.Fonte = spFonte.SelectedItem.ToString();
                configPrint.Tamanho = Int32.Parse(spTamanho.SelectedItem.ToString());

                printer.setConfigImpressao(configPrint);

                printer.ImprimeTexto(mensagem);

                printer.ImpressoraOutput();


            }
            catch (GediException e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
            }

        }

        protected void BtnImprimirImagem_Click(object sender, EventArgs args)
        {
            configPrint.IWidth = 430;
            configPrint.IHeight = 700;
            printer.setConfigImpressao(configPrint);
            printer.ImprimeImagem("invoice");
            configPrint.IWidth = 400;
            configPrint.IHeight = 150;
            printer.setConfigImpressao(configPrint);
            printer.ImprimeImagem("gertec");
            configPrint.IWidth = 300;
            configPrint.IHeight = 400;
            printer.setConfigImpressao(configPrint);
            printer.ImprimeImagem("gertecone");
            printer.ImpressoraOutput();
        }

        protected void BtnImprimirBarCode_Click(object sender, EventArgs args)
        {
            if (txtMensagem.Text.Equals(""))
            {
                Toast.MakeText(ApplicationContext, "Preencha um texto", ToastLength.Long).Show();
            }
            else
            {
                printer.ImprimeBarCode(
                    txtMensagem.Text,
                    Int32.Parse(spCodeHeight.SelectedItem.ToString()),
                    Int32.Parse(spCodeWidth.SelectedItem.ToString()),
                    spTipoCode.SelectedItem.ToString());
                printer.ImpressoraOutput();
            }

        }
    }
}