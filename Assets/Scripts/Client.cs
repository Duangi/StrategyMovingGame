using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Text;
using System.Net.Sockets;
using System.Net;
using System;

using System.Threading;




public class Client : MonoBehaviour
{
    public Socket socket;
    private byte[] data;
    private bool connected = false;
    public Button btnConnect;
    public Button btnSend;
    private bool isSendBtnClicked = false;
    [SerializeField] private Text text;
    
    //定义ip和端口号
    private int _port = 10799;
    private string _ip = "182.92.106.181";

    //防止还没连接上就开始读写数据
    private Semaphore sendSemaphore = new Semaphore(0,1);
    private Semaphore receiveSemaphore = new Semaphore(0,1);

    public string message;
    public List<String> messages = new List<string>();

    public Semaphore messagesChange = new Semaphore(0,1);//对消息List的锁，主线程和子线程有且仅有一个能读写这个数据。

    public int matchNumber=1;
    private String str= "";
    
    private void Start(){
        data = new byte[1024];
        ThreadStart receive = new ThreadStart(receiveMessageFromServer);
        Thread r = new Thread(receive);
        r.Start();
        message = "connect,"+matchNumber;
    }
    

    private void Update(){
        if(!str.Equals("")){
            text.text = str;
            str = "";
        }
        if(connected){
            btnConnect.interactable = false;
        }
        if(isSendBtnClicked){
            btnSend.interactable = false;
        }
    }
    public void btnConnectClicked(){
        //先定义一个需要运行的函数，类似java中的run函数
        ThreadStart conn = new ThreadStart(connectToServer);
        //新建线程
        Thread c = new Thread(conn);
        //运行线程
        c.Start();
    }

    public void sendButtonClicked(){
        ThreadStart send = new ThreadStart(sendMessageToServer);
        Thread s = new Thread(send);
        s.Start();
    }
    public void sendMessageToServer(){
        sendSemaphore.WaitOne();
        try{
            byte[] buffer = new byte[1024];
            buffer = Encoding.UTF8.GetBytes(message);
            socket.Send(buffer);
            Debug.Log(message);
        }catch{

        }
        finally{
            sendSemaphore.Release();
            isSendBtnClicked = true;
        }
    }
    // public void sendMessageThread(string msg){
    //     ThreadStart send = new ThreadStart(sendMessageToServer);
    //     Thread s = new Thread(send);
    //     s.Start();
    // }
    // public void sendMessageToServer(string msg){
    //     sendSemaphore.WaitOne();
    //     try{
    //         byte[] buffer = new byte[1024];
    //         buffer = Encoding.UTF8.GetBytes(msg);
    //         socket.Send(buffer);
    //     }catch{

    //     }
    //     finally{
    //         sendSemaphore.Release();
    //     }
    // }
    
    public void receiveMessageFromServer(){
        receiveSemaphore.WaitOne();
        while(true){
            socket.Receive(data);
            //string str = System.Text.Encoding.Default.GetString ( byteArray );
            string s = null;
            s = System.Text.Encoding.Default.GetString(data);
            
            //Debug.Log(s);
            str = s;
            string[] msg = s.Split(',');
            if(msg[0].Equals("player")){
                str = "连接成功，你的角色为player"+msg[1];
                if(int.Parse(msg[1]) == 2){
                    matchNumber++;
                }
            }
            //当该线程收到从服务器转发的数据，立马锁住messages，将消息保存好，立马释放锁
            messagesChange.WaitOne();
            messages.Add(s);
            messagesChange.Release();
        }
        //receiveSemaphore.Release();
    }

    public void connectToServer(){
        try{
            //绑定ip和端口号
            socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            
            IPAddress ip = IPAddress.Parse(_ip);
            IPEndPoint point = new IPEndPoint(ip, _port);

            //连接到socket
            socket.Connect(point);
        }catch(Exception){
            Debug.Log("ip或端口号错误");
        }finally{
            if(socket!=null){
                if(socket.Connected){
                    connected = true;
                    str = "服务器连接成功"; 
                    Debug.Log("连接成功");
                }
                receiveSemaphore.Release();
                sendSemaphore.Release();
                messagesChange.Release();
            }
        }
    }

    public void startGame(){
        SceneManager.LoadScene("Main");
        DontDestroyOnLoad(gameObject);
    }

    public void sendMsgFromGame(string msg){
        message = msg.PadRight(25,' ');
        sendButtonClicked();
    }
}
