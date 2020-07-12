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
    private Socket socket;
    private byte[] data;
    private bool connected = false;
    [SerializeField] private Text text;
    
    //定义ip和端口号
    private int _port = 10799;
    private string _ip = "182.92.106.181";

    //防止还没连接上就开始读写数据
    private Semaphore sendSemaphore = new Semaphore(0,1);
    private Semaphore receiveSemaphore = new Semaphore(0,1);

    public List<String> messages = new List<string>();

    public Semaphore messagesChange = new Semaphore(0,1);//对消息List的锁，主线程和子线程有且仅有一个能读写这个数据。
    private void Start(){
        data = new byte[1024];
        ThreadStart receive = new ThreadStart(receiveMessageFromServer);
        Thread r = new Thread(receive);
        r.Start();
    }
    private String str= "";

    private void Update(){
        if(!str.Equals("")){
            text.text = str;
            str = "";
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
            string msg = "connect,1";
            byte[] buffer = new byte[1024];
            buffer = Encoding.UTF8.GetBytes(msg);
            socket.Send(buffer);
        }catch{

        }
        finally{
            sendSemaphore.Release();
        }
    }
    
    public void receiveMessageFromServer(){
        receiveSemaphore.WaitOne();
        while(true){
            socket.Receive(data);
            //string str = System.Text.Encoding.Default.GetString ( byteArray );
            string s = System.Text.Encoding.Default.GetString(data);
            //Debug.Log(s);
            Debug.Log("1");
            str = s;
            messages.Add(s);
        }
        receiveSemaphore.Release();
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
                Debug.Log("连接成功");
            }
            receiveSemaphore.Release();
            sendSemaphore.Release();
        }
        }
    }

    public void startGame(){
        SceneManager.LoadScene("Main");
        DontDestroyOnLoad(gameObject);
    }
}
