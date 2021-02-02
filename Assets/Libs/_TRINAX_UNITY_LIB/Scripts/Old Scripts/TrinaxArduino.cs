//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.IO.Ports;
//using System;
//using System.Text;

//public class TrinaxArduino : MonoBehaviour
//{
//    #region SINGLETON
//    public static TrinaxArduino Instance;
//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }
//    #endregion

//    public static Action<string> OnReceivedFromArduino;
//    public bool isLoaded = false;
//    public string Port1 { get; set; }
//    public string Port2 { get; set; }
//    public int baudRate = 9600;

//    SerialPort stream1;
//    SerialPort stream2;
//    const string COM_PORT_PREFIX = "COM";
//    const string CLOSE_ALL_ARDUINO_BUTTONS = "Z";
//    const string OPEN_ALL_ARDUINO_BUTTONS = "X";

//    public SerialPort GetStream1()
//    { return stream1; }

//    public SerialPort GetStream2()
//    { return stream2; }

//    bool IsSecondArduinoConnected { get { return !string.IsNullOrEmpty(Port2); } }

//    async void Start()
//    {
//        await new WaitUntil(() => MainManager.Instance.IsReady );
//        Init();
//    }

//    void Init()
//    {
//        StartArduinos();
//        OpenAllArduinoButtons();
//    }

//    public void Populate(GlobalSettings settings)
//    {
//        Port1 = settings.COMPORT1;
//        Port2 = settings.COMPORT2;
//    }

//    void StartArduinos()
//    {
//        try
//        {
//            stream1 = new SerialPort(FormatComPort(Port1), baudRate);
//            stream1.ReadTimeout = 10;
//            stream1.Open();
//            Debug.Log("Stream 1 opened, COMPORT: " + Port1);
//        }
//        catch (Exception e)
//        {
//            Debug.Log(e);
//            return;
//        }

//        try
//        {
//            if (IsSecondArduinoConnected)
//            {
//                stream2 = new SerialPort(FormatComPort(Port2), baudRate);
//                stream2.ReadTimeout = 10;
//                stream2.Open();
//                Debug.Log("Stream 2 opened, COMPORT: " + Port2);
//            }
//        }
//        catch (Exception e)
//        {
//            Debug.Log(e);
//            return;
//        }

//        StartCoroutine(AsynchronousReadFromArduino(stream1, OnReadSuccess));
//        StartCoroutine(AsynchronousReadFromArduino(stream2, OnReadSuccess));
//    }

//    string FormatComPort(string target)
//    {
//        string port = target;
//        if (port.Contains(COM_PORT_PREFIX) || port.Contains(COM_PORT_PREFIX.ToLower()))
//            port = port.Substring(3);
//        else
//            return port = COM_PORT_PREFIX + port;

//        port = /*"\\\\.\\" + */COM_PORT_PREFIX + port;
//        //string port = target;
//        //string actualPortString = "";
//        //actualPortString = port.Length > 1 ? "\\\\.\\" + target : target;
//        return port;
//    }

//    void OnReadSuccess(string line)
//    {
//        if (line == null) return;
//        if (line == "") return;

//        //Debug.Log("---Arduino---" + line.Trim() + "---");

//        line = line.Trim();
//        //if (MainManager.Instance.IsReady && UIManager.Instance.IsReady)
//        //{
//        //    if (line.Length > 0)
//        //    {
//        //        //Debug.Log(line);
//        //        //MainManager.Instance.OnReceivedFromArduino(line);
//        //        OnReceivedFromArduino?.Invoke(line);
//        //    }
//        //}
//    }

//    private void OnDisable()
//    {
//        Close();
//    }

//    public void Close()
//    {
//        if (stream1 != null)
//        {
//            stream1.Close();
//        }

//        if (IsSecondArduinoConnected && stream2 != null)
//        {
//            stream2.Close();
//        }

//        StopAllCoroutines();
//    }

//    public void Restart()
//    {
//        Close();
//        StartArduinos();
//    }

//    public void OpenAllArduinoButtons()
//    {
//        Debug.Log("Opening all buttons");
//        TrinaxArduino.Instance.WriteToArduino(TrinaxArduino.Instance.GetStream1(), OPEN_ALL_ARDUINO_BUTTONS.ToCharArray());
//        TrinaxArduino.Instance.WriteToArduino(TrinaxArduino.Instance.GetStream2(), OPEN_ALL_ARDUINO_BUTTONS.ToCharArray());
//    }

//    public void CloseAllArduinoButtons()
//    {
//        Debug.Log("Closing all buttons");
//        TrinaxArduino.Instance.WriteToArduino(TrinaxArduino.Instance.GetStream1(), CLOSE_ALL_ARDUINO_BUTTONS.ToCharArray());
//        TrinaxArduino.Instance.WriteToArduino(TrinaxArduino.Instance.GetStream2(), CLOSE_ALL_ARDUINO_BUTTONS.ToCharArray());
//    }

//    public void WriteToArduino(SerialPort stream, char[] message)
//    {
//        string temp = new string(message);
//        //Debug.Log("Writing " + temp + " to " + stream.PortName);

//        if (stream != null && !stream.IsOpen)
//            return;

//        //int payload = 0;
//        //if (int.TryParse(message, out payload))
//        //{
//        if (stream != null && stream.IsOpen)
//        {
//            //byte[] bytesToSend = Encoding.ASCII.GetBytes(message);
//            stream.Write(message, 0, message.Length);
//            stream.BaseStream.Flush();
//        }
//        //}
//        //else
//        //{
//        //    //Debug.Log("Message is not a int");
//        //}
//    }

//    public IEnumerator AsynchronousReadFromArduino(SerialPort strm, Action<string> callback)
//    {
//        string dataString = null;

//        do
//        {
//            yield return new WaitForSeconds(0.020f);

//            try
//            {
//                dataString = strm.ReadLine();
//            }
//            catch (TimeoutException)
//            {
//                dataString = null;
//            }

//            if (dataString != null)
//            {
//                callback(dataString);
//                yield return null;
//            }
//            //else
//            //yield return new WaitForSeconds(0.15f);

//        } while (true);
//    }
//}
