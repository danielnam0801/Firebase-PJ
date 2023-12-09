using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UserType
{
    Friend,
    Recommand
}
public class FriendsElement : MonoBehaviour
{
    public UserType type;
    private void Start()
    {
        Button btn = transform.Find("Button").GetComponent<Button>();
        if (type == UserType.Friend)
        {
            btn.onClick.AddListener(()=> AuthManager.Instance.RemoveFriends(this.transform));       
        }
        else
        {
            btn.onClick.AddListener(() => AuthManager.Instance.AddFriend(this.transform));
        }
    }
}
