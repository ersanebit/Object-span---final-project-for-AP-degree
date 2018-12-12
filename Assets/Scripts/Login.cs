using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour {

    [Header("Login Credentials")]
    public GameObject user;
    public GameObject pass;
    public Button loginPlayerBtn;
    public string Scene;

    private string Username;
    private string Password;

    // Use this for initialization
    void Start ()
    {
		loginPlayerBtn.onClick.AddListener(Log);
    }

    public void Log()
    {
        StartCoroutine(CheckDatabase());
    }

    IEnumerator CheckDatabase()
    {
        string url = "http://localhost/accounts/log.php";
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", Username);
        form.AddField("passwordPost", Password);

        WWW www = new WWW(url, form);
        yield return www;
        Debug.LogWarning(www.text);
        if(www.text == "login success")
        {
            MENU_ACTION_Scene(Scene);
        }
        

    }

    public void MENU_ACTION_Scene(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }
        // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (user.GetComponent<InputField>().isFocused)
            {
                pass.GetComponent<InputField>().Select();
            }
        }

        Username = user.GetComponent<InputField>().text;
        Password = pass.GetComponent<InputField>().text;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Password != "" && Username != "")
            {
                CheckDatabase();
            }
            else
            {
                Debug.LogWarning("Empty Credentials");
            }
        }
    }
}
