using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class AuthManager : Singleton<AuthManager>
{
    [Header("Firebase")]
    public FirebaseAuth auth; //인증 관리 객체
    public FirebaseUser User; //사용자
    public DatabaseReference DBref; //데이터베이스 인스턴스

    [Header("Login With Name")]
    public TMP_InputField nameLoginField;
    public TMP_InputField passwordLoginFieldname;

    [Header("Login With Email")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginFieldemail;

    [Header("Login")]
    public TMP_Text warningLoginText;


    public TMP_Text userNameText;

    [Header("Register")]
    public TMP_InputField emailRegisterField;
    public TMP_InputField userNameRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordCheckRegisterField;
    public TMP_Text warningRegisterText;
    public GameObject warningTextobj;

    [Header("Change Password")]
    public TMP_InputField emailPasswordField;
    public TMP_InputField changePasswordField;
    public TMP_InputField changeNewPasswordField;

    [Header("Friend")]
    [SerializeField] GameObject friendElement;
    [SerializeField] GameObject recommendElement;
    [SerializeField] Transform friendContent;
    [SerializeField] Transform recommandContent;


    private string strLastLogin;
    public string StrLastLogin => strLastLogin;

    private int totalConnectDay;
    public int TotalConnectDay => totalConnectDay;

    private int seqConnectDay;
    public int SeqConnectDay => seqConnectDay;

    private int rewardInit = 0; // == 2  successful init;

    //StaticEvent
    private int developerCoin;
    private string strWeather;

    private int staticEventLoadDone = 0;
    private int staticEventSize = 2;

    public int GetDeveloperCoin()
    {
        return developerCoin;
    }
    public string GetWeather()
    {
        return strWeather;
    }


    private void Awake()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                auth = FirebaseAuth.DefaultInstance;
                DBref = FirebaseDatabase.DefaultInstance.RootReference;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    private void Init()
    {
        DBref.Child("users").Child(User.UserId).Child("TotalConnectDay").GetValueAsync().ContinueWith((task) =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Value != null)
                {
                    totalConnectDay = int.Parse(snapshot.Value.ToString());
                    Debug.Log($"TotalConnectDay :{strLastLogin}");
                }
                else
                {
                    totalConnectDay = 0;
                }
                rewardInit++;
            }
        });
        DBref.Child("users").Child(User.UserId).Child("SeqConnectDay").GetValueAsync().ContinueWith((task) =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.Value != null)
                {
                    seqConnectDay = int.Parse(snapshot.Value.ToString());
                    Debug.Log($"SeqConnectDay :{seqConnectDay}");
                }
                else
                {
                    seqConnectDay = 0;
                }
                rewardInit++;
            }
        });

        DBref.Child("users").Child(User.UserId).Child("Friends").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapShot = task.Result;
                if (snapShot != null && snapShot.Value != null)
                {
                    friendAll = snapShot.Value.ToString();
                }
            }
        });
    }

    public void ChangePassword()
    {

        string email = emailPasswordField.text;
        auth.SendPasswordResetEmailAsync(emailPasswordField.text).ContinueWith(task => {
            if (task.IsCanceled)
            {
                ErrorText("SendPasswordResetEmailAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                ErrorText("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("Password reset email sent successfully.");
        });
    }

       
   
    private IEnumerator Register(string email, string password, string userName)
    {
        if (userName == "")
        {
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordCheckRegisterField.text)
        {
            warningRegisterText.text = "Password does not Match!";
        }
        else
        {
            var task = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to Register:{task.Exception}");
                FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException;
                AuthError errorcode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorcode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
                warningTextobj.SetActive(true);
            }
            else
            {
                warningTextobj.SetActive(false);
                User = task.Result.User;
                if(User != null)
                {
                    FirebaseUser user = auth.CurrentUser;
                    if(user != null)
                    {
                        UserProfile profile = new UserProfile { DisplayName = userName };
                        var profileTask = user.UpdateUserProfileAsync(profile);
                        yield return new WaitUntil(() => profileTask.IsCompleted);
                        if(profileTask.Exception != null)
                        {
                            Debug.LogWarning(message:$"Failed to register:{profileTask.Exception}");
                            FirebaseException profileEx= profileTask.Exception.GetBaseException() as FirebaseException;
                            AuthError profileErrorCode = (AuthError)profileEx.ErrorCode;
                            warningRegisterText.text = "Username Set Failed!";
                        }
                        else
                        {
                            UIManager.Instance.LoginPanelwithEmail();
                            Debug.Log("User Profile Updated Successfully");
                            warningRegisterText.text = "";
                            StartCoroutine(SaveUserName());
                            StartCoroutine(SaveRewardData());
                        }
                    }
                }

            }

        }

    }

    public void OnRegister()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, userNameRegisterField.text));
    }

    private IEnumerator LoginWithEmail(string email, string password)
    {
        var task = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => task.IsCompleted);
        
        if (task.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to Login:{task.Exception}");
            FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException;
            ErrorCheck(firebaseEx);
        }
        else
        {
            //warningTextobj.SetActive(false);
            User = task.Result.User;
            Debug.Log($"User Signed in Successfully: {User.Email}, {User.DisplayName}");
            Init();
            //값 변경될 때 마다 이벤트 호출
            //DBref.Child("users").Child(User.UserId).Child("LastLogin").ValueChanged += LoadLastLogin;
            UIManager.Instance.CloseLogin();
            StartCoroutine(LoadUserName());
            StartCoroutine(SaveLoginData());
            StartCoroutine(SetUsers());
            StartCoroutine(LoadStaticEvent());
        }
    }

    //private IEnumerator LoginWithName(string name, string password)
    //{
    //    var task = auth.Sign(name, password);
    //    yield return new WaitUntil(() => task.IsCompleted);

    //    if (task.Exception != null)
    //    {
    //        Debug.LogWarning(message: $"Failed to Login:{task.Exception}");
    //        FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException;
    //        ErrorCheck(firebaseEx);
    //    }
    //    else
    //    {
    //        warningTextobj.SetActive(false);
    //        User = task.Result.User;
    //        Debug.Log($"User Signed in Successfully: {User.Email}, {User.DisplayName}");

    //        //값 변경될 때 마다 이벤트 호출
    //        DBref.Child("users").Child(User.UserId).Child("LastLogin").ValueChanged += LoadLastLogin;

    //        UIManager.Instance.CloseLogin();
    //        StartCoroutine(LoadUserName());
    //        StartCoroutine(SaveLoginData());
    //        StartCoroutine(LoadWeather());
    //    }
    //}

    private void ErrorCheck(FirebaseException firebaseEx)
    {
        AuthError errorcode = (AuthError)firebaseEx.ErrorCode;

        string message = "Login Failed!";
        switch (errorcode)
        {
            case AuthError.MissingEmail:
                message = "Missing Email";
                break;
            case AuthError.MissingPassword:
                message = "Missing Password";
                break;
            case AuthError.WrongPassword:
                message = "Wrong Password";
                break;
            case AuthError.InvalidEmail:
                message = "Invalid Email";
                break;
            case AuthError.UserNotFound:
                message = "Account does not Exist";
                break;
        }
        ErrorText(message);
    }

    //private void LoadLastLogin(object sender, ValueChangedEventArgs e)
    //{
    //    if(e.DatabaseError != null)
    //    {
    //        Debug.LogError(e.DatabaseError.Message);
    //        return;
    //    }
    //    else
    //    {
    //        DBref.Child("users").Child(User.UserId).Child("RewardLogin")
    //            .GetValueAsync().ContinueWith(task =>
    //            {
    //                if (task.IsCompleted)
    //                {
    //                    DataSnapshot snapshot = task.Result;
    //                    if(snapshot!=null && snapshot.Value != null)
    //                    {
    //                        strLastLogin = snapshot.Value.ToString();
    //                        Debug.Log($"Reward Login :{strLastLogin}");
    //                    }
    //                }
    //            });
    //    }
    //}

    public IEnumerator OnReward()
    {
        yield return new WaitUntil(()=> rewardInit == 2);
        var Task = DBref.Child("users").Child(User.UserId).Child("RewardLogin").GetValueAsync();
        yield return new WaitUntil(()=> Task.IsCompleted);
        DataSnapshot snapShot = Task.Result;
        DateTime lastReward = DateTime.ParseExact(snapShot.Value.ToString(), "yyyyMMddHHmmss", null);
        DateTime lastLogin = DateTime.ParseExact(strLastLogin, "yyyyMMddHHmmss", null);
        Debug.Log("LastLoginnnnnnnnnn : " + strLastLogin.ToString());
       // bool canReward = DateTime.Compare(lastLogin, lastReward.AddDays(1)) > 0;
        bool canReward = DateTime.Compare(lastLogin.AddDays(1), lastReward.AddDays(1)) > 0; //디버깅용
        // date2 = DateTime.Now.ToString("yyyyMMddHHmmss");
        if(canReward)
        {
            bool isSeqConnect = DateTime.Compare(lastLogin, lastReward.AddDays(2)) < 0; // 2일이상 접속하지 않았을때 // 연속접속 끊김
            seqConnectDay = isSeqConnect ? seqConnectDay + 1 : 1;
            totalConnectDay += 1;


            DBref.Child("users").Child(User.UserId).Child("RewardLogin").SetValueAsync(GetNow()).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"Reward LoginDate Updated:{lastLogin}");
                }
                else
                {
                    Debug.Log(task.Exception.ToString());
                }
            });


            DBref.Child("users").Child(User.UserId).Child("TotalConnectDay").SetValueAsync(totalConnectDay).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"TotalConnectDay Updated: {totalConnectDay}");
                }
                else
                {
                    Debug.Log(task.Exception.ToString());
                }
            });


            DBref.Child("users").Child(User.UserId).Child("SeqConnectDay").SetValueAsync(seqConnectDay).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"SeqConnectDay Updated:{seqConnectDay}");
                }
                else
                {
                    Debug.Log(task.Exception.ToString());
                }
            });



            RewardManager.Instance.WeekReward();
            Debug.Log("보상 받음");
        }
    }

    public void LoginButtonWithEmail()
    {
        StartCoroutine(LoginWithEmail(emailLoginField.text, passwordLoginFieldemail.text));
    }
    public void LoginButtonWithName()
    {
        //StartCoroutine(Login(nameLoginField.text, passwordLoginFieldname.text));
    }

    private IEnumerator SaveUserName()
    {
        var DBTask = DBref.Child("users").Child(User.UserId).Child("UserName")
            .SetValueAsync(userNameRegisterField.text);
        yield return new WaitUntil(() => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Save Task failed with {DBTask.Exception}");
        }
        else Debug.Log("Save Completed");
    }

    private IEnumerator LoadUserName()
    {
        var DBTask = DBref.Child("users").Child(User.UserId).Child("UserName")
            .GetValueAsync();
        yield return new WaitUntil(() => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Load Task failed with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            Debug.Log("Load Completed");
            userNameText.text = $"Username: {snapshot.Value}";
        }
    }

    public IEnumerator LoadStaticEvent()
    {
        StartCoroutine(LoadWeather());
        StartCoroutine(LoadDeveloperGift());
        yield return new WaitUntil(() => staticEventSize == staticEventLoadDone);

        UIManager.Instance.StartGame();
    }

    private IEnumerator LoadDeveloperGift()
    {
        var DBTask = DBref.Child("DeveloperGift").GetValueAsync();
        yield return new WaitUntil(() => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Load Task failed with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            if (snapshot != null && snapshot.Value != null)
            {
                Debug.Log("Load Completed");
                developerCoin = int.Parse(snapshot.Value.ToString());
            }
        }
        staticEventLoadDone++;
        
    }

    private IEnumerator LoadWeather()
    {
        var DBTask = DBref.Child("Weather").GetValueAsync();
        yield return new WaitUntil(() => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Load Task failed with {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            if (snapshot != null && snapshot.Value != null)
            {
                Debug.Log("Load Completed");
                strWeather = snapshot.Value.ToString();
            }
        }
        staticEventLoadDone++;
    }


    //최초 회원가입 사용자 보상 초기화
    private IEnumerator SaveRewardData()
    {
        var DBTask = DBref.Child("users").Child(User.UserId).Child("RewardLogin")
            .SetValueAsync("00000000000000");
        yield return new WaitUntil(() => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to save task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log("Reward Data Initailized");
        }
    }

    public string GetNow()
    {
        return DateTime.Now.ToString("yyyyMMddHHmmss");
    }
    private IEnumerator SaveLoginData()
    {
        string currentDateTime = GetNow();
        var DBTask = DBref.Child("users").Child(User.UserId).Child("LastLogin").SetValueAsync(currentDateTime);
        strLastLogin = currentDateTime;

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to Save task with {DBTask.Exception}");

        }
        else
        {
            StartCoroutine(OnReward());
            Debug.Log("Login Date update: " + currentDateTime);
        }
    }
    public void ErrorText(string error)
    {
        warningTextobj.SetActive(true);
        warningRegisterText.text = error;
    }

    public void SetUsersFunc()
    {
        StartCoroutine(SetUsers());
    }

    public IEnumerator SetUsers()
    {
        var task = DBref.Child("users").GetValueAsync();
        yield return new WaitUntil(predicate: ()=> task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {task.Exception}");
        }
        else
        {
            DataSnapshot snapShot = task.Result;

            foreach (Transform child in friendContent.transform)
            {
                Destroy(child.gameObject);
            } foreach (Transform child in recommandContent.transform)
            {
                Destroy(child.gameObject);
            }

            List<string> friends = new List<string>();


            DataSnapshot friendsSnapShot = snapShot.Child(User.UserId).Child("Friends");


            if (friendsSnapShot != null && friendsSnapShot.Value != null)
            {
                string friendsStr = friendsSnapShot.Value.ToString();
                friends = friendsStr.Split(",").ToList();
                friends.Remove("");
            }

            foreach(DataSnapshot childSnapShot in snapShot.Children)
            {
                string username = childSnapShot.Child("UserName").Value.ToString();

                GameObject friend;
                if (friends.Contains(username))
                {
                    friend = Instantiate(friendElement, friendContent);
                }
                else
                {
                    friend = Instantiate(recommendElement, recommandContent);
                }
                friend.transform.Find("NickName").GetComponent<TextMeshProUGUI>().text = username;
            }
        }
    }
    string friendAll = string.Empty;
    public void AddFriend(Transform transform)
    {
        string friend = transform.Find("NickName").GetComponent<TextMeshProUGUI>().text;
        friendAll += friend + ',';
       
        DBref.Child("users").Child(User.UserId).Child("Friends").SetValueAsync(friendAll).ContinueWith((task) =>
        {
            if(task.IsCompleted)
            {
                transform.SetParent(friendContent);
            }
        });
    }

    public void RemoveFriends(Transform transform)
    {
        //string friend = string.Empty;
        //DBref.Child("users").Child(User.UserId).Child("Friends").GetValueAsync().ContinueWith(task =>
        //{
        //    if (task.IsCompleted)
        //    {
        //        if (task.Result != null && task.Result.Value != null)
        //            friend = task.Result.Value.ToString() + name + ",";
        //    }
        //});
        //DBref.Child("users").Child(User.UserId).Child("Friends").SetValueAsync(friend).ContinueWith((task) =>
        //{
        //    if (task.IsCompleted)
        //    {
        //        SetUsers();
        //    }
        //});
    }
}
