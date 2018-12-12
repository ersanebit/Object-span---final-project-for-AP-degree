using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour
{

    [Header("Register Credentials")]
    public GameObject user;
    public GameObject pass;
    public GameObject rePass;
    public Button createPlayerBtn;
    public Text su;

    private string Username;
    private string Password;
    private string RePassword;
    private string form;


    void Start()
    {
        createPlayerBtn.onClick.AddListener(Reg);
    }
    public void Reg()
    {
        StartCoroutine(RegisterToDB());
    }

    IEnumerator RegisterToDB()
    {
        string url = "http://localhost/accounts/reg.php";
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", Username);
        form.AddField("passwordPost", Password);

        WWW www = new WWW(url, form);
        su.text = "Success!";
        Debug.LogWarning("Success " + Username + " " + Password + " " + www);
        yield return www;
        
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
            if (pass.GetComponent<InputField>().isFocused)
            {
                rePass.GetComponent<InputField>().Select();
            }
        }

        Username = user.GetComponent<InputField>().text;
        Password = pass.GetComponent<InputField>().text;
        RePassword = rePass.GetComponent<InputField>().text;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Password != "" && Username != "" && RePassword != "" && RePassword == Password)
            {
                RegisterToDB();
            }
            else
            {
                Debug.LogWarning("Empty Credentials or Passwords do not match.");
            }
        }
    }
}
