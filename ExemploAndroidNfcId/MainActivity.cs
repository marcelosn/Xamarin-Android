using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Nfc;
using Android.Content;
using System.Text;
using Android.Nfc.Tech;

namespace ExemploAndroidNfcId
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TextView txtLeitura;
        private NfcAdapter nfcAdapter;
        private MifareClassic mifareClassic;
        private static int Contador = 0;
        private Tag tag;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
            txtLeitura = FindViewById<TextView>(Resource.Id.txtLeitura);

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnResume()
        {
            base.OnResume();
            //Check if we have an NFC adapter.
            if (nfcAdapter != null && nfcAdapter.IsEnabled)
            {
                
                IntentFilter tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
                IntentFilter ndefDetected = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
                IntentFilter techDetected = new IntentFilter(NfcAdapter.ActionTechDiscovered);
                IntentFilter idDetected = new IntentFilter((NfcAdapter.ExtraId));
                IntentFilter idExtraTag = new IntentFilter((NfcAdapter.ExtraTag));

                IntentFilter[] nfcIntentFilter = new IntentFilter[] { techDetected, tagDetected, ndefDetected, idDetected, idExtraTag };

                //Enable the foreground dispatch.
                nfcAdapter.EnableForegroundDispatch
                (
                    this,
                    PendingIntent.GetActivity(this, 0, new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop), 0),
                    nfcIntentFilter,
                    new string[][] { new string[] { "android.nfc.tech.Ndef", "android.nfc.action.NDEF_DISCOVERED" } }
                );
            }

        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;
            if (tag == null){
                Toast.MakeText(this, "Não foi possível ler o cartão.", ToastLength.Long).Show();
            }else{
                LerCartaoNfc();
            }
            
        }


        // Faz a leitura do ID do cartão
        protected void LerCartaoNfc()
        {
            mifareClassic = MifareClassic.Get(tag);
            Contador += 1;
            txtLeitura.Text = "Leitura: " + Contador.ToString() + "\nId do Cartão: " + idCartao();

        }

        public string idCartao()
        {

            byte[] idCartao = mifareClassic.Tag.GetId();
            long result = 0;

            if (idCartao == null) return "";

            for (int i = idCartao.Length - 1; i >= 0; --i)
            {
                result <<= 8;
                result |= idCartao[i] & 0x0FF;
            }

            return result.ToString();
        }
    }
}