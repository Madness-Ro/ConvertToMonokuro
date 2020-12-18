using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static Hair_StylishUP.MainModel;

namespace Hair_StylishUP
{
    public class MainVM : INotify
    {
        bool isTask = true;
        Camera camera;
        Protocol protocol;
 
        #region プロパティ
        
        //Base64の文字列
        public string BASE64String { get; set; }

        //Getプロトコル文字列
        private string get_data;
        public string Get_Data
        {
            get
            {
                return get_data;
            }
            set
            {
                this.get_data = value;
                OnPropertyChanged(nameof(Get_Data));
                return;
            }
        }

        //Postプロトコル文字列
        private string post_data;
        public string Post_Data
        {
            get
            {
                return post_data;
            }
            set
            {
                this.post_data = value;
                OnPropertyChanged(nameof(Post_Data));
                return;
            }
        }
        
        //カメラで映されている画像のプロパティ
        private WriteableBitmap bmp;
        public WriteableBitmap Bmp
        {
            get
            {
                return this.bmp;
            }
            set
            {
                this.bmp = value;
                OnPropertyChanged(nameof(Bmp));
                return;
            }
        }

        //撮影した画像を保存するプロパティ
        private WriteableBitmap picture;
        public WriteableBitmap Picture
        {
            get
            {
                return this.picture;
            }
            set
            {
                this.picture = value;
                OnPropertyChanged(nameof(Picture));
                return;
            }
        }

        //変換された画像を保存するプロパティ
        private BitmapImage convert_bmp;
        public BitmapImage Convert_Bmp
        {
            get
            {
                return this.convert_bmp;
            }
            set
            {
                this.convert_bmp = value;
                OnPropertyChanged(nameof(Convert_Bmp));
                return;
            }
        }
        #endregion

        #region ボタンコマンド
        //カメラON
        private DelegateCommand startcapturecommand;
        public DelegateCommand StartCaptureCommand
        {
            get { return startcapturecommand ?? (startcapturecommand = new DelegateCommand(StartCapture)); }
        }

        //カメラOFF
        private DelegateCommand stopcapturecommand;
        public DelegateCommand StopCaptureCommand
        {
            get { return stopcapturecommand ?? (stopcapturecommand = new DelegateCommand(StopCapture)); }
        }

        //撮影
        private DelegateCommand takepicturecommand;
        public DelegateCommand TakePictureCommand
        {
            get { return takepicturecommand ?? (takepicturecommand = new DelegateCommand(TakePicture)); }
        }

        //GETリクエストボタン
        private DelegateCommand getrequestcommand;
        public DelegateCommand GetRequestCommand
        { 
            get {return getrequestcommand ?? (getrequestcommand = new DelegateCommand(this.GetRequest));}
        }

        //POSTリクエストボタン
        private DelegateCommand postrequestcommand;
        public DelegateCommand PostRequestCommand
        {
            
            get {return postrequestcommand ?? (postrequestcommand = new DelegateCommand(this.PostRequest)); }
        }

        //DEBUGボタン
        private DelegateCommand debugcommand;
        public DelegateCommand DebugCommand
        {
            get { return debugcommand ?? (debugcommand = new DelegateCommand(Debug_Code)); }
        }

        #endregion

        public MainVM()
        {
            protocol = new Protocol();
        }

        #region ボタンコマンドメソッド

        //カメラONの処理
        private async void StartCapture()　//非同期処理 async await
        {
            isTask = true;
            camera = new Camera();
            await ShowImage();
        }

        //カメラOFFの処理
        private void StopCapture()
        {
            isTask = false;
            Bmp = null;
            
        }

        //撮影処理
        private void TakePicture()
        {
            BASE64String = camera.TakePicture();
            this.Picture = camera.Picture;

        }

        //GETリクエスト処理
        private void GetRequest()
        {
            protocol.GetRequest();
            Get_Data = protocol.pro_Get_Data;
        }

        //POSTリクエスト処理
        private void PostRequest()
        {
            protocol.PostRequest(BASE64String);
            Post_Data = protocol.pro_Post_Data;
            Convert_Bmp = protocol.pro_Convert_Bmp;
        }

        //DEBUGボタン処理
        public void Debug_Code()
        {
           // var filename = @"C:\Users\sympo\Documents\RO13\就プレー\test.bmp";

           // ImagetoBase64(filename);
            
           // if(Base64String != null)
          //  {
          //      Base64toImage(Base64String, @"C:\Users\sympo\Documents\RO13\就プレー\Sinresult.bmp");

          //  }
           /* var json = new JSON_file
            {
                Img = Base64String
            };
            //var json_seri = JsonConvert.SerializeObject(json);
            Convert_Bmp = ByteArrayToImage(json.Img);
            //Convert_Bmp = image;*/
        }
        #endregion

        #region タスクの速度
        /// Task
        /// 30秒Delayで画面を表示(30fps?)
        private async Task ShowImage()
        {
            while (isTask)
            {
                Bmp = camera.Capture();
                if (Bmp == null) break;

                await Task.Delay(30);
            }
        }
        #endregion 
    }
}
