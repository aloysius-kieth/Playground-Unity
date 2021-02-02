//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//using System.Linq;
//using System.Threading.Tasks;

///*
//This script is used to read all the data coming from the device. For instance,
//If arduino send ->
//								{"1",
//								"2",
//								"3",}
//readQueue() will return ->
//								"1", for the first call
//								"2", for the second call
//								"3", for the thirst call

//This is the perfect script for integration that need to avoid data loose.
//If you need speed and low latency take a look to wrmhlReadLatest.
//*/

//public class wrmhlRead : MonoBehaviour
//{
//    #region SINGLETON
//    public static wrmhlRead Instance { get; set; }
//    private void Awake()
//    {
//        if (Instance != null && Instance != this)
//            Destroy(gameObject);
//        else
//            Instance = this;
//    }
//    #endregion

//    bool isReady = false;

//    wrmhl myDevice = new wrmhl(); // wrmhl is the bridge beetwen your computer and hardware.

//    [Tooltip("SerialPort of your device.")]
//    private string portName;

//    [Tooltip("Baudrate (Match this with arduino script!)")]
//    public int baudRate = 250000;


//    [Tooltip("Timeout (MilleSeconds)")]
//    public int ReadTimeout = 20000;

//    [Tooltip("QueueLenght")]
//    public int QueueLenght = 1;

//    async void Start()
//    {
//        await new WaitUntil(() => Trinax.Instance.isReady);
//        //Init();
//    }

//    public void Init()
//    {
//        isReady = false;
//        portName = Trinax.Instance.globalSettings.COMPORT1;

//        bool checkPortExists = System.IO.Ports.SerialPort.GetPortNames().Any(x => x == portName);

//        if (!checkPortExists)
//        {
//            Debug.LogWarning(portName + " does not exist!");
//            return;
//        }

//        Debug.Log("Now using " + portName);
//        myDevice.set(portName, baudRate, ReadTimeout, QueueLenght); // This method set the communication with the following vars;
//                                                                    //                              Serial Port, Baud Rates, Read Timeout and QueueLenght.
//                                                                    // This method open the Serial communication with the vars previously given.
//        myDevice.connect();

//        isReady = true;
//    }

//    // Update is called once per frame
//    public void Update()
//    {
//        // if (!Trinax.Instance.isReady || !isReady || GameManager.Instance.IsGameOver) return;
//        //if (PlayerController.Instance.useKeyboard) return;
//        //if(PlayerController.Instance != null) {
//        wrmhlReadPoll();
//        //}
//        //if (Input.GetKeyDown(KeyCode.F5))
//        //{
//        //    comPortPanel.SetActive(true);
//        //    Debug.Log(portName);

//        //}

//    }

//    //public void OnSubmitPortNum()
//    //{
//    //  //  portName = comPort.text;
//    //    //comPortPanel.SetActive(false);

//    // //   Debug.Log("++"+portName);
//    //}


//    public void wrmhlReadPoll()
//    {
//        string queueBuffer = myDevice.readQueue();
//        //print(queueBuffer);
//        if (queueBuffer != null)
//        {
//            // myDevice.read() return the data coming from the device using thread.
//            if (queueBuffer.EndsWith("Press"))
//            {
//                //if (queueBuffer.StartsWith("L"))
//                //    PlayerController.Instance.HandleRedBtnPress();
//                //else if (queueBuffer.StartsWith("C"))
//                //    PlayerController.Instance.HandleYellowBtnPress();
//                //else if (queueBuffer.StartsWith("R"))
//                //    PlayerController.Instance.HandleBlueBtnPress();

//                print(queueBuffer);
//            }

//            else if (queueBuffer.EndsWith("Release"))
//            {
//                //if (queueBuffer.StartsWith("L"))
//                //    PlayerController.Instance.HandleRedBtnRelease();
//                //else if (queueBuffer.StartsWith("C"))
//                //    PlayerController.Instance.HandleYellowBtnRelease();
//                //else if (queueBuffer.StartsWith("R"))
//                //    PlayerController.Instance.HandleBlueBtnRelease();

//                print(queueBuffer);
//            }
//        }
//    }

//    public void disconnectToPort()
//    {
//        myDevice.close();
//    }


//    void OnApplicationQuit()
//    { // close the Thread and Serial Port
//        if (!isReady) return;
//        disconnectToPort();

//    }
//}
