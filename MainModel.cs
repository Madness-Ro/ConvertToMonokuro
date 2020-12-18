using OpenCvSharp;
using System;
using System.Text;
using OpenCvSharp.Extensions;
using System.Windows.Media.Imaging;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;

namespace Hair_StylishUP
{
    /// <summary>
    /// ビジネスモデル
    /// </summary>
    public class MainModel
    {
        /// <summary>
        /// JSONクラス
        /// </summary>
        public class JSON_file
        {
            [JsonProperty("img")]
            public string Img { get; set; }
        }

        public MainModel()
        {
        }
        /// <summary>
        /// カメラクラス
        /// </summary>
        public class Camera
        {
            VideoCapture capture;
            Mat frame;
            Encode encode;
            
            // 16ビットモノクロ画像をバイト配列をMatへ読み書きしたい
            //int width = 256;
            //int height = 256;
            //ushort[] source_array = new ushort[256 * 256];

            //コンストラクタ
            public Camera()
            {
                ///カメラはパソコンについているメインカメラを使う0はDefualtのカメラ
                capture = new VideoCapture(0);
                if (!capture.IsOpened())
                    throw new Exception("capture initialization failed");
                ///クラスのインスタンス生成
                frame = new Mat();
                encode = new Encode();

            }

            public WriteableBitmap Picture { get; set; }　//カメラクラスのプロパティ

            /// <summary>
            /// カメラONしたら起動
            /// </summary>
            /// <returns>WriteableBitmap型で返す。</returns>
            public WriteableBitmap Capture()
            {
                capture.Read(frame);
                if (frame.Empty())
                    return null;
                var image = frame.Clone();
                BitmapConverter.ToBitmap(image).Save(@"C:\Users\sympo\Documents\RO13\就プレー\testt.bmp");

                return frame.ToWriteableBitmap();
            }

            /// <summary>
            /// 撮影ボタンを押した時の処理
            /// </summary>
            /// <returns></returns>
            public string TakePicture()
            {
                Picture = frame.ToWriteableBitmap(); //画面スクリーンショット

                //WriteableBitmap型からバイナリーデータを抽出
                var temp = encode.BitmapFromWriteableBitmap(Picture);
                string b64 = encode.BytetoBase64(temp); //BASE64変換

                return b64;
                //  var open = @"C:\Users\sympo\Documents\RO13\就プレー\testt.bmp";
                // CreateThumbnail(open, Picture);
            }
        }

        /// <summary>
        /// エンコードクラス
        /// </summary>
        public class Encode
        { 
            public Encode() {}

            public byte[] BitmapFromWriteableBitmap(WriteableBitmap writeBmp)
            {
                //メモリストリームを使ってBitmapエンコーダーを使ってバイナリーコードを抽出
                using (MemoryStream outStream = new MemoryStream())
                {
                    BitmapEncoder enc = new BmpBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create((BitmapSource)writeBmp));
                    enc.Save(outStream);
                    byte[] temp = outStream.ToArray();
                    return temp;
                }

            }

            //Convertライブラリを使ってBASE64文字列に変換
            public string BytetoBase64(byte[] bt)
            {
                string temp = Convert.ToBase64String(bt);

                return temp;
            }

            //BASE64を文字列貰ってバイナリー化しLOCALに保存してBitmapに変換する。
            public BitmapImage Base64toImage(string base64, string filename)
            {
                ImageFormat format = ImageFormat.Bmp;

                var temp = Convert.FromBase64String(base64);

                FileStream fp = new FileStream(filename, FileMode.Create);

                fp.Write(temp, 0, temp.Length);
                fp.Close();
                var bitmap = new BitmapImage(new Uri(filename));
                return bitmap;

            }
        }

        /// <summary>
        /// Protocolクラス
        /// </summary>
        public class Protocol
        {
            #region プロパティ
            public string pro_Get_Data { get; private set; }
            public string pro_Post_Data { get; private set; }
            
            public BitmapImage pro_Convert_Bmp { get; private set; }
            #endregion

            Encode protocol_encode;

            const string URL = "https://8a79c9486148.ngrok.io";
            
            public Protocol()
            {
                protocol_encode = new Encode();
            }

            //WebRequestのライブラリでGETリクエスト要請
            public void GetRequest()
            {
                WebRequest request = WebRequest.Create(URL); // 호출할 url
                request.Method = "GET";

                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseFromServer = reader.ReadToEnd();

                this.pro_Get_Data = responseFromServer;

                reader.Close();
                dataStream.Close();
                response.Close();
            }

            //PostRequestのライブラリでPOSTリクエスト要請
            public void PostRequest(string b64)
            {
                var json = new JSON_file
                {
                    Img = b64
                };

                var json_seri = JsonConvert.SerializeObject(json);

                pro_Post_Data = json_seri;

                Byte[] bytes = Encoding.ASCII.GetBytes(json_seri);

                // Create a request using a URL that can receive a post. 
                WebRequest request = WebRequest.Create(URL + "/echo_back");
                // Set the Method property of the request to POST.
                request.Method = "POST";
                // Set the ContentType property of the WebRequest.
                request.ContentType = "application/x-www-form-urlencoded";
                // Set the ContentLength property of the WebRequest.
                request.ContentLength = bytes.Length;
                // Get the request stream.
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(bytes, 0, bytes.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                pro_Get_Data = reader.ReadToEnd();

                //貰ったJSON文字列をデシリアライズする。
                json = JsonConvert.DeserializeObject<JSON_file>(pro_Get_Data);

                //LOCALに保存
                var filename = @"C:\Users\sympo\Documents\RO13\就プレー\Sin_Result.bmp";

                //変換したIMAGEを返す。
                pro_Convert_Bmp = protocol_encode.Base64toImage(json.Img, filename);

                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();

            }
        }

    }
}
