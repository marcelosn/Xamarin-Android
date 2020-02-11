using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using System.Threading;
using BR.Com.Gertec.Gedi;
using BR.Com.Gertec.Gedi.Interfaces;
using Newtonsoft.Json;
using Android.Content;
using AlertDialog = Android.App.AlertDialog;
using System.Text;
using GertecImpressao;
using BR.Com.Gertec.Gedi.Exceptions;
using Ger7Tef;
using Android.Util;

namespace Ger7Xamarin
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private IGEDI iGedi;
        private IPRNTR iPrint;

        private Spinner spParcelas;
        private EditText editValorPagamento;
        private RadioButton rbCredito, rbDebito, rbVoucher;
        private RadioButton rbParcLoja, rbParcAdm;
        private CheckBox cbImpressao;

        private Button btnTransacao, btnCancela, btnFuncoes;

        Operacao operacao;
        Random numAleatorio = new Random();

        private static String GER7_VENDA = "1";
        private static String GER7_CANCELAMENTO = "2";
        private static String GER7_FUNCOES = "3";

        private static String GER7_DESABILITA_IMPRESSAO = "0";
        private static String GER7_HABILITA_IMPRESSAO = "1";

        private static String GER7_CREDITO = "1";
        private static String GER7_DEBITO = "2";
        private static String GER7_VOUCHER = "4";

        private static String GER7_SEMPARCELAMENTO = "0";
        private static String GER7_PARCELADO_LOJA = "1";
        private static String GER7_PARCELADO_ADM = "2";

        int id;

        ConfigPrint configPrint;
        GertecPrinter printer;
        Ger7 ger7;
        RetornoGer7 retornoGer7;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // Faz a inicialização de todos os Text's existentes
            iniEditText();

            // Faz a inicialização de todos os Radio Buttons existentes
            iniRadioButtons();

            // Faz a inicialização de todos os Spinner existentes
            iniSpinner();

            // Faz a inicialização da todos os Bottons
            iniButtons();

            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.parcelas, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spParcelas.Adapter = adapter;

            cbImpressao = FindViewById<CheckBox>(Resource.Id.cbImpressao);

            configPrint = new ConfigPrint();
            StartGediAplication();
            ger7 = new Ger7();

        }

        protected override void OnResume()
        {
            base.OnResume();
            geraValorVenda();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void iniEditText()
        {
            editValorPagamento = FindViewById<EditText>(Resource.Id.editValorPagamento);
        }

        private void iniRadioButtons()
        {
            // Radio Buttons opções de Pagamento
            rbCredito = FindViewById<RadioButton>(Resource.Id.rbCredito);
            rbDebito = FindViewById<RadioButton>(Resource.Id.rbDebito);
            rbVoucher = FindViewById<RadioButton>(Resource.Id.rbVoucher);

            // Radio Buttons Tipos de financiamento
            rbParcLoja = FindViewById<RadioButton>(Resource.Id.rbParcLoja);
            rbParcAdm = FindViewById<RadioButton>(Resource.Id.rbParcAdm);
        }

        private void iniSpinner()
        {
            spParcelas = FindViewById<Spinner>(Resource.Id.spParcelas);
        }

        private void iniButtons()
        {
            btnTransacao = FindViewById<Button>(Resource.Id.btnTransacao);
            btnCancela = FindViewById<Button>(Resource.Id.btnCancelar);
            btnFuncoes = FindViewById<Button>(Resource.Id.btnFuncoes);

            btnTransacao.Click += delegate
            {
                Ger7Venda();
            };

            btnCancela.Click += delegate
            {
                Ger7cancelaOperacao();
            };

            btnFuncoes.Click += delegate
            {
                Ger7funcoes();
            };

        }

        
        private void StartGediAplication()
        {
            new Thread(startGedi).Start();
        }

        private void startGedi()
        {
            this.iGedi = GEDI.GetInstance(ApplicationContext);
            this.iPrint = iGedi.PRNTR;
        }
        

        private void geraValorVenda()
        {
            editValorPagamento.Text = String.Format(@"{0:r}", Math.Round(numAleatorio.NextDouble() * 10, 2));
        }

        private void Ger7Venda()
        {            
            id = numAleatorio.Next(0, 10000);
            operacao = new Operacao();

            // Tipo Op: Venda, Cancelamento, Funções
            operacao.type = GER7_VENDA;
            
            // id Op
            operacao.id = id.ToString().PadLeft(6,'0');

            // Valor Op
            operacao.amount = editValorPagamento.Text.Replace(".","").Replace(",", "");
            
            // Quantidade Parcelas
            operacao.installments = spParcelas.SelectedItem.ToString();
            
            // Define o tipo de parcelamento.
            if ( operacao.installments == "0" || operacao.installments == "1")
            {
                operacao.instmode = GER7_SEMPARCELAMENTO;
            }else if (rbParcAdm.Checked)
            {
                operacao.instmode = GER7_PARCELADO_ADM;
            }
            else
            {
                operacao.instmode = GER7_PARCELADO_LOJA;
            }

            
            // Valida tipo de operação
            if (rbCredito.Checked) {
                operacao.product = GER7_CREDITO;
            }
            else if (rbDebito.Checked) {
                operacao.product = GER7_DEBITO;
            }
            else if(rbVoucher.Checked){
                operacao.product = GER7_VOUCHER;
            }

            
            // Valida se api irá fazer a impressão
            if (cbImpressao.Checked)
            {
                operacao.receipt = GER7_HABILITA_IMPRESSAO;
            }
            else
            {
                operacao.receipt = GER7_DESABILITA_IMPRESSAO;
            }

            try
            {
                ger7.Ger7Execult(this, operacao);
            }catch(Exception e)
            {
                Toast.MakeText(ApplicationContext, e.Message, ToastLength.Long).Show();
            }
            

        }

        private void Ger7cancelaOperacao()
        {
            id = numAleatorio.Next(0, 10000);
            operacao = new Operacao();

            // Tipo Op: Venda, Cancelamento, Funções
            operacao.type = GER7_CANCELAMENTO;

            // id Op
            operacao.id = id.ToString().PadLeft(6, '0');

            try
            {
                ger7.Ger7Execult(this, operacao);
            }
            catch (Exception e)
            {
                Toast.MakeText(ApplicationContext, e.Message, ToastLength.Long).Show();
            }
        }

        private void Ger7funcoes()
        {
            id = numAleatorio.Next(0, 10000);
            operacao = new Operacao();

            // Tipo Op: Venda, Cancelamento, Funções
            operacao.type = GER7_FUNCOES;

            // id Op
            operacao.id = id.ToString().PadLeft(6, '0');

            try
            {
                ger7.Ger7Execult(this, operacao);
            }
            catch (Exception e)
            {
                Toast.MakeText(ApplicationContext, e.Message, ToastLength.Long).Show();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                retornoGer7 = ger7.TrataRetornoGer7(requestCode, resultCode, data);
                Ger7ValidRetorno(retornoGer7);
            }
            catch(GediException e)
            {
                Toast.MakeText(ApplicationContext, e.Message, ToastLength.Long).Show();
            }
            catch (Exception e)
            {
                Toast.MakeText(ApplicationContext, e.Message, ToastLength.Long).Show();
            }
            
            
        }

        private void Ger7ValidRetorno(RetornoGer7 retorno)
        {

            String[] cupom;
            String[] cupomEstabelecimento;
            String[] cupomCliente;

            StringBuilder builder = new StringBuilder();
            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            AlertDialog alert = dialog.Create();
            if (retorno.response == "0" && retorno.errcode == "0")
            {

                alert.SetTitle("Transação Aprovada");
                builder.Append("Authorization: " + retorno.authorization);
                builder.Append("\nID: " + retorno.id);
                builder.Append("\nProduto: " + retorno.product);
                builder.Append("\nLabel: " + retorno.label);
                builder.Append("\nSTAN: " + retorno.stan);
                builder.Append("\nAID: " + retorno.aid);
                builder.Append("\nRRN: " + retorno.rrn);
                builder.Append("\nHorario: " + retorno.time);
                builder.Append("\nVersion: " + retorno.version);
                if(retorno.amount != null)
                {
                    builder.Append("\nValor: " + retorno.amount.Substring(0, retorno.amount.Length - 2).TrimStart('0') + "," + retorno.amount.Substring(retorno.amount.Length - 2));
                }
                alert.SetMessage(builder.ToString());
                alert.SetButton("Cancelar", (c, ev) => { });
                alert.SetButton2("Imprimir", (c, ev) => {
                    try
                    {
                        configPrint = new ConfigPrint();
                        configPrint.Negrito = true;

                        if (printer == null) {
                            printer = new GertecPrinter(this.iGedi, this.iPrint);
                        }
                            
                        printer.setConfigImpressao(configPrint);
                        cupom = retorno.print.Split('\f');
                        cupomEstabelecimento = cupom[0].Split("\n");
                        cupomCliente = cupom[1].Split("\n");

                        printer.ImprimeTexto("************[ESTABELECIMENTO]************");
                        foreach (string linha in cupomEstabelecimento)
                        {
                            printer.ImprimeTexto(linha);
                        }

                        printer.AvancaLinha(20);

                        printer.ImprimeTexto("****************[CLIENTE]****************");
                        foreach (string linha in cupomCliente)
                        {
                            printer.ImprimeTexto(linha);
                        }

                        printer.ImpressoraOutput();
                    }catch(GediException e)
                    {
                        // throw new GediException(e.ErrorCode);
                        Toast.MakeText(ApplicationContext, e.Message, ToastLength.Long).Show();
                    }catch(Exception e)
                    {
                        // throw new Exception(e.Message);
                        Toast.MakeText(ApplicationContext, e.Message, ToastLength.Long).Show();
                    }
                    finally
                    {
                        printer.ImpressoraOutput();
                    }

                });
            }
            else
            {
                alert.SetTitle("Transação Erro");
                builder.Append("Código: " + retorno.errcode);
                builder.Append("\nDescrição: " + retorno.errmsg);
                alert.SetMessage(builder.ToString());
                alert.SetButton("OK", (c, ev) => { });
            }

            alert.Show();
        }
    }
}