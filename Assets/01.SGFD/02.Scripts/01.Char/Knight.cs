using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : PlayerCtrl
{
    private  void Awake()
    {
        base.Awake(); // 부모 클래스의 Awake 메서드 호출
    }

    // Start 메서드는 PlayerCtrl 클래스에서 상속받기 때문에 제거
    private  void Start()
    {
    }

    // Update 메서드는 PlayerCtrl 클래스에서 상속받기 때문에 제거
    private  void Update()
    {
        base.Update(); // 부모 클래스의 Update 메서드 호출
    }
   
}
